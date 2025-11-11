$(document).ready(function () {
    $('#register-form').on('submit', function (e) {
        e.preventDefault();

        var form = $(this);
        if (!form.valid()) {
            return;
        }

        var formData = form.serialize();
        var emailAddress = $('#register-form [name="Email"]').val();

        var $btn = $("#register-btn");
        var originalText = $btn.html();
        $btn.prop('disabled', true);
        $btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...');

        $.ajax({
            type: 'POST',
            url: '/Auth/ValidateRegistrationInfo',
            data: formData,
            success: function (response) {
                if (response.success) {
                    $("#verify-email-display").text(emailAddress);
                    $("#verify-email-input").val(emailAddress);

                    var modal = new bootstrap.Modal($('#verify-code-modal'));
                    modal.show();
                } else {
                    $('#register-form').html(response.html);
                    $.validator.unobtrusive.parse($('#register-form'));
                }
            },
            error: function (xhr) {
                toastr.error(xhr.responseText);
            },
            complete: function () {
                if (!$("#verify-code-modal").hasClass("show")) {
                    $btn.prop('disabled', false).html(originalText);
                }
            }
        });
    });

    $('#verify-form').on('submit', function (e) {
        e.preventDefault();

        var form = $(this);
        if(!form.valid()) {
            return;
        }
        var formData = form.serialize();

        var $btn = $("#verify-btn");
        var originalText = $btn.html();
        $btn.prop('disabled', true);
        $btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...');
        $.ajax({
            type: 'POST',
            url: '/Auth/VerifyEmailCode',
            data: formData,
            success: function (response) {
                if (response.success) {
                    var verifyModal = bootstrap.Modal.getInstance($('#verify-code-modal'));
                    verifyModal.hide();

                    var successModal = new bootstrap.Modal($('#success-modal'));
                    successModal.show();
                } else {
                    $('#verify-form').html(response.html);
                    $.validator.unobtrusive.parse($('#verify-form'));
                }
            },
            error: function (xhr) {
                if (xhr.status === 400) {
                    var verifyModal = bootstrap.Modal.getInstance($('#verify-code-modal'));
                    verifyModal.hide();

                    var errorModal = new bootstrap.Modal($('#session-expired-modal'));
                    errorModal.show();
                } else {
                    toastr.error(xhr.responseText);
                }
            },
            complete: function () {
                $btn.prop('disabled', false).html(originalText);
            }
        });
    });
});