using System;
using System.Linq;
using System.Web;

namespace WebApiProxy
{
    internal class CorrelationTokenFactory
    {
        public readonly string CorrelationTokenHeaderName;
        
        private HttpContext _httpContext;

        public CorrelationTokenFactory(HttpContext httpContext, string name)
        {
            _httpContext = httpContext;
            CorrelationTokenHeaderName = name;
        }

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
    }
}
