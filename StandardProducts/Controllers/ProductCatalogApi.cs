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

    public class ProductCatalogApiController : Controller {
        private readonly IProductCatalogController _implementation;

        public ProductCatalogApiController(IProductCatalogController productCatalogController) {
            _implementation = productCatalogController;
        }
        /// <summary>
        /// Returns a list of Product Spec Characteristic
        /// </summary>
        /// <remarks>Returns a list of Product Spec Characteristic</remarks>
        /// <param name="getPscListRequest"></param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request e.g. validation error shows data issues</response>
        /// <response code="415">Non-parseable request/response</response>
        /// <response code="500">Internal server error</response>
        /// <response code="503">Connection failure</response>
        /// <response code="504">External interface timed out</response>
        /// <response code="0">Unexpected error</response>
        [HttpPost]
        [Route("/api/StandardProducts/GetPscList")]
        [ValidateModelState]
        [SwaggerOperation("GetPscListPost")]
        [SwaggerResponse(statusCode: 200, type: typeof(PscListResult), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(ProblemDetails), description: "Bad Request e.g. validation error shows data issues")]
        [SwaggerResponse(statusCode: 415, type: typeof(ProblemDetails), description: "Non-parseable request/response")]
        [SwaggerResponse(statusCode: 500, type: typeof(ProblemDetails), description: "Internal server error")]
        [SwaggerResponse(statusCode: 503, type: typeof(ProblemDetails), description: "Connection failure")]
        [SwaggerResponse(statusCode: 504, type: typeof(ProblemDetails), description: "External interface timed out")]
        [SwaggerResponse(statusCode: 0, type: typeof(ProblemDetails), description: "Unexpected error")]
        public async Task<IActionResult> GetPscListPost([FromBody]GetPscListRequest getPscListRequest) {
            return new ObjectResult(await _implementation.GetPscListPostAsync(getPscListRequest));
        }

        /// <summary>
        /// Mark a value of Product Spec Characteristic as visible or unvisible in Product details view
        /// </summary>
        /// <remarks>Mark a value of Product Spec Characteristic as visible or unvisible in Product details view</remarks>
        /// <param name="setPscvVisibilityRequest"></param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request e.g. validation error shows data issues</response>
        /// <response code="415">Non-parseable request/response</response>
        /// <response code="500">Internal server error</response>
        /// <response code="503">Connection failure</response>
        /// <response code="504">External interface timed out</response>
        /// <response code="0">Unexpected error</response>
        [HttpPost]
        [Route("/api/StandardProducts/SetPscvVisibility")]
        [ValidateModelState]
        [SwaggerOperation("SetPscvVisibilityPost")]
        [SwaggerResponse(statusCode: 400, type: typeof(ProblemDetails), description: "Bad Request e.g. validation error shows data issues")]
        [SwaggerResponse(statusCode: 415, type: typeof(ProblemDetails), description: "Non-parseable request/response")]
        [SwaggerResponse(statusCode: 500, type: typeof(ProblemDetails), description: "Internal server error")]
        [SwaggerResponse(statusCode: 503, type: typeof(ProblemDetails), description: "Connection failure")]
        [SwaggerResponse(statusCode: 504, type: typeof(ProblemDetails), description: "External interface timed out")]
        [SwaggerResponse(statusCode: 0, type: typeof(ProblemDetails), description: "Unexpected error")]
        public async Task<IActionResult> SetPscvVisibilityPost([FromBody]SetPscvVisibilityRequest setPscvVisibilityRequest) {
            return new ObjectResult(await _implementation.SetPscvVisibilityPostAsync(setPscvVisibilityRequest));
        }
    }
}
