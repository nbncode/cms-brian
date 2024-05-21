'use strict';

var filterData = null;
const dashboardController = '/Dashboard/';
const dateTypeMtd = 'MTD', dateTypeCd = 'CustomDate';
const invDetail = 'invDetail', invForBranch = 'invForBranch';

var dbInv = {

    initInv: () => {
        dbCommon.bindDatepicker();

        filterData = { DateType: dateTypeMtd };
        dbInv.fetchInvByBranch();
    },

    showLoader: (tag) => {
        let div = '';
        let loadMsg = '';

        switch (tag) {
            case invDetail:
                div = $('#divInvByBranch');
                loadMsg = 'Loading inventory details...';
                break;

            case invForBranch:
                div = $('#divInvForBranch');
                loadMsg = 'Loading inventory for branch...';
                break;
        }

        const loader = `<div class="loader"><div class="kt-spinner home-page kt-spinner--v2 kt-spinner--md kt-spinner--info" style="display: block;"></div><span>${loadMsg}</span></div>`;
        div.html(loader);
    },

    applyFilter: () => {
        let startDate = $('#startDate').val();
        let endDate = $('#endDate').val();

        if (startDate && endDate) {
            // Warning - Don't try to change format of the dates
            // moment create date object using the format of datepicker
            const mStartDate = moment(startDate, 'DD-MM-YYYY', true);
            const mEndDate = moment(endDate, 'DD-MM-YYYY', true);

            if (mEndDate.isSameOrAfter(mStartDate)) {
                // So that MVC bind date properly
                startDate = mStartDate.format('YYYY-MM-DD');
                endDate = mEndDate.format('YYYY-MM-DD');

                filterData = {
                    DateType: dateTypeCd,
                    StartDate: startDate,
                    EndDate: endDate
                };

                dbInv.fetchInvByBranch();
            } else {
                common.LoadErrorMessage('End Date must be greater than Start Date');
            }

        } else {
            common.LoadErrorMessage('Start Date and End Date cannot be blank.');
        }
    },

    fetchInvByDateFilter: () => {
        const option = $('#ddlDateFilter').val();

        $('#divStartDate').toggle(option === dateTypeCd);
        $('#divEndDate').toggle(option === dateTypeCd);
        $('#divFilter').toggle(option === dateTypeCd);

        // Fetch all if selected option is not 'custom date'
        if (option !== dateTypeCd) {
            filterData = { DateType: option };
            dbInv.fetchInvByBranch();
        }
    },

    fetchInvByBranch: () => {
        console.log('Fetch inventories group by branch');


        // Create table
        var table = `<div class="table-responsive">
                        <table class="table table-bordered w-auto table-center" id="InvByBranch">
                            <thead class="thead-light">
                                <tr>
                                   <th>Sr No</th>
                                   <th>Branch Code</th>
                                   <th>Branch Name</th>
                                   <th>Stock Days</th>
                                   <th>Avg. Sales (Lacs)</th>
                                   <th>Stock (In lacs)</th>
                                   <th>No. of Partlines In Stock</th>
                                   <th>Action</th>
                                </tr>
                            </thead>
                             <tfoot>
		                        <tr>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                </tr>
	                        </tfoot>
                        </table>
                    </div>`;

        // Prepare columns
        var columns = [
            { "data": 'SlNo' },
            { "data": 'BranchCode' },
            { "data": 'BranchName' },
            { "data": 'StockDays' },
            { "data": 'AverageSales' },
            { "data": 'StockPrice' },
            { "data": 'PartLinesInStock' },
            {
                "render": function (data, type, row, meta) {
                    return `<a href="javascript:void(0);" onclick="dbInv.fetchInvForBranch('${row.BranchCode}')">View</a>`;
                }
            }
        ];

        // Create set footer function
        const setFooter = function (api, intVal) {
            // api is DataTable method
            // intVal is method defined in db.common.js

            const totalStockDays = api.column(3).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalAvgSales = api.column(4).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalStock = api.column(5).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalNoStock = api.column(6).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

            $(api.column(1).footer()).html('<b>Total</b>');
            $(api.column(3).footer()).html(`<b>${totalStockDays}</b>`);
            $(api.column(4).footer()).html(`<b>${totalAvgSales}</b>`);
            $(api.column(5).footer()).html(`<b>${totalStock}</b>`);
            $(api.column(6).footer()).html(`<b>${totalNoStock}</b>`);
        };

        $('#divInvByBranch').empty();
        $('#divInvByBranch').html(table);
        dbCommon.bindDataToTable('/dashboard/FetchInvByBranch', filterData, columns, '#InvByBranch', '#divInvByBranch', setFooter);
    },

    fetchBranchInv: (branchCode) => {
        console.log(`Fetch inventories for branch with '${branchCode}' branch code`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            BranchCode: branchCode
        };

        // Create table
        var table = `<div class="table-responsive">
                        <table class="table table-bordered w-auto table-center" id="InvBranchInv">
                            <thead class="thead-light">
                                <tr>
                                   <th>Branch Code</th>
                                   <th>Branch Name</th>
                                   <th>Stock Days</th>
                                   <th>Avg. Sales (Lacs)</th>
                                   <th>Stock (In lacs)</th>
                                   <th>No. of Partlines In Stock</th>
                                </tr>
                            </thead>
                        </table>
                    </div>`;

        // Prepare columns
        var columns = [
            { "data": 'SlNo' },
            { "data": 'BranchCode' },
            { "data": 'BranchName' },
            { "data": 'StockDays' },
            { "data": 'AverageSales' },
            { "data": 'StockPrice' },
            { "data": 'PartLinesInStock' },
        ];

        $('#divBranchInv').empty();
        $('#divBranchInv').html(table);
        dbCommon.bindDataToTable('/dashboard/FetchInvByBranch', dbFilter, columns, '#InvBranchInv', '#divBranchInv');
    },

    fetchInvForBranch: (branchCode) => {
        dbInv.fetchBranchInv(branchCode);
        console.log(`Fetch inventories for branch with '${branchCode}' branch code`);
      
        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            BranchCode: branchCode
        };

        $('#divSearch').hide();
        $('#divInvBranch').hide();

        //// Prepare inventory branch row
        //var curRow = $(anchor).closest('tr');
        //const row = `<tr>
        //                <td>${branchCode}</td>
        //                <td>${curRow.find('td:eq(2)').text()}</td>
        //                <td>${curRow.find('td:eq(3)').text()}</td>
        //                <td>${curRow.find('td:eq(4)').text()}</td>
        //                <td>${curRow.find('td:eq(5)').text()}</td>
        //                <td>${curRow.find('td:eq(6)').text()}</td>  
        //            </tr>`;
        //$('#divInvBranch').show();
        ////$('#divInvBranch > table > tbody').append(row);
        //$('#divInvBranch > table > tbody').html(row);






        // Create table
        var table = `<div class="table-responsive">
                        <table class="table table-bordered w-auto table-center" id="InvForBranch">
                            <thead class="thead-light">
                                <tr>
                                   <th>Sr No</th>
                                   <th>Part Group</th>
                                   <th>Part Category</th>
                                   <th>Root Part Number</th>
                                   <th>Part Number</th>
                                   <th>Part Description</th>
                                   <th>MRP</th>
                                   <th>Avg. Consumption (3 Months)</th>
                                   <th>No of Stock</th>
                                </tr>
                            </thead>
                             <tfoot>
		                        <tr>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                </tr>
	                        </tfoot>
                        </table>
                    </div>`;

        // Prepare columns
        var columns = [
            { "data": 'SlNo' },
            { "data": 'PartGroup' },
            { "data": 'PartCategory' },
            { "data": 'RootPartNumber' },
            { "data": 'PartNumber' },
            { "data": 'PartDescription' },
            { "data": 'Mrp' },
            { "data": 'AvgConsumption' },
            { "data": 'NumberOfStock' }
        ];

        $('#divInvForBranch').empty();
        $('#divInvForBranch').html(table);
        dbCommon.bindDataToTable('/dashboard/FetchInvForBranch', dbFilter, columns, '#InvForBranch', '#divInvForBranch');

        // Show search option if result found
        //if ($('#divInvForBranch .table-responsive > table').length) {
        //if ($('#InvForBranch').length) {
        //    $('#divSearch').show();
        //    $('#txtPartNumber').show();
        //} else {
        //    $('#txtPartNumber').hide();
        //}
    },

    searchBwInvByPartNum: () => {
        var partNumber = $('#txtPartNumber').val().toLowerCase().trim();
        debugger;
        const rows = $('#InvForBranch > tbody > tr');
        rows.filter(function () {
            const tr = $(this);
            const partNumInCol = tr.find('td').eq(4).html().toLowerCase().trim();
            tr.toggle(partNumInCol.indexOf(partNumber) > -1);
        });
    }
};