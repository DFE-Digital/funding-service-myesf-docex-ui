(function () {
    $("#nonJavaScript").addClass("hidden");

    function fileValidation(file) {

        var isValid = true;

        $("#file-size-error").addClass("hidden");
        $("#file-format-error").addClass("hidden");
        $("#file-empty-error").addClass("hidden");
        $("#validation-error-div").addClass("hidden");

        // Allowed file type
        var allowedExtensions = [];
        $(".AllowedFileExtensions").each(function () {
            allowedExtensions.push($(this).val())
        });

        var ext = file.name.split('.').pop().toUpperCase();

        if ($.inArray(ext, allowedExtensions) == -1) {
            $("#file-format-error").removeClass("hidden");
            isValid = false;
        }

        // The size of the file.
        const fsize = file.size;
        const fsizeMbytes = Math.floor((fsize / (1024 * 1024)));

        if (fsize == 0) {
            $("#file-empty-error").removeClass("hidden");
            isValid = false;
        }
        if (fsizeMbytes >= $("#MaxFileUploadSize").val()) {
            $("#file-size-error").removeClass("hidden");
            isValid = false;
        }

        if (!isValid) {
            $("#validation-error-div").removeClass("hidden");
        }

        return isValid;
    }

    $("#fileInput").change(function () {
        var files = this.files;
        if (files.length == 1) {

            var file = files[0];
            $("#document a")
                .html(file.name)
                .attr("href", URL.createObjectURL(file))
                .attr("download", file.name);
            $(".upload-file-name").html(file.name);

            if (fileValidation(file)) {
                $("#form-upload").addClass("hidden");
                $("#documentsToSend").removeClass("hidden");
                $("#sendButton").removeClass("hidden");
            }
        }
    });

    $(".remove-link").click(function () {
        $("#uploadFileDiv").addClass("hidden");
        $("#select-document-message").addClass("hidden");
        $("#removeFileDiv").removeClass("hidden");
        $("#global-breadcrumb ol li").last().remove();
        $("#global-breadcrumb ol").append('<li><a href="#" class="back-to-send-document">Send your document</a></li>');
        $("#global-breadcrumb ol").append('<li>Remove a document</li>');

        $(".back-to-send-document").on("click", function () {
            $("#removeFileDiv").addClass("hidden");
            $("#uploadFileDiv").removeClass("hidden");
            $("#select-document-message").removeClass("hidden");
            $("#global-breadcrumb ol li").last().remove();
            $("#global-breadcrumb ol li").last().html('Send your document');

            // remove previous error messages
            $(".error-summary, .error-message").addClass("hidden");
            $("fieldset").first().removeClass("after-error-summary");
            $(".error-message").parent().removeClass("form-group-error");
        });
    });

    $("#remove-document-form").submit(function (e) {
        var selectedOption = $(".single-choice-action-form-option:checked");

        if (selectedOption.length === 1) {
            if (selectedOption.val() === "true") {
                window.location.replace("select-document-type");
            } else {
                $("#removeFileDiv").addClass("hidden");
                $("#uploadFileDiv").removeClass("hidden");
                $("#select-document-message").removeClass("hidden");
                $("#global-breadcrumb ol li").last().remove();
                $("#global-breadcrumb ol li").last().html('Send your document');
                $(".single-choice-action-form-option").prop("checked", false);

                $('.error-summary, .error-message').addClass('hidden');
                $('fieldset').first().removeClass('after-error-summary');
                $('.error-message').parent().removeClass('form-group-error');
            }
        }

        if (selectedOption.length === 0) {
            $('.error-summary, .error-message').removeClass('hidden');
            $('.error-message').parent().addClass('form-group-error');
            $('fieldset').first().addClass('after-error-summary');
        }

        e.preventDefault();
    });

})();