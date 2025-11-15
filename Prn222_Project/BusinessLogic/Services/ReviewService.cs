using BusinessLogic.DTOs.Request.Review;
using BusinessLogic.DTOs.Response.Category;
using BusinessLogic.DTOs.Response.Product;
using BusinessLogic.DTOs.Response.Review;
using BusinessLogic.Interface;
using Common.Helpers;
using DataAccess.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services {
    public class ReviewService : IReviewService {
        private readonly IReviewRepo _reviewRepo;
        private readonly IProductRepo _productRepo;

        public ReviewService(IReviewRepo reviewRepo, IProductRepo productRepo) {
            _reviewRepo = reviewRepo;
            _productRepo = productRepo;
        }

        public async Task<PagedResult<ReviewDetailResponse>> GetProductDetailsAsync(SearchDetailRequest searchRequest) {
            var pageResult = await _reviewRepo.GetReviewDetailsAsync(searchRequest.SellerId, searchRequest.ProductId, searchRequest.FromDate, searchRequest.ToDate, searchRequest.Pagination!.PageIndex, searchRequest.Pagination!.PageSize);

            // 2. Map kết quả (IGrouping) sang Response DTO
            // 2. Map kết quả (Review) sang Response DTO
            var reviews = pageResult.Items.Select(review => new ReviewDetailResponse {
                Id = review.Id,
                Reviewer = review.Reviewer!.Username!, // (Đảm bảo Reviewer được include)
                Rating = review.Rating,
                Comment = review.Comment,
                ReviewDate = review.CreatedAt ?? DateTime.Now
            }).ToList();

            // 3. (Quan trọng) Lấy thông tin sản phẩm để hiển thị tiêu đề
            var product = await _productRepo.GetByIdAsync(searchRequest.ProductId); // (Giả định)

            return new PagedResult<ReviewDetailResponse> {
                Items = reviews,
                TotalRecord = pageResult.TotalRecord,
                PageIndex = pageResult.PageIndex,
                PageSize = pageResult.PageSize,
                // (Gửi dữ liệu phụ về Controller để render tiêu đề)
                AdditionalData = new {
                    ProductTitle = product?.Title ?? "Product",
                    ProductId = searchRequest.ProductId
                }
            };
        }

        public async Task<PagedResult<ReviewSummaryResponse>> SearchProductSummariesAsync(SearchSummaryRequest searchRequest) {
            // 1. Gọi Repo (Giờ trả về PagedResult<RepoReviewSummary>)
            var pageResult = await _reviewRepo.GetReviewSummariesAsync(
                searchRequest.SellerId,
                searchRequest.SearchTerm,
                searchRequest.CategoryId,
                searchRequest.Pagination!.PageIndex,
                searchRequest.Pagination!.PageSize
            );

            // 2. Map từ RepoReviewSummary (Data) -> ReviewSummaryResponse (Business)
            // (FIX) 'item' bây giờ là 'RepoReviewSummary'
            var summaries = pageResult.Items.Select(item => new ReviewSummaryResponse {
                Product = new ProductResponse {
                    // (FIX) Truy cập thẳng vào 'item.Product'
                    Id = item.Product.Id,
                    Title = item.Product.Title,
                    ImageUrl = item.Product.Images, // Giả định

                    // (FIX) Kiểm tra null cho Category
                    Category = item.Product.Category != null ? new CategoryResponse {
                        Id = item.Product.Category.Id,
                        Name = item.Product.Category.Name
                    } : null
                },
                // (FIX) Lấy dữ liệu đã được tính toán sẵn
                AverageRating = item.AverageRating,
                TotalReviews = item.TotalReviews
            }).ToList();

            // 3. Trả về PagedResult
            return new PagedResult<ReviewSummaryResponse> {
                Items = summaries,
                TotalRecord = pageResult.TotalRecord,
                PageIndex = pageResult.PageIndex,
                PageSize = pageResult.PageSize
            };
        }
    }
}
