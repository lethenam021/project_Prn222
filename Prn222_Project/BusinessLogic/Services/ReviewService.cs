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

        public ReviewService(IReviewRepo reviewRepo) {
            _reviewRepo = reviewRepo;
        }

        public async Task<PagedResult<ReviewResponse>> SearchReviewAsync(SearchRequest searchRequest) {
            var pageResult = await _reviewRepo.GetReviewsAsync(searchRequest.SellerId, searchRequest.SearchTerm, searchRequest.FromDate, searchRequest.ToDate, pageIndex: searchRequest.Pagination!.PageIndex, pageSize: searchRequest.Pagination!.PageSize);

            var reviews = pageResult.Items.Select(review => new ReviewResponse {
                Id = review.Id,
                Product = new ProductResponse {
                    Id = review.Product!.Id,
                    Title = review.Product.Title!,
                    Category = new CategoryResponse {
                        Id = review.Product.Category!.Id,
                        Name = review.Product.Category.Name!
                    },
                    ImageUrl = review.Product.Images ?? null!
                },
                Reviewer = review.Reviewer!.Username!,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt!.Value
            }).ToList();

            return new PagedResult<ReviewResponse> {
                Items = reviews,
                TotalRecord = pageResult.TotalRecord,
                PageIndex = pageResult.PageIndex,
                PageSize = pageResult.PageSize
            };
        }
    }
}
