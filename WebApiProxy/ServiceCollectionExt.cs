#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using WebApiProxy.Proxy;
using System;
using System.Net.Http;

namespace WebApiProxy
{
    /// <summary>
    /// Web api services extension
    /// </summary>
    public static class ServiceCollectionExt
    {
        /// <summary>
        /// Register a web api client (proxy) for interface 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddWebApiEndpoints(this IServiceCollection services, params WebApiEndpoint[] options)
        {
            services.AddHttpContextAccessor();
            services.AddTransient<HttpContext>(provider =>
            {
                var accessor = provider.GetService<IHttpContextAccessor>();
                return accessor.HttpContext;
            });

            var factory = WebApiProxyFactory.Get();
            foreach(var client in options)
            {
                services.AddTransient(client.Type, provider =>
                {
                    var proxy = factory.CreateDynamicProxy(client.Type) as Proxy.WebApiProxy;
                    proxy.Endpoint = client.BaseUri;
                    proxy.Options = client.Options;
                    proxy.HttpClient = provider.GetService<HttpClient>() ?? new HttpClient();
                    proxy.HttpContext = provider.GetService<HttpContext>();
                    proxy.CorrelationTokenFactory = provider.GetService<CorrelationTokenFactory>();

                    return proxy;
                });
            }

            return services;
        }

        /// <summary>
        /// If you are going to use correlation tokens, you should register it before
        /// </summary>
        /// <param name="services"></param>
        /// <param name="correlationTokenName">Not required, by default "Correlation-Token"</param>
        /// <returns></returns>
        public static IServiceCollection AddCorrelationToken(this IServiceCollection services, string correlationTokenName = null)
        {
            services.AddTransient<CorrelationTokenFactory>(provider =>
            {
                var context = provider.GetService<HttpContext>();
                return new CorrelationTokenFactory(context, correlationTokenName);
            });

            return services;
        }
    }
}
#endif
