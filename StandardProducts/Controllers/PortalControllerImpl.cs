using Elasticsearch.Net;
using Nest;
using NLog;
using StandardProducts.Constants;
using StandardProducts.Models;
using StandardProducts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StandardProducts.Extensions;
using StandardProducts.Filters;
using StandardProducts.Infrastructure;
using StandardProducts.Models.ElasticSearch;
using StandardProducts.Models.Mongo;
using Portal = StandardProducts.Models.Portal;

namespace StandardProducts.Controllers {

    public interface IPortalController {
        /// <summary>Returns detailed information about a Portal by it name</summary>
        /// <returns>Success</returns>
        Task<Portal> PortalPostAsync(PortalRequest portalRequest);

        /// <summary>Returns detailed information about a service Portal by it name</summary>
        /// <returns>Success</returns>
        Task<Portal> ServicePortalPostAsync(ServicePortalRequest portalRequest);
    }

    public class PortalControllerImpl : IPortalController {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly IMappingService _mappingService;
        private readonly IElasticClient _elasticClient;
        private readonly IElasticSearchConfiguration _elasticSearchConfiguration;
        private readonly IProductsService _productsService;
        private readonly IPortalService _portalService;
        private readonly IKumsFilter _kumsFilter;

        public PortalControllerImpl(IElasticClient elasticClient,
            IMappingService mappingService,
            IProductsService productsService,
            IElasticSearchConfiguration elasticSearchConfiguration,
            IKumsFilter kumsFilter,
            IPortalService portalService) {
            _elasticClient = elasticClient;
            _mappingService = mappingService;
            _productsService = productsService;
            _elasticSearchConfiguration = elasticSearchConfiguration;
            _kumsFilter = kumsFilter;
            _portalService = portalService;
        }

        public async Task<Portal> ServicePortalPostAsync(ServicePortalRequest portalRequest) {
            _kumsFilter.ValidateListOfKums(portalRequest.KumsIds);
            try {
                var portal = await _mappingService.GetServicePortalInfoByUid(portalRequest.PortalUid);
                if (portal == null) {
                    return null;
                }

                return portal;
            } catch (Exception ex) {
                Logger.Error(ex,
                             $"Error in request PortalsPostAsync for KUMS '{portalRequest.KumsIds.KumsIdsAsString()}'.");
                if (ex is ElasticsearchClientException elasticEx) {
                    Logger.Error($"Elastic Search DebugInformation: {elasticEx.DebugInformation}");
                }

                throw;
            }
        }

        public async Task<Portal> PortalPostAsync(PortalRequest portalRequest) {
            _kumsFilter.ValidateListOfKums(portalRequest.KumsIds);
            try {
                var portal = await _mappingService.GetPortalInfoByUidAsync(portalRequest.PortalUid);
                if (portal == null) {
                    return null;
                }

                var portalProducts =
                    await _mappingService.GetProductsTypeMappingByPortalAsync(portalRequest.PortalUid,
                                                                              PortalsAreaEnum
                                                                                  .ProductAndServicesPortals);

                if (portalProducts == null) {
                    return null;
                }

                if (!_kumsFilter.IsWholeSale()) {
                    Logger.Debug($"Start of SearchByProductList elasticsearch query. Search index is '{_elasticSearchConfiguration.ProductsIndexName}'");
                    Func<QueryContainerDescriptor<Bestand>, QueryContainer> filterQuery = query => {
                        return query.Bool(b => {
                            return b.Filter(f => f.Terms(t => t.Field(d => d.KundeId).Terms(portalRequest.KumsIds)),
                                            f => f.Terms(t => t
                                                             .Field(d => d.Level3ProductGroup.Suffix(CommonConstants
                                                                        .Keyword))
                                                             .Terms(portalProducts.ToList())));
                        });
                    };
                    var asyncResponse = await _elasticClient.SearchAsync<Bestand>(s => {
                        return s.Index(_elasticSearchConfiguration.ProductsIndexName).Type<Bestand>()
                            .Size(0) // do not request search hits (increases performance)
                            .Query(filterQuery).Aggregations(a => a.Terms(_productsService.AggregateProductGroups,
                                                                          p => p
                                                                              .Field(b =>
                                                                                  b.Level3ProductGroup
                                                                                      .Suffix(CommonConstants
                                                                                          .Keyword))
                                                                              .Size(CommonConstants.MaxRecordsCount)
                                                                              .Aggregations(aa =>
                                                                                  aa.TopHits(_productsService
                                                                                       .AggregateTopHits,
                                                                                   th => th.Size(1)
                                                                                       .Source(src =>
                                                                                           src
                                                                                               .Includes(i =>
                                                                                                   i.Field(b =>
                                                                                                           b.SsaProduktId)
                                                                                                       .Field(b =>
                                                                                                           b.Level1ProductMainCategory)
                                                                                                       .Field(b =>
                                                                                                           b.Level3ProductGroup)))))));
                    });

                    var productGroupCounts = _productsService.GetProductGroupCounts(asyncResponse);
                    portal.Level1ProductMainCategories = productGroupCounts.OrderByDescending(t => t.Count)
                        .Select(t => t.Level1ProductMainCategory).Distinct().ToList();


                    await _portalService.SetSpecialCustomersLinksAsync(portalRequest.KumsIds,
                                                                       new List<Portal> {
                                                                           portal
                                                                       });
                }

                Logger.Debug($"Request PortalPostAsync for KUMS '{portalRequest.KumsIds.KumsIdsAsString()}' "
                             + $"and PortalUid '{portalRequest.PortalUid}' completed");

                return portal;
            } catch (Exception ex) {
                Logger.Error(ex,
                             $"Error in request PortalsPostAsync for KUMS '{portalRequest.KumsIds.KumsIdsAsString()}'.");
                if (ex is ElasticsearchClientException elasticEx) {
                    Logger.Error($"Elastic Search DebugInformation: {elasticEx.DebugInformation}");
                }

                throw;
            }
        }
    }

}