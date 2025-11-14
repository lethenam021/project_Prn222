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
        Task<PagedResult<ReviewResponse>> SearchReviewAsync(SearchRequest searchRequest);
    }
}
