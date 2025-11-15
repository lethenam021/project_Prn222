using BusinessLogic.DTOs.Request.Review;
using BusinessLogic.DTOs.Response.Review;
using Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interface {
    public interface IReviewService {
        Task<PagedResult<ReviewSummaryResponse>> SearchProductSummariesAsync(SearchSummaryRequest searchRequest);

        Task<PagedResult<ReviewDetailResponse>> GetProductDetailsAsync(SearchDetailRequest searchRequest);

        Task AddReplyAsync(AddReplyRequest request);

        Task<ReviewDetailResponse> GetReviewAsync(int reviewId);
    }
}
