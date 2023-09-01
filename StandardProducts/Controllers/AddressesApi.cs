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

    public class AddressesApiController : Controller {
        private readonly IAddressesController _implementation;

        public AddressesApiController(IAddressesController addressesController) {
            _implementation = addressesController;
        }
        /// <summary>
        /// Addresses count for KUMS
        /// </summary>
        /// <remarks>The Addresses count endpoint returns the count of locations for certain customer KUMS number</remarks>
        /// <param name="addressesCountRequest"></param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request e.g. validation error shows data issues</response>
        /// <response code="415">Non-parseable request/response</response>
        /// <response code="500">Internal server error</response>
        /// <response code="503">Connection failure</response>
        /// <response code="504">External interface timed out</response>
        /// <response code="0">Unexpected error</response>
        [HttpPost]
        [Route("/api/StandardProducts/AddressesCount")]
        [ValidateModelState]
        [SwaggerOperation("AddressesCountPost")]
        [SwaggerResponse(statusCode: 200, type: typeof(AddressesCountResult), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(ProblemDetails), description: "Bad Request e.g. validation error shows data issues")]
        [SwaggerResponse(statusCode: 415, type: typeof(ProblemDetails), description: "Non-parseable request/response")]
        [SwaggerResponse(statusCode: 500, type: typeof(ProblemDetails), description: "Internal server error")]
        [SwaggerResponse(statusCode: 503, type: typeof(ProblemDetails), description: "Connection failure")]
        [SwaggerResponse(statusCode: 504, type: typeof(ProblemDetails), description: "External interface timed out")]
        [SwaggerResponse(statusCode: 0, type: typeof(ProblemDetails), description: "Unexpected error")]
        public async Task<IActionResult> AddressesCountPost([FromBody]AddressesCountRequest addressesCountRequest) {
            return new ObjectResult(await _implementation.AddressesCountPostAsync(addressesCountRequest));
        }

        /// <summary>
        /// Get list of Locations, where current location is location A or location B
        /// </summary>
        /// <remarks>Get list of Locations, where current location is location A or location B</remarks>
        /// <param name="getAddressesDetailsRequest"></param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request e.g. validation error shows data issues</response>
        /// <response code="415">Non-parseable request/response</response>
        /// <response code="500">Internal server error</response>
        /// <response code="503">Connection failure</response>
        /// <response code="504">External interface timed out</response>
        /// <response code="0">Unexpected error</response>
        [HttpPost]
        [Route("/api/StandardProducts/GetAddressDetails")]
        [ValidateModelState]
        [SwaggerOperation("GetAddressDetailsPost")]
        [SwaggerResponse(statusCode: 200, type: typeof(AddressDetailsResult), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(ProblemDetails), description: "Bad Request e.g. validation error shows data issues")]
        [SwaggerResponse(statusCode: 415, type: typeof(ProblemDetails), description: "Non-parseable request/response")]
        [SwaggerResponse(statusCode: 500, type: typeof(ProblemDetails), description: "Internal server error")]
        [SwaggerResponse(statusCode: 503, type: typeof(ProblemDetails), description: "Connection failure")]
        [SwaggerResponse(statusCode: 504, type: typeof(ProblemDetails), description: "External interface timed out")]
        [SwaggerResponse(statusCode: 0, type: typeof(ProblemDetails), description: "Unexpected error")]
        public async Task<IActionResult> GetAddressDetailsPost([FromBody]GetAddressesDetailsRequest getAddressesDetailsRequest) {
            return new ObjectResult(await _implementation.GetAddressDetailsPostAsync(getAddressesDetailsRequest));
        }

        /// <summary>
        /// Returns all searchable columns for Address.
        /// </summary>
        /// <remarks>The endpoint returns a list of searchable Address columns with translation and validators</remarks>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request e.g. validation error shows data issues</response>
        /// <response code="415">Non-parseable request/response</response>
        /// <response code="500">Internal server error</response>
        /// <response code="503">Connection failure</response>
        /// <response code="504">External interface timed out</response>
        /// <response code="0">Unexpected error</response>
        [HttpPost]
        [Route("/api/StandardProducts/GetAddressSearchableColumns")]
        [ValidateModelState]
        [SwaggerOperation("GetAddressSearchableColumnsPost")]
        [SwaggerResponse(statusCode: 200, type: typeof(SearchableColumnsResult), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(ProblemDetails), description: "Bad Request e.g. validation error shows data issues")]
        [SwaggerResponse(statusCode: 415, type: typeof(ProblemDetails), description: "Non-parseable request/response")]
        [SwaggerResponse(statusCode: 500, type: typeof(ProblemDetails), description: "Internal server error")]
        [SwaggerResponse(statusCode: 503, type: typeof(ProblemDetails), description: "Connection failure")]
        [SwaggerResponse(statusCode: 504, type: typeof(ProblemDetails), description: "External interface timed out")]
        [SwaggerResponse(statusCode: 0, type: typeof(ProblemDetails), description: "Unexpected error")]
        public async Task<IActionResult> GetAddressSearchableColumnsPost() {
            return new ObjectResult(await _implementation.GetAddressSearchableColumnsPostAsync());
        }

        /// <summary>
        /// Get list of wholesale Locations for specific Kums Ids
        /// </summary>
        /// <remarks>Get list of wholesale Locations for specific Kums Ids</remarks>
        /// <param name="getAddressesByKumsListRequest"></param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request e.g. validation error shows data issues</response>
        /// <response code="415">Non-parseable request/response</response>
        /// <response code="500">Internal server error</response>
        /// <response code="503">Connection failure</response>
        /// <response code="504">External interface timed out</response>
        /// <response code="0">Unexpected error</response>
        [HttpPost]
        [Route("/api/StandardProducts/GetAddressesByKumsList")]
        [ValidateModelState]
        [SwaggerOperation("GetAddressesByKumsListPost")]
        [SwaggerResponse(statusCode: 200, type: typeof(GetAddressesResult), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(ProblemDetails), description: "Bad Request e.g. validation error shows data issues")]
        [SwaggerResponse(statusCode: 415, type: typeof(ProblemDetails), description: "Non-parseable request/response")]
        [SwaggerResponse(statusCode: 500, type: typeof(ProblemDetails), description: "Internal server error")]
        [SwaggerResponse(statusCode: 503, type: typeof(ProblemDetails), description: "Connection failure")]
        [SwaggerResponse(statusCode: 504, type: typeof(ProblemDetails), description: "External interface timed out")]
        [SwaggerResponse(statusCode: 0, type: typeof(ProblemDetails), description: "Unexpected error")]
        public async Task<IActionResult> GetAddressesByKumsListPost([FromBody]GetAddressesByKumsListRequest getAddressesByKumsListRequest) {
            return new ObjectResult(await _implementation.GetAddressesByKumsListPostAsync(getAddressesByKumsListRequest));
        }
    }
}
