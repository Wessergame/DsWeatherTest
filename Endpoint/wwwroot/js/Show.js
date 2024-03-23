let totalPages = 1;

function formatDate(date) {
    const dateParsed = new Date(date);
    var year = dateParsed.getFullYear();
    var month = (dateParsed.getMonth() + 1).toString().padStart(2, '0');
    var day = dateParsed.getDate().toString().padStart(2, '0');
    return day + '.' + month + '.' + year;
}

function formatTime(time) {
    const [hoursNum, minutesNum] = time.split(':').map(Number);
    const hours = hoursNum.toString().padStart(2, '0');
    const minutes = minutesNum.toString().padStart(2, '0');
    return hours + ':' + minutes;
}

function formatDouble(number) {
    return number !== null ? number.toFixed(2) : '';
}

function wrapElementWithTableCell(html) {
    return '<tr><td colspan="12" class="text-center">' + html + '</td></tr>';
}

function loadData(pageNumber) {
    $('#tableBody').empty();
    $('#paginationList').empty();
    $('#tableBody').append(wrapElementWithTableCell(
        '<div class="d-flex align-items-center justify-content-center">' +
        '<div class="spinner-border m-5" role="status">' +
        '<span class="visually-hidden">Loading...</span>' +
        '</div></div>'));

    const selectedMonths = $('.monthCheckbox:checked').map(function () {
        return $(this).val();
    }).get();
    const selectedYears = $('.yearCheckbox:checked').map(function () {
        return $(this).val();
    }).get();
    const pageSize = $('#pageSizeInput').val();

    $.ajax({
        url: '/api/Ajax/GetWeatherData',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            selectedMonths: selectedMonths.length === 0 ? null : selectedMonths,
            selectedYears: selectedYears.length === 0 ? null : selectedYears,
            pageNumber: pageNumber,
            pageSize: pageSize
        }),
        success: function (data) {
            $('#tableBody').empty();
            if (data.totalData === 0)
                $('#tableBody').append(wrapElementWithTableCell('Нет данных для отображения'));

            totalPages = Math.ceil(data.totalData / pageSize);
            generatePagination(pageNumber)
            $.each(data.weathers, function (index, item) {
                let newRow =
                    '<tr>' +
                    '<td>' + formatDate(item.date) + '</td>' +
                    '<td>' + formatTime(item.time) + '</td>' +
                    '<td>' + formatDouble(item.temperature) + '</td>' +
                    '<td>' + formatDouble(item.relativeHumidity) + '</td>' +
                    '<td>' + formatDouble(item.dewPoint) + '</td>' +
                    '<td>' + formatDouble(item.atmosphericPressure) + '</td>' +
                    '<td>' + (item.windDirections != null ? item.windDirections.join(', ') : '') + '</td>' +
                    '<td>' + formatDouble(item.windSpeed) + '</td>' +
                    '<td>' + formatDouble(item.cloudiness) + '</td>' +
                    '<td>' + (item.lowerLimitCloud != null ? item.lowerLimitCloud : '') + '</td>' +
                    '<td>' + (item.visibility != null ? item.visibility : '') + '</td>' +
                    '<td>' + (item.weatherPhenomena != null ? item.weatherPhenomena : '') + '</td>' +
                    '</tr>';

                $('#tableBody').append(newRow);
            });
        }
    });
}

$(document).ready(function () {
    loadData(1);

    $('.monthCheckbox, .yearCheckbox, #pageSizeInput').change(function () {
        loadData(1);
    });
});

function generatePagination(page, expand = false) {
    const dividerStart = 6;
    const dividerEnd = 5;

    if (page > totalPages) page = 1;

    const paginationList = $('#paginationList');
    paginationList.empty();

    if (!expand && page - dividerStart > 1) {
        paginationList.append(
            '<li class="page-item" id="prevPagination">' +
            '<a class="page-link" aria-label="Previous" onclick="changePage(1)">' +
            '<span aria-hidden="true">&laquo;</span>' +
            '</a>' +
            '</li>');

        if (page - dividerStart - 2 > 1) {
            paginationList.append(
                '<li class="page-item">' +
                '<a class="page-link" onclick="changePage(1)">1</a>' +
                '</li>');

            paginationList.append(
                '<li class="page-item">' +
                '<a class="page-link" onclick="changePage(2)">2</a>' +
                '</li>');

            paginationList.append(
                '<li class="page-item">' +
                '<a class="page-link" onclick="generatePagination(' + page + ', true)">...</a>' +
                '</li>');
        }
    }

    for (
        let i = (page - dividerStart < 1 || expand ? 1 : page - dividerStart);
        i <= (page + dividerEnd > totalPages || expand ? totalPages : page + dividerEnd);
        i++) {
        paginationList.append(
            '<li class="page-item' + (i === page ? ' active' : '') + '">' +
            '<a class="page-link" onclick="changePage(' + i + ')">' + i + '</a>' +
            '</li>');
    }

    if (!expand && page < totalPages - dividerEnd) {
        if (page < totalPages - dividerEnd - 2) {
            paginationList.append(
                '<li class="page-item">' +
                '<a class="page-link" onclick="generatePagination(' + page + ', true)">...</a>' +
                '</li>');
            paginationList.append(
                '<li class="page-item">' +
                '<a class="page-link" onclick="changePage(' + (totalPages - 1) + ')">' + (totalPages - 1) + '</a>' +
                '</li>');

            paginationList.append('<li class="page-item">' +
                '<a class="page-link" onclick="changePage(' + totalPages + ')">' + totalPages + '</a>' +
                '</li>');
        }

        paginationList.append(
            '<li class="page-item" id ="nextPagination">' +
            '<a class="page-link" aria-label="Next" onclick="changePage(' + totalPages + ')">' +
            '<span aria-hidden="true">&raquo;</span>' +
            '</a>' +
            '</li>');
    }
}

function changePage(pageNumber) {
    if (pageNumber < 1) pageNumber = 1;
    else if (pageNumber >= totalPages) pageNumber = totalPages;

    loadData(pageNumber);
}

function enforceMinMax(el) {
    if (el.value != "") {
        if (parseInt(el.value) < parseInt(el.min)) {
            el.value = el.min;
        }
        if (parseInt(el.value) > parseInt(el.max)) {
            el.value = el.max;
        }
    }
}

function handleCheckboxClick(event) {
    event.stopPropagation();
}