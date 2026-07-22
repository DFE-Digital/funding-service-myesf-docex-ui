document.addEventListener("DOMContentLoaded", function () {

    const currentDate = new Date();

    //MI Report From date inputs
    const miReportFromDayInputElement = document.getElementById('mi-report-fromday-input');
    const miReportFromMonthInputElement = document.getElementById('mi-report-frommonth-input');
    const miReportFromYearInputElement = document.getElementById('mi-report-fromyear-input');

    //MI Report To date inputs
    const miReportToDayInputElement = document.getElementById('mi-report-today-input');
    const miReportToMonthInputElement = document.getElementById('mi-report-tomonth-input');
    const miReportToYearInputElement = document.getElementById('mi-report-toyear-input');

    //MI Report Form group side error ribbons
    const miReportFromDateFormGroupElement = document.getElementById('mi-report-fromdate-form-group');
    const miReportToDateFormGroupElement = document.getElementById('mi-report-todate-form-group');

    //MI Report Error text
    const miReportFromDateError = document.getElementById("mi-report-fromdate-error");
    const miReportToDateError = document.getElementById("mi-report-todate-error");

    const miReportSubmitButton = document.getElementById("mi-report-submit");

    //MI Report validation variables
    let miReportIsToDateValid;
    let miReportIsFromDateValid;

    document.getElementById("mi-report-form").addEventListener("submit", function (e) {
        e.preventDefault();

        //Clear errors
        clearErrors();

        //Validate
        let fromDate;
        let toDate

        miReportIsFromDateValid = isValidDateParameters(miReportFromDayInputElement, miReportFromMonthInputElement, miReportFromYearInputElement);

        if (!miReportIsFromDateValid) {
            showErrorMessage(miReportFromDateFormGroupElement, miReportFromDateError, 'Invalid date');
            miReportIsFromDateValid = false;
        } else {
            fromDate = new Date(miReportFromYearInputElement.value, miReportFromMonthInputElement.value - 1, miReportFromDayInputElement.value);

            var threeYearsAgoDate = new Date(new Date().setUTCFullYear(currentDate.getUTCFullYear() - 3));

            if (fromDate < threeYearsAgoDate) {
                showErrorMessage(miReportFromDateFormGroupElement, miReportFromDateError, 'The date range must be within the last 3 years');
                miReportIsFromDateValid = false;
            }

            if (fromDate > currentDate) {
                showErrorMessage(miReportFromDateFormGroupElement, miReportFromDateError, 'Date should not be a future date');
                miReportIsFromDateValid = false;
            }
        }

        miReportIsToDateValid = isValidDateParameters(miReportToDayInputElement, miReportToMonthInputElement, miReportToYearInputElement);

        if (!miReportIsToDateValid) {
            showErrorMessage(miReportToDateFormGroupElement, miReportToDateError, 'Invalid date');
            miReportIsFromDateValid = false;
        } else {
            toDate = new Date(miReportToYearInputElement.value, miReportToMonthInputElement.value - 1, miReportToDayInputElement.value);

            if (toDate > currentDate) {
                showErrorMessage(miReportToDateFormGroupElement, miReportToDateError, 'Date should not be a future date');
                miReportIsToDateValid = false;
            }

            if (fromDate > toDate) {
                showErrorMessage(miReportFromDateFormGroupElement, miReportFromDateError, "'From' date cannot be later than 'To' date'");
                miReportIsToDateValid = false;
            }

            if (miReportIsFromDateValid && ((toDate - fromDate) / 86400000) >= 31) {
                showErrorMessage(miReportFromDateFormGroupElement, miReportFromDateError, 'The date range cannot be more than 31 days');
                showErrorMessage(miReportToDateFormGroupElement, miReportToDateError, 'The date range cannot be more than 31 days');
                miReportIsFromDateValid = false;
                miReportIsToDateValid = false;
            }
        }

        if (miReportIsFromDateValid && miReportIsToDateValid) {
            e.target.submit();

            miReportSubmitButton.disabled = true;
            
            miReportFromDateFormGroupElement.addEventListener('keyup', function (event) {
                miReportSubmitButton.disabled = false;
            });

            miReportToDateFormGroupElement.addEventListener('keyup', function (event) {
                miReportSubmitButton.disabled = false;
            });
        }
    });

    function isValidDateParameters(dayInputElement, monthInputElement, yearInputElement) {

        let dateValidParts = [true, true, true];
        const numberOfdays = new Date(yearInputElement.value, monthInputElement.value, 0).getDate();

        if (dayInputElement.value === "" || !(dayInputElement.value >= 1 && dayInputElement.value <= numberOfdays)) {
            dateValidParts[0] = false;
            dayInputElement.classList.add('govuk-input--error')
        }

        if (monthInputElement.value === "" || !(monthInputElement.value >= 1 && monthInputElement.value <= 12)) {
            dateValidParts[1] = false;
            monthInputElement.classList.add('govuk-input--error')
        }

        if (yearInputElement.value === "" || !(yearInputElement.value > 0 && yearInputElement.value <= 9999)) {
            dateValidParts[2] = false;
            yearInputElement.classList.add('govuk-input--error')
        }

        return dateValidParts.every(value => value === true);
    }

    function showErrorMessage(formGroupElement, htmlElement, errorMessage) {
        htmlElement.innerText = errorMessage;
        htmlElement.style.display = 'block';
        formGroupElement.classList.add("govuk-form-group--error");
    }

    function clearErrors() {
        miReportIsFromDateValid = true;
        miReportIsToDateValid = true;

        miReportFromDateError.innerText = '';
        miReportToDateError.innerText = '';

        miReportFromDayInputElement.classList.remove("govuk-input--error");
        miReportFromMonthInputElement.classList.remove("govuk-input--error");
        miReportFromYearInputElement.classList.remove("govuk-input--error");

        miReportToDayInputElement.classList.remove("govuk-input--error");
        miReportToMonthInputElement.classList.remove("govuk-input--error");
        miReportToYearInputElement.classList.remove("govuk-input--error");

        miReportFromDateFormGroupElement.classList.remove("govuk-form-group--error");
        miReportToDateFormGroupElement.classList.remove("govuk-form-group--error");
    }
});