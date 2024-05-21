'use strict';

var filterData = null;
const dashboardController = '/Dashboard/';
const dateTypeMtd = 'MTD', dateTypeCd = 'CustomDate';
const osDetail = 'osDetail';
const rowOsDetailCust = 'rowOsDetailCust';
const sewOsDetailCust = 'sewOsDetailCust';
const cswOsDetailCust = 'cswOsDetailCust';

var dbOuts = {

    initOs: () => {
        dbCommon.bindDatepicker();

        filterData = { DateType: dateTypeMtd };
        dbOuts.fetchOsBySubGroup();
    },

    showLoader: (tag) => {
        let div = '';
        let loadMsg = '';

        switch (tag) {
            case osDetail:
                div = $('#divOsBySubGroup');
                loadMsg = 'Loading outstanding by sub group...';
                break;

            case rowOsDetailCust:
                div = $('#divSubGroupOsDetail');
                loadMsg = 'Loading RO wise outstanding detail...';
                break;
            case sewOsDetailCust:
                div = $('#divSubGroupOsDetail');
                loadMsg = 'Loading SE wise outstanding detail...';
                break;
            case cswOsDetailCust:
                div = $('#divSubGroupOsDetail');
                loadMsg = 'Loading CS wise outstanding detail...';
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

                dbOuts.fetchOsBySubGroup();
            } else {
                common.LoadErrorMessage('End Date must be greater than Start Date');
            }

        } else {
            common.LoadErrorMessage('Start Date and End Date cannot be blank.');
        }
    },

    fetchOsByDateFilter: () => {
        const option = $('#ddlDateFilter').val();

        $('#divStartDate').toggle(option === dateTypeCd);
        $('#divEndDate').toggle(option === dateTypeCd);
        $('#divFilter').toggle(option === dateTypeCd);

        // Fetch all if selected option is not 'custom date'
        if (option !== dateTypeCd) {
            filterData = { DateType: option };
            dbOuts.fetchOsBySubGroup();
        }
    },

    emptyContainers: () => {
        $('#divSubGroupOsDetail').empty();

        $('#divRowBranchCustomerSaleDetail').empty();
        $('#divRowCustomerSaleDetail').empty();

        $('#divSewBranchCustomerSaleDetail').empty();
        $('#divSewCustomerSaleDetail').empty();

        $('#divCswCustomerOsDetail').empty();

    },

    fetchOsBySubGroup: () => {
        console.log('Fetch outstanding by sub group');

        dbOuts.showLoader(osDetail);
        dbCommon.request(dashboardController + 'FetchOsBySubGroup', filterData, function (response) {
            $('#divOsBySubGroup').html(response);
        });
    },

    fetchGroupWiseOsDetail: (category) => {
        filterData.Category = null;
        console.log(`Fetch outstanding detail by '${category}' category`);
        dbOuts.emptyContainers();

        filterData.Category = category;

        let groupOsDetail = '';
        let actionMethod = '';

        $('#divRoContainer').toggle(category === 'RO');
        $('#divSeContainer').toggle(category === 'SE');
        $('#divCsContainer').toggle(category === 'CS');

        switch (category) {
            case 'RO':
                actionMethod = 'FetchRoWiseOsCustomerType';
                groupOsDetail = rowOsDetailCust;
                break;

            case 'SE':
                actionMethod = 'FetchSeWiseOsCustomerType';
                groupOsDetail = sewOsDetailCust;
                break;

            case 'CS':
                actionMethod = 'FetchCsWiseOsCustomerType';
                groupOsDetail = cswOsDetailCust;
                break;
        }
        if (actionMethod) {
            const dbFilter = {
                DateType: filterData.DateType,
                StartDate: filterData.StartDate,
                EndDate: filterData.EndDate,
                Category: category
            };

            dbOuts.showLoader(groupOsDetail);
            dbCommon.request(dashboardController + actionMethod, dbFilter, function (response) {
                $('#divSubGroupOsDetail').html(response);
            });
        } else {
            console.error('No action method was set.');
        }

    },

    // Ro wise outstanding
    fetchRoWiseBranchOsDetail: (customerType) => {
        filterData.CustomerType = null;
        filterData.CustomerType = customerType;
        console.log(`Fetch RO wise outstanding detail by '${customerType}' customer type`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            Category: filterData.Category,
            CustomerType: filterData.CustomerType
        };

        // Create table
        var table = `<div class="table-responsive">
                        <table class="table table-bordered w-auto table-center" id="RoWiseBranchOs">
                            <thead class="thead-light">
                                <tr>
                                    <th>Sr No</th>
                                    <th>Branch Code</th>
                                    <th>Branch Name</th>
                                    <th>Total Customers</th>
                                    <th>Outstanding Days</th>
                                    <th>Outstanding</th>
                                    <th>Credit Limit</th>
                                    <th>Critical payment</th>
                                    <th>0-14 Days</th>
                                    <th>14-28 Days</th>
                                    <th>28-50 Days</th>
                                    <th>50-70 days</th>
                                    <th>> 70 days</th>
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
            { "data": 'NoOfCustomers' },
            { "data": 'OutstandingDays' },
            { "data": 'Outstanding' },
            { "data": 'CreditLimit' },
            { "data": 'CriticalPayment' },
            { "data": 'ZeroToFourteenDays' },
            { "data": 'FourteenToTwentyeightDays' },
            { "data": 'TwentyeightToFiftyDays' },
            { "data": 'FiftyToSeventyDays' },
            { "data": 'MoreThenSeventyDays' },
            {
                "render": function (data, type, row, meta) {
                    return `<a href="javascript:void(0);" onclick="dbOuts.fetchRoWiseCustomerOsDetail('${row.BranchCode}')">View</a>`;
                }
            }
        ];

        // Create set footer function
        const setFooter = function (api, intVal) {
            // api is DataTable method
            // intVal is method defined in db.common.js

            const totalNoOfCustomers = api.column(3).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalOutstandingDays = api.column(4).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalOutstanding = api.column(5).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalCreditLimit = api.column(6).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

            const totalCP = api.column(7).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const total0To14Days = api.column(8).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const total14To28Days = api.column(9).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const total28To50Days = api.column(10).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const total50To70Days = api.column(11).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalMore70Days = api.column(12).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

            $(api.column(1).footer()).html('<b>Total</b>');
            $(api.column(3).footer()).html(`<b>${totalNoOfCustomers}</b>`);
            $(api.column(4).footer()).html(`<b>${totalOutstandingDays}</b>`);
            $(api.column(5).footer()).html(`<b>${totalOutstanding}</b>`);
            $(api.column(6).footer()).html(`<b>${totalCreditLimit}</b>`);
            $(api.column(7).footer()).html(`<b>${totalCP}</b>`);
            $(api.column(8).footer()).html(`<b>${total0To14Days}</b>`);
            $(api.column(9).footer()).html(`<b>${total14To28Days}</b>`);
            $(api.column(10).footer()).html(`<b>${total28To50Days}</b>`);
            $(api.column(11).footer()).html(`<b>${total50To70Days}</b>`);
            $(api.column(12).footer()).html(`<b>${totalMore70Days}</b>`);
        };


        $('#divRowBranchCustomerSaleDetail').empty();
        $('#divRowBranchCustomerSaleDetail').html(table);
        dbCommon.bindDataToTable('/dashboard/FetchRoWiseOsBranchDetails', dbFilter, columns, '#RoWiseBranchOs', '#divRowBranchCustomerSaleDetail', setFooter);

    },

    fetchRoWiseCustomerOsDetail: (branchCode) => {

        console.log(`Fetch RO wise outstanding detail by '${branchCode}' branch code`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            Category: filterData.Category,
            CustomerType: filterData.CustomerType,
            BranchCode: branchCode
        };

        // Create table
        var table = `<div class="table-responsive">
                        <table class="table table-bordered w-auto table-center" id="RoWiseCustomerOs">
                            <thead class="thead-light">
                                <tr>
                                    <th>Sr No</th>
                                    <th>Customer Code</th>
                                    <th>Customer Name</th>
                                    <th>Customer Type</th>
                                    <th>Outstanding Days</th>
                                    <th>Outstanding</th>
                                    <th>Credit Limit</th>
                                    <th>Critical payment</th>
                                    <th>0-14 Days</th>
                                    <th>14-28 Days</th>
                                    <th>28-50 Days</th>
                                    <th>50-70 days</th>
                                    <th>> 70 days</th>
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
            { "data": 'CustomerCode' },
            { "data": 'CustomerName' },
            { "data": 'CustomerType' },
            { "data": 'OutstandingDays' },
            { "data": 'Outstanding' },
            { "data": 'CreditLimit' },
            { "data": 'CriticalPayment' },
            { "data": 'ZeroToFourteenDays' },
            { "data": 'FourteenToTwentyeightDays' },
            { "data": 'TwentyeightToFiftyDays' },
            { "data": 'FiftyToSeventyDays' },
            { "data": 'MoreThenSeventyDays' }
        ];

        // Create set footer function
        const setFooter = function (api, intVal) {
            // api is DataTable method
            // intVal is method defined in db.common.js
           
            const totalOutstandingDays = api.column(4).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalOutstanding = api.column(5).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalCreditLimit = api.column(6).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

            const totalCP = api.column(7).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const total0To14Days = api.column(8).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const total14To28Days = api.column(9).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const total28To50Days = api.column(10).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const total50To70Days = api.column(11).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalMore70Days = api.column(12).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

            $(api.column(1).footer()).html('<b>Total</b>');
            $(api.column(4).footer()).html(`<b>${totalOutstandingDays}</b>`);
            $(api.column(5).footer()).html(`<b>${totalOutstanding}</b>`);
            $(api.column(6).footer()).html(`<b>${totalCreditLimit}</b>`);
            $(api.column(7).footer()).html(`<b>${totalCP}</b>`);
            $(api.column(8).footer()).html(`<b>${total0To14Days}</b>`);
            $(api.column(9).footer()).html(`<b>${total14To28Days}</b>`);
            $(api.column(10).footer()).html(`<b>${total28To50Days}</b>`);
            $(api.column(11).footer()).html(`<b>${total50To70Days}</b>`);
            $(api.column(12).footer()).html(`<b>${totalMore70Days}</b>`);
        };

        $('#divRowCustomerSaleDetail').empty();
        $('#divRowCustomerSaleDetail').html(table);
        dbCommon.bindDataToTable('/dashboard/FetchRoWiseCustomerOsDetails', dbFilter, columns, '#RoWiseCustomerOs', '#divRowCustomerSaleDetail', setFooter);

    },


    // SE wise outstanding
    fetchSeWiseBranchOsDetail: (customerType) => {
        filterData.CustomerType = null;
        filterData.CustomerType = customerType;
        console.log(`Fetch SE wise outstanding detail by '${customerType}' customer type`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            Category: filterData.Category,
            CustomerType: filterData.CustomerType
        };

        // Create table
        var table = `<div class="table-responsive">
                        <table class="table table-bordered w-auto table-center" id="SeWiseBranchOs">
                            <thead class="thead-light">
                                <tr>
                                    <th>Sr No</th>
                                    <th>Branch Code</th>
                                    <th>Sales Exe. Name</th>
                                    <th>Total Customers</th>
                                    <th>Outstanding Days</th>
                                    <th>Outstanding</th>
                                    <th>Credit Limit</th>
                                    <th>Critical payment</th>
                                    <th>0-14 Days</th>
                                    <th>14-28 Days</th>
                                    <th>28-50 Days</th>
                                    <th>50-70 days</th>
                                    <th>> 70 days</th>
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
            { "data": 'SalesExecutiveName' },
            { "data": 'NoOfCustomers' },
            { "data": 'OutstandingDays' },
            { "data": 'Outstanding' },
            { "data": 'CreditLimit' },
            { "data": 'CriticalPayment' },
            { "data": 'ZeroToFourteenDays' },
            { "data": 'FourteenToTwentyeightDays' },
            { "data": 'TwentyeightToFiftyDays' },
            { "data": 'FiftyToSeventyDays' },
            { "data": 'MoreThenSeventyDays' },
            {
                "render": function (data, type, row, meta) {
                    return `<a href="javascript:void(0);" onclick="dbOuts.fetchSeWiseCustomerOsDetail('${row.BranchCode}')">View</a>`;
                }
            }
        ];

        // Create set footer function
        const setFooter = function (api, intVal) {
            // api is DataTable method
            // intVal is method defined in db.common.js

            const totalNoOfCustomers = api.column(3).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalOutstandingDays = api.column(4).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalOutstanding = api.column(5).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalCreditLimit = api.column(6).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

            const totalCP = api.column(7).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const total0To14Days = api.column(8).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const total14To28Days = api.column(9).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const total28To50Days = api.column(10).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const total50To70Days = api.column(11).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalMore70Days = api.column(12).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

            $(api.column(1).footer()).html('<b>Total</b>');
            $(api.column(3).footer()).html(`<b>${totalNoOfCustomers}</b>`);
            $(api.column(4).footer()).html(`<b>${totalOutstandingDays}</b>`);
            $(api.column(5).footer()).html(`<b>${totalOutstanding}</b>`);
            $(api.column(6).footer()).html(`<b>${totalCreditLimit}</b>`);
            $(api.column(7).footer()).html(`<b>${totalCP}</b>`);
            $(api.column(8).footer()).html(`<b>${total0To14Days}</b>`);
            $(api.column(9).footer()).html(`<b>${total14To28Days}</b>`);
            $(api.column(10).footer()).html(`<b>${total28To50Days}</b>`);
            $(api.column(11).footer()).html(`<b>${total50To70Days}</b>`);
            $(api.column(12).footer()).html(`<b>${totalMore70Days}</b>`);
        };


        $('#divSewBranchCustomerSaleDetail').empty();
        $('#divSewBranchCustomerSaleDetail').html(table);
        dbCommon.bindDataToTable('/dashboard/FetchSeWiseOsBranchDetails', dbFilter, columns, '#SeWiseBranchOs', '#divSewBranchCustomerSaleDetail', setFooter);

    },

    fetchSeWiseCustomerOsDetail: (branchCode) => {

        console.log(`Fetch SE wise outstanding detail by '${branchCode}' branch code`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            Category: filterData.Category,
            CustomerType: filterData.CustomerType,
            BranchCode: branchCode
        };

        // Create table
        var table = `<div class="table-responsive">
                        <table class="table table-bordered w-auto table-center" id="SeWiseCustomerOs">
                            <thead class="thead-light">
                                <tr>
                                    <th>Sr No</th>
                                    <th>Customer Code</th>
                                    <th>Customer Name</th>
                                    <th>Customer Type</th>
                                    <th>Outstanding Days</th>
                                    <th>Outstanding</th>
                                    <th>Credit Limit</th>
                                    <th>Critical payment</th>
                                    <th>0-14 Days</th>
                                    <th>14-28 Days</th>
                                    <th>28-50 Days</th>
                                    <th>50-70 days</th>
                                    <th>> 70 days</th>
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
            { "data": 'CustomerCode' },
            { "data": 'CustomerName' },
            { "data": 'CustomerType' },
            { "data": 'OutstandingDays' },
            { "data": 'Outstanding' },
            { "data": 'CreditLimit' },
            { "data": 'CriticalPayment' },
            { "data": 'ZeroToFourteenDays' },
            { "data": 'FourteenToTwentyeightDays' },
            { "data": 'TwentyeightToFiftyDays' },
            { "data": 'FiftyToSeventyDays' },
            { "data": 'MoreThenSeventyDays' }
        ];

        // Create set footer function
        const setFooter = function (api, intVal) {
            // api is DataTable method
            // intVal is method defined in db.common.js

            const totalOutstandingDays = api.column(4).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalOutstanding = api.column(5).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalCreditLimit = api.column(6).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

            const totalCP = api.column(7).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const total0To14Days = api.column(8).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const total14To28Days = api.column(9).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const total28To50Days = api.column(10).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const total50To70Days = api.column(11).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalMore70Days = api.column(12).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

            $(api.column(1).footer()).html('<b>Total</b>');
            $(api.column(4).footer()).html(`<b>${totalOutstandingDays}</b>`);
            $(api.column(5).footer()).html(`<b>${totalOutstanding}</b>`);
            $(api.column(6).footer()).html(`<b>${totalCreditLimit}</b>`);
            $(api.column(7).footer()).html(`<b>${totalCP}</b>`);
            $(api.column(8).footer()).html(`<b>${total0To14Days}</b>`);
            $(api.column(9).footer()).html(`<b>${total14To28Days}</b>`);
            $(api.column(10).footer()).html(`<b>${total28To50Days}</b>`);
            $(api.column(11).footer()).html(`<b>${total50To70Days}</b>`);
            $(api.column(12).footer()).html(`<b>${totalMore70Days}</b>`);
        };

        $('#divSewCustomerSaleDetail').empty();
        $('#divSewCustomerSaleDetail').html(table);
        dbCommon.bindDataToTable('/dashboard/FetchSeWiseCustomerOsDetails', dbFilter, columns, '#SeWiseCustomerOs', '#divSewCustomerSaleDetail', setFooter);

    },

    // CS wise outstanding
    fetchCsWiseCustomerOsDetail: (customerType) => {

        console.log(`Fetch Cs wise outstanding detail by '${customerType}' customer type`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            Category: filterData.Category,
            CustomerType: customerType
            
        };

        // Create table
        var table = `<div class="table-responsive">
                        <table class="table table-bordered w-auto table-center" id="CsWiseCustomerOs">
                            <thead class="thead-light">
                                <tr>
                                    <th>Sr No</th>
                                    <th>Branch Code</th>
                                    <th>Customer Code</th>
                                    <th>Customer Name</th>
                                    <th>Customer Type</th>
                                    <th>Outstanding Days</th>
                                    <th>Outstanding</th>
                                    <th>Credit Limit</th>
                                    <th>Critical payment</th>
                                    <th>0-14 Days</th>
                                    <th>14-28 Days</th>
                                    <th>28-50 Days</th>
                                    <th>50-70 days</th>
                                    <th>> 70 days</th>
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
            { "data": 'CustomerCode' },
            { "data": 'CustomerName' },
            { "data": 'CustomerType' },
            { "data": 'OutstandingDays' },
            { "data": 'Outstanding' },
            { "data": 'CreditLimit' },
            { "data": 'CriticalPayment' },
            { "data": 'ZeroToFourteenDays' },
            { "data": 'FourteenToTwentyeightDays' },
            { "data": 'TwentyeightToFiftyDays' },
            { "data": 'FiftyToSeventyDays' },
            { "data": 'MoreThenSeventyDays' }
        ];

        // Create set footer function
        const setFooter = function (api, intVal) {
            // api is DataTable method
            // intVal is method defined in db.common.js

            const totalOutstandingDays = api.column(5).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalOutstanding = api.column(6).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalCreditLimit = api.column(7).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

            const totalCP = api.column(8).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const total0To14Days = api.column(9).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const total14To28Days = api.column(10).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const total28To50Days = api.column(11).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const total50To70Days = api.column(12).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalMore70Days = api.column(13).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

            $(api.column(1).footer()).html('<b>Total</b>');
            $(api.column(5).footer()).html(`<b>${totalOutstandingDays}</b>`);
            $(api.column(6).footer()).html(`<b>${totalOutstanding}</b>`);
            $(api.column(7).footer()).html(`<b>${totalCreditLimit}</b>`);
            $(api.column(8).footer()).html(`<b>${totalCP}</b>`);
            $(api.column(9).footer()).html(`<b>${total0To14Days}</b>`);
            $(api.column(10).footer()).html(`<b>${total14To28Days}</b>`);
            $(api.column(11).footer()).html(`<b>${total28To50Days}</b>`);
            $(api.column(12).footer()).html(`<b>${total50To70Days}</b>`);
            $(api.column(13).footer()).html(`<b>${totalMore70Days}</b>`);
        };

        $('#divCswCustomerOsDetail').empty();
        $('#divCswCustomerOsDetail').html(table);
        dbCommon.bindDataToTable('/dashboard/FetchCsWiseCustomerOsDetails', dbFilter, columns, '#CsWiseCustomerOs', '#divCswCustomerOsDetail', setFooter);

    }
};