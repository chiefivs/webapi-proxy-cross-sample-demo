/*
 * Customer Locations and Products API of MyEnterprise
 *
 * Reads customer locations and products
 *
 * The version of the OpenAPI document: 1.0.0
 * 
 * Generated by: https://openapi-generator.tech
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StandardProducts.Attributes;
using StandardProducts.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ProblemDetails = StandardProducts.Models.ProblemDetails;

namespace StandardProducts.Controllers {

    public class ProductDocumentApiController : Controller {
        private readonly IProductDocumentController _implementation;

        public ProductDocumentApiController(IProductDocumentController productDocumentController) {
            _implementation = productDocumentController;
        }
        /// <summary>
        /// Get documents config info for all products
        /// </summary>
        /// <remarks>The endpoint returns the list of products documents config info</remarks>
        /// <param name="getAllProductsDocumentsRequest"></param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request e.g. validation error shows data issues</response>
        /// <response code="415">Non-parseable request/response</response>
        /// <response code="500">Internal server error</response>
        /// <response code="503">Connection failure</response>
        /// <response code="504">External interface timed out</response>
        /// <response code="0">Unexpected error</response>
        [HttpPost]
        [Route("/api/StandardProducts/GetAllProductsDocuments")]
        [ValidateModelState]
        [SwaggerOperation("GetAllProductsDocumentsPost")]
        [SwaggerResponse(statusCode: 200, type: typeof(AllProductsDocumentsResult), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(ProblemDetails), description: "Bad Request e.g. validation error shows data issues")]
        [SwaggerResponse(statusCode: 415, type: typeof(ProblemDetails), description: "Non-parseable request/response")]
        [SwaggerResponse(statusCode: 500, type: typeof(ProblemDetails), description: "Internal server error")]
        [SwaggerResponse(statusCode: 503, type: typeof(ProblemDetails), description: "Connection failure")]
        [SwaggerResponse(statusCode: 504, type: typeof(ProblemDetails), description: "External interface timed out")]
        [SwaggerResponse(statusCode: 0, type: typeof(ProblemDetails), description: "Unexpected error")]
        public async Task<IActionResult> GetAllProductsDocumentsPost([FromBody]GetAllProductsDocumentsRequest getAllProductsDocumentsRequest) {
            return new ObjectResult(await _implementation.GetAllProductsDocumentsPostAsync(getAllProductsDocumentsRequest));
        }

        /// <summary>
        /// Get product document file content
        /// </summary>
        /// <remarks>returns file</remarks>
        /// <param name="getProductDocumentFileRequest"></param>
        /// <response code="200">A product document file as byte array.</response>
        /// <response code="400">Bad Request e.g. validation error shows data issues</response>
        /// <response code="415">Non-parseable request/response</response>
        /// <response code="500">Internal server error</response>
        /// <response code="503">Connection failure</response>
        /// <response code="504">External interface timed out</response>
        /// <response code="0">Unexpected error</response>
        [HttpPost]
        [Route("/api/StandardProducts/GetProductDocumentFile")]
        [ValidateModelState]
        [SwaggerOperation("GetProductDocumentFilePost")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "A product document file as byte array.")]
        [SwaggerResponse(statusCode: 400, type: typeof(ProblemDetails), description: "Bad Request e.g. validation error shows data issues")]
        [SwaggerResponse(statusCode: 415, type: typeof(ProblemDetails), description: "Non-parseable request/response")]
        [SwaggerResponse(statusCode: 500, type: typeof(ProblemDetails), description: "Internal server error")]
        [SwaggerResponse(statusCode: 503, type: typeof(ProblemDetails), description: "Connection failure")]
        [SwaggerResponse(statusCode: 504, type: typeof(ProblemDetails), description: "External interface timed out")]
        [SwaggerResponse(statusCode: 0, type: typeof(ProblemDetails), description: "Unexpected error")]
        public async Task<IActionResult> GetProductDocumentFilePost([FromBody]GetProductDocumentFileRequest getProductDocumentFileRequest) {
            return new ObjectResult(await _implementation.GetProductDocumentFilePostAsync(getProductDocumentFileRequest));
        }
    }
}
