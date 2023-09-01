using StandardProducts.Models;
using StandardProducts.Services;
using System;
using System.Threading.Tasks;
using StandardProducts.Extensions;

namespace StandardProducts.Controllers {

    public interface IProductDocumentController {
        /// <summary>
        /// <see cref="ProductDocumentApiController.GetProductDocumentFilePost"/>
        /// </summary>
        Task<byte[]> GetProductDocumentFilePostAsync(GetProductDocumentFileRequest input);

        /// <summary>
        /// <see cref="ProductDocumentApiController.GetAllProductsDocumentsPost"/>
        /// </summary>
        Task<AllProductsDocumentsResult> GetAllProductsDocumentsPostAsync(GetAllProductsDocumentsRequest input);
    }

    public class ProductDocumentControllerImpl : IProductDocumentController {
        private readonly IProductDocumentService _productDocumentService;

        public ProductDocumentControllerImpl(IProductDocumentService productDocumentService) {
            _productDocumentService = productDocumentService;
        }

        /// <inheritdoc />
        public async Task<byte[]> GetProductDocumentFilePostAsync(GetProductDocumentFileRequest input) {
            if (!await _productDocumentService.ProductHasDocumentAsync(input.ProductGroups)) {
                throw new Exception($"No documents found for product groups {input.ProductGroups.KumsIdsAsString()};"
                                    + $" file name {input.FileName}");
            }

            var fileData = await _productDocumentService.GetProductDocumentFileAsync(input.FileName);
            return fileData;
        }

        /// <inheritdoc />
        public async Task<AllProductsDocumentsResult> GetAllProductsDocumentsPostAsync(GetAllProductsDocumentsRequest input) {
            var response = await _productDocumentService.GetAllProductsDocumentsAsync(input.Level3ProductsGroups, input.KumsIdFilters);

            var result = new AllProductsDocumentsResult {
                Items = response
            };
            return result;
        }
    }

}