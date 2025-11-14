// ==========================================
// HÀM TẢI CHI TIẾT (CẤP 2) BẰNG AJAX
// (Sửa: Giờ nhận cả filter data)
// ==========================================
function loadDetails(productId, pageIndex, pageSize, fromDate, toDate) {
    var $detailView = $("#detail-view");
    var $summaryView = $("#summary-view");

    // Hiển thị loading (Chỉ khi tải lần đầu)
    if (pageIndex === 1 && !fromDate && !toDate) {
        $detailView.html('<div class="text-center p-5"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>');
        // Chuyển view
        $summaryView.hide();
        $detailView.show();
    } else {
        // Nếu đang filter hoặc lật trang (đã ở Cấp 2)
        // Hiển thị loading nhỏ trong bảng
        $("#detail-table-container").html('<div class="text-center p-3"><div class="spinner-border spinner-border-sm text-primary" role="status"></div></div>');
        $("#detail-pagination-container").html("");
    }

    // --- (MỚI) 1. TẠO URL VÀ PUSH STATE ---
    var urlParams = new URLSearchParams();
    urlParams.set("ProductId", productId);
    urlParams.set("Pagination.PageIndex", pageIndex);
    urlParams.set("Pagination.PageSize", pageSize);
    if (fromDate) urlParams.set("FromDate", fromDate);
    if (toDate) urlParams.set("ToDate", toDate);

    // Lấy đường dẫn cơ sở (ví dụ: /product-reviews)
    var newUrl = window.location.pathname + '?' + urlParams.toString();
    var newTitle = "Product Details"; // Sẽ cập nhật sau

    // Đẩy URL mới vào thanh địa chỉ
    history.pushState({ view: 'detail' }, newTitle, newUrl);
    // ------------------------------------

    // Gọi AJAX đến Action "get-details"
    $.ajax({
        type: "GET",
        url: "/product-reviews/get-details",
        data: {
            ProductId: productId,
            "Pagination.PageIndex": pageIndex,
            "Pagination.PageSize": pageSize,
            FromDate: fromDate,
            ToDate: toDate
        },
        dataType: "json",
        success: function (response) {
            if (response.success) {
                // Load toàn bộ partial (gồm filter, bảng, paging) vào div
                $detailView.html(response.html);
            } else {
                $detailView.html('<div class="alert alert-danger">' + (response.message || 'Failed to load details.') + '</div>');
            }
        },
        error: function () {
            $detailView.html('<div class="alert alert-danger">An unknown error occurred while loading details.</div>');
        }
    });
}

// ==========================================
// HÀM XỬ LÝ PHÂN TRANG (Pagination)
// ==========================================
function handlePageChange(newIndex, newSize) {
    var $detailView = $("#detail-view");

    if ($detailView.is(":visible")) {
        // --- Đang ở Cấp 2 (Detail View) ---
        var $form = $("#review-detail-filter-form");
        var productId = $form.find("input[name='ProductId']").val();

        // Lấy filter của Cấp 2
        var fromDate = $form.find("input[name='FromDate']").val();
        var toDate = $form.find("input[name='ToDate']").val();

        // Cập nhật Paging (trong form Cấp 2)
        $form.find("input[name='Pagination.PageIndex']").val(newIndex);
        $form.find("input[name='Pagination.PageSize']").val(newSize);

        loadDetails(productId, newIndex, newSize, fromDate, toDate);

    } else {
        // --- Đang ở Cấp 1 (Summary View) ---
        var $form = $("#review-filter-form");
        $form.find("input[name='Pagination.PageIndex']").val(newIndex);
        $form.find("input[name='Pagination.PageSize']").val(newSize);
        $form.trigger("submit");
    }
}

// ==========================================
// (MỚI) HELPER: HIỂN THỊ VIEW TÓM TẮT
// ==========================================
function showSummaryView(pushState = false) {
    var $detailView = $("#detail-view");
    var $summaryView = $("#summary-view");

    $detailView.hide();
    $detailView.html("");
    $summaryView.show();
    document.title = "Product Reviews";

    if (pushState) {
        // Đẩy URL của trang summary (lấy từ form)
        var $form = $("#review-filter-form");
        var summaryParams = $form.serialize();
        var newUrl = window.location.pathname + '?' + summaryParams;
        history.pushState({ view: 'summary' }, "Product Reviews", newUrl);
    }
}

$(document).ready(function () {
    // (Các biến $summaryTableContainer, $detailView... giữ nguyên)
    var $summaryTableContainer = $("#summary-table-container");
    var $summaryPaginationContainer = $("#summary-pagination-container");
    var $detailView = $("#detail-view");
    var $summaryView = $("#summary-view");

    // ==========================================
    // (MỚI) HELPER: ĐỌC URL KHI TẢI TRANG (F5)
    // ==========================================
    function loadViewFromUrl() {
        var params = new URLSearchParams(window.location.search);

        if (params.has("ProductId")) {
            // Nếu URL có ProductId -> Tải trang chi tiết
            var productId = parseInt(params.get("ProductId"));
            var pageIndex = parseInt(params.get("Pagination.PageIndex")) || 1;
            var pageSize = parseInt(params.get("Pagination.PageSize")) || 6;
            var fromDate = params.get("FromDate") || null;
            var toDate = params.get("ToDate") || null;

            loadDetails(productId, pageIndex, pageSize, fromDate, toDate);
        } else {
            // Nếu không -> Tải trang tóm tắt
            showSummaryView(false); // Chỉ hiện/ẩn
            $("#review-filter-form").trigger("submit"); // Kích hoạt filter
        }
    }

    // ==========================================
    // (MỚI) XỬ LÝ NÚT BACK/FORWARD CỦA TRÌNH DUYỆT
    // ==========================================
    window.onpopstate = function (event) {
        // Khi người dùng bấm Back/Forward,
        // chỉ cần đọc lại URL và tải view tương ứng
        loadViewFromUrl();
    };

    // ==========================================
    // === XỬ LÝ CHO CẤP 1 (SUMMARY VIEW) ===
    // ==========================================

    // --- 1. XỬ LÝ LỌC (FILTER) CHO CẤP 1 (SUMMARY) ---
    $(document).on("submit", "#review-filter-form", function (e) {
        e.preventDefault();

        var $form = $(this);
        var formData = $form.serialize();
        var url = $form.attr("action");
        // (SỬA) Chỉ push state nếu URL khác
        var newUrl = window.location.pathname + '?' + formData;
        if (window.location.search !== '?' + formData) {
            history.pushState({ view: 'summary' }, "Product Reviews", newUrl);
        }

        $summaryTableContainer.html('<div id="table-loading-spinner" class="text-center p-5"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>');
        $summaryPaginationContainer.html("");

        $.ajax({
            type: "GET",
            url: url,
            data: formData,
            dataType: "json",
            success: function (response) {
                if (response.success) {
                    $summaryTableContainer.html(response.html.summaryTable);
                    $summaryPaginationContainer.html(response.html.pagination);
                } else {
                    $summaryTableContainer.html('<div class="alert alert-danger">' + (response.message || 'Failed to load list.') + '</div>');
                }
            },
            error: function () {
                $summaryTableContainer.html('<div class="alert alert-danger">An unknown error occurred while filtering.</div>');
            }
        });
    });

    // --- (MỚI) THÊM LOGIC CLICK CHO NÚT SEARCH (CẤP 1) ---
    $(document).on("click", "#filter-btn", function (e) {
        e.preventDefault();
        var $form = $(this).closest("form");

        // (MỚI) Đây là nơi reset Paging
        $form.find("input[name='Pagination.PageIndex']").val(1);
        $form.find("input[name='Pagination.PageSize']").val(6);

        // Kích hoạt submit (để hàm ở trên chạy)
        $form.trigger("submit");
    });

    // --- 2. XỬ LÝ NÚT RESET FILTER (CẤP 1) ---
    $(document).on("click", "#reset-btn", function (e) {
        e.preventDefault();
        var $form = $(this).closest("form");
        $form.find('input[type="text"], input[type="search"]').val('');
        $form.find('select').val(''); // Reset select
        $form.find("input[name='Pagination.PageIndex']").val(1);
        $form.find("input[name='Pagination.PageSize']").val(6);
        $form.trigger("submit");
    });

    // --- 3. TỰ ĐỘNG FILTER KHI VÀO TRANG (TẢI CẤP 1) ---
    loadViewFromUrl();

    // --- 4. CLICK "VIEW DETAILS" (CHUYỂN SANG CẤP 2) ---
    $(document).on("click", ".view-details-link", function (e) {
        e.preventDefault();
        var productId = $(this).data("product-id");
        // Tải chi tiết (trang 1, size 6, không filter)
        loadDetails(productId, 1, 6, null, null);
    });

    // ==========================================
    // === XỬ LÝ CHO CẤP 2 (DETAIL VIEW) ===
    // ==========================================

    // --- 5. CLICK "BACK TO SUMMARY" (TRỞ LẠI CẤP 1) ---
    $(document).on("click", "#back-to-summary-btn", function (e) {
        e.preventDefault();
        showSummaryView(true);
    });

    // --- 6. (MỚI) XỬ LÝ LỌC (FILTER) CHO CẤP 2 (DETAIL) ---
    $(document).on("submit", "#review-detail-filter-form", function (e) {
        e.preventDefault();
        var $form = $(this);
        var productId = $form.find("input[name='ProductId']").val();
        var fromDate = $form.find("input[name='FromDate']").val();
        var toDate = $form.find("input[name='ToDate']").val();

        // Reset về trang 1 khi filter
        $form.find("input[name='Pagination.PageIndex']").val(1);
        var pageSize = $form.find("input[name='Pagination.PageSize']").val();

        loadDetails(productId, 1, pageSize, fromDate, toDate);
    });

    // --- 7. (MỚI) XỬ LÝ NÚT RESET FILTER (CẤP 2) ---
    $(document).on("click", "#detail-reset-btn", function (e) {
        e.preventDefault();
        var $form = $("#review-detail-filter-form");
        $form.find('input[type="date"]').val(''); // Reset ngày

        // Reset về trang 1
        $form.find("input[name='Pagination.PageIndex']").val(1);

        // Trigger submit để tải lại
        $form.trigger("submit");
    });

    // --- 8. VALIDATE DATE (CHO CẤP 2) ---
    $(document).on("change", "#detail-filter-from-date", function () {
        var fromDate = $(this).val();
        if (fromDate) {
            $("#detail-filter-to-date").attr("min", fromDate);
        }
    });
    $(document).on("change", "#detail-filter-to-date", function () {
        var toDate = $(this).val();
        if (toDate) {
            $("#detail-filter-from-date").attr("max", toDate);
        }
    });

    // --- 9. XỬ LÝ CLICK "SEND FEEDBACK" (TRONG CẤP 2) ---
    $(document).on("click", ".send-feedback-btn", function (e) {
        e.preventDefault();
        var reviewId = $(this).data("review-id"); // (Cần đảm bảo Id có trong VM)
        alert("Chức năng 'Send Feedback' cho Review ID: " + reviewId);
    });

    // --- 10. XỬ LÝ "READ MORE" (VẪN GIỮ) ---
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
});