(function () {
    $(document).on("click", 'button[type = "submit"]', function () {
        $('form').data('error-source', $(this).data("error-source"));

        if ($(this).data("allow-submit-without-selection")) {
            $('form').data('allow-submit-without-selection', true);
        } else {
            $('form').data('allow-submit-without-selection', false);
        }
    });

    $("form").on("submit",
        function (e) {
            $('.error-summary').addClass('hidden');
            $('.error-summary-list span').addClass('hidden');

            if ($(this).data('allow-submit-without-selection')) {
                return;
            }

            var selectedOption = $(".multiple-choice-action-form-option:checked");

            if (selectedOption.length === 0) {
                var selectedButtonData = $(this).data('error-source');
                if (typeof (selectedButtonData) !== "undefined") {
                    $('.error-summary').removeClass('hidden');
                    $('.error-summary-list span.' + selectedButtonData).removeClass('hidden');
                    $('.error-summary').focus();
                }

                e.preventDefault();
            }

            if (selectedOption.length === 1) {
                var selectedAction = $(selectedOption).data("action");
                if (typeof (selectedAction) !== "undefined") {
                    $(this).attr("action", selectedAction);
                }
            }
        });
})();