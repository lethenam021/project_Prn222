using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers {
    public class VoucherController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
