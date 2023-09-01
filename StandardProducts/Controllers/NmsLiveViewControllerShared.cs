using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using NLog;
using StandardProducts.Constants;
using StandardProducts.Filters;
using StandardProducts.Infrastructure;
using StandardProducts.Models;
using StandardProducts.Models.ElasticSearch;
using StandardProducts.Models.Output;

namespace StandardProducts.Controllers {

    public interface INmsLiveViewShared {
        /// <summary>
        /// Returns a ProductGroupCount object if at least on kums has NMS Live View product
        /// </summary>
        Task<ProductGroupCount> GetNmsLiveViewProductGroupCount(List<KumsIdFilter> kumsIdFilters);

        /// <summary>
        /// Returns a ProductGroupCount object if at least on kums has NMS Traffic Monitoring product
        /// </summary>
        Task<ProductGroupCount> GetNmsTrafficMonitoringProductGroupCount(List<KumsIdFilter> kumsIdFilters);
    }

    public class NmsLiveViewShared : INmsLiveViewShared {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly IElasticClient _elasticClient;
        private readonly IElasticSearchConfiguration _elasticSearchConfiguration;
        private readonly IKumsFilter _kumsFilter;

        public NmsLiveViewShared(IElasticClient elasticClient,
            IElasticSearchConfiguration elasticSearchConfiguration,
            IKumsFilter kumsFilter) {
            _elasticClient = elasticClient;
            _elasticSearchConfiguration = elasticSearchConfiguration;
            _kumsFilter = kumsFilter;
        }

        public async Task<ProductGroupCount> GetNmsLiveViewProductGroupCount(List<KumsIdFilter> kumsIdFilters) {
            var result = new ProductGroupCount {
                    Level1ProductMainCategory = NmsLiveViewConstants.Level1ProductMainCategory,
                    Level2ProductSubCategory = NmsLiveViewConstants.Level2ProductSubCategory,
                    Level3ProductGroup = NmsLiveViewConstants.Level3ProductGroup
                };
            var kumsIds = _kumsFilter.FindKumsByProducts(kumsIdFilters,
                                                         result.Level2ProductSubCategory,
                                                         result.Level3ProductGroup);
            if (!kumsIds.Any()) {
                return null;
            }

            if (await HasNmsLiveViewProduct(kumsIds)) {
                return result;
            }

            return null;
        }

        public async Task<ProductGroupCount>
            GetNmsTrafficMonitoringProductGroupCount(List<KumsIdFilter> kumsIdFilters) {
            var result = new ProductGroupCount {
                    Level1ProductMainCategory = NmsTrafficMonitoringConstants.Level1ProductMainCategory,
                    Level2ProductSubCategory = NmsTrafficMonitoringConstants.Level2ProductSubCategory,
                    Level3ProductGroup = NmsTrafficMonitoringConstants.Level3ProductGroup
                };
            var kumsIds = _kumsFilter.FindKumsByProducts(kumsIdFilters,
                                                         result.Level2ProductSubCategory,
                                                         result.Level3ProductGroup);
            if (!kumsIds.Any()) {
                return null;
            }

            if (await HasNmsTrafficMonitoringProduct(kumsIds)) {
                return result;
            }

            return null;
        }

        public async Task<bool> HasNmsLiveViewProduct(IList<string> kumsIds) {
            Logger.Debug($"Start of Has Nms Live View product ElasticSearch query. Search index is '{_elasticSearchConfiguration.NmsIndexName}'");
            var asyncResponse = await _elasticClient.CountAsync<NmsLiveService>(s => {
                return s.Index(_elasticSearchConfiguration.NmsIndexName).AllTypes()
                    .Query(q => q.Bool(queryBool => queryBool.Filter(GetNmsLiveFilters(kumsIds))));
            });
            Logger.Debug($"Completed Has Nms Live View product ElasticSearch query. Search index is '{_elasticSearchConfiguration.NmsIndexName}'.");

            // if at least one KUMS value has "nms_live_view" = true
            return asyncResponse.Count > 0;
        }

        public async Task<bool> HasNmsTrafficMonitoringProduct(IList<string> kumsIds) {
            Logger.Debug($"Start of Has Nms Live View product ElasticSearch query. Search index is '{_elasticSearchConfiguration.NmsIndexName}'");
            var asyncResponse = await _elasticClient.CountAsync<NmsLiveService>(s => {
                return s.Index(_elasticSearchConfiguration.NmsIndexName).AllTypes()
                    .Query(q => q.Bool(queryBool => queryBool.Filter(GetNmsTrafficMonitoringFilters(kumsIds))));
            });
            Logger.Debug($"Completed Has Nms Live View product ElasticSearch query. Search index is '{_elasticSearchConfiguration.NmsIndexName}'.");

            // if at least one KUMS value has "nms_live_view" = true
            return asyncResponse.Count > 0;
        }

        public Func<QueryContainerDescriptor<NmsLiveService>, QueryContainer>[] GetNmsTrafficMonitoringFilters(
            IList<string> kumsId) {
            // Create next part of query: get KUMS having "nms_traffic_monitoring" = true
            var list = new Func<QueryContainerDescriptor<NmsLiveService>, QueryContainer>[] {
                filter => filter.Terms(terms => terms.Field(termField => termField.Kums.Suffix(CommonConstants.Keyword))
                                           .Terms(kumsId)),
                filter => filter.Term(term => term
                                          .Field(termField =>
                                                     termField.NmsTrafficMonitoring.Suffix(CommonConstants.Keyword))
                                          .Value("true"))
            };
            return list;
        }

        public Func<QueryContainerDescriptor<NmsLiveService>, QueryContainer>[]
            GetNmsLiveFilters(IList<string> kumsId) {
            // Create next part of query: get KUMS having "nms_live_view" = true
            var list = new Func<QueryContainerDescriptor<NmsLiveService>, QueryContainer>[] {
                filter => filter.Terms(terms => terms.Field(termField => termField.Kums.Suffix(CommonConstants.Keyword))
                                           .Terms(kumsId)),
                filter => filter.Term(term => term
                                          .Field(termField => termField.NmsLiveView.Suffix(CommonConstants.Keyword))
                                          .Value("true"))
            };
            return list;
        }
    }

}