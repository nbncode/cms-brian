'use strict';

var dbCommon = {
    bindDatepicker: () =>
    {
        const arrows = {
            leftArrow: '<i class="la la-angle-left"></i>',
            rightArrow: '<i class="la la-angle-right"></i>'
        };

        const config = {
            autoclose: true,
            orientation: 'bottom right',
            todayHighlight: true,
            templates: arrows
        };

        $('#startDate').datepicker(config);
        $('#endDate').datepicker(config);
    },

    request: (url, data, successCallback) =>
    {
        $.ajax({
            type: 'GET',
            data: data,
            url: url,
            success: function (response)
            {
                successCallback(response);
            },
            error: function (xhr)
            {
                const errorMessage = `${xhr.status}:${xhr.statusText}`;
                console.error(errorMessage);
                console.log(xhr.responseText);
            }
        });
    },

    applyDataTable: () => 
    {
        $('table.dataTable').DataTable({
            // Ref - https://stackoverflow.com/a/4964423/1041457
            // Disable default ordering so as to show schemes sorted by created date
            "order": []
        });
    },

    intVal: (i) =>
    {
        return typeof i === 'string' ? i.replace(/[\%]/g, '') * 1 : typeof i === 'number' ? i : 0;
    },

    bindDataToTable: (url, filter, columns, tableId, divId, setFooter, enableSearch) =>
    {
        $(tableId).DataTable({
            "processing": true,
            "serverSide": true,
            "responsive": true,
            "filter": enableSearch !== undefined ? true : false,
            "ordering": false,
            "orderMulti": false,
            "autoWidth": false,
            "bLengthChange": false, // Hide 'Show 10 entries'
            "pageLength": 50,
            "ajax": {
                "url": url,
                "data": { dbFilter: filter }, // https://datatables.net/reference/option/ajax.data
                "type": 'POST',
                "datatype": 'json'
            },
            "columns": columns,  // https://datatables.net/examples/ajax/objects.html
            "footerCallback": function () // https://datatables.net/examples/advanced_init/footer_callback.html
            {
                if (setFooter) // Check if not null or undefined
                {
                    const api = this.api();
                    setFooter(api, dbCommon.intVal);
                }
            }
        });

        // Don't show alert. Show error instead
        $.fn.dataTable.ext.errMode = 'none';
        $(tableId).on('error.dt',
            function (e, settings, techNote, message) 
            {
                $(divId).html(`<p class="kt-font-bold"><i class="kt-font-warning flaticon-warning-sign"></i>&nbsp;${message}</p>`);
            });
    }
};