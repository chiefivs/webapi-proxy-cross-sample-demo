using WebApiProxy.Manager.ProxyGenerator;

namespace WebApiProxy.Proxy
{
    internal class WebApiProxyFactory: DynamicProxyFactory<WebApiProxy>
    {
        public WebApiProxyFactory() : base(new DynamicInterfaceImplementor())
        {

        }

    }
}
