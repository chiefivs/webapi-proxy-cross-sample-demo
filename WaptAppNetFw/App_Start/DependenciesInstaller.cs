using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using WaptAppNetFw.Controllers;
using StandardProducts;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using WebApiProxy;

namespace WaptAppNetFw
{
    public class DependenciesInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<HttpClient>().UsingFactoryMethod(() =>
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Context", "{\"System\":\"MA1B\"}");
                client.DefaultRequestHeaders.Add("Accept-Language", "en");//Thread.CurrentThread.CurrentUICulture.Name.Substring(0, 2));
                return client;
            }).LifestyleTransient());

            var uri = new Uri("http://localhost:8000");
            container.RegisterWebApiEndpoints(
                new WebApiEndpoint<IProductCatalogApi>(uri)
            );
        }
    }
}