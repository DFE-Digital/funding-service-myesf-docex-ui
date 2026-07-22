// added in order to toggle "Request an accessible format" content. used only by ReceivedDocuments.cshtml
$("#btn-received-documents-accessibility").click(function () {
    $('#received-documents-accessibility-content').toggle();
});

// added in order to toggle the display of memry cache in support tools content. used only by OrgDataValidationSearchResult.cshtml
$("#chkbx-display-memory-cache").click(function () {
    $('#memory-cache-heading').toggle();
    $('#memory-cache-content').toggle();
    $('#memory-cache-content-raw').toggle();
});

//added in order to provide conditional showing/hiding Period filter depending on the direction. used only by DeleteDocuments.cshtml
$("input[name='ExchangeDocumentDirection']").click(function () {
    customShowHideFilters();
});

function customShowHideFilters() {

    if ($('#radio-uploaded-by-provider') && $('#text-box-period') && $('#radio-uploaded-by-provider')[0].checked) {

        $('#text-box-period').val('');

        $('#period-container').hide();
    }
    else {
        $('#period-container').show();
    }
}
