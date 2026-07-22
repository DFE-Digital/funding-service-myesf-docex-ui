(function () {
    const Pager = ".pager";
    const GoToPage = ".go-to-page";
    const PagerPrev = ".pager-prev";
    const PagerNext = ".pager-next";

    // Hide the non-JS search button:
    $(".search-input").removeClass("search-input-with-button");
    $(".search-submit").addClass("hidden");

    $("#searchAcademies").on("input", function () {
        updateResults();
    });

    $("#clearSearch").on("click", function (e) {
        e.preventDefault();
        $("#searchAcademies").val("");
        updateResults();
    });

    setupPageChangeEvent(true);

    function setupPageChangeEvent(setupPrevNext) {
        var selectors = [];

        selectors.push(GoToPage);

        if (setupPrevNext) {
            selectors.push(PagerPrev)
            selectors.push(PagerNext);
        }

        $(selectors.join(',')).on("click", function (e) {
            e.preventDefault();
            var pageNumber = parseInt($(this).data("page"));
            showPage(pageNumber);
        });
    }

    function updateResults() {
        $(".searchable-radio").removeClass("hidden").removeClass("filtered-out");

        // If there's a pager, get the page size:
        var pageSize = -1;
        if ($(Pager).length) {
            pageSize = parseInt($(Pager).data("page-size"));
        }

        // If there's a search term, show the 'clear search' button, otherwise hide it:
        var searchTerm = $("#searchAcademies").val();
        if (searchTerm.length > 0) {
            $("#clearSearch").removeClass("hidden");
        }
        else {
            $("#clearSearch").addClass("hidden");
        }

        // Apply the search filter and hide items if we've gone past the first page:
        var items = 0;
        $(".searchable-radio").each(function () {
            var searchValue = $(this).find("input").data("search-value");
            if (searchTerm.length > 0 &&
                searchValue.toLowerCase().indexOf(searchTerm.toLowerCase()) < 0) {
                $(this).addClass("hidden").addClass("filtered-out");
            }
            else {
                items++;
                if (pageSize > 0 && items > pageSize) {
                    $(this).addClass("hidden");
                }
            }
        });

        // Set the total academies message:
        if (items == 1) {
            $("#totalAcademiesMessage").html("1 academy");
        }
        else {
            $("#totalAcademiesMessage").html(items + " academies");
        }

        if (pageSize > 0) {
            resetPagination(pageSize, items, 1);
        }
    }

    function showPage(pageNumber) {
        var pageSize = -1;
        if ($(Pager).length) {
            pageSize = parseInt($(Pager).data("page-size"));
        }

        if (pageSize <= 0) {
            return;
        }

        var firstNumber = ((pageNumber - 1) * pageSize) + 1;
        var lastNumber = pageNumber * pageSize;

        var items = 0;
        $(".searchable-radio").not(".filtered-out").each(function () {
            items++;
            if (items >= firstNumber && items <= lastNumber) {
                $(this).removeClass("hidden");
            }
            else {
                $(this).addClass("hidden");
            }
        });

        resetPagination(pageSize, items, pageNumber);
    }

    function resetPagination(pageSize, items, pageNumber) {
        if (pageSize >= items) {
            $(Pager).addClass("hidden");
            return;
        }

        $(Pager).removeClass("hidden");

        if (pageNumber === 1) {
            $(PagerPrev).addClass("hidden");
        }
        else {
            $(PagerPrev).removeClass("hidden").data("page", pageNumber - 1);
        }

        var currentPage = 1;
        $(".pager-items").html("");
        var remainingItems = items;
        while (remainingItems > 0) {
            if (currentPage != pageNumber) {
                $(".pager-items").append("<li><a href='#' class='go-to-page' data-page='" + currentPage + "'>" + currentPage + "</a></li>");
            }
            else {
                $(".pager-items").append("<li>" + currentPage + "</li>");
            }

            remainingItems -= pageSize;
            currentPage++;
        }

        var firstNumber = ((pageNumber - 1) * pageSize) + 1;
        var lastNumber = pageNumber * pageSize;

        if (lastNumber >= items) {
            lastNumber = items;
            $(PagerNext).addClass("hidden");
        }
        else {
            $(PagerNext).removeClass("hidden").data("page", pageNumber + 1);
        }

        $(".pager-summary").html("Showing " + firstNumber + " to " + lastNumber + " of " + items + " academies");
        setupPageChangeEvent(false);
    }
})();