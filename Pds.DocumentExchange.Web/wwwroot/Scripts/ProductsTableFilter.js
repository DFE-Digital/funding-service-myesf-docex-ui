$(document).ready(function () {
    const ascending = 'M8.1875 9.5L10.9609 3.95703L13.7344 9.5H8.1875Z';
    const descending = 'M13.7344 12.0781L10.9609 17.6211L8.1875 12.0781H13.7344Z';

    // setting the default descending icon for identifier column
    $($('.products-table th')[1])
    .find('.sort-icon path')
        .attr('id', 'descending')
        .attr('d', descending)

$('.products-table th').click(function () {
    var selectedHeaderIndex = $(this).index();

    // edit hyperlink action
    if (selectedHeaderIndex === 6) return 0;

    var isAscending = -1;
    var sortDirection = $(this).find('.sort-icon path');
    const currentDirection = $(sortDirection).attr('id');

    if (currentDirection === 'ascending') {
        $(sortDirection).attr('id', 'descending').attr('d', descending)
        isAscending = -1;
    }
    else {
        $(sortDirection).attr('id', 'ascending').attr('d', ascending)
        isAscending = 1;
    }

    var rows = $('.products-table tbody tr').get();
    
    rows.sort(function(a, b) {
        var x = $(a).children('td').eq(selectedHeaderIndex).text().trim();
        var y = $(b).children('td').eq(selectedHeaderIndex).text().trim();

        if (selectedHeaderIndex == 0) {
            x = parseDate(x);
            y = parseDate(y);
            return (x < y ? -1 : x > y ? 1 : 0) * isAscending;
        }

        return isNaN(x) && isNaN(y)
            ? (x.localeCompare(y)) * isAscending
            : (parseFloat(x) - parseFloat(y)) * isAscending;
    });

    $.each(rows, function (index, row) { $('.products-table tbody').append(row); });

    // reset other header icons
    $('th').not(this).each(function() {
        $(this).find('.sort-icon path').attr('id', 'default-sort-direction').attr('d', `${ascending} ${descending}`)
        isAscending = -1;
    });
});

function parseDate(value) {
    if (value === '--' || !value) return new Date(0);

    var parts = value.split('/');
    return new Date(parts[2], parts[1] - 1, parts[0]);
    }
});