using System.Threading.Tasks;
using System.Web.Mvc;
using StandardProducts;

namespace WaptAppNetFw.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductCatalogApi _productsApi;

        public HomeController(IProductCatalogApi productsApi)
        {
            _productsApi = productsApi;
        }

        public async Task<ActionResult> Index()
        {
            var model = await _productsApi.GetProductsList();
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}