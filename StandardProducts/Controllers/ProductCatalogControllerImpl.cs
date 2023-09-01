using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StandardProducts.Models;

namespace StandardProducts.Controllers {
    public class ProductCatalogControllerImpl : IProductCatalogApi {
        private static List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "Olives", Price = 1.2 },
            new Product { Id = 1, Name = "Oysters", Price = 10.25 },
            new Product { Id = 1, Name = "Squids", Price = 5.12 },
            new Product { Id = 1, Name = "Prawns", Price = 1.34 },
            new Product { Id = 1, Name = "Peanuts", Price = 3.2 }
        };

       public async Task<IEnumerable<Product>> GetProductsList()
       {
           return _products.OrderBy(x => x.Id);
       }

        public async Task AddProduct(Product product)
        {
            var lastid = _products.Any() ? _products.Max(x => x.Id) : 0;
            product.Id = lastid + 1;

            _products.Add(product);
        }

        public async Task DelProduct(int id)
        {
            var product = _products.FirstOrDefault(x => x.Id == id);
            if (product != null)
                _products.Remove(product);
        }
    }

}