'use strict';

var filterData = null;
const dashboardController = '/Dashboard/';
const dateTypeMtd = 'MTD', dateTypeCd = 'CustomDate';
const colDetail = 'colDetail';
const rowColDetail = 'rowColDetail';

var dbCol = {

    initCol: () =>
    {
        dbCommon.bindDatepicker();

        filterData = { DateType: dateTypeMtd };
        dbCol.fetchColBySubGroup();
    },

    showLoader: (tag) =>
    {
        let div = '';
        let loadMsg = '';

        switch (tag)
        {
            case colDetail:
                div = $('#divColBySubGroup');
                loadMsg = 'Loading collection by sub group...';
                break;

            case rowColDetail:
                div = $('#divRowColDetail');
                loadMsg = 'Loading RO wise collection detail...';
                break;
        }

        const loader = `<div class="loader"><div class="kt-spinner home-page kt-spinner--v2 kt-spinner--md kt-spinner--info" style="display: block;"></div><span>${loadMsg}</span></div>`;
        div.html(loader);
    },

    applyFilter: () =>
    {
        let startDate = $('#startDate').val();
        let endDate = $('#endDate').val();

        if (startDate && endDate)
        {
            // Warning - Don't try to change format of the dates
            // moment create date object using the format of datepicker
            const mStartDate = moment(startDate, 'DD-MM-YYYY', true);
            const mEndDate = moment(endDate, 'DD-MM-YYYY', true);

            if (mEndDate.isSameOrAfter(mStartDate))
            {
                // So that MVC bind date properly
                startDate = mStartDate.format('YYYY-MM-DD');
                endDate = mEndDate.format('YYYY-MM-DD');

                filterData = {
                    DateType: dateTypeCd,
                    StartDate: startDate,
                    EndDate: endDate
                };

                dbCol.fetchColBySubGroup();
            } else
            {
                common.LoadErrorMessage('End Date must be greater than Start Date');
            }

        } else
        {
            common.LoadErrorMessage('Start Date and End Date cannot be blank.');
        }
    },

    fetchColByDateFilter: () =>
    {
        const option = $('#ddlDateFilter').val();

        $('#divStartDate').toggle(option === dateTypeCd);
        $('#divEndDate').toggle(option === dateTypeCd);
        $('#divFilter').toggle(option === dateTypeCd);

        // Fetch all if selected option is not 'custom date'
        if (option !== dateTypeCd)
        {
            filterData = { DateType: option };
            dbCol.fetchColBySubGroup();
        }
    },

    fetchColBySubGroup: () =>
    {
        console.log('Fetch collection by sub group');

        $('#divRowColDetail').empty();
        $('#divRowBranchColDetail').empty();

        dbCol.showLoader(colDetail);
        dbCommon.request(dashboardController + 'FetchColBySubGroup', filterData, function (response)
        {
            $('#divColBySubGroup').html(response);
        });
    },

    fetchColDetailForSubGroup: (subGroup) =>
    {
        console.log(`Fetch collection detail for '${subGroup}' sub group`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            Category: subGroup
        };

        dbCol.showLoader(rowColDetail);
        dbCommon.request(dashboardController + 'FetchRoWiseColDetail', dbFilter, function (response)
        {
            $('#divRoContainer').show();
            $('#divRowColDetail').html(response);
        });
    },

    fetchRoWiseBranchColDetail: (customerType) => 
    {
        console.log(`Fetch RO wise branch collection detail for '${customerType}' customer type`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            CustomerType: customerType
        };

        const table = `<table class="table table-bordered w-auto table-center" id="colBranchCustomerDetail">
                <thead class="thead-light">
                    <tr>
                        <th>Sr No</th>
                        <th>Branch Code</th>
                        <th>Branch Name</th>
                        <th>Total Customers</th>
                    </tr>
                </thead>                
                <tfoot>
                    <tr>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                    </tr>
                </tfoot>
            </table>`;

        // Prepare columns
        var columns = [
            { "data": 'SlNo' },
            { "data": 'BranchCode' },
            { "data": 'BranchName' },
            { "data": 'TotalCustomers' }
        ];

        // Create set footer function
        const setFooter = function (api, intVal)
        {
            // api is DataTable method
            // intVal is method defined in db.common.js

            const totalCustomers = api.column(3).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

            $(api.column(1).footer()).html('<b>Grand Total</b>');
            $(api.column(3).footer()).html(`<b>${totalCustomers}</b>`);
        };

        $('#divRowBranchColDetail').html(table);
        dbCommon.bindDataToTable('/dashboard/FetchRoWiseBranchColDetail', dbFilter, columns, '#colBranchCustomerDetail', '#divRowCustomerCboDetail', setFooter);
    }
};