namespace Presentation.ViewModel.Data.Emails {
    public class ReviewReplyEmailViewModel {
        public string BuyerName { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string? ProductImageUrl { get; set; }
        public DateTime ReviewDate { get; set; }

        // Đây là nội dung HTML từ Quill.js
        public string SellerReplyMessage { get; set; } = null!;
    }
}
