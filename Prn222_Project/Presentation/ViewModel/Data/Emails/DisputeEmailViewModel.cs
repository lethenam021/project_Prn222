namespace Presentation.ViewModel.Data.Emails
{
    public class DisputeEmailViewModel
    {
        public string BuyerName { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public string Reason { get; set; } = string.Empty; // nội dung email bạn nhập
    }
}
