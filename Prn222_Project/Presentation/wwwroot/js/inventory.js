// ==========================================
// HÀM XỬ LÝ PHÂN TRANG (Pagination)
// (Hàm này được gọi bởi pagination.js)
// ==========================================
function handlePageChange(newIndex, newSize) {
    var $form = $("#inventory-filter-form");

    // 1. Cập nhật giá trị Paging vào form ẩn
    $form.find("input[name='Pagination.PageIndex']").val(newIndex);
    $form.find("input[name='Pagination.PageSize']").val(newSize);

    // 2. Gửi (submit) lại form filter
    $form.trigger("submit");
}



$(document).ready(function () {
    var $tableContainer = $("#inventory-table-container");
    var $filterForm = $("#inventory-filter-form");
    var $paginationContainer = $("#inventory-pagination-container"); // (MỚI)

    $(document).on("submit", "#inventory-filter-form", function (e) {
        e.preventDefault(); // Ngăn form post

        // (Chúng ta phải tìm form bên trong 'this' vì 'this' 
        // chính là cái form đã trigger sự kiện)
        var $form = $(this);
        var formData = $form.serialize();

        // Lấy URL từ action (an toàn hơn)
        var url = $form.attr("action");
        var fullUrl = url + '?' + formData;

        // Cập nhật URL trên thanh địa chỉ
        history.pushState(null, null, fullUrl);

        // Hiển thị loading (dùng spinner có sẵn)
        $tableContainer.html('<div id="table-loading-spinner" class="text-center p-5"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>');
        $paginationContainer.html(""); // (MỚI) Xóa paging cũ

        // Gửi AJAX (GET)
        $.ajax({
            type: "GET",
            url: url, // Chỉ dùng action (vì data đã chứa query)
            data: formData,
            dataType: "json", // Mong đợi JSON
            success: function (response) {
                if (response.success) {
                    // Cập nhật CẢ HAI phần
                    // Chúng ta phải tìm lại $filterFormContainer vì nó đã bị thay thế
                    $("#inventory-filter-form").html(response.html.filter);
                    $tableContainer.html(response.html.table);
                    $paginationContainer.html(response.html.pagination); // (MỚI)
                } else {
                    // Lỗi (server trả về success = false)
                    $tableContainer.html('<div class="alert alert-danger">' + (response.message || 'Failed to load list.') + '</div>');
                }
            },
            error: function () {
                // Lỗi (500, 404)
                $tableContainer.html('<div class="alert alert-danger">An unknown error occurred while filtering.</div>');
            }
        });
    });

    // --- (MỚI) LOGIC CLICK CHO NÚT SEARCH ---
    // (Giống hệt trang Review)
    $(document).on("click", "#filter-btn", function (e) {
        e.preventDefault();
        var $form = $(this).closest("form");

        // Reset Paging về 1 và 6
        $form.find("input[name='Pagination.PageIndex']").val(1);
        $form.find("input[name='Pagination.PageSize']").val(6);

        $form.trigger("submit");
    });

        // ---------------------------------------------
    // --- LOGIC MỚI: CHỈNH SỬA SỐ LƯỢNG (INVENTORY) ---
    // ---------------------------------------------

    // Hàm helper: Bật/Tắt chế độ Edit cho một dòng
    function setRowEditing(row, isEditing) {
        var $row = $(row);
        var $stepper = $row.find('.quantity-stepper');
        var $display = $row.find('.quantity-display');
        var $input = $stepper.find('.quantity-input');
        var $buttons = $stepper.find('.btn-plus, .btn-minus');
        var $cancelBtn = $row.find('.btn-cancel-quantity');

        if (isEditing) {
            // --- Bật chế độ Edit ---
            $display.addClass('d-none'); // Ẩn text
            $stepper.removeClass('d-none'); // Hiện stepper
            $cancelBtn.removeClass('d-none');
            
            $stepper.addClass('is-editing');
            $input.prop('disabled', false);
            $buttons.prop('disabled', false);
        } else {
            // --- Tắt chế độ Edit ---
            $display.removeClass('d-none'); // Hiện text
            $stepper.addClass('d-none'); // Ẩn stepper
            $cancelBtn.addClass('d-none');

            $stepper.removeClass('is-editing');
            $input.prop('disabled', true);
            $buttons.prop('disabled', true);
        }
    }

    // 1. Khi click nút "Edit" / "Save"
    $(document).on("click", ".btn-edit-quantity", function () {
        var $button = $(this);
        var $row = $button.closest("tr");
        var $input = $row.find(".quantity-input");
        var $status = $row.find(".save-status");
        var currentState = $button.data("state");
        var productId = $button.data("product-id");

        // Xóa thông báo cũ
        $status.html("");

        if (currentState === "edit") {
            // --- Chuyển sang chế độ "Save" ---
            
            // Bật input (bằng hàm helper)
            setRowEditing($row, true);
            
            // Đổi nút
            $button.data("state", "save");
            $button.text("Save");
            $button.removeClass("btn-secondary").addClass("btn-primary");

        } else if (currentState === "save") {
            // --- Chuyển sang chế độ "Edit" (SAU KHI AJAX) ---
            
            var newQuantity = $input.val();

            // Hiển thị loading
            $button.prop("disabled", true);
            $button.html('Save <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>');
            var filterData = $("#inventory-filter-form").serialize();

            // Gọi AJAX (giống code cũ)
            $.ajax({
                type: "POST",
                url: "/inventory/update-quantity?" + filterData, // (Đảm bảo URL này đúng)
                data: {
                    productId: productId, 
                    quantity: newQuantity
                },
                success: function (response) {
                    if (response.success) {
                        $("#inventory-filter-form").html(response.html.filter);
                        $("#inventory-table-container").html(response.html.table);
                        $("#inventory-pagination-container").html(response.html.pagination);
                        toastr.success("Quantity updated successfully!");
                    } else {
                        toastr.error(response.message);
                    }
                },
                error: function (xhr) {
                    toastr.error(xhr.responseText);
                }
            });
        }
    });

    // --- THÊM HÀM MỚI: XỬ LÝ NÚT CANCEL ---
    $(document).on("click", ".btn-cancel-quantity", function () {
        var $button = $(this);
        var $row = $button.closest("tr");
        var $mainButton = $row.find(".btn-edit-quantity");
        var $input = $row.find(".quantity-input");
        var $span = $row.find(".quantity-display");

        // 1. Tắt chế độ edit (ẩn input, ẩn nút cancel, hiện span)
        setRowEditing($row, false);

        // 2. Khôi phục giá trị của input về giá trị của span
        $input.val($span.text());

        // 3. Khôi phục nút "Save" về "Update Quantity"
        $mainButton.data("state", "edit");
        $mainButton.text("Update Quantity"); // <-- SỬA TEXT
        $mainButton.removeClass("btn-primary").addClass("btn-secondary");

        // 4. Xóa thông báo lỗi (nếu có)
        $row.find(".save-status").html("");
    });

    // --- BẮT ĐẦU SỬA ĐỔI ---

    // Biến toàn cục để giữ interval
    var quantityInterval;
    var quantityTimeout; // Biến giữ timeout ban đầu

    // Hàm helper (giúp lặp lại code)
    function increaseValue($input) {
        var currentValue = parseInt($input.val());
        if (!isNaN(currentValue)) {
            $input.val(currentValue + 1);
        }
    }

    function decreaseValue($input) {
        var currentValue = parseInt($input.val());
        if (!isNaN(currentValue) && currentValue > 0) {
            $input.val(currentValue - 1);
        }
    }

    // 2. Khi *NHẤN VÀ GIỮ* nút + (Plus)
    // (Xóa .on("click") cũ)
    $(document).on("mousedown", ".btn-plus", function () {
        var $input = $(this).siblings(".quantity-input");

        // 1. Tăng 1 lần ngay lập tức
        increaseValue($input);

        // 2. Đặt một timeout để bắt đầu lặp lại sau 400ms
        quantityTimeout = setTimeout(function () {
            // 3. Bắt đầu lặp lại (mỗi 100ms)
            quantityInterval = setInterval(function () {
                increaseValue($input);
            }, 100); // Tốc độ lặp
        }, 400); // Thời gian chờ ban đầu
    });

    // 3. Khi *NHẤN VÀ GIỮ* nút - (Minus)
    // (Xóa .on("click") cũ)
    $(document).on("mousedown", ".btn-minus", function () {
        var $input = $(this).siblings(".quantity-input");

        // 1. Giảm 1 lần ngay lập tức
        decreaseValue($input);

        // 2. Đặt timeout
        quantityTimeout = setTimeout(function () {
            // 3. Bắt đầu lặp lại
            quantityInterval = setInterval(function () {
                decreaseValue($input);
            }, 100);
        }, 400);
    });

    // 4. KHI *NHẢ CHUỘT* (mouseup) HOẶC *RỜI CHUỘT* (mouseleave)
    // (Dừng việc lặp lại)
    $(document).on("mouseup mouseleave", ".btn-plus, .btn-minus", function () {
        // Dừng lặp lại
        clearInterval(quantityInterval);
        // Dừng timeout ban đầu (nếu chưa kịp chạy)
        clearTimeout(quantityTimeout);
    });

    // 4. VALIDATE MỚI: Khi người dùng TỰ GÕ VÀO Ô INPUT
    // (Sự kiện 'change' chạy khi người dùng gõ xong và click ra ngoài)
    $(document).on("change", ".quantity-input", function () {
        var $input = $(this);
        var intValue = parseInt($input.val());

        // Kiểm tra: Nếu không phải là số (NaN) HOẶC nhỏ hơn 0
        if (isNaN(intValue) || intValue < 0) {
            // Set lại value = 1 (theo yêu cầu của bạn)
            $input.val(1);
        } else {
            // Nếu là số hợp lệ (ví dụ: 0, 5, 10),
            // chỉ cần gán lại giá trị "sạch" (để loại bỏ số thập phân)
            $input.val(intValue);
        }
    });

    $("#inventory-filter-form").trigger("submit");

    // --- 2. XỬ LÝ NÚT RESET FILTER ---
    // (Cũng phải dùng event delegation)
    $(document).on("click", "#reset-btn", function (e) {
        e.preventDefault();
        // Tìm form cha
        var $form = $(this).closest("form");
        $form.find('input[type="text"], input[type="search"]').val('');

        $form.find('select').prop('selectedIndex', 0);

        $form.find("input[name='Pagination.PageIndex']").val(1);
        $form.find("input[name='Pagination.PageSize']").val(6);

        // Gửi form (đã reset)
        $form.trigger("submit");
    });
    
});