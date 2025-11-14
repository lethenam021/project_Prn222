using Common.Helpers;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo {
    public interface IReviewRepo {
        Task<PagedResult<Review>> GetReviewsAsync(int sellerId, string? productName, DateTime? fromDate, DateTime? toDate, int pageIndex, int pageSize);
    }
}
