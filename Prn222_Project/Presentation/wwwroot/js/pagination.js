/*
 * == FILE JAVASCRIPT PHÂN TRANG CHUNG ==
 *
 * File này lắng nghe các sự kiện Paging và gọi một
 * hàm "callback" (phải được định nghĩa ở file JS cụ thể).
 *
 * Hàm callback phải là:
 * handlePageChange(newPageIndex, newPageSize)
 */
$(document).ready(function () {

    // Hàm helper "callback"
    // Nó sẽ gọi hàm 'handlePageChange' (nếu tồn tại)
    function callSpecificHandler(newIndex, newSize) {
        if (typeof handlePageChange === 'function') {
            handlePageChange(newIndex, newSize);
        } else {
            console.error("Function 'handlePageChange(newIndex, newSize)' is not defined in the page-specific JS file.");
        }
    }

    // Hàm helper để tìm form
    function getFilterForm() {
        // Tìm bất kỳ form nào có ID kết thúc bằng "-filter-form"
        // (Ví dụ: "review-filter-form", "inventory-filter-form")
        return $("form[id$='-filter-form']");
    }

    // 1. XỬ LÝ CLICK NÚT "NEXT" / "PREVIOUS"
    $(document).on("click", ".pagination-link", function (e) {
        e.preventDefault();
        var $filterForm = getFilterForm();
        if ($filterForm.length === 0) return;

        // Lấy PageIndex MỚI từ nút
        var newIndex = $(this).data("page");
        // Lấy PageSize HIỆN TẠI từ dropdown
        var currentSize = $("#page-size-select").val();

        // Gọi callback với cả hai giá trị
        callSpecificHandler(newIndex, currentSize);
    });

    // 2. XỬ LÝ THAY ĐỔI "PAGE SIZE"
    $(document).on("change", "#page-size-select", function () {
        var $filterForm = getFilterForm();
        if ($filterForm.length === 0) return;

        // Lấy PageSize MỚI từ dropdown
        var newSize = $(this).val();
        // Khi đổi PageSize, luôn reset về Trang 1
        var newIndex = 1;

        // Gọi callback với cả hai giá trị
        callSpecificHandler(newIndex, newSize);
    });
});