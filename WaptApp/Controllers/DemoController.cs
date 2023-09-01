using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using StandardProducts;
using StandardProducts.Models;

namespace WaptApp.Controllers
{
    [ApiController]
    public class DemoController : ControllerBase
    {
        private readonly IProductCatalogApi _productsApi;

        public DemoController(IProductCatalogApi productsApi)
        {
            _productsApi = productsApi;
        }

        [HttpGet]
        [Route("demo/getProductsList")]
        public async Task<IEnumerable<Product>> GetProductsList()
        {
            return await _productsApi.GetProductsList();
        }
    }
}
