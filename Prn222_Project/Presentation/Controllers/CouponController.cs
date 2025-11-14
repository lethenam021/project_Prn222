using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DataAccess.Models;
using BusinessLogic.Interface;
using Presentation.ViewModel;

namespace Presentation.Controllers {
    public class CouponController : Controller {
        private readonly ILogger<CouponController> _logger;
    }
}
