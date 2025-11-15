$(document).ready(function () {
    $('#login-form').on('submit', function (e) {
        e.preventDefault();

        var form = $(this);
        if (!form.valid()) {
            return;
        }

        var url = form.attr("action");
        var formData = form.serialize();

        var $btn = $("#login-btn");
        var originalText = $btn.html();
        $btn.prop('disabled', true);
        $btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...');

        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (response) {
                if (response.success) {
                    window.location.href = response.redirectUrl;
                } else {
                    $('#login-form').html(response.html);
                    $.validator.unobtrusive.parse($('#login-form'));
                }
            },
            error: function (xhr) {
                toastr.error(xhr.responseText);
            },
            complete: function () {
                $btn.prop('disabled', false).html(originalText);
            }
        });
    });
});