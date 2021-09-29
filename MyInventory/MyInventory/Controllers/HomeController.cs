using System.Web.Mvc;

namespace MyInventory.Controllers
{
    public class HomeController : Controller
    {
        // GET: Index
        public ActionResult Index()
        {
            //If the user is already logged in, take them to their warehouses
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Warehouse", new { area = "InventoryManagement" });

            //Otherwise, show them the home page
            return View();
        }

        // GET: About
        public ActionResult About()
        {
            return View();
        }

        // GET: Contact
        public ActionResult Contact()
        {
            return View();
        }
    }
}