#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Http;
#endif
using System;
using System.Linq;
#if NET46_OR_GREATER
using System.Web;
#endif

namespace WebApiProxy
{
    internal class CorrelationTokenFactory
    {
        public readonly string CorrelationTokenHeaderName;
        private HttpContext _httpContext;

        public CorrelationTokenFactory(HttpContext httpContext, string name)
        {
            _httpContext = httpContext;
            CorrelationTokenHeaderName = name ?? "Correlation-Token";
        }

        #if NET5_0_OR_GREATER
        public string GetCorrelationToken()
        {
            var token = _httpContext?.Request?.Headers
                .GetCommaSeparatedValues(CorrelationTokenHeaderName)
                .FirstOrDefault();

            if(token == null)
                token = _httpContext?.Response?.Headers
                    .GetCommaSeparatedValues(CorrelationTokenHeaderName)
                    .FirstOrDefault();

            return string.IsNullOrEmpty(token) ? Guid.NewGuid().ToString() : token;
        }
        #endif
        #if NET46_OR_GREATER
        public string GetCorrelationToken()
        {
            var token = GetCommaSeparatedValues(_httpContext?.Request?.Headers.Get(CorrelationTokenHeaderName))
                .FirstOrDefault();

            if(token == null)
                token = GetCommaSeparatedValues(_httpContext?.Response?.Headers.Get(CorrelationTokenHeaderName))
                    .FirstOrDefault();

            return string.IsNullOrEmpty(token) ? Guid.NewGuid().ToString() : token;
        }

        private string[] GetCommaSeparatedValues(string value)
        {
            return (value ?? "")
                .Split(new[] {',', ';'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(i => i.Trim())
                .ToArray();
        }
        #endif
    }
}
