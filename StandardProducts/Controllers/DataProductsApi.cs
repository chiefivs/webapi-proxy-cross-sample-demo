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

    public class DataProductsApiController : Controller {
        private readonly IDataProductsController _implementation;

        public DataProductsApiController(IDataProductsController dataProductsController) {
            _implementation = dataProductsController;
        }
        /// <summary>
        /// Get data product details by KumsId and LineNumber
        /// </summary>
        /// <remarks>Get data product details by KumsId and LineNumber</remarks>
        /// <param name="getDataProductDetailsRequest"></param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request e.g. validation error shows data issues</response>
        /// <response code="415">Non-parseable request/response</response>
        /// <response code="500">Internal server error</response>
        /// <response code="503">Connection failure</response>
        /// <response code="504">External interface timed out</response>
        /// <response code="0">Unexpected error</response>
        [HttpPost]
        [Route("/api/StandardProducts/GetDataProductDetails")]
        [ValidateModelState]
        [SwaggerOperation("GetDataProductDetailsPost")]
        [SwaggerResponse(statusCode: 200, type: typeof(DataProduct), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(ProblemDetails), description: "Bad Request e.g. validation error shows data issues")]
        [SwaggerResponse(statusCode: 415, type: typeof(ProblemDetails), description: "Non-parseable request/response")]
        [SwaggerResponse(statusCode: 500, type: typeof(ProblemDetails), description: "Internal server error")]
        [SwaggerResponse(statusCode: 503, type: typeof(ProblemDetails), description: "Connection failure")]
        [SwaggerResponse(statusCode: 504, type: typeof(ProblemDetails), description: "External interface timed out")]
        [SwaggerResponse(statusCode: 0, type: typeof(ProblemDetails), description: "Unexpected error")]
        public async Task<IActionResult> GetDataProductDetailsPost([FromBody]GetDataProductDetailsRequest getDataProductDetailsRequest) {
            return new ObjectResult(await _implementation.GetDataProductDetailsPostAsync(getDataProductDetailsRequest));
        }

        /// <summary>
        /// Get count of DataProducts (MPLS or/and Etherlink)
        /// </summary>
        /// <remarks>Get count of DataProducts (MPLS or/and Etherlink). Allow searching</remarks>
        /// <param name="getDataProductsCountRequest"></param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request e.g. validation error shows data issues</response>
        /// <response code="415">Non-parseable request/response</response>
        /// <response code="500">Internal server error</response>
        /// <response code="503">Connection failure</response>
        /// <response code="504">External interface timed out</response>
        /// <response code="0">Unexpected error</response>
        [HttpPost]
        [Route("/api/StandardProducts/GetDataProductsCount")]
        [ValidateModelState]
        [SwaggerOperation("GetDataProductsCountPost")]
        [SwaggerResponse(statusCode: 200, type: typeof(DataProductsCountResult), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(ProblemDetails), description: "Bad Request e.g. validation error shows data issues")]
        [SwaggerResponse(statusCode: 415, type: typeof(ProblemDetails), description: "Non-parseable request/response")]
        [SwaggerResponse(statusCode: 500, type: typeof(ProblemDetails), description: "Internal server error")]
        [SwaggerResponse(statusCode: 503, type: typeof(ProblemDetails), description: "Connection failure")]
        [SwaggerResponse(statusCode: 504, type: typeof(ProblemDetails), description: "External interface timed out")]
        [SwaggerResponse(statusCode: 0, type: typeof(ProblemDetails), description: "Unexpected error")]
        public async Task<IActionResult> GetDataProductsCountPost([FromBody]GetDataProductsCountRequest getDataProductsCountRequest) {
            return new ObjectResult(await _implementation.GetDataProductsCountPostAsync(getDataProductsCountRequest));
        }

        /// <summary>
        /// Get list of DataProducts (MPLS or/and Etherlink)
        /// </summary>
        /// <remarks>Get sorted, paged list of DataProducts (MPLS or/and Etherlink). Allow searching</remarks>
        /// <param name="getDataProductsRequest"></param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request e.g. validation error shows data issues</response>
        /// <response code="415">Non-parseable request/response</response>
        /// <response code="500">Internal server error</response>
        /// <response code="503">Connection failure</response>
        /// <response code="504">External interface timed out</response>
        /// <response code="0">Unexpected error</response>
        [HttpPost]
        [Route("/api/StandardProducts/GetDataProducts")]
        [ValidateModelState]
        [SwaggerOperation("GetDataProductsPost")]
        [SwaggerResponse(statusCode: 200, type: typeof(DataProductsResult), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(ProblemDetails), description: "Bad Request e.g. validation error shows data issues")]
        [SwaggerResponse(statusCode: 415, type: typeof(ProblemDetails), description: "Non-parseable request/response")]
        [SwaggerResponse(statusCode: 500, type: typeof(ProblemDetails), description: "Internal server error")]
        [SwaggerResponse(statusCode: 503, type: typeof(ProblemDetails), description: "Connection failure")]
        [SwaggerResponse(statusCode: 504, type: typeof(ProblemDetails), description: "External interface timed out")]
        [SwaggerResponse(statusCode: 0, type: typeof(ProblemDetails), description: "Unexpected error")]
        public async Task<IActionResult> GetDataProductsPost([FromBody]GetDataProductsRequest getDataProductsRequest) {
            return new ObjectResult(await _implementation.GetDataProductsPostAsync(getDataProductsRequest));
        }

        /// <summary>
        /// Returns all searchable columns for DataProducts.
        /// </summary>
        /// <remarks>The endpoint returns a list of searchable DataProducts columns with translation and validators</remarks>
        /// <param name="getDataProductsSearchableColumnsRequest"></param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request e.g. validation error shows data issues</response>
        /// <response code="415">Non-parseable request/response</response>
        /// <response code="500">Internal server error</response>
        /// <response code="503">Connection failure</response>
        /// <response code="504">External interface timed out</response>
        /// <response code="0">Unexpected error</response>
        [HttpPost]
        [Route("/api/StandardProducts/GetDataProductsSearchableColumns")]
        [ValidateModelState]
        [SwaggerOperation("GetDataProductsSearchableColumnsPost")]
        [SwaggerResponse(statusCode: 200, type: typeof(SearchableColumnsResult), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(ProblemDetails), description: "Bad Request e.g. validation error shows data issues")]
        [SwaggerResponse(statusCode: 415, type: typeof(ProblemDetails), description: "Non-parseable request/response")]
        [SwaggerResponse(statusCode: 500, type: typeof(ProblemDetails), description: "Internal server error")]
        [SwaggerResponse(statusCode: 503, type: typeof(ProblemDetails), description: "Connection failure")]
        [SwaggerResponse(statusCode: 504, type: typeof(ProblemDetails), description: "External interface timed out")]
        [SwaggerResponse(statusCode: 0, type: typeof(ProblemDetails), description: "Unexpected error")]
        public async Task<IActionResult> GetDataProductsSearchableColumnsPost([FromBody]GetDataProductsSearchableColumnsRequest getDataProductsSearchableColumnsRequest) {
            return new ObjectResult(await _implementation.GetDataProductsSearchableColumnsPostAsync(getDataProductsSearchableColumnsRequest));
        }
    }
}
