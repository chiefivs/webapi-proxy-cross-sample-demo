#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Mvc;
#endif
#if NET46_OR_GREATER
using System.Web.Http;
#endif
using StandardProducts.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StandardProducts {

    public interface IProductCatalogApi
    {
        [HttpGet]
        [Route("api/products/list")]
        Task<IEnumerable<Product>> GetProductsList();

        [HttpPost]
        [Route("api/products/add")]
        Task AddProduct(Product product);

        [HttpDelete]
        [Route("api/products/delete")]
        Task DelProduct(int id);

    }
}
