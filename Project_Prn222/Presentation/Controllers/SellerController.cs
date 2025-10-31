using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class SellerController : Controller
    {
        public IActionResult Store()
        {
            return View();
        }
        public IActionResult ManagerProduct()
        {
            return View();
        }
        public IActionResult NewProduct()
        {
            return View();
        }
    }
}
