using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLogic.Interface;
using Hangfire;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;
using Presentation.ViewModel.Data.Emails; // nếu email model ở Presentation


namespace Presentation.Controllers
{
    public class DisputeController : Controller
    {
        private readonly IDisputeService _disputeService;
        private readonly IEmailService _emailService; // email service của bạn
        private readonly IBackgroundJobClient _jobClient;

        public DisputeController(IDisputeService disputeService, IEmailService emailService, IBackgroundJobClient jobClient)
        {
            _disputeService = disputeService;
            _emailService = emailService;
            _jobClient = jobClient;
        }


        // GET: list
        [HttpGet]
        public async Task<IActionResult> ManageDispute()
        {
            int sellerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!); 
            var model = await _disputeService.GetDisputesForSellerAsync(sellerId);
            return View(model);
        }

        // GET: detail
        [HttpGet]
        public async Task<IActionResult> DisputeDetail(int id)
        {
            int sellerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var model = await _disputeService.GetDisputeDetailAsync(id, sellerId);
            if (model == null) return NotFound();
            return View(model);
        }

        // POST: Accept
        [HttpPost]
        public async Task<IActionResult> Accept(int id, string refundType, string emailBody)
        {
            // 1. Update DB
            await _disputeService.AcceptAsync(id, refundType);

            // 2. Lấy lại detail để có email buyer
            int sellerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var dispute = await _disputeService.GetDisputeDetailAsync(id, sellerId);
            if (dispute != null && !string.IsNullOrEmpty(dispute.DisputerEmail))
            {
                var emailModel = new DisputeEmailViewModel
                {
                    BuyerName = dispute.DisputerName ?? "",
                    OrderId = dispute.OrderId,
                    Reason = emailBody      // nội dung bạn nhập ở popup
                };

                string htmlBody = await this.RenderViewAsync(
            "~/Views/Shared/EmailTemplates/Dispute/_AcceptEmail",
            emailModel,
            true
        );

                _jobClient.Enqueue<IEmailService>(
                   service => service.SendEmailAsync(dispute.DisputerEmail!, "Respond to disputes", htmlBody)
               );
            }

            return RedirectToAction(nameof(ManageDispute));
        }

        // POST: Reject
        [HttpPost]
        public async Task<IActionResult> Reject(int id, string reason, string emailBody)
        {
            // 1. Update DB
            await _disputeService.RejectAsync(id, reason);

            // 2. Gửi email
            int sellerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var dispute = await _disputeService.GetDisputeDetailAsync(id, sellerId);
            if (dispute != null && !string.IsNullOrEmpty(dispute.DisputerEmail))
            {
                var emailModel = new DisputeEmailViewModel
                {
                    BuyerName = dispute.DisputerName ?? "",
                    OrderId = dispute.OrderId,
                    Reason = emailBody     
                };

                string htmlBody = await this.RenderViewAsync(
"~/Views/Shared/EmailTemplates/Dispute/_RejectEmail",
emailModel,
true
);

                _jobClient.Enqueue<IEmailService>(
                   service => service.SendEmailAsync(dispute.DisputerEmail!, "Respond to disputes", htmlBody)
               );
            }

            return RedirectToAction(nameof(ManageDispute));
        }
    }
}
