#if NET46_OR_GREATER
using System.Net.Http;
using System.Web;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using WebApiProxy.Proxy;

namespace WebApiProxy
{
    public static class WindsorContainerExt
    {
        public static void RegisterWebApiEndpoints(this IWindsorContainer container, params WebApiEndpoint[] options)
        {
            container.Register(Component.For<HttpContext>().UsingFactoryMethod(() => HttpContext.Current).LifestyleTransient());

            var factory = new WebApiProxyFactory();
            foreach (var option in options)
            {
                container.Register(Component.For(option.Type).UsingFactoryMethod(() =>
                {
                    HttpClient client;
                    try
                    {
                        client = container.Resolve<HttpClient>();
                    }
                    catch
                    {
                        client = new HttpClient();
                    }

                    CorrelationTokenFactory tokenFactory;
                    try
                    {
                        tokenFactory = container.Resolve<CorrelationTokenFactory>();
                    }
                    catch
                    {
                        tokenFactory = null;
                    }

                    var context = container.Resolve<HttpContext>();
                    //var endpoint = new WebApiEndpoint<IAddressesApi>(new Uri("http://localhost:8000"));
                    var proxy = factory.CreateDynamicProxy(option.Type) as Proxy.WebApiProxy;
                    proxy.Endpoint = option.BaseUri;
                    proxy.Options = option.Options;
                    proxy.HttpClient = client;
                    proxy.HttpContext = context;
                    proxy.CorrelationTokenFactory = tokenFactory;

                    return proxy;
                }).LifestyleTransient());

            }


        }

        public static void RegisterCorrelationToken(this IWindsorContainer container, string correlationTokenName = null)
        {
            container.Register(Component.For<CorrelationTokenFactory>().UsingFactoryMethod(() =>
            {
                var context = container.Resolve<HttpContext>();
                return new CorrelationTokenFactory(context, correlationTokenName);
            }).LifestyleTransient());
        }
    }
}
#endif