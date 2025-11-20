// BusinessLogic/Interface/IDisputeService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogic.DTOs.Response.Dispute;

namespace BusinessLogic.Interface
{
    public interface IDisputeService
    {
        Task<IEnumerable<DisputeResponse>> GetDisputesForSellerAsync(int sellerId);
        Task<DisputeResponse?> GetDisputeDetailAsync(int disputeId, int sellerId);

        // Chỉ update DB, không xử lý email
        Task AcceptAsync(int disputeId, string refundType);
        Task RejectAsync(int disputeId, string reason);
    }
}
