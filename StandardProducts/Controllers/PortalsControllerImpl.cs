using AutoMapper;
using NLog;
using StandardProducts.Extensions;
using StandardProducts.Filters;
using StandardProducts.Models;
using StandardProducts.Models.Mongo;
using StandardProducts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Portal = StandardProducts.Models.Portal;

namespace StandardProducts.Controllers {

    public interface IPortalsController {
        /// <summary>Returns detailed information about Portals</summary>
        /// <returns>Success</returns>
        Task<PortalsResult> PortalsPostAsync(PortalsRequest input);

        /// <summary>
        /// <see cref="PortalsApiController.PortalsBySubCategoryPost"/>
        /// </summary>
        Task<PortalsResult> PortalsBySubCategoryPostAsync(PortalsBySubCategoryRequest request);

        /// <summary>
        /// Returns service portals
        /// </summary>
        /// <param name="servicePortalsRequest"></param>
        /// <returns></returns>
        Task<PortalsResult> ServicePortalsPostAsync(ServicePortalsRequest servicePortalsRequest);

        /// <summary>
        /// <see cref="PortalsApiController.GetAllPortalsPost"/>
        /// </summary>
        /// <returns></returns>
        Task<PortalsResult> GetAllPortalsPostAsync();

        /// <summary>
        /// <see cref="PortalsApiController.GetAllServicePortalsPost"/>
        /// </summary>
        /// <returns></returns>
        Task<PortalsResult> GetAllServicePortalsPostAsync();
    }

    public class PortalsControllerImpl : IPortalsController {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly IMappingService _mappingService;
        private readonly IProductsService _productsService;
        private readonly INmsLiveViewShared _nmsLiveViewControllerShared;
        private readonly INumberingServiceProductService _numberingServiceProductService;
        private readonly IPortalService _portalService;
        private readonly IKumsFilter _kumsFilter;
        private readonly IMapper _mapper;

        public PortalsControllerImpl(IMappingService mappingService,
            IProductsService productsService,
            INmsLiveViewShared nmsLiveViewControllerShared,
            INumberingServiceProductService numberingServiceProductService,
            IKumsFilter kumsFilter,
            IPortalService portalService,
            IMapper mapper) {
            _mappingService = mappingService;
            _productsService = productsService;
            _nmsLiveViewControllerShared = nmsLiveViewControllerShared;
            _numberingServiceProductService = numberingServiceProductService;
            _kumsFilter = kumsFilter;
            _portalService = portalService;
            _mapper = mapper;
        }

        public async Task<PortalsResult> PortalsPostAsync(PortalsRequest input) {
            var allKums = _kumsFilter.ValidateListOfKums(input.KumsIdFilters);

            // Prepare data from data sources
            var allMongoPortals = await _mappingService.GetAllMongoPortalsAsync();
            var finalPortalsList = new List<Portal>();
            if (!_kumsFilter.IsWholeSale()) {
                var productsTask = _productsService.SearchProducts(input.KumsIdFilters);
                var nmsTrafficMonitoringProductGroupTask =
                    _nmsLiveViewControllerShared.GetNmsTrafficMonitoringProductGroupCount(input.KumsIdFilters);
                var numberingProductGroupTask =
                    _numberingServiceProductService.GetNumberingProductGroupCount(input.KumsIdFilters);

                await Task.WhenAll(productsTask, nmsTrafficMonitoringProductGroupTask, numberingProductGroupTask);

                // Combine products
                var productGroupCounts = _productsService.GetProductGroupCounts(productsTask.Result);
                if (nmsTrafficMonitoringProductGroupTask.Result != null) {
                    productGroupCounts.Add(nmsTrafficMonitoringProductGroupTask.Result);
                }

                if (numberingProductGroupTask.Result != null) {
                    productGroupCounts.Add(numberingProductGroupTask.Result);
                }

                // Group by Level3 product name
                var level3ProductsNames = productGroupCounts.Select(t => t.Level3ProductGroup).ToList();
                if (!string.IsNullOrEmpty(input.ProductLevel3Category)) {
                    level3ProductsNames = level3ProductsNames.Where(t => t == input.ProductLevel3Category).ToList();
                }

                var filteredMongoPortalsList = allMongoPortals
                    .Where(t => t.Area == PortalsAreaEnum.ProductAndServicesPortals.ToString()
                                && t.ProductLevel3Categories.Intersect(level3ProductsNames).Any()).ToList();

                finalPortalsList = _mapper.Map<List<Portal>>(filteredMongoPortalsList);
                if (!string.IsNullOrEmpty(input.ProductLevel1Category)) {
                    finalPortalsList = finalPortalsList
                        .Where(t => ( t.Level1ProductMainCategories?.Contains(input.ProductLevel1Category) ?? true )
                                    || t.Level1ProductMainCategories == null || !t.Level1ProductMainCategories.Any())
                        .ToList();
                }

                // if we need a list of All Portals for Meine Portale tab
                //set the portals lvl 1 category to be the one that fits best (max hits) for the different lvl3s
                var portalsList2 = finalPortalsList.Where(x => x.Level1ProductMainCategories == null
                                                               || !x.Level1ProductMainCategories.Any()).ToList();

                var productLevel3Categories = filteredMongoPortalsList
                    .Where(x => x.Level1ProductMainCategories == null || !x.Level1ProductMainCategories.Any())
                    .ToDictionary(k => k.Uid, v => v.ProductLevel3Categories);

                foreach (var portal in portalsList2) {
                    portal.Level1ProductMainCategories = productGroupCounts
                        .Where(t => productLevel3Categories[portal.Uid].Contains(t.Level3ProductGroup))
                        .OrderByDescending(t => t.Count).Select(t => t.Level1ProductMainCategory).Distinct().ToList();
                }

                Logger.Debug($"Found {finalPortalsList.Count} portals that match for LVL3 for KUMS '{allKums.KumsIdsAsString()}'");

                await _portalService.SetSpecialCustomersLinksAsync(allKums, finalPortalsList);
            }

            Logger.Debug($"Found {finalPortalsList.Count} portals that match for LVL 1 or LVL3 for KUMS '{allKums.KumsIdsAsString()}'");
            var allProductIndependentPortals = allMongoPortals.Where(t => t.IsAlwaysDisplayed);
            // add the product independent portals without duplicates
            foreach (var portal in _mapper.Map<List<Portal>>(allProductIndependentPortals)
                .Where(portal => finalPortalsList.All(x => x.Uid != portal.Uid))) {
                finalPortalsList.Add(portal);
            }

            Logger.Debug($"Found {finalPortalsList.Count} portals that match for LVL 1 or LVL3 or project independent for KUMS '{allKums.KumsIdsAsString()}'");

            // map products with portals
            var portalsResult = new PortalsResult {
                Items = finalPortalsList
            };
            return portalsResult;
        }

        public async Task<PortalsResult> PortalsBySubCategoryPostAsync(PortalsBySubCategoryRequest request) {
            var allKums = _kumsFilter.ValidateListOfKums(request.KumsIdFilters);
            var products = await _productsService.SearchProducts(request.KumsIdFilters);

            // extract product mappings from top hits in aggregate
            var productGroupCounts = _productsService.GetProductGroupCounts(products);
            var nmsTrafficMonitoringProductGroup =
                await _nmsLiveViewControllerShared.GetNmsTrafficMonitoringProductGroupCount(request.KumsIdFilters);
            if (nmsTrafficMonitoringProductGroup != null) {
                productGroupCounts.Add(nmsTrafficMonitoringProductGroup);
            }

            // group by category
            var level3Products = productGroupCounts
                .Where(x => string.Equals(x.Level1ProductMainCategory,
                                          request.ProductMainCategory,
                                          StringComparison.OrdinalIgnoreCase)
                            && string.Equals(x.Level2ProductSubCategory,
                                             request.ProductSubCategory,
                                             StringComparison.OrdinalIgnoreCase)).Select(x => x.Level3ProductGroup)
                .ToList();

            var portals = await _mappingService.GetPortalsTypeMappingByProductsAsync(level3Products, true);
            var portalsList = portals.ToList();
            if (!string.IsNullOrEmpty(request.ProductMainCategory)) {
                portalsList = portalsList
                    .Where(t => ( t.Level1ProductMainCategories?.Contains(request.ProductMainCategory) ?? true )
                                || t.Level1ProductMainCategories == null || !t.Level1ProductMainCategories.Any())
                    .ToList();
            }

            // if we need a list of All Portals for Meine Portale tab
            foreach (var portal in portalsList) {
                // get all products of this portal
                var portalProducts =
                    await _mappingService.GetProductsTypeMappingByPortalAsync(portal.Uid,
                                                                              PortalsAreaEnum
                                                                                  .ProductAndServicesPortals);

                portal.Level1ProductMainCategories = productGroupCounts
                    .Where(t => portalProducts.Contains(t.Level3ProductGroup)).OrderByDescending(t => t.Count)
                    .Select(t => t.Level1ProductMainCategory).Distinct().ToList();
            }

            await _portalService.SetSpecialCustomersLinksAsync(allKums, portalsList);

            // map products with portals
            var portalsResult = new PortalsResult {
                Items = portalsList
            };
            return portalsResult;
        }

        public async Task<PortalsResult> ServicePortalsPostAsync(ServicePortalsRequest servicePortalsRequest) {
            _kumsFilter.ValidateListOfKums(servicePortalsRequest.KumsIds);

            try {
                var servicePortals = await _mappingService.GetServicePortalsAsync(servicePortalsRequest);
                return new PortalsResult {
                    Items = servicePortals.ToList()
                };
            } catch (Exception ex) {
                Logger.Error(ex,
                             $"Error in request ServicePortalsPostAsync for KUMS '{servicePortalsRequest.KumsIds.KumsIdsAsString()}', "
                             + $"With a Type: {servicePortalsRequest.Type}.");
                throw;
            }
        }

        public async Task<PortalsResult> GetAllPortalsPostAsync() {
            var servicePortals = await _mappingService.GetAllPortalsAsync();
            return new PortalsResult {
                Items = servicePortals.ToList()
            };
        }

        public async Task<PortalsResult> GetAllServicePortalsPostAsync() {
            var servicePortals = await _mappingService.GetAllServicePortalsAsync();
            return new PortalsResult {
                Items = servicePortals.ToList()
            };
        }
    }

}