using WebApiProxy.Manager.ProxyGenerator;

namespace WebApiProxy.Proxy
{
    internal class WebApiProxyFactory: DynamicProxyFactory<WebApiProxy>
    {
        private static WebApiProxyFactory _instance;

        public WebApiProxyFactory() : base(new DynamicInterfaceImplementor())
        {

        }

        public static WebApiProxyFactory Get()
        {
            return _instance ?? (_instance = new WebApiProxyFactory());
        }
    }
}
