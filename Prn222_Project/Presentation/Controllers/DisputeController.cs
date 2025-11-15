using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;
using BusinessLogic.Interface;
using Hangfire;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;
using Presentation.ViewModel.Data.Emails;

namespace Presentation.Controllers
{
    [Authorize(Roles = "Seller")]
    public class DisputeController : Controller
    {
        private readonly IDisputeService _disputeService;
        private readonly IEmailService _emailService;
        private readonly IBackgroundJobClient _jobClient;

        public DisputeController(
            IDisputeService disputeService,
            IEmailService emailService,
            IBackgroundJobClient jobClient)
        {
            _disputeService = disputeService;
            _emailService = emailService;
            _jobClient = jobClient;
        }

        // GET: list + search
        [HttpGet]
        public async Task<IActionResult> ManageDispute(string? disputer, string? product)
        {
            int sellerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Lấy tất cả dispute của seller
            var disputes = await _disputeService.GetDisputesForSellerAsync(sellerId);

            // Filter theo Disputer
            if (!string.IsNullOrWhiteSpace(disputer))
            {
                disputes = disputes.Where(d =>
                    !string.IsNullOrEmpty(d.DisputerName) &&
                    d.DisputerName.Contains(disputer, StringComparison.OrdinalIgnoreCase));
            }

            // Filter theo Product
            if (!string.IsNullOrWhiteSpace(product))
            {
                disputes = disputes.Where(d =>
                    !string.IsNullOrEmpty(d.ProductTitles) &&
                    d.ProductTitles.Contains(product, StringComparison.OrdinalIgnoreCase));
            }

            // Để giữ lại giá trị trên view
            ViewBag.Disputer = disputer;
            ViewBag.Product = product;

            return View(disputes.ToList());
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
            await _disputeService.AcceptAsync(id, refundType);

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
            await _disputeService.RejectAsync(id, reason);

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
