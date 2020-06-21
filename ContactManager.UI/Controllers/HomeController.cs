using System.Web.Mvc;

namespace Contactservice.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Contact Manager";
            return View();
        }
    }
}