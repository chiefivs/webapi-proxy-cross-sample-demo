using System.Threading.Tasks;
using StandardProducts.Filters;
using StandardProducts.Models;
using StandardProducts.Services;

namespace StandardProducts.Controllers {

    public interface IProductOverviewController {
        /// <summary>
        /// <see cref="ProductOverviewApiController.GetProductsOverviewPost"/>
        /// </summary>
        Task<ProductsOverviewResult> GetProductsOverviewPostAsync(GetDataProductsOverviewRequest input);
    }

    public class ProductOverviewControllerImpl : IProductOverviewController {
        private readonly IProductOverviewService _productOverviewService;
        private readonly IKumsFilter _kumsFilter;

        public ProductOverviewControllerImpl(IKumsFilter kumsFilter, IProductOverviewService productOverviewService) {
            _kumsFilter = kumsFilter;
            _productOverviewService = productOverviewService;
        }

        /// <inheritdoc />
        public async Task<ProductsOverviewResult> GetProductsOverviewPostAsync(GetDataProductsOverviewRequest input) {
            _kumsFilter.ValidateListOfKums(input.KumsIds);
            var items = await _productOverviewService.GetProductsOverviewAsync(input.KumsIds);

            var result = new ProductsOverviewResult {
                ProductOverviews = items
            };
            return result;
        }
    }

}