function handlePageChange(newIndex, newSize) {
    var $form = $("#review-filter-form");

    // 1. Cập nhật giá trị PageIndex VÀ PageSize trong form ẩn
    $form.find("input[name='Pagination.PageIndex']").val(newIndex);
    $form.find("input[name='Pagination.PageSize']").val(newSize);

    // 2. Gửi (submit) lại form filter
    $form.trigger("submit");
}


$(document).ready(function () {

    var $tableContainer = $("#review-table-container");
    var $paginationContainer = $("#review-pagination-container"); 

    // --- 1. XỬ LÝ LỌC (FILTER) ---
    // (Dùng Event Delegation để sự kiện không bị mất sau khi AJAX)
    $(document).on("submit", "#review-filter-form", function (e) {
        e.preventDefault(); // Ngăn form post

        var $form = $(this);
        var formData = $form.serialize();

        var url = $form.attr("action");
        var fullUrl = url + '?' + formData;

        // Cập nhật URL trên thanh địa chỉ
        history.pushState(null, null, fullUrl);

        // Hiển thị loading spinner
        $tableContainer.html('<div id="table-loading-spinner" class="text-center p-5"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>');
        $paginationContainer.html(""); // Xóa paging cũ

        // Gửi AJAX (GET)
        $.ajax({
            type: "GET",
            url: url,
            data: formData,
            dataType: "json", // Mong đợi JSON
            success: function (response) {
                if (response.success) {
                    // Cập nhật CẢ HAI phần
                    $("#review-filter-form").html(response.html.filter);
                    $tableContainer.html(response.html.table);
                    $paginationContainer.html(response.html.pagination); 
                } else {
                    $tableContainer.html('<div class="alert alert-danger">' + (response.message || 'Failed to load list.') + '</div>');
                }
            },
            error: function () {
                $tableContainer.html('<div class="alert alert-danger">An unknown error occurred while filtering.</div>');
            }
        });
    });

    // --- 2. XỬ LÝ NÚT RESET FILTER ---
    // (Dùng Event Delegation)
    $(document).on("click", "#reset-btn", function (e) {
        e.preventDefault();
        var $form = $(this).closest("form");

        // Reset thủ công (an toàn hơn .reset())
        $form.find('input[type="text"], input[type="search"]').val('');
        $form.find('input[type="date"]').val('');

        // Reset cả Paging về trang 1 (SỬA LỖI: Dùng 'name')
        $form.find("input[name='Pagination.PageIndex']").val(1);

        // Reset cả PageSize về giá trị đầu tiên (mặc định là 6) (SỬA LỖI: Dùng 'name')
        $form.find("input[name='Pagination.PageSize']").val(6); 

        $form.trigger("submit");
    });

    // --- 3. TỰ ĐỘNG FILTER KHI VÀO TRANG ---
    // (Tìm form và trigger nó lần đầu tiên)
    $("#review-filter-form").trigger("submit");

    $(document).on("click", ".read-more-link", function (e) {
        e.preventDefault();

        var $link = $(this);
        // Tìm 'div' comment ngay bên trên nó
        var $comment = $link.siblings(".comment-truncate");

        // Thêm/Xóa class 'is-expanded' (class này đã được định nghĩa
        // trong file _ReviewTablePartial.cshtml)
        $comment.toggleClass("is-expanded");

        // Đổi text của link
        if ($comment.hasClass("is-expanded")) {
            $link.text("Read Less");
        } else {
            $link.text("Read More");
        }
    });

    // Khi chọn 'From Date', 'To Date' không thể nhỏ hơn
    $(document).on("change", "#filter-from-date", function () {
        var fromDate = $(this).val();
        if (fromDate) {
            $("#filter-to-date").attr("min", fromDate);
        }
    });

    // Khi chọn 'To Date', 'From Date' không thể lớn hơn
    $(document).on("change", "#filter-to-date", function () {
        var toDate = $(this).val();
        if (toDate) {
            $("#filter-from-date").attr("max", toDate);
        }
    });
});