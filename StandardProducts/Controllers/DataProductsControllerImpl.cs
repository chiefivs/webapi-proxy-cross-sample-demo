using System.Linq;
using System.Threading.Tasks;
using NLog;
using StandardProducts.Filters;
using StandardProducts.Models;
using StandardProducts.Services;

namespace StandardProducts.Controllers {

    public interface IDataProductsController {
        /// <summary>
        /// <see cref="DataProductsApiController.GetDataProductsPost"/>
        /// </summary>
        Task<DataProductsResult> GetDataProductsPostAsync(GetDataProductsRequest input);

        /// <summary>
        /// <see cref="DataProductsApiController.GetDataProductDetailsPost"/>
        /// </summary>
        Task<DataProduct> GetDataProductDetailsPostAsync(GetDataProductDetailsRequest input);

        /// <summary>
        /// <see cref="DataProductsApiController.GetDataProductsCountPost"/>
        /// </summary>
        Task<DataProductsCountResult> GetDataProductsCountPostAsync(GetDataProductsCountRequest input);

        /// <summary>
        /// <see cref="DataProductsApiController.GetDataProductsSearchableColumnsPost"/>
        /// </summary>
        Task<SearchableColumnsResult> GetDataProductsSearchableColumnsPostAsync(GetDataProductsSearchableColumnsRequest input);
    }

    public class DataProductsControllerImpl : IDataProductsController {
        private readonly IDataProductService _dataProductService;
        private readonly IKumsFilter _kumsFilter;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public DataProductsControllerImpl(IDataProductService dataProductService, IKumsFilter kumsFilter) {
            _dataProductService = dataProductService;
            _kumsFilter = kumsFilter;
        }

        /// <inheritdoc />
        public async Task<DataProductsResult> GetDataProductsPostAsync(GetDataProductsRequest input) {
            _kumsFilter.ValidateListOfKums(input.KumsIds);



            Logger.Info($"GetDataProductsPostAsync: Read max {input.MaxResultCount} products for {string.Join(',', input.KumsIds)} ");
            var (items, totalCount) = await _dataProductService.GetDataProductsAsync(input.KumsIds,
                                                                                     input.MaxResultCount,
                                                                                     input.SkipCount,
                                                                                     input.Sorting,
                                                                                     input.ProductTypeFilter,
                                                                                     input.AdvancedSearch);
            Logger.Info($"GetDataProductsPostAsync: return {totalCount} entries for {string.Join(',', input.KumsIds)}");

            var result = new DataProductsResult {
                DataProducts = items,
                TotalCount = totalCount
            };

            return result;
        }

        /// <inheritdoc />
        public async Task<DataProduct> GetDataProductDetailsPostAsync(GetDataProductDetailsRequest input) {
            var dataProduct = await _dataProductService.GetDataProductDetailsAsync(input.KumsId, input.LineNumber, string.Empty);
            return dataProduct;
        }

        /// <inheritdoc />
        public async Task<DataProductsCountResult> GetDataProductsCountPostAsync(GetDataProductsCountRequest input) {
            _kumsFilter.ValidateListOfKums(input.KumsIds);
            var totalCount = await _dataProductService.GetDataProductsCountAsync(input.KumsIds,
                                                                                 input.AdvancedSearch,
                                                                                 input.ProductTypeFilter);
            var result = new DataProductsCountResult {
                TotalCount = totalCount
            };

            return result;
        }

        /// <inheritdoc />
        public async Task<SearchableColumnsResult> GetDataProductsSearchableColumnsPostAsync(GetDataProductsSearchableColumnsRequest input) {
            var result = await _dataProductService.GetDataProductsSearchableColumnsAsync(input.ProductTypeFilter);

            return new SearchableColumnsResult {
                Items = result.ToList()
            };
        }

    }

}