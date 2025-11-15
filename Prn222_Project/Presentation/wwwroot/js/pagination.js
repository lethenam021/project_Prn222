/*
 * == FILE JAVASCRIPT PHÂN TRANG CHUNG (ĐÃ SỬA LỖI) ==
 *
 * File này lắng nghe các sự kiện Paging và gọi một
 * hàm "callback" (phải được định nghĩa ở file JS cụ thể).
 *
 * Hàm callback phải là:
 * handlePageChange(newPageIndex, newPageSize)
 */
$(document).ready(function () {

    // Hàm helper "callback"
    function callSpecificHandler(newIndex, newSize) {
        if (typeof handlePageChange === 'function') {
            // (Hàm này được định nghĩa trong product-reviews.js)
            handlePageChange(newIndex, newSize);
        } else {
            console.error("Function 'handlePageChange(newIndex, newSize)' is not defined in the page-specific JS file.");
        }
    }

    // 1. XỬ LÝ CLICK NÚT "NEXT" / "PREVIOUS"
    $(document).on("click", ".pagination-link", function (e) {
        e.preventDefault();

        // Lấy PageIndex MỚI từ nút
        var newIndex = $(this).data("page");

        // (SỬA) Tìm 'div' cha chung gần nhất
        var $container = $(this).closest(".d-flex.justify-content-between");

        // (SỬA) Tìm PageSize HIỆN TẠI trong 'div' cha đó
        var currentSize = $container.find(".page-size-select").val();

        // Gọi callback với cả hai giá trị
        callSpecificHandler(newIndex, currentSize);
    });

    // 2. XỬ LÝ THAY ĐỔI "PAGE SIZE"
    // (SỬA) Lắng nghe CLASS thay vì ID
    $(document).on("change", ".page-size-select", function (e) {
        e.preventDefault();

        // Lấy PageSize MỚI từ dropdown
        var newSize = $(this).val();
        // Khi đổi PageSize, luôn reset về Trang 1
        var newIndex = 1;

        // Gọi callback với cả hai giá trị
        callSpecificHandler(newIndex, newSize);
    });
});