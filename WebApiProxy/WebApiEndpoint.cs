using System;
using System.Collections;
using System.Collections.Generic;

namespace WebApiProxy
{
    /// <summary>
    /// Endpoint configuration
    /// </summary>
    public class WebApiEndpoint
    {
        internal Type Type;

        internal Uri BaseUri;

        internal WebApiEndpointOptions Options;

        /// <summary>
        /// Endpoint parameters for interface
        /// </summary>
        /// <param name="type">Interface type</param>
        /// <param name="baseUri">Base uri for endpoint</param>
        /// <param name="options">Not required. By default Content-Type is 'application/json'</param>
        public WebApiEndpoint(Type type, Uri baseUri, WebApiEndpointOptions options = null)
        {
            BaseUri = baseUri;
            Type = type;
            Options = options ?? new WebApiEndpointOptions();
        }
    }

    /// <summary>
    /// Web api endpoint parameters
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WebApiEndpoint<T> : WebApiEndpoint
    {
        /// <summary>
        /// Endpoint parameters for interface by type
        /// </summary>
        /// <param name="baseUri">Base uri for endpoint</param>
        /// <param name="options">Not required. By default Content-Type is 'application/json'</param>
        public WebApiEndpoint(Uri baseUri, WebApiEndpointOptions options = null) : base(typeof(T), baseUri, options)
        {}
    }

    /// <summary>
    /// Additional options for web api endpoint
    /// </summary>
    public class WebApiEndpointOptions
    {
        /// <summary>
        /// Content type
        /// </summary>
        public string ContentType { get; set; } = ContentTypes.Json;

        public IEnumerable<string> TransferHeaders { get; set; } 
    }
}
