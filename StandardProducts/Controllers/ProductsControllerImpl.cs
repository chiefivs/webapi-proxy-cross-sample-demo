using Elasticsearch.Net;
using Nest;
using NLog;
using StandardProducts.Constants;
using StandardProducts.Extensions;
using StandardProducts.Models;
using StandardProducts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StandardProducts.Models.ElasticSearch;
using Newtonsoft.Json;
using StandardProducts.Filters;
using StandardProducts.Infrastructure;
using StandardProducts.Models.Mongo;

namespace StandardProducts.Controllers {

    public interface IProductsController {
        /// <summary>Returns a list of Products for a loation</summary>
        /// <returns>Success</returns>
        Task<ProductForLocationResult> ProductsByLocationPostAsync(ProductsByLocationRequest productsByLocationRequest);

        /// <summary>Product Categories of Products enabled for certain KUMS numbers</summary>
        /// <returns>Success</returns>
        Task<ProductCategoriesResult> ProductCategoriesPostAsync(ProductCategoriesRequest request);

        /// <summary>Product Categories without 'Show all' link</summary>
        /// <returns>Success</returns>
        Task<ProductCategoriesAllLinkVisibilityResult> ProductCategoriesAllLinkVisibilityGetAsync();

        /// <summary>All Products contained in a Product Group for certain KUMS numbers</summary>
        /// <returns>Success</returns>
        Task<ProductsByGroupResult> ProductsByGroupPostAsync(ProductsByGroupRequest productsByGroupRequest);

        /// <summary>All Services related to given Product, VertragId, Rufnummer and AonKundennummerId</summary>
        /// <returns>Success</returns>
        Task<ProductServicesResult> ProductServicesPostAsync(ProductServicesRequest productServicesRequest);

        /// <summary>
        /// Has an HCS product for KUMS
        /// </summary>
        /// <remarks>the method checks if the customer has an HCS product</remarks>
        Task<HasHcsProductResult> HasHcsProductPostAsync(HasHcsProductRequest request);

        /// <summary>Returns detailed information about an MPLS product</summary>
        /// <returns>Success</returns>
        Task<MPLSDetails> MplsDetailsPostAsync(MplsDetailsRequest mplsDetailsRequest);

        /// <summary>
        /// Returns detailed info about numbering services
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<NumberingServiceProductDetailsResult> NumberingServiceProductDetailsPostAsync(
            NumberingServiceProductDetailsRequest request);

        /// <summary>
        /// Returns detailed info about licenses
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<LicenseProductDetailsResult> LicenseProductDetailsPostAsync(LicenseProductDetailsRequest request);

        /// <summary>
        /// Returns detailed information about products
        /// </summary>
        /// <remarks>The ProductsWithLocations endpoint returns a paged object that contains all information about all product</remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ProductsWithLocationsResult> ProductsWithLocationsPostAsync(ProductsWithLocationsRequest request);

        /// <summary>
        /// Gets details about a product by lvl 1-3 and aoncustomer number
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ProductDetail> ProductsByNameAndAonCustomerNumberPostAsync(ProductsByNameAndAonCustomerNumber request);

        /// <summary>
        /// <see cref="ProductsApiController.ProductsBySubCategoryPost"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ProductsBySubCategoryResult> ProductsBySubCategoryPostAsync(ProductsBySubCategoryRequest request);

        /// <summary>
        /// Products for subscription by call number
        /// </summary>
        /// <remarks>the method returns single subscription with products</remarks>
        Task<ProductsForSubscriptionResult> ProductsForSubscriptionPostAsync(ProductsForSubscriptionRequest request);

        /// <summary>
        /// The corresponding kums has a product
        /// </summary>
        /// <param name="getGrantedKumsForMonitoringProductRequest"></param>
        /// <returns></returns>
        Task<GetGrantedKumsForMonitoringProductResult> GetGrantedKumsForMonitoringProductPostAsync(
            GetGrantedKumsForMonitoringProductRequest getGrantedKumsForMonitoringProductRequest);

        /// <summary>
        /// <see cref="ProductsApiController.GetCustomerMarketplaceSubscriptionsPost"/>
        /// </summary>
        Task<GetCustomerMarketplaceSubscriptionsResult> GetCustomerMarketplaceSubscriptionsPostAsync(
            GetCustomerMarketplaceSubscriptionsRequest request);

        /// <summary>
        /// <see cref="ProductsApiController.GetMarketplaceProductDetailsPost"/>
        /// </summary>
        Task<GetMarketplaceProductDetailsResult> GetMarketplaceProductDetailsPostAsync(
            GetMarketplaceProductDetailsRequest request);

        /// <summary>
        /// <see cref="ProductsApiController.GetTusResellerProductDetailsPost"/>
        /// </summary>
        Task<TusResellerProductDetailsResult> GetTusResellerProductDetailsPostAsync(
            GetTusResellerProductDetailsRequest request);

        /// <summary>
        /// <see cref="ProductsApiController.GetAvailableAccessLinesPost"/>
        /// </summary>
        Task<AvailableAccessLinesResult> GetAvailableAccessLinesPostAsync(GetAvailableAccessLinesRequest request);

        /// <summary>
        /// <see cref="ProductsApiController.GetTusZielerweiterungProductDetailsPost"/>
        /// </summary>
        Task<TusZielerweiterungProductDetailsResult> GetTusZielerweiterungProductDetailsPostAsync(
            GetTusZielerweiterungProductDetailsRequest request);

        /// <summary>
        /// <see cref="ProductsApiController.GetExternalSlaCodesPost"/>
        /// </summary>
        /// <returns></returns>
        Task<ExternalSlaCodes> GetExternalSlaCodesPostAsync(GetExternalSlaCodesRequest request);

        /// <summary>
        /// <see cref="ProductsApiController.GetProductsLevel2ByLocationsPost"/>
        /// </summary>
        /// <returns></returns>
        Task<GetProductsLevel2ByLocationsResult> GetProductsLevel2ByLocationsPostAsync(
            GetProductsLevel2ByLocationsRequest request);

        /// <summary>
        /// <see cref="ProductsApiController.GetProductDetailsExportConfigPost"/>
        /// </summary>
        /// <returns></returns>
        Task<GetProductDetailsExportConfigResult> GetProductDetailsExportConfigPostAsync(
            GetProductDetailsExportConfigRequest request);

        /// <summary>
        /// <see cref="ProductsApiController.GetGrantedKumsForDataCenterProductPost"/>
        /// </summary>
        /// <returns></returns>
        Task<GetGrantedKumsForDataCenterResult> GetGrantedKumsForDataCenterProductPostAsync(
            GetGrantedKumsForDataCenterProductRequest getGrantedKumsForDataCenterProductRequest);

        /// <summary>
        /// <see cref="ProductsApiController.GetTusEquipmentPost"/>
        /// </summary>
        /// <returns></returns>
        Task<GetTusEquipmentsResult> GetTusEquipmentPostAsync(GetTusEquipmentsRequest getTusEquipmentsRequest);

        /// <summary>
        /// <see cref="ProductsApiController.NumberingServiceProductAdvancedSearchPost"/>
        /// </summary>
        /// <returns></returns>
        Task<NumberingServiceProductDetailsResult> NumberingServiceProductAdvancedSearchPostAsync(
            NumberingServiceProductAdvancedSearchRequest numberingServiceProductAdvancedSearchRequest);

        /// <summary>
        /// <see cref="ProductsApiController.GetEtherlinkProductsPost"/>
        /// </summary>
        /// <returns></returns>
        Task<DataProductsResult> GetEtherlinkProductsPostAsync(GetEtherlinkRequest getEtherlinkRequest);


        /// <summary>
        /// <see cref="ProductsApiController.GetEtherlinkProductDetailsPost"/>
        /// </summary>
        /// <returns></returns>
        Task<DataProduct> GetEtherlinkProductDetailsPostAsync(GetEtherlinkDetailsRequest getEtherlinkDetailsRequest);
    }

    public class ProductsControllerImpl : IProductsController {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly IMplsService _service;
        private readonly IMappingService _mappingService;
        private readonly IElasticClient _elasticClient;
        private readonly IElasticSearchConfiguration _elasticSearchConfiguration;
        private readonly INmsLiveViewShared _nmsLiveViewControllerShared;
        private readonly IProductsService _productsService;
        private readonly IMarketplaceProductsService _marketplaceProductsService;
        private readonly INumberingServiceProductService _numberingProductsService;
        private readonly ILicenseProductService _licenseProductService;
        private readonly ILocationsService _locationsService;
        private readonly ICustomerInventoryService _customerInventoryService;
        private readonly IKumsFilter _kumsFilter;
        private readonly IObjectSecurityService _objectSecurityService;
        private readonly ITusService _tusService;
        private readonly IDataProductService _dataProductService;

        public ProductsControllerImpl(IMplsService service,
            IElasticClient elasticClient,
            IMappingService mappingService,
            INmsLiveViewShared nmsLiveViewControllerShared,
            IProductsService productsService,
            ILocationsService locationsService,
            ICustomerInventoryService customerInventoryService,
            IMarketplaceProductsService marketplaceProductsService,
            INumberingServiceProductService numberingProductsService,
            ILicenseProductService licenseProductService,
            IElasticSearchConfiguration elasticSearchConfiguration,
            IKumsFilter kumsFilter,
            IObjectSecurityService objectSecurityService,
            ITusService tusService,
            IDataProductService dataProductService) {
            _service = service;
            _elasticClient = elasticClient;
            _mappingService = mappingService;
            _nmsLiveViewControllerShared = nmsLiveViewControllerShared;
            _productsService = productsService;
            _locationsService = locationsService;
            _customerInventoryService = customerInventoryService;
            _marketplaceProductsService = marketplaceProductsService;
            _numberingProductsService = numberingProductsService;
            _licenseProductService = licenseProductService;
            _elasticSearchConfiguration = elasticSearchConfiguration;
            _kumsFilter = kumsFilter;
            _objectSecurityService = objectSecurityService;
            _tusService = tusService;
            _dataProductService = dataProductService;
        }

        public async Task<ProductForLocationResult> ProductsByLocationPostAsync(
            ProductsByLocationRequest productsByLocationRequest) {
            _kumsFilter.ValidateListOfKums(productsByLocationRequest.KumsIds);
            try {
                var response = await GetProductsByLocation(productsByLocationRequest.KumsIds,
                                                           productsByLocationRequest.LocationId,
                                                           productsByLocationRequest.MaincategoryFilter);

                var detailsTypeMappings = _mappingService
                    .GetProductDetailsTypeMappingsBySsaIds(response.Documents.Select(d => d.SsaProduktId).Distinct())
                    .ToDictionary(m => m.SsaProduktId, m => m);

                var productDetailsGroups = _productsService.ProductsDistinct(response.Documents);

                var result = new ProductForLocationResult {
                    ProductsWithCategory = new List<ProductsWithCategory>()
                };

                foreach (var grp in productDetailsGroups) {
                    var bestand = grp.First();

                    var mapping = bestand.SsaProduktId != null && detailsTypeMappings.ContainsKey(bestand.SsaProduktId)
                        ? detailsTypeMappings[bestand.SsaProduktId]
                        : null;

                    result.ProductsWithCategory.Add(new ProductsWithCategory {
                        AonCustomerNumber = bestand.AonKundennummerId,
                        ProductMainCategory = bestand.Level1ProductMainCategory,
                        ProductSubCategory = bestand.Level2ProductSubCategory,
                        ProductGroup = bestand.Level3ProductGroup,
                        ProductService = bestand.Level4Product,
                        ProductNumber = bestand.Rufnummer,
                        Rufnummer = bestand.Rufnummer,
                        VertragId = bestand.VertragId,
                        DetailType = mapping?.DetailsType.ToString(),
                        CallNumber = new CallNumber {
                            CC = bestand.CallNumberCc,
                            SN = bestand.CallNumberSn,
                            NDC = bestand.CallNumberNdc
                        },
                        Kums = bestand.KundeId,
                        LkmsId = bestand.LocationId,
                        LocationDescription = bestand.LocationDescription
                    });
                }

                // get products by location from ma1b_products-data-connection index
                var response2 = await GetDataProductsByLocationAsync(productsByLocationRequest.KumsIds,
                                                                     productsByLocationRequest.LocationId);
                foreach (var dataProduct in response2.Documents) {
                    result.ProductsWithCategory.Add(new ProductsWithCategory {
                        AonCustomerNumber = null,
                        ProductMainCategory = EtherLinkProductConstants.Level1ProductMainCategory,
                        ProductSubCategory = EtherLinkProductConstants.Level2ProductSubCategory,
                        ProductGroup = EtherLinkProductConstants.Level3ProductGroup,
                        ProductService = EtherLinkProductConstants.Level3ProductGroup,
                        ProductNumber = null,
                        Rufnummer = null,
                        VertragId = null,
                        DetailType = null,
                        CallNumber = new CallNumber(),
                        Kums = dataProduct.CustomerNumber,
                        LkmsId = dataProduct.LkmsIdA == productsByLocationRequest.LocationId
                            ? dataProduct.LkmsIdA
                            : dataProduct.LkmsIdB,
                        LocationDescription = dataProduct.LkmsIdA == productsByLocationRequest.LocationId
                            ? dataProduct.LocationA
                            : dataProduct.LocationB,
                        LineId = dataProduct.LineId,
                        LineNumber = dataProduct.LineNumber
                    });
                }

                result.GroupCounts = productDetailsGroups.Select(grp => grp.First())
                    .GroupBy(grp => grp.Level1ProductMainCategory).Select(grp => new GroupCount {
                        GroupName = grp.Key,
                        Count = grp.Count()
                    }).ToList();

                Logger.Debug($"Request ProductsByLocationPostAsync for KUMS '{productsByLocationRequest.KumsIds.KumsIdsAsString()}', Lokation ID '{productsByLocationRequest.LocationId}' "
                             + $"and Product Main Category Filter '{productsByLocationRequest.MaincategoryFilter}' completed. "
                             + $"Returned '{result.ProductsWithCategory.Count}' results ('{response.Total - result.ProductsWithCategory.Count}' duplicates have been removed).");

                return result;
            } catch (Exception ex) {
                Logger.Error(ex,
                             $"Error in request ProductsByLocationPostAsync for KUMS '{productsByLocationRequest.KumsIds.KumsIdsAsString()}', "
                             + $"Lokation ID '{productsByLocationRequest.LocationId}' "
                             + $"and Product Main Category Filter '{productsByLocationRequest.MaincategoryFilter}'.");
                if (ex is ElasticsearchClientException elasticEx) {
                    Logger.Error($"Elastic Search DebugInformation: {elasticEx.DebugInformation}");
                }

                throw;
            }
        }

        public async Task<ProductCategoriesAllLinkVisibilityResult> ProductCategoriesAllLinkVisibilityGetAsync() {
            try {
                var response = await _mappingService.GetProductCategoriesWithoutAllLink();

                return new ProductCategoriesAllLinkVisibilityResult {
                    Items = response.ToList()
                };
            } catch (Exception ex) {
                Logger.Error(ex, "Error in request ProductCategoriesAllLinkVisibilityAsync");

                throw;
            }
        }

        /// <summary>
        /// returns (level 4) products for given level 1 - 3 and a potential search string
        /// </summary>
        /// <param name="productsByGroupRequest"></param>
        /// <returns></returns>
        public async Task<ProductsByGroupResult>
            ProductsByGroupPostAsync(ProductsByGroupRequest productsByGroupRequest) {
            var kumsList = _kumsFilter.ValidateListOfKums(productsByGroupRequest.KumsIdFilters);
            var requestLogData = $"KUMS '{kumsList.KumsIdsAsString()}', "
                                 + $"and Product Group '{productsByGroupRequest.ProductGroup}', "
                                 + $"ProductMainCategory '{productsByGroupRequest.ProductMainCategory}', "
                                 + $"ProductSubCategory '{productsByGroupRequest.ProductSubCategory}'";
            if (!string.IsNullOrEmpty(productsByGroupRequest.SearchString)) {
                requestLogData += $", Search value: '{productsByGroupRequest.SearchString}',"
                                  + $"Search column: '{productsByGroupRequest.SearchProfileName}'";
            }

            try {
                var defaultMustSection = new List<Func<QueryContainerDescriptor<Bestand>, QueryContainer>> {
                    _ => _kumsFilter.GetKumsFilter(productsByGroupRequest.KumsIdFilters,
                                                   $"{Bestand.KumsFieldName}.{CommonConstants.Keyword}",
                                                   $"{Bestand.Level2ProductSubCategoryFieldName}.{CommonConstants.Keyword}",
                                                   $"{Bestand.Level3FieldName}.{CommonConstants.Keyword}")
                };
                var filterQuery = GetProductsByGroupFilterQuery(productsByGroupRequest);

                var response = await _elasticClient.SearchAsync<Bestand>(s => {
                    return s.Index(_elasticSearchConfiguration.ProductsIndexName).Type<Bestand>()
                        .Size(CommonConstants.MaxRecordsCount)
                        .Source(source => source.Includes(includes =>
                                                              includes.Fields(f => f.SsaProduktId,
                                                                              f => f.LocationId,
                                                                              f => f.Staat,
                                                                              f => f.Ort,
                                                                              f => f.State,
                                                                              f => f.Plz,
                                                                              f => f.Strasse,
                                                                              f => f.Hausnummer,
                                                                              f => f.Level4Product,
                                                                              f => f.VertragId,
                                                                              f => f.Rufnummer,
                                                                              f => f.CombinedProductNumber,
                                                                              f => f.AonKundennummerId,
                                                                              f => f.KundeId,
                                                                              f => f.CustomerLocationName,
                                                                              f => f.CallNumberCc,
                                                                              f => f.CallNumberSn,
                                                                              f => f.CallNumberNdc,
                                                                              f => f.DownloadBandwidth,
                                                                              f => f.UploadBandwidth,
                                                                              f => f.BranchNumber,
                                                                              f => f.CpeLoopbackIp,
                                                                              f => f.CpeHostname,
                                                                              f => f.CustomerNetwork,
                                                                              f => f.RedundancyType,
                                                                              f => f.NmsProactive,
                                                                              f => f.NmsPerformanceReport,
                                                                              f => f.NmsTrafficReport,
                                                                              f => f.NmsCustomerAccess,
                                                                              f => f.MplsVpnName,
                                                                              f => f.MplsVpnId,
                                                                              f => f.MplsProfile,
                                                                              f => f.NmsSlaCode,
                                                                              f => f.Level1ProductMainCategory,
                                                                              f => f.Level2ProductSubCategory,
                                                                              f => f.Latitude,
                                                                              f => f.Longitude))).Query(q => {
                            return q.Bool(bo => bo.Must(defaultMustSection).Filter(filterQuery));
                        });
                });

                var detailsTypeMappings = _mappingService
                    .GetProductDetailsTypeMappingsBySsaIds(response.Documents.Select(d => d.SsaProduktId).Distinct())
                    .ToDictionary(m => m.SsaProduktId, m => m);

                var productDetailsGroups = _productsService.ProductsDistinct(response.Documents);
                var totalCount = productDetailsGroups.Count;

                var productDetails = productDetailsGroups.Select(grp => {
                    var bestand = grp.First();
                    var mapping = bestand.SsaProduktId != null && detailsTypeMappings.ContainsKey(bestand.SsaProduktId)
                        ? detailsTypeMappings[bestand.SsaProduktId]
                        : null;

                    long? locationId = null;
                    if (long.TryParse(bestand.LocationId, out var parsedLocationId)) {
                        locationId = parsedLocationId;
                    }

                    return new ProductDetail {
                        LocationId = locationId,
                        LocationCity = bestand.Ort,
                        LocationCountry = bestand.Staat,
                        LocationState = bestand.State,
                        LocationStreet = bestand.StreetDescription,
                        LocationZipCode = bestand.Plz,
                        ProductName = bestand.Level4Product,
                        VertragId = bestand.VertragId,
                        Rufnummer = bestand.Rufnummer,
                        ProductNumber = bestand.CombinedProductNumber,
                        LocationDescription = string.IsNullOrEmpty(bestand.CustomerLocationName)
                            ? bestand.LocationDescription
                            : bestand.CustomerLocationName,
                        AonCustomerNumber = bestand.AonKundennummerId,
                        DetailType = mapping?.DetailsType.ToString(),
                        Kums = bestand.KundeId,
                        DownloadBandwidth = bestand.DownloadBandwidth,
                        UploadBandwidth = bestand.UploadBandwidth,
                        CallNumber = new CallNumber {
                            CC = bestand.CallNumberCc,
                            SN = bestand.CallNumberSn,
                            NDC = bestand.CallNumberNdc
                        },
                        BranchNumber = bestand.BranchNumber,
                        CpeLoopbackIp = bestand.CpeLoopbackIp,
                        CpeHostname = bestand.CpeHostname,
                        CustomerNetwork = bestand.CustomerNetwork,
                        RedundancyType = bestand.RedundancyType,
                        NmsProactive = bestand.NmsProactive,
                        NmsPerformanceReport = bestand.NmsPerformanceReport,
                        NmsTrafficReport = bestand.NmsTrafficReport,
                        NmsCustomerAccess = bestand.NmsCustomerAccess,
                        MplsVpnName = bestand.MplsVpnName,
                        MplsVpnId = bestand.MplsVpnId,
                        MplsProfile = bestand.MplsProfile,
                        NmsSlaCode = bestand.NmsSlaCode,
                        ProductMainCategory = bestand.Level1ProductMainCategory,
                        ProductSubCategory = bestand.Level2ProductSubCategory,
                        LocationLatitude = bestand.Latitude,
                        LocationLongitude = bestand.Longitude
                    };
                });

                if (productsByGroupRequest.SkipCount.HasValue && productsByGroupRequest.MaxResultCount.HasValue) {
                    productDetails = productDetails.Skip(productsByGroupRequest.SkipCount.Value)
                        .Take(productsByGroupRequest.MaxResultCount.Value);
                }

                var result = new ProductsByGroupResult {
                    /*  ProductGroup = new ProductGroup {
                          Name = productsByGroupRequest.ProductGroup
                      },*/
                    MaxResultCount = productsByGroupRequest.MaxResultCount,
                    SkipCount = productsByGroupRequest.SkipCount,
                    TotalCount = totalCount,
                    ProductDetails = productDetails.ToList()
                };

                Logger.Debug($"Request ProductsByGroupPostAsync for {requestLogData} completed. "
                             + $"Returned '{totalCount}' results ('{response.Total - totalCount}' duplicates have been removed).");

                return result;
            } catch (Exception ex) {
                Logger.Error(ex, $"Error in request ProductsByGroupPostAsync for '{requestLogData}'.");
                if (ex is ElasticsearchClientException elasticEx) {
                    Logger.Error($"Elastic Search DebugInformation: {elasticEx.DebugInformation}");
                }

                throw;
            }
        }

        /// <summary>
        /// returns level 5 services for a given vertragsId, Rufnummer and AoncustomerNumber combination, for a defined kumslist
        /// </summary>
        /// <param name="productServicesRequest"></param>
        /// <returns></returns>
        public async Task<ProductServicesResult>
            ProductServicesPostAsync(ProductServicesRequest productServicesRequest) {
            _kumsFilter.ValidateListOfKums(productServicesRequest.KumsIds);
            var requestLogData = $"KUMS '{productServicesRequest.KumsIds.KumsIdsAsString()}', "
                                 + $"VertragId '{productServicesRequest.VertragId}', "
                                 + $"Rufnummer '{productServicesRequest.Rufnummer}' "
                                 + $"and AonCustomerNumber '{productServicesRequest.AonCustomerNumber}'";

            try {
                var filterQuery = GetProductServicesFilterQuery(productServicesRequest.KumsIds, productServicesRequest);

                var response =
                    await _elasticClient.SearchAsync<Bestand>(s => s.Index(_elasticSearchConfiguration.ServicesIndex)
                                                                  .Type<Bestand>().Query(filterQuery)
                                                                  .Size(CommonConstants.MaxRecordsCount)
                                                                  .Sort(x =>
                                                                            x.Ascending(y =>
                                                                                y.Level5Service
                                                                                    .Suffix(CommonConstants
                                                                                        .Keyword))));

                var productServices = response.Documents.Select(bestand => bestand.Level5Service).ToList();

                var result = new ProductServicesResult {
                    ProductServices = productServices
                };

                Logger.Debug($"Request ProductServicesPostAsync for {requestLogData} completed. "
                             + $"Returned '{productServices.Count}' results.");

                return result;
            } catch (Exception ex) {
                Logger.Error(ex, $"Error in request ProductServicesPostAsync for '{requestLogData}'.");
                if (ex is ElasticsearchClientException elasticEx) {
                    Logger.Error($"Elastic Search DebugInformation: {elasticEx.DebugInformation}");
                }

                throw;
            }
        }


        public async Task<ProductDetail> ProductsByNameAndAonCustomerNumberPostAsync(
            ProductsByNameAndAonCustomerNumber request) {
            _kumsFilter.ValidateListOfKums(request.KumsIds);
            var bestand = await _productsService.SearchProductWithLevelsAsync(request.KumsIds,
                                                                              request.ProductMainCategory,
                                                                              request.ProductSubCategory,
                                                                              request.ProductGroup,
                                                                              request.AonCustomerNumber);

            if (bestand == null)
                return null;
            var result = new ProductDetail {
                LocationId = !string.IsNullOrWhiteSpace(bestand.LocationId)
                    ? long.Parse(bestand.LocationId)
                    : new long?(),
                LocationCity = bestand.Ort,
                LocationCountry = bestand.Staat,
                LocationState = bestand.State,
                LocationStreet = bestand.StreetDescription,
                LocationZipCode = bestand.Plz,
                ProductName = bestand.Level4Product,
                VertragId = bestand.VertragId,
                Rufnummer = bestand.Rufnummer,
                ProductNumber = bestand.CombinedProductNumber,
                LocationDescription = string.IsNullOrEmpty(bestand.CustomerLocationName)
                    ? bestand.LocationDescription
                    : bestand.CustomerLocationName,
                AonCustomerNumber = bestand.AonKundennummerId,
                Kums = bestand.KundeId,
                CallNumber = new CallNumber {
                    CC = bestand.CallNumberCc,
                    SN = bestand.CallNumberSn,
                    NDC = bestand.CallNumberNdc,
                }
            };
            return result;
        }

        /// <inheritdoc />
        public async Task<ProductsBySubCategoryResult> ProductsBySubCategoryPostAsync(
            ProductsBySubCategoryRequest request) {
            _kumsFilter.ValidateListOfKums(request.KumsIds);
            var (products, totalCount) = await _productsService.ProductsBySubCategoryAsync(request);
            return new ProductsBySubCategoryResult {
                ProductDetails = products,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc />
        public async Task<ProductsForSubscriptionResult> ProductsForSubscriptionPostAsync(
            ProductsForSubscriptionRequest request) {
            try {
                var productSubscription =
                    await _customerInventoryService.GetProductsBySubscriptionAsync(request.CallNumber.CC,
                     request.CallNumber.NDC,
                     request.CallNumber.SN,
                     request.StopWordsFilteringEnabled ?? true,
                     request.ProductOfferingFilteringEnabled ?? true);
                return new ProductsForSubscriptionResult {
                    ProductSubscription = productSubscription
                };
            } catch (Exception ex) {
                Logger.Error(ex,
                             $"Error in request {nameof(ProductsForSubscriptionPostAsync)} for CC:'{request.CallNumber.CC}', NDC:'{request.CallNumber.NDC}', SN:'{request.CallNumber.SN}'.");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<GetGrantedKumsForMonitoringProductResult> GetGrantedKumsForMonitoringProductPostAsync(
            GetGrantedKumsForMonitoringProductRequest getGrantedKumsForMonitoringProductRequest) {
            _kumsFilter.ValidateListOfKums(getGrantedKumsForMonitoringProductRequest.KumsIds);
            var result =
                await _productsService.GetGrantedKumsForMonitoringProductAsync(getGrantedKumsForMonitoringProductRequest
                                                                                   .KumsIds);

            return new GetGrantedKumsForMonitoringProductResult {
                KumsIds = result.ToList()
            };
        }

        public async Task<GetGrantedKumsForDataCenterResult> GetGrantedKumsForDataCenterProductPostAsync(
            GetGrantedKumsForDataCenterProductRequest getGrantedKumsForDataCenterProductRequest) {
            _kumsFilter.ValidateListOfKums(getGrantedKumsForDataCenterProductRequest.KumsIds);
            var result =
                await _productsService.CheckForDataCenterKumsAsync(getGrantedKumsForDataCenterProductRequest.KumsIds);

            return new GetGrantedKumsForDataCenterResult {
                KumsIds = result.ToList()
            };
        }

        /// <inheritdoc />
        public async Task<GetCustomerMarketplaceSubscriptionsResult> GetCustomerMarketplaceSubscriptionsPostAsync(
            GetCustomerMarketplaceSubscriptionsRequest request) {
            var response =
                await _marketplaceProductsService.GetCustomerMarketplaceSubscriptionsAsync(request.KumsId,
                 request.ProductGroup);

            var result = new GetCustomerMarketplaceSubscriptionsResult {
                Items = response
            };
            return result;
        }

        /// <inheritdoc />
        public async Task<GetMarketplaceProductDetailsResult> GetMarketplaceProductDetailsPostAsync(
            GetMarketplaceProductDetailsRequest request) {
            _kumsFilter.ValidateListOfKums(request.KumsIds);
            var response =
                await _marketplaceProductsService.GetMarketplaceProductDetailsAsync(request.KumsIds,
                 request.ProductGroup);

            var result = new GetMarketplaceProductDetailsResult {
                Items = response
            };
            return result;
        }

        /// <inheritdoc />
        public async Task<TusResellerProductDetailsResult> GetTusResellerProductDetailsPostAsync(
            GetTusResellerProductDetailsRequest request) {
            _kumsFilter.ValidateListOfKums(request.KumsIds);
            try {
                var details = await _service.GetTusResellerProductDetailsAsync(request.MplsAccessLineType,
                                                                               request.AonCustomerNumber,
                                                                               request.KumsIds);
                return details;
            } catch (Exception ex) {
                Logger.Error(ex,
                             $"Error in request GetTusResellerProductDetailsPostAsync for AON Customer Number '{request.AonCustomerNumber}'.");
                if (ex is ElasticsearchClientException elasticEx) {
                    Logger.Error($"Elastic Search DebugInformation: {elasticEx.DebugInformation}");
                }

                throw;
            }
        }

        /// <inheritdoc />
        public async Task<AvailableAccessLinesResult> GetAvailableAccessLinesPostAsync(
            GetAvailableAccessLinesRequest request) {
            _kumsFilter.ValidateListOfKums(request.KumsIds);
            try {
                var details = await _service.GetAvailableAccessLinesAsync(request.AonCustomerNumber, request.KumsIds);
                return details;
            } catch (Exception ex) {
                Logger.Error(ex,
                             $"Error in request GetAvailableAccessLinesAsync for AON Customer Number '{request.AonCustomerNumber}'.");
                if (ex is ElasticsearchClientException elasticEx) {
                    Logger.Error($"Elastic Search DebugInformation: {elasticEx.DebugInformation}");
                }

                throw;
            }
        }

        /// <inheritdoc />
        public async Task<TusZielerweiterungProductDetailsResult> GetTusZielerweiterungProductDetailsPostAsync(
            GetTusZielerweiterungProductDetailsRequest request) {
            try {
                var details =
                    await _objectSecurityService.GetTusZielerweiterungProductDetailsAsync(request.OpenNetAccount);
                return details;
            } catch (Exception ex) {
                Logger.Error(ex,
                             $"Error in request GetTusZielerweiterungProductDetailsPostAsync for OpenNetAccount '{request.OpenNetAccount}'.");
                if (ex is ElasticsearchClientException elasticEx) {
                    Logger.Error($"Elastic Search DebugInformation: {elasticEx.DebugInformation}");
                }

                throw;
            }
        }

        /// <inheritdoc />
        public async Task<ExternalSlaCodes> GetExternalSlaCodesPostAsync(GetExternalSlaCodesRequest request) {
            var result = new ExternalSlaCodes {
                Items = new List<ExternalSlaCode>()
            };
            foreach (var internalSlaCode in request.SlaCodes) {
                var externalSlaCode = _mappingService.GetExternalSlaCode(internalSlaCode);
                result.Items.Add(new ExternalSlaCode {
                    ExternalCode = string.IsNullOrEmpty(externalSlaCode) ? internalSlaCode : externalSlaCode,
                    InternalCode = internalSlaCode
                });
            }

            return await Task.FromResult(result);
        }

        /// <inheritdoc />
        public async Task<GetProductsLevel2ByLocationsResult> GetProductsLevel2ByLocationsPostAsync(
            GetProductsLevel2ByLocationsRequest request) {
            var response =
                await _productsService.GetProductsLevel2ByLocationsPostAsync(request.KumsIds, request.LocationIds);
            response.AddRange(await _productsService.GetDataConnectionProductsByLocationsPostAsync(request.KumsIds,
                               request.LocationIds));

            var result = response.GroupBy(t => t.KumsLocationId).Select(t => new ProductsLevel2ByLocation {
                KumsLocationId = t.Key,
                KumsId = t.First().KumsId,
                LocationId = t.First().LocationId,
                ProductLevel2Names = t.SelectMany(t2 => t2.ProductLevel2Names).Distinct().ToList()
            }).ToList();

            return new GetProductsLevel2ByLocationsResult {
                Items = result
            };
        }

        /// <inheritdoc />
        public async Task<GetProductDetailsExportConfigResult> GetProductDetailsExportConfigPostAsync(
            GetProductDetailsExportConfigRequest request) {
            var response =
                await _productsService.GetProductDetailsExportConfigPostAsync(request.IsForExternal ?? true,
                                                                              request.ProductMainCategory,
                                                                              request.ProductSubCategory,
                                                                              request.ProductGroup);

            return new GetProductDetailsExportConfigResult {
                Items = response
            };
        }

        /// <summary>
        /// Has an HCS product for KUMS
        /// </summary>
        /// <remarks>the method checks if the customer has an HCS product</remarks>
        /// <returns></returns>
        public async Task<HasHcsProductResult> HasHcsProductPostAsync(HasHcsProductRequest request) {
            var allKums = _kumsFilter.ValidateListOfKums(request.KumsIdFilters);
            try {
                // get products by portal uid
                var portalProducts =
                    await _mappingService.GetProductsTypeMappingByPortalAsync(PortalConstants.HcsProductPortalUid,
                                                                              PortalsAreaEnum
                                                                                  .ProductAndServicesPortals);
                if (portalProducts == null) {
                    return new HasHcsProductResult {
                        HasProduct = false
                    };
                }

                var portalProductsList = portalProducts.ToList();

                Logger.Debug($"Found portal product names: {string.Join(";", portalProductsList)}");

                // search for product
                var products = await _productsService.SearchProducts(request.KumsIdFilters, portalProductsList);
                var productGroups = products.Aggs.Terms(_productsService.AggregateProductGroups);
                var hasHcsProduct =
                    productGroups.Buckets.Any(t => t.Key == $"{HcsProductConstants.Level1ProductMainCategory}/"
                                                  + $"{HcsProductConstants.Level2ProductSubCategory}/"
                                                  + $"{HcsProductConstants.Level3ProductGroup}");
                Logger.Debug("Found products:" + JsonConvert.SerializeObject(productGroups));

                return new HasHcsProductResult {
                    HasProduct = hasHcsProduct
                };
            } catch (Exception ex) {
                Logger.Error(ex,
                             $"Error in request {nameof(HasHcsProductPostAsync)} for '{allKums.KumsIdsAsString()}'.");
                if (ex is ElasticsearchClientException elasticEx) {
                    Logger.Error($"Elastic Search DebugInformation: {elasticEx.DebugInformation}");
                }

                throw;
            }
        }

        /// <summary>
        /// returns all possible level 3 groups, grouped by level 1 and 2, also with a potential group filter
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ProductCategoriesResult> ProductCategoriesPostAsync(ProductCategoriesRequest request) {
            var kumsIds = _kumsFilter.ValidateListOfKums(request.KumsIdFilters);
            try {
                // extract product mappings from top hits in aggregate
                var productsTask = _productsService.SearchProducts(request.KumsIdFilters);
                var groupItemTasks = new[] {
                    _nmsLiveViewControllerShared.GetNmsLiveViewProductGroupCount(request.KumsIdFilters),
                    _productsService.GetMonitoringProductGroupCountAsync(request.KumsIdFilters),
                    _productsService.GetMdcProductGroupCountAsync(request.KumsIdFilters),
                    _nmsLiveViewControllerShared.GetNmsTrafficMonitoringProductGroupCount(request.KumsIdFilters),
                    _productsService.GetPaymentProductGroupCount(request.KumsIdFilters),
                    _productsService.GetAddedServiceNumbersProductGroupCount(request.KumsIdFilters),
                    _productsService.GetMicrosoftLicensesProductGroupCount(request.KumsIdFilters),
                    _productsService.GetVirtualFirewallCountAsync(request.KumsIdFilters),
                    _productsService.GetDaaSProductCountAsync(request.KumsIdFilters),
                    _productsService.GetEtherLinkProductCountAsync(request.KumsIdFilters)
                };

                var marketplaceProductGroupCountTask =
                    _marketplaceProductsService.GetMarketplaceProductGroupCountAsync(request.KumsIdFilters);

                // try to start all of the async operations before the first occurrence of "await"

                Logger.Trace("All tasks has been scheduled");

                var productGroupCounts = _productsService.GetProductGroupCounts(await productsTask);
                Logger.Trace("productsTask task has been loaded.");

                var awaitResult = await Task.WhenAll(groupItemTasks);
                Logger.Trace("groupItemTasks tasks has been loaded.");
                foreach (var groupItemTask in awaitResult) {
                    if (groupItemTask != null) {
                        productGroupCounts.Add(groupItemTask);
                    }
                }


                var marketplaceProductGroupCount = await marketplaceProductGroupCountTask;
                Logger.Trace("marketplaceProductGroupCountTask task has been loaded.");
                if (marketplaceProductGroupCount != null) {
                    productGroupCounts.AddRange(marketplaceProductGroupCount);
                }

                Logger.Trace("All scheduled tasks has been loaded.");

                productGroupCounts.RemoveAll(t => t.Level1ProductMainCategory
                                                  == MarketplaceProductConstants.Level1ProductMainCategory
                                                  && t.Level2ProductSubCategory == MarketplaceProductConstants
                                                      .Level2ProductToExclude);

                // group by category and count
                var productGroupsByCategory = productGroupCounts.GroupBy(m => new {
                    m.Level1ProductMainCategory,
                    m.Level2ProductSubCategory
                });

                var groupCounts = productGroupCounts.GroupBy(m => m.Level1ProductMainCategory)
                    .Select(grp => new GroupCount {
                        Count = grp.Distinct(new ProductMappingCountComparer()).Count(),
                        GroupName = grp.Key
                    }).ToList();

                var hasNoGroupFilter = string.IsNullOrWhiteSpace(request.GroupFilter);
                var productGroupComparer = new ProductGroupEqualityComparer();
                var result = new ProductCategoriesResult {
                    ProductCategories = productGroupsByCategory
                        .Where(grp => hasNoGroupFilter
                                      || grp.Key.Level1ProductMainCategory.Equals(request.GroupFilter,
                                       StringComparison.OrdinalIgnoreCase)).Select(grp => new ProductCategory {
                            ProductMainCategory = grp.Key.Level1ProductMainCategory,
                            ProductSubCategory = grp.Key.Level2ProductSubCategory,
                            ProductGroups = grp.Select(p => new ProductGroup {
                                Name = p.Level3ProductGroup
                            }).Distinct(productGroupComparer).OrderBy(pg => pg.Name).ToList()
                        }).OrderBy(pc => pc.ProductMainCategory).ThenBy(pc => pc.ProductSubCategory).ToList(),
                    GroupCounts = groupCounts,
                    TotalGroupCount = groupCounts.Sum(gc => gc.Count)
                };

                Logger.Debug($"Request ProductCategoriesPostAsync for KUMS '{kumsIds.KumsIdsAsString()}' "
                             + $"and Product Main Category Filter '{request.GroupFilter}' completed. "
                             + $"Returned '{result.TotalGroupCount}' product category groupings.");

                return result;
            } catch (Exception ex) {
                Logger.Error(ex,
                             $"Error in request ProductCategoriesPostAsync for KUMS '{kumsIds.KumsIdsAsString()}' "
                             + $"and Product Main Category Filter '{request.GroupFilter}'.");
                if (ex is ElasticsearchClientException elasticEx) {
                    Logger.Error($"Elastic Search DebugInformation: {elasticEx.DebugInformation}");
                }

                throw;
            }
        }

        public async Task<MPLSDetails> MplsDetailsPostAsync(MplsDetailsRequest mplsDetailsRequest) {
            _kumsFilter.ValidateListOfKums(mplsDetailsRequest.KumsIds);
            try {
                return await _service.GetMplsDetails(mplsDetailsRequest.AonCustomerNumber, mplsDetailsRequest.KumsIds);
            } catch (Exception ex) {
                Logger.Error(ex,
                             $"Error in request MplsDetailsPostAsync for AON Customer Number '{mplsDetailsRequest.AonCustomerNumber}'.");
                if (ex is ElasticsearchClientException elasticEx) {
                    Logger.Error($"Elastic Search DebugInformation: {elasticEx.DebugInformation}");
                }

                throw;
            }
        }

        public async Task<NumberingServiceProductDetailsResult> NumberingServiceProductDetailsPostAsync(
            NumberingServiceProductDetailsRequest request) {
            _kumsFilter.ValidateListOfKums(request.KumsIds);
            try {
                return await _numberingProductsService.GetDetailsAsync(request.KumsIds,
                                                                       request.SearchString,
                                                                       request.SearchProfileName,
                                                                       request.SkipCount,
                                                                       request.MaxResultCount);
            } catch (Exception ex) {
                Logger.Error(ex, "Error in request NumberingServiceProductDetailsPostAsync.");
                if (ex is ElasticsearchClientException elasticEx) {
                    Logger.Error($"Elastic Search DebugInformation: {elasticEx.DebugInformation}");
                }

                throw;
            }
        }

        public async Task<LicenseProductDetailsResult> LicenseProductDetailsPostAsync(
            LicenseProductDetailsRequest request) {
            _kumsFilter.ValidateListOfKums(request.KumsIds);
            try {
                return await _licenseProductService.GetLicensesDetailsAsync(request.KumsIds,
                                                                            request.SkipCount,
                                                                            request.MaxResultCount,
                                                                            request.SearchString,
                                                                            request.SearchProfileName);
            } catch (Exception ex) {
                Logger.Error(ex, "Error in request LicenseProductDetailsPostAsync.");
                if (ex is ElasticsearchClientException elasticEx) {
                    Logger.Error($"Elastic Search DebugInformation: {elasticEx.DebugInformation}");
                }

                throw;
            }
        }


        public async Task<ProductsWithLocationsResult> ProductsWithLocationsPostAsync(
            ProductsWithLocationsRequest request) {
            _kumsFilter.ValidateListOfKums(request.KumsIds);
            try {
                var products = await GetProductsByLocation(request.KumsIds,
                                                           takeNotHiddenOnMapOnly: request.TakeNotHiddenOnMapOnly
                                                               .GetValueOrDefault(),
                                                           level2ProductSubCategories: request
                                                               .Level2ProductSubCategories);
                var productDetailsGroups = _productsService.ProductsDistinct(products.Documents);
                var distinctProducts = new List<ProductWithLocation>();
                foreach (var group in productDetailsGroups) {
                    var bestand = group.First();
                    distinctProducts.Add(new ProductWithLocation {
                        // Product Details
                        AonCustomerNumber = bestand.AonKundennummerId,
                        ProductSubCategory = bestand.Level2ProductSubCategory,
                        ProductGroup = bestand.Level3ProductGroup,
                        ProductService = bestand.Level4Product,
                        ProductNumber = bestand.Rufnummer,
                        Rufnummer = bestand.Rufnummer,
                        VertragId = bestand.VertragId,
                        Kums = bestand.KundeId,
                        LkmsId = bestand.LocationId,

                        // Location Details
                        City = bestand.Ort,
                        Country = bestand.Staat,
                        State = bestand.State,
                        Street = bestand.StreetDescription,
                        Postcode = bestand.Plz,
                        BranchNumber = string.IsNullOrWhiteSpace(bestand.BranchNumber) ? "" : bestand.BranchNumber,
                        LocationDescription = string.IsNullOrEmpty(bestand.CustomerLocationName)
                            ? bestand.LocationDescription
                            : bestand.CustomerLocationName,
                    });
                }

                // get products by customers from ma1b_products-data-connection index
                var dataProducts = await GetDataProductsByLocationAsync(request.KumsIds);
                foreach (var dataProduct in dataProducts.Documents) {
                    distinctProducts.Add(new ProductWithLocation {
                        AonCustomerNumber = null,
                        ProductMainCategory = EtherLinkProductConstants.Level1ProductMainCategory,
                        ProductSubCategory = EtherLinkProductConstants.Level2ProductSubCategory,
                        ProductGroup = EtherLinkProductConstants.Level3ProductGroup,
                        ProductService = dataProduct.ServiceId,
                        ProductNumber = null,
                        Rufnummer = null,
                        VertragId = null,
                        DetailType = null,
                        CallNumber = new CallNumber(),
                        Kums = dataProduct.CustomerNumber,
                        LkmsId = dataProduct.LkmsIdA,
                        LocationDescription = dataProduct.LocationA,
                        LineId = dataProduct.LineId,
                        LineNumber = dataProduct.LineNumber
                    });
                    distinctProducts.Add(new ProductWithLocation {
                        AonCustomerNumber = null,
                        ProductMainCategory = EtherLinkProductConstants.Level1ProductMainCategory,
                        ProductSubCategory = EtherLinkProductConstants.Level2ProductSubCategory,
                        ProductGroup = EtherLinkProductConstants.Level3ProductGroup,
                        ProductService = dataProduct.ServiceId,
                        ProductNumber = null,
                        Rufnummer = null,
                        VertragId = null,
                        DetailType = null,
                        CallNumber = new CallNumber(),
                        Kums = dataProduct.CustomerNumber,
                        LkmsId = dataProduct.LkmsIdB,
                        LocationDescription = dataProduct.LocationB,
                        LineId = dataProduct.LineId,
                        LineNumber = dataProduct.LineNumber
                    });
                }

                var result = new ProductsWithLocationsResult {
                    TotalCount = productDetailsGroups.Count,
                    Items = distinctProducts.Skip(request.SkipCount.GetValueOrDefault())
                        .Take(request.MaxResultCount.GetValueOrDefault(CommonConstants.MaxRecordsCount)).ToList()
                };


                return result;
            } catch (Exception exception) {
                Logger.Error(exception,
                             $"Error in request {nameof(ProductsWithLocationsPostAsync)} for kums '{request.KumsIds.KumsIdsAsString()}'.");
                if (exception is ElasticsearchClientException elasticEx) {
                    Logger.Error($"Elastic Search DebugInformation: {elasticEx.DebugInformation}");
                }

                throw;
            }
        }

        public async Task<GetTusEquipmentsResult> GetTusEquipmentPostAsync(GetTusEquipmentsRequest input) {
            var (items, totalCount) = await _tusService.GetTusEquipmentsAsync(input.KumsIds,
                                                                              input.CallNumber,
                                                                              input.SearchString,
                                                                              input.SearchProfileName,
                                                                              input.SkipCount,
                                                                              input.MaxResultCount);
            var result = new GetTusEquipmentsResult {
                Items = items.ToList(),
                ItemsCount = totalCount
            };

            return result;
        }

        public async Task<NumberingServiceProductDetailsResult> NumberingServiceProductAdvancedSearchPostAsync(
            NumberingServiceProductAdvancedSearchRequest input) {
            _kumsFilter.ValidateListOfKums(input.KumsIds);

            var serviceResult = await _numberingProductsService.GetNumberingProductsAdvancedSearchAsync(input.KumsIds,
             input.MaxResultCount,
             input.SkipCount,
             input.Sorting,
             input.AdvancedSearch,
             input.IsInternalUser);
            return serviceResult;
        }

        public async Task<DataProductsResult> GetEtherlinkProductsPostAsync(GetEtherlinkRequest input) {
            var kumsList = _kumsFilter.ValidateListOfKums(input.KumsIdFilters);
            Logger.Info($"GetEtherlinkProductsPostAsync: Read max {input.MaxResultCount} products for {string.Join(',', kumsList)} ");
            var result = await _dataProductService.GetEtherlinkProducts(kumsList,
                                                                        input.MaxResultCount,
                                                                        input.SkipCount,
                                                                        input.Sorting,
                                                                        input.SearchProfileName,
                                                                        input.SearchString);
            Logger.Info($"GetEtherlinkProductsPostAsync: return {result.TotalCount} entries for {string.Join(',', kumsList)}");


            return result;
        }

        public async Task<DataProduct> GetEtherlinkProductDetailsPostAsync(GetEtherlinkDetailsRequest input) {
            var dataProduct =
                await _dataProductService.GetDataProductDetailsAsync(input.KumsId, input.LineNumber, input.LineId);
            return dataProduct;
        }


        #region Helper methods

        #region ProductGroup

        private static Func<QueryContainerDescriptor<Bestand>, QueryContainer> GetKundeIdTerm(IList<string> kumsIds) {
            return f => f.Terms(terms => terms.Field(termsField => termsField.KundeId).Terms(kumsIds));
        }

        /// <summary>
        /// Logic should be similar to <see cref="ProductsService.GetProductsBySubCategoryQuery"/>
        /// </summary>
        /// <param name="productsByGroupRequest"></param>
        /// <returns></returns>
        private Func<QueryContainerDescriptor<Bestand>, QueryContainer> GetProductsByGroupFilterQuery(
            ProductsByGroupRequest productsByGroupRequest) {
            var defaultFilterSection = GetProductsByGroupFilterSection(productsByGroupRequest);
            var searchValue = productsByGroupRequest.SearchString;
            if (string.IsNullOrEmpty(searchValue)) {
                return query => { return query.Bool(b => b.Filter(defaultFilterSection)); };
            }

            var columnsToSearchList =
                _mappingService.GetColumnsToSearchProducts(productsByGroupRequest.SearchProfileName);

            if (columnsToSearchList.Count == 0) {
                throw new ArgumentException("No columns to search on");
            }

            if (columnsToSearchList.Count == 1) {
                return query => {
                    return query.Bool(b => {
                        return b.Filter(defaultFilterSection)
                            .Must(must => must.Match(match => match.Field(columnsToSearchList[0]).Query(searchValue)
                                                         .Operator(Operator.And)));
                    });
                };
            }

            return query => {
                return query.Bool(b => {
                    var shouldSections = new List<Func<QueryContainerDescriptor<Bestand>, QueryContainer>>();

                    foreach (var column in columnsToSearchList) {
                        shouldSections.Add(query1 => {
                            return query1.Match(x => x.Field(column).Query(searchValue).Operator(Operator.And));
                        });
                    }

                    return b.Filter(defaultFilterSection).Should(shouldSections).MinimumShouldMatch(1);
                });
            };
        }

        private static List<Func<QueryContainerDescriptor<Bestand>, QueryContainer>> GetProductsByGroupFilterSection(
            ProductsByGroupRequest productsByGroupRequest) {
            var filterSection = new List<Func<QueryContainerDescriptor<Bestand>, QueryContainer>> {
                s => s.Term(t => t.Level1ProductMainCategory.Suffix(CommonConstants.Keyword),
                            productsByGroupRequest.ProductMainCategory),
                s => s.Term(t => t.Level2ProductSubCategory.Suffix(CommonConstants.Keyword),
                            productsByGroupRequest.ProductSubCategory),
                s => s.Terms(t => t.Field(f => f.Level3ProductGroup.Suffix(CommonConstants.Keyword))
                                 .Terms(productsByGroupRequest.ProductGroup)),
                s => s.Exists(e => e.Field(b => b.Level4Product)),
                s => s.Bool(b => b.MinimumShouldMatch(1).Should(
                                                                // empty string is a non-null value, therefore it has to be included with an extra query
                                                                si => si.Term(t => {
                                                                    return t.Verbatim()
                                                                        .Field(field =>
                                                                                   field.Level5Service
                                                                                       .Suffix(CommonConstants.Keyword))
                                                                        .Value(string.Empty);
                                                                }),
                                                                si =>
                                                                    si.Bool(bi =>
                                                                                bi.MustNot(mn =>
                                                                                    mn.Exists(e =>
                                                                                        e.Field(f => f
                                                                                            .Level5Service))))))
            };
            return filterSection;
        }

        private static Func<QueryContainerDescriptor<Bestand>, QueryContainer> GetProductServicesFilterQuery(
            IList<string> kumsIds,
            ProductServicesRequest productServicesRequest) {
            var kumsTerm = GetKundeIdTerm(kumsIds);

            var defaultMustSection = new List<Func<QueryContainerDescriptor<Bestand>, QueryContainer>> {
                kumsTerm
            };

            var defaultFilterSection = new List<Func<QueryContainerDescriptor<Bestand>, QueryContainer>> {
                s => s.Term(b => b.VertragId.Suffix(CommonConstants.Keyword), productServicesRequest.VertragId),
                s => s.Term(b => b.Rufnummer.Suffix(CommonConstants.Keyword), productServicesRequest.Rufnummer),
                s => s.Term(b => b.AonKundennummerId.Suffix(CommonConstants.Keyword),
                            productServicesRequest.AonCustomerNumber),
                s => s.Bool(b => b.Must(m => m.Exists(e => e.Field(f => f.Level5Service)),
                                        // empty string is a non-null value, therefore it has to be excluded with an extra query
                                        m => m.Bool(bi => bi.MustNot(mn => {
                                            return mn.Term(t => t.Verbatim()
                                                               .Field(field =>
                                                                          field.Level5Service.Suffix(CommonConstants
                                                                              .Keyword)).Value(string.Empty));
                                        }))))
            };

            if (string.IsNullOrEmpty(productServicesRequest.AonCustomerNumber)) {
                defaultFilterSection.Add(s => s.Bool(b => {
                    return b.MustNot(mustNot => mustNot.Exists(row => row.Field(field => field.AonKundennummerId)));
                }));
            }

            return query => { return query.Bool(b => b.Must(defaultMustSection).Filter(defaultFilterSection)); };
        }


        private async Task<ISearchResponse<Bestand>> GetProductsByLocation(IEnumerable<string> kumsIds,
            string locationId = null,
            string mainCategoryFilter = null,
            bool takeNotHiddenOnMapOnly = false,
            List<string> level2ProductSubCategories = null) {
            Func<QueryContainerDescriptor<Bestand>, QueryContainer> takeNotHiddenOnMapOnlyFilter = null;
            if (takeNotHiddenOnMapOnly) {
                takeNotHiddenOnMapOnlyFilter = filter =>
                    filter.Bool(filterBool =>
                                    filterBool.MustNot(f => f.Match(match => match.Query(true.ToString())
                                                                        .Field(x => x.IsHiddenOnMap))));
            }

            var response = await _elasticClient.SearchAsync<Bestand>(s => {
                return s.Index(_elasticSearchConfiguration.ProductsIndexName).Type<Bestand>().Query(q => {
                    return q.Bool(bo => {
                        return bo.Filter(f => f.Terms(t => t.Field(b => b.KundeId).Terms(kumsIds)),
                                         f => f.Term(b => b.LocationId, locationId),
                                         f => f.Term(b => b.Level1ProductMainCategory.Suffix(CommonConstants.Keyword),
                                                     mainCategoryFilter),
                                         f => f.Terms(t => t
                                                          .Field(field =>
                                                                     field.Level2ProductSubCategory
                                                                         .Suffix(CommonConstants.Keyword))
                                                          .Terms(level2ProductSubCategories)),
                                         f => f.Exists(e => e.Field(b => b.Level4Product)),
                                         f => f.Exists(e => e.Field(b => b.LocationId)),
                                         f => f.Bool(filterBool =>
                                                         filterBool.MustNot(_locationsService
                                                                                .GetLocationsMustNotSection<
                                                                                    Bestand>())),
                                         f => f.Bool(filterBool => {
                                             // empty string is a non-null value, therefore it has to be included with an extra query
                                             return filterBool.MinimumShouldMatch(1)
                                                 .Should(should => should.Term(t => t.Verbatim()
                                                                                   .Field(x =>
                                                                                       x.Level5Service
                                                                                           .Suffix(CommonConstants
                                                                                               .Keyword))
                                                                                   .Value(string.Empty)),
                                                         should => should.Bool(shouldBool => {
                                                             return shouldBool.MustNot(mustNot => {
                                                                 return mustNot.Exists(e => e.Field(field => field
                                                                     .Level5Service));
                                                             });
                                                         }));
                                         }),
                                         takeNotHiddenOnMapOnlyFilter);
                    });
                }).Size(CommonConstants.MaxRecordsCount);
            });
            return response;
        }

        private async Task<ISearchResponse<DataProductIndex>> GetDataProductsByLocationAsync(
            IEnumerable<string> kumsIds,
            string locationId = null) {
            var response = await _elasticClient.SearchAsync<DataProductIndex>(s => {
                return s.Index(_elasticSearchConfiguration.DataProductIndexName).Query(q => {
                    return q.Bool(bo => {
                        return
                            bo.Filter(f => f.Terms(t => t.Field(b => b.CustomerNumber.Suffix(CommonConstants.Keyword))
                                                       .Terms(kumsIds)),
                                      f => f.Bool(filterBool => {
                                          return filterBool.MinimumShouldMatch(1)
                                              .Should(should =>
                                                          should.Term(t => t
                                                                          .Field($"{DataProductIndex.LkmsIdAFieldName}.{CommonConstants.Keyword}")
                                                                          .Value(locationId)),
                                                      should =>
                                                          should.Term(t => t
                                                                          .Field($"{DataProductIndex.LkmsIdBFieldName}.{CommonConstants.Keyword}")
                                                                          .Value(locationId)));
                                      }));
                    });
                }).Size(CommonConstants.MaxRecordsCount);
            });
            return response;
        }

        #endregion

        #endregion Helper methods
    }

}