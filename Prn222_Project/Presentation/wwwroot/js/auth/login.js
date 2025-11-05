$(document).ready(function () {
    $('#login-form').on('submit', function (e) {
        e.preventDefault();

        if (!$(this).valid()) {
            return;
        }

        $.ajax({
            type: 'POST',
            url: '/Auth/Login',
            data: $(this).serialize(),
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
            }
        });
    });
});