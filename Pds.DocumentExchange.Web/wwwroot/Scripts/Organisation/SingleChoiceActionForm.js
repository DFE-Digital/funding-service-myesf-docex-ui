(function () {
    $("form").on("submit",
        function(e) {
            var selectedOption = $(".single-choice-action-form-option:checked");

            $('.error-summary, .error-message').addClass('hidden');
            $('fieldset').first().removeClass('after-error-summary');
            $('.error-message').parent().removeClass('form-group-error');

            if (selectedOption.length === 0) {
                $('.error-summary, .error-message').removeClass('hidden');
                $('.error-message').parent().addClass('form-group-error');
                $('fieldset').first().addClass('after-error-summary');
                
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