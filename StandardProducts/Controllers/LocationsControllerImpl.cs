using Elasticsearch.Net;
using Nest;
using NLog;
using StandardProducts.Constants;
using StandardProducts.Models;
using StandardProducts.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StandardProducts.Extensions;
using StandardProducts.Filters;
using StandardProducts.Infrastructure;
using StandardProducts.Models.ElasticSearch;

namespace StandardProducts.Controllers {

    public interface ILocationsController {
        /// <summary>Locations for KUMS</summary>
        /// <returns>Success</returns>
        Task<LocationsResult> LocationsPostAsync(LocationsRequest locationsRequest);
        Task<LocationsWsResult> LocationsForMapWsPostAsync(LocationsWsRequest request);

        /// <summary>Locations count for KUMS</summary>
        /// <returns>Success</returns>
        Task<LocationsCountResult> LocationsCountPostAsync(LocationsCountRequest locationsCountRequest);

        /// <summary>Location for KUMS by LocationId</summary>
        /// <returns>Success</returns>
        Task<Location> LocationPostAsync(LocationRequest locationRequest);

        /// <summary>
        /// <see cref="LocationsApiController.UpdateLocationPost"/>
        /// </summary>
        /// <param name="updateLocationRequest"></param>
        /// <returns></returns>
        Task<ActionResult> UpdateLocationPostAsync(UpdateLocationRequest updateLocationRequest);

        /// <summary>
        /// <see cref="LocationsApiController.CheckFullLoadProgressPost"/>
        /// </summary>
        /// <returns></returns>
        Task<CheckFullLoadProgressResult> CheckFullLoadProgressPostAsync();

        /// <summary>
        /// <see cref="LocationsApiController.GetProductGroupFiltersPost"/>
        /// </summary>
        /// <returns></returns>
        Task<GetProductGroupFiltersResult> GetProductGroupFiltersPostAsync();
    }

    public class LocationsControllerImpl : ILocationsController {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly IMappingService _mappingService;
        private readonly IElasticClient _elasticClient;
        private readonly IElasticSearchConfiguration _elasticSearchConfiguration;
        private readonly IConnectionService _connectionService;
        private readonly IUpdateService _updateService;
        private readonly ILocationsService _locationsService;
        private readonly IAddressesService _addressesService;
        private readonly IKumsFilter _kumsFilter;

        public LocationsControllerImpl(IElasticClient elasticClient,
            IMappingService mappingService,
            IConnectionService connectionService,
            IUpdateService updateService,
            ILocationsService locationsService,
            IAddressesService addressesService,
            IElasticSearchConfiguration elasticSearchConfiguration,
            IKumsFilter kumsFilter) {
            _elasticClient = elasticClient;
            _mappingService = mappingService;
            _connectionService = connectionService;
            _updateService = updateService;
            _locationsService = locationsService;
            _addressesService = addressesService;
            _elasticSearchConfiguration = elasticSearchConfiguration;
            _kumsFilter = kumsFilter;
        }

        /// <inheritdoc />
        public async Task<LocationsResult> LocationsPostAsync(LocationsRequest locationsRequest) {
            _kumsFilter.ValidateListOfKums(locationsRequest.KumsIds);
            var kumsIdString = locationsRequest.KumsIds.KumsIdsAsString();
            var requestLogData = $"KUMS '{kumsIdString}'";
            if (!string.IsNullOrEmpty(locationsRequest.SearchString)) {
                requestLogData += $", Search value: '{locationsRequest.SearchString}',"
                                  + $"Search column: '{locationsRequest.SearchProfileName}'";
            }

            try {
                var locationResult = await _elasticClient.SearchAsync<Standort>(search => {
                    var searchResult = search.Index(_elasticSearchConfiguration.LocationIndexName).Type<Standort>()
                        .Sort(x => x.Ascending(y => y.LocationId.Suffix(CommonConstants.Keyword)))
                        .Query(GetLocationsQuery(locationsRequest.KumsIds,
                                                 locationsRequest.SearchProfileName,
                                                 locationsRequest.SearchString,
                                                 locationsRequest.TakeNotHiddenOnMapOnly ?? false));

                    if (locationsRequest.SkipCount != null) {
                        searchResult = searchResult.Skip(locationsRequest.SkipCount.Value);
                    }

                    searchResult =
                        searchResult.Size(locationsRequest.MaxResultCount ?? CommonConstants.MaxRecordsCount);
                    return searchResult;
                });

                Logger.Debug($"Request LocationsPostAsync for '{requestLogData}' completed."
                             + $"Total number of documents matching the search query criteria: '{locationResult.Total}'. "
                             + $"Number of documents returned after paging: '{locationResult.Hits.Count}'.");

                var locationsDto = new LocationsResult {
                    TotalCount = (int) locationResult.Total,
                    Items = locationResult.Hits.Select(x => LocationToDto(x.Source)).ToList()
                };
                return locationsDto;
            } catch (Exception ex) {
                Logger.Error(ex, $"Error in request LocationsPostAsync for '{requestLogData}'.");
                if (ex is ElasticsearchClientException elasticEx) {
                    Logger.Error($"Elastic Search DebugInformation: {elasticEx.DebugInformation}");
                }

                throw;
            }
        }

        /// <inheritdoc />
        public async Task<LocationsWsResult> LocationsForMapWsPostAsync(LocationsWsRequest request) {
            var locations = await _addressesService.GetLocationsForMapWsAsync(request.KumsIds, request.AdvancedSearch);

            return new LocationsWsResult {
                TotalCount = locations.Count,
                Items = locations
            };
        }

        /// <inheritdoc />
        public async Task<LocationsCountResult> LocationsCountPostAsync(LocationsCountRequest locationsCountRequest) {
            _kumsFilter.ValidateListOfKums(locationsCountRequest.KumsIds);
            var kumsIdString = locationsCountRequest.KumsIds.KumsIdsAsString();
            try {
                var locationResult = await _elasticClient.CountAsync<Standort>(search => {
                    var searchResult = search.Index(_elasticSearchConfiguration.LocationIndexName).Type<Standort>()
                        .Query(GetLocationsQuery(locationsCountRequest.KumsIds,
                                                 "",
                                                 "",
                                                 locationsCountRequest.TakeNotHiddenOnMapOnly ?? false));
                    return searchResult;
                });

                Logger.Debug($"Request LocationsCountPostAsync for KUMS '{kumsIdString}' completed. "
                             + $"Total number of documents matching the search query criteria: '{locationResult.Count}'.");

                var locationsCountDto = new LocationsCountResult {
                    Count = (int) locationResult.Count
                };
                return locationsCountDto;
            } catch (Exception ex) {
                Logger.Error(ex, $"Error in request LocationsPostAsync for KUMS '{kumsIdString}'.");
                if (ex is ElasticsearchClientException elasticEx) {
                    Logger.Error($"Elastic Search DebugInformation: {elasticEx.DebugInformation}");
                }

                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Location> LocationPostAsync(LocationRequest locationRequest) {
            _kumsFilter.ValidateListOfKums(locationRequest.KumsIds);

            // should never happen due to swagger rules
            Debug.Assert(locationRequest.LocationId != null,
                         $"{nameof(locationRequest)}.{nameof(locationRequest.LocationId)} != null");

            var kumsIdString = locationRequest.KumsIds.KumsIdsAsString();
            var locationDataObject =
                await _locationsService.LocationByIdAsync(locationRequest.KumsIds, locationRequest.LocationId.Value);
            if (locationDataObject == null) {
                throw new
                    Exception($"Can not find Location for KUMS '{kumsIdString}' and Location ID '{locationRequest.LocationId}'.");
            }

            var locationDto = LocationToDto(locationDataObject);

            Logger.Debug($"Request LocationPostAsync for KUMS '{kumsIdString}' and Location ID '{locationRequest.LocationId}' completed.");
            return locationDto;
        }

        /// <inheritdoc />
        public async Task<ActionResult> UpdateLocationPostAsync(UpdateLocationRequest updateLocationRequest) {
            var listOfIndexes = new List<string>(new[] {
                _elasticSearchConfiguration.LocationIndexName, _elasticSearchConfiguration.ProductsIndexName,
                _elasticSearchConfiguration.PaymentProductIndexName
            });
            //write the updates into the queue: all changes should be reflected in DB
            //use one connection and transaction for both db updates
            await using (var connection = _connectionService.NewSqlConnection) {
                await connection.OpenAsync();
                await using var transaction = connection.BeginTransaction();
                foreach (var indexName in listOfIndexes) {
                    await MoveCustomerLocationToQueue(updateLocationRequest.CustomerLocationId,
                                                      updateLocationRequest.IsHiddenOnMap ?? false,
                                                      indexName,
                                                      connection,
                                                      transaction);
                }

                transaction.Commit();
            }

            return new OkResult();
        }

        /// <inheritdoc />
        public async Task<CheckFullLoadProgressResult> CheckFullLoadProgressPostAsync() {
            try {
                var result = new CheckFullLoadProgressResult {
                    IsBusy =
                        await _updateService.IsImportRunningForIndexAsync(_elasticSearchConfiguration.LocationIndexName)
                };
                return result;
            } catch (Exception ex) {
                Logger.Error(ex, "Error in request CheckFullLoadProgressPostAsync.");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<GetProductGroupFiltersResult> GetProductGroupFiltersPostAsync() {
            var response = await _mappingService.GetProductGroupFiltersAsync();
            var result = new GetProductGroupFiltersResult {
                Items = new List<ProductGroupFilter>()
            };

            foreach (var groupFilter in response.GroupBy(t => t.FilterType)) {
                var productGroupFilter = new ProductGroupFilter {
                    FilterType = groupFilter.Key,
                    ProductLevel2NamesFilterItems = groupFilter.Select(t => t.Level2).OrderBy(t => t).ToList()
                };
                result.Items.Add(productGroupFilter);
            }

            return result;
        }


        #region Helper methods

        private static Location LocationToDto(Standort location) {
            return new() {
                LocationId = int.Parse(location.LocationId),
                City = location.City,
                Country = location.Country,
                State = location.State,
                LocationDescription = string.IsNullOrEmpty(location.CustomerLocationName)
                    ? location.Description
                    : location.CustomerLocationName,
                Kums = location.CustomerNumber,
                LineId = location.LineId,
                LineNumber = location.LineNumber,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Postcode = location.Postcode,
                Street = location.StreetDescription,
                StreetName = location.Street,
                HouseNumber = location.HouseNumber,
                IsHiddenOnMap = location.IsHiddenOnMap,
                BranchNumber = string.IsNullOrWhiteSpace(location.BranchNumber) ? "" : location.BranchNumber
            };
        }

        private Func<QueryContainerDescriptor<Standort>, QueryContainer> GetLocationsQuery(IList<string> kumsIds,
            string columnName,
            string searchValue,
            bool takeNotHiddenOnly) {
            var kumsTerm = GetCustomerNumberTerm(kumsIds);

            if (string.IsNullOrEmpty(searchValue)) {
                return query => {
                    if (takeNotHiddenOnly) {
                        return query.Bool(b => b.Filter(kumsTerm,
                                                        filter => filter.Bool(filterBool =>
                                                                                  filterBool.MustNot(_locationsService
                                                                                                         .GetLocationsMustNotSection
                                                                                                         <Standort
                                                                                                         >())),
                                                        filter => filter.Bool(filterBool =>
                                                                                  filterBool.MustNot(f =>
                                                                                                         f.Match(match =>
                                                                                                                     match
                                                                                                                         .Query(true
                                                                                                                                    .ToString())
                                                                                                                         .Field(x =>
                                                                                                                                    x.IsHiddenOnMap))))));
                    }

                    return query.Bool(b => b.Filter(kumsTerm,
                                                    filter => filter.Bool(filterBool =>
                                                                              filterBool.MustNot(_locationsService
                                                                                                     .GetLocationsMustNotSection
                                                                                                     <Standort
                                                                                                     >()))));
                };
            }

            var columnsToSearchList = _mappingService.GetColumnsToSearchLocations(columnName);

            if (columnsToSearchList.Count == 0) {
                throw new ArgumentException("No columns to search on");
            }

            if (columnsToSearchList.Count == 1) {
                return query => {
                    return query.Bool(b => {
                        if (takeNotHiddenOnly) {
                            return b.Filter(kumsTerm,
                                            filter => filter.Bool(filterBool =>
                                                                      filterBool.MustNot(_locationsService
                                                                                             .GetLocationsMustNotSection
                                                                                                 <Standort>())),
                                            filter => filter.Bool(filterBool =>
                                                                      filterBool.MustNot(f => f.Match(match => match
                                                                                                          .Query(true
                                                                                                                     .ToString())
                                                                                                          .Field(x => x
                                                                                                                     .IsHiddenOnMap)))))
                                .Must(f => f.Match(t => t.Field(columnsToSearchList[0]).Query(searchValue)
                                                       .Operator(Operator.And)));
                        }

                        return b.Filter(kumsTerm,
                                        filter => filter.Bool(filterBool =>
                                                                  filterBool.MustNot(_locationsService
                                                                                         .GetLocationsMustNotSection<
                                                                                             Standort>())))
                            .Must(f => f.Match(t => t.Field(columnsToSearchList[0]).Query(searchValue)
                                                   .Operator(Operator.And)));
                    });
                };
            }

            return query => {
                return query.Bool(b => {
                    if (takeNotHiddenOnly) {
                        return b.Filter(kumsTerm,
                                        filter => filter.Bool(filterBool =>
                                                                  filterBool.MustNot(_locationsService
                                                                                         .GetLocationsMustNotSection<
                                                                                             Standort>())),
                                        filter => filter.Bool(filterBool =>
                                                                  filterBool.MustNot(f => f.Match(match => match
                                                                                                      .Query(true
                                                                                                                 .ToString())
                                                                                                      .Field(x => x
                                                                                                                 .IsHiddenOnMap)))))
                            .Must(must => must.MultiMatch(multiMatch =>
                                                              multiMatch.Type(TextQueryType.MostFields)
                                                                  .Query(searchValue)
                                                                  .Operator(Operator.And)
                                                                  .Fields(x => x.Fields(columnsToSearchList
                                                                                            .ToArray()))));
                    }

                    return b
                        .Filter(kumsTerm,
                                filter => filter.Bool(filterBool =>
                                                          filterBool.MustNot(_locationsService
                                                                                 .GetLocationsMustNotSection<Standort
                                                                                 >())))
                        .Must(must => must.MultiMatch(multiMatch =>
                                                          multiMatch.Type(TextQueryType.MostFields).Query(searchValue)
                                                              .Operator(Operator.And)
                                                              .Fields(x => x.Fields(columnsToSearchList.ToArray()))));
                });
            };
        }


        private static Func<QueryContainerDescriptor<Standort>, QueryContainer> GetCustomerNumberTerm(
            IList<string> kumsIds) {
            return f => f.Terms(terms => terms.Field(termsField => termsField.CustomerNumber).Terms(kumsIds));
        }

        private static async Task MoveCustomerLocationToQueue(long? customerLocationId,
            bool isHiddenOnMap,
            string indexName,
            SqlConnection connection,
            SqlTransaction transaction) {
            var isHiddenOnMapInt = isHiddenOnMap ? 1 : 0;
            await using var insertCommand = new SqlCommand($@"INSERT INTO [dbo].[CustomerLocationQueue] 
                            ([Name], [State], [CreationTime], [CustomerLocationId], [IsHiddenOnMap]) VALUES (
                            '{indexName}', '{QueueStates.New}', '{
                                DateTime.Now.ToString(CultureInfo.InvariantCulture)
                            }', {customerLocationId}, {isHiddenOnMapInt})",
                                                           connection,
                                                           transaction);
            await insertCommand.ExecuteNonQueryAsync();
        }

        #endregion
    }

}