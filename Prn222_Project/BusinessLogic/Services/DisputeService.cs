// BusinessLogic/Services/DisputeService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.DTOs.Response.Dispute;
using BusinessLogic.Interface;
using Common.Enums;
using DataAccess.IRepo;

namespace BusinessLogic.Services
{
    public class DisputeService : IDisputeService
    {
        private readonly IDisputeRepo _disputeRepo;

        public DisputeService(IDisputeRepo disputeRepo)
        {
            _disputeRepo = disputeRepo;
        }

        public async Task<IEnumerable<DisputeResponse>> GetDisputesForSellerAsync(int sellerId)
        {
            var disputes = await _disputeRepo.GetDisputesForSellerAsync(sellerId);

            return disputes.Select(d => new DisputeResponse
            {
                Id = d.Id,
                OrderId = d.OrderId ?? 0,

                DisputerId = d.RaisedBy ?? 0,
                DisputerName = d.RaisedByNavigation?.Username,
                DisputerEmail = d.RaisedByNavigation?.Email,

                OrderDate = d.Order?.OrderDate,
                TotalPrice = d.Order?.TotalPrice,

                ProductTitles = d.Order != null
                    ? string.Join(", ",
                        d.Order.OrderItems
                            .Where(oi => oi.Product != null)
                            .Select(oi => oi.Product!.Title))
                    : string.Empty,

                Description = d.Description,
                Status = d.Status ?? DisputeStatus.Pending.ToString(),
                Resolution = d.Resolution
            }).ToList();
        }

        public async Task<DisputeResponse?> GetDisputeDetailAsync(int disputeId, int sellerId)
        {
            var dispute = await _disputeRepo.GetByIdAsync(disputeId);
            if (dispute == null) return null;

            // optional: check dispute có thuộc seller này không
            var belongsToSeller = dispute.Order?.OrderItems
                .Any(oi => oi.Product != null && oi.Product.SellerId == sellerId) ?? false;

            if (!belongsToSeller) return null;

            return new DisputeResponse
            {
                Id = dispute.Id,
                OrderId = dispute.OrderId ?? 0,

                DisputerId = dispute.RaisedBy ?? 0,
                DisputerName = dispute.RaisedByNavigation?.Username,
                DisputerEmail = dispute.RaisedByNavigation?.Email,

                OrderDate = dispute.Order?.OrderDate,
                TotalPrice = dispute.Order?.TotalPrice,

                ProductTitles = dispute.Order != null
                    ? string.Join(", ",
                        dispute.Order.OrderItems
                            .Where(oi => oi.Product != null)
                            .Select(oi => oi.Product!.Title))
                    : string.Empty,

                Description = dispute.Description,
                Status = dispute.Status ?? DisputeStatus.Pending.ToString(),
                Resolution = dispute.Resolution
            };
        }

        // Chỉ update trạng thái & resolution
        public async Task AcceptAsync(int disputeId, string refundType)
        {
            var dispute = await _disputeRepo.GetByIdAsync(disputeId);
            if (dispute == null) return;

            dispute.Status = DisputeStatus.Accepted.ToString();   // "Accepted"
            dispute.Resolution = refundType;                      // "100% refund" / "Partial refund"

            await _disputeRepo.UpdateAsync(dispute);
        }

        public async Task RejectAsync(int disputeId, string reason)
        {
            var dispute = await _disputeRepo.GetByIdAsync(disputeId);
            if (dispute == null) return;

            dispute.Status = DisputeStatus.Rejected.ToString();   // "Rejected"
            dispute.Resolution = reason;                          // lý do reject

            await _disputeRepo.UpdateAsync(dispute);
        }
    }
}
