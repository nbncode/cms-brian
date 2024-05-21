'use strict';

var filterData = null, partCategory = null;

const dashboardController = '/Dashboard/';
const dateTypeMtd = 'MTD', dateTypeCd = 'CustomDate';

const cboSubGroup = 'cboSubGroup';
const roCboDetail = 'roCboDetail', roWiseBranchCboDetail = 'roWiseBranchCboDetail', roWiseBranchCustomerCboDetail = 'roWiseBranchCustomerCboDetail', roWiseCustomerPartsDetail = 'roWiseCustomerPartsDetail';
const seCboDetail = 'seCboDetail', seWiseBranchCboDetail = 'seWiseBranchCboDetail', seWiseBranchCustomerCboDetail = 'seWiseBranchCustomerCboDetail',
    seWiseCustomerPartsDetail = 'seWiseCustomerPartsDetail';

var dbCbo = {

    initCbo: () =>
    {
        dbCommon.bindDatepicker();

        filterData = { DateType: dateTypeMtd };
        dbCbo.fetchCboBySubGroup();
    },

    showLoader: (tag) =>
    {
        let div = '';
        let loadMsg = '';

        switch (tag)
        {
            case cboSubGroup:
                div = $('#divCboBySubGroup');
                loadMsg = 'Loading cbo by sub-group...';
                break;

            case roCboDetail:
                div = $('#divSubGroupCboDetail');
                loadMsg = 'Loading RO CBO detail...';
                break;

            case roWiseBranchCboDetail:
                div = $('#divRowBranchCboDetail');
                loadMsg = 'Loading RO CBO detail by branch...';
                break;

            case roWiseBranchCustomerCboDetail:
                div = $('#divRowBranchCustomerCboDetail');
                loadMsg = `Loading RO wise branch's customer CBO detail...`;
                break;

            case roWiseCustomerPartsDetail:
                div = $('#divRowCustomerPartsDetail');
                loadMsg = 'Loading RO customer parts detail...';
                break;

            case seCboDetail:
                div = $('#divSubGroupCboDetail');
                loadMsg = 'Loading SE CBO detail...';
                break;

            case seWiseBranchCboDetail:
                div = $('#divSewBranchCboDetail');
                loadMsg = 'Loading SE CBO detail by branch...';
                break;

            case seWiseBranchCustomerCboDetail:
                div = $('#divSewBranchCustomerCboDetail');
                loadMsg = `Loading SE wise branch's customer CBO detail...`;
                break;

            case seWiseCustomerPartsDetail:
                div = $('#divSewCustomerPartsDetail');
                loadMsg = 'Loading SE customer parts detail...';
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

                dbCbo.fetchCboBySubGroup();
            } else
            {
                common.LoadErrorMessage('End Date must be greater than Start Date');
            }

        } else
        {
            common.LoadErrorMessage('Start Date and End Date cannot be blank.');
        }
    },

    fetchCboByDateFilter: () =>
    {
        const option = $('#ddlDateFilter').val();

        $('#divStartDate').toggle(option === dateTypeCd);
        $('#divEndDate').toggle(option === dateTypeCd);
        $('#divFilter').toggle(option === dateTypeCd);

        // Fetch all if selected option is not 'custom date'
        if (option !== dateTypeCd)
        {
            filterData = { DateType: option };
            dbCbo.fetchCboBySubGroup();
        }
    },

    emptyContainers: () =>
    {
        $('#divSubGroupCboDetail').empty();

        $('#divRowBranchCboDetail').empty();
        $('#divRowBranch').hide();
        $('#divRowBranchCustomerCboDetail').empty();
        $('#divRowCustomerCboDetail').empty();
        $('#divRowCustomerDetail').empty();
        $('#divRowCustomerPartsDetail').empty();

        $('#divSewBranchCboDetail').empty();
        $('#divSewBranch').hide();
        $('#divSewBranchCustomerCboDetail').empty();
        $('#divSewCustomerCboDetail').empty();
        $('#divSewCustomerDetail').empty();
        $('#divSewCustomerPartsDetail').empty();
    },

    fetchCboBySubGroup: () =>
    {
        console.log('Fetch sub-group cbos');

        dbCbo.emptyContainers();

        dbCbo.showLoader(cboSubGroup);
        dbCommon.request(dashboardController + 'FetchCboBySubGroup', filterData, function (response)
        {
            $('#divCboBySubGroup').html(response);
        });
    },

    fetchCboDetailForSubGroup: (subGroup) =>
    {
        console.log(`Fetch cbo detail for '${subGroup}' sub group`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate
        };

        dbCbo.emptyContainers();

        let subGroupCboDetail = '';
        let actionMethod = '';

        $('#divRoContainer').toggle(subGroup === 'RO');
        $('#divSeContainer').toggle(subGroup === 'SE');

        switch (subGroup)
        {
            case 'RO':
                actionMethod = 'FetchRoWiseCboDetail';
                subGroupCboDetail = roCboDetail;
                break;

            case 'SE':
                actionMethod = 'FetchSeWiseCboDetail';
                subGroupCboDetail = seCboDetail;
                break;
        }

        dbCbo.showLoader(subGroupCboDetail);
        dbCommon.request(dashboardController + actionMethod, dbFilter, function (response)
        {
            $('#divSubGroupCboDetail').html(response);
        });
    },

    fetchBranchCboDetail: (subGroup, customerType) => 
    {
        console.log(`Fetch ${subGroup} wise branch cbo detail for '${customerType}' customer type`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            CustomerType: customerType
        };

        switch (subGroup)
        {
            case 'RO':
                dbCbo.showLoader(roWiseBranchCboDetail);
                dbCommon.request(dashboardController + 'FetchRoWiseBranchCboDetail', dbFilter, function (response)
                {
                    $('#divRowBranchCboDetail').html(response);
                });
                break;

            case 'SE':
                dbCbo.showLoader(seWiseBranchCboDetail);
                dbCommon.request(dashboardController + 'FetchSeWiseBranchCboDetail', dbFilter, function (response)
                {
                    $('#divSewBranchCboDetail').html(response);
                });
                break;
        }
    },

    fetchBranchCustomerCboDetail: (subGroup, branchCode, anchor) =>
    {
        console.log(`Fetch ${subGroup} wise branch customer cbo detail for '${branchCode}' branch code`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            BranchCode: branchCode
        };

        // Prepare cbo branch row
        var currentRow = $(anchor).closest('tr');
        const row = `<tr>
                        <td>${branchCode}</td>
                        <td>${currentRow.find('td:eq(2)').text()}</td>
                        <td>${currentRow.find('td:eq(3)').text()}</td>
                        <td>${currentRow.find('td:eq(4)').text()}</td>
                        <td>${currentRow.find('td:eq(5)').text()}</td>
                        <td>${currentRow.find('td:eq(6)').text()}</td>      
                        <td>${currentRow.find('td:eq(7)').text()}</td>   
                        <td>${currentRow.find('td:eq(8)').text()}</td>
                        <td>${currentRow.find('td:eq(9)').text()}</td>
                        <td>${currentRow.find('td:eq(10)').text()}</td>                       
                     </tr>`;

        switch (subGroup)
        {
            case 'RO':
                $('#divRowBranch').hide();
                $('#divRowBranch > table > tbody > tr').remove();
                $('#divRowBranchCustomerCboDetail').empty();
                $('#divRowCustomerCboDetail').empty();
                $('#divRowCustomerDetail').empty();

                dbCbo.showLoader(roWiseBranchCustomerCboDetail);
                dbCommon.request(dashboardController + 'FetchRoWiseBranchCustomerCboDetail', dbFilter, function (response)
                {
                    $('#divRowBranch').show();
                    $('#divRowBranch > table > tbody').append(row);

                    $('#divRowBranchCustomerCboDetail').html(response);
                });
                break;

            case 'SE':
                $('#divSewBranch').hide();
                $('#divSewBranch > table > tbody > tr').remove();
                $('#divSewBranchCustomerCboDetail').empty();
                $('#divSewCustomerCboDetail').empty();
                $('#divSewCustomerDetail').empty();

                dbCbo.showLoader(seWiseBranchCustomerCboDetail);
                dbCommon.request(dashboardController + 'FetchSeWiseBranchCustomerCboDetail', dbFilter, function (response)
                {
                    $('#divSewBranch').show();
                    $('#divSewBranch > table > tbody').append(row);

                    $('#divSewBranchCustomerCboDetail').html(response);
                });
                break;
        }
    },

    fetchCustomerCboDetail: (subGroup, customerType, branchCode) => 
    {
        console.log(`Fetch ${subGroup} wise customer cbo detail for '${customerType}' customer type and '${branchCode}' branch code`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            CustomerType: customerType,
            BranchCode: branchCode
        };

        // Create table
        var table = `<div class="table-responsive">
                        <table class="table table-bordered w-auto table-center" id="cboCustomerDetail">
                            <thead class="thead-light">
                                <tr>
                                    <th>Sr No</th>
                                    <th>Customer Code</th>
                                    <th>Customer Name</th>
                                    <th>Customer Type</th>
                                    <th>No.CBO Customers</th>
                                    <th>No.CBO Orders</th>
                                    <th>CBO (In lacs)</th>
                                    <th>CBO (Lacs) 0-7 Days</th>
                                    <th>CBO (Lacs) 7- 15 Days</th>
                                    <th>CBO (Lacs) >15 Days</th>
                                    <th>Sales (In Lacs)</th>
                                    <th>% CBO</th>
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
            { "data": 'NumberOfCboCustomers' },
            { "data": 'NumberOfCboOrders' },
            { "data": 'CboPrice' },
            { "data": 'CboPrice0To7Days' },
            { "data": 'CboPrice7To15Days' },
            { "data": 'CboPriceMoreThan15Days' },
            { "data": 'Sales' },
            { "data": 'CboPercentageTxt' },
            {
                "render": function (data, type, row, meta)
                {
                    return `<a href="javascript:void(0);" onclick="dbCbo.fetchCustomerDetail('${subGroup}','${row.CustomerCode
                        }','${dbFilter.CustomerType}','${dbFilter.BranchCode}')">View</a>`;
                }
            }
        ];

        // Create set footer function
        const setFooter = function (api, intVal)
        {
            // api is DataTable method
            // intVal is method defined in db.common.js

            const totalCboCustomer = api.column(4).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalCboOrder = api.column(5).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalCbo = api.column(6).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalCbo0To7 = api.column(7).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalCbo7To15 = api.column(8).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalCboMoreThan15 = api.column(9).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalSale = api.column(10).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalCboPercent = api.column(11).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

            $(api.column(1).footer()).html('<b>Total</b>');
            $(api.column(4).footer()).html(`<b>${totalCboCustomer}</b>`);
            $(api.column(5).footer()).html(`<b>${totalCboOrder}</b>`);
            $(api.column(6).footer()).html(`<b>${totalCbo}</b>`);
            $(api.column(7).footer()).html(`<b>${totalCbo0To7}</b>`);
            $(api.column(8).footer()).html(`<b>${totalCbo7To15}</b>`);
            $(api.column(9).footer()).html(`<b>${totalCboMoreThan15}</b>`);
            $(api.column(10).footer()).html(`<b>${totalSale}</b>`);
            $(api.column(11).footer()).html(`<b>${totalCboPercent}%</b>`);
        };

        switch (subGroup)
        {
            case 'RO':
                $('#divRowCustomerDetail').empty();
                $('#divRowCustomerCboDetail').html(table);
                dbCommon.bindDataToTable('/dashboard/FetchRoWiseCustomerCboDetail', dbFilter, columns, '#cboCustomerDetail', '#divRowCustomerCboDetail', setFooter);
                break;

            case 'SE':
                $('#divSewCustomerDetail').empty();
                $('#divSewCustomerCboDetail').html(table);
                dbCommon.bindDataToTable('/dashboard/FetchSeWiseCustomerCboDetail', dbFilter, columns, '#cboCustomerDetail', '#divSewCustomerCboDetail', setFooter);
                break;
        }
    },

    fetchCustomerDetail: (subGroup, customerCode, customerType, branchCode) =>
    {
        console.log(`Fetch ${subGroup} wise particular customer detail for '${customerCode}' customer code, '${customerType}' customer type and '${branchCode}' branch code`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            CustomerType: customerType,
            BranchCode: branchCode,
            CustomerCode: customerCode
        };

        // Create table
        var table = `<div class="table-responsive">
                        <table class="table table-bordered w-auto table-center" id="customerDetail">
                            <thead class="thead-light">
                                <tr>
                                    <th>Sr No</th>
                                    <th>Customer Code</th>
                                    <th>Customer Name</th>
                                    <th>BO Number</th>
                                    <th>No of Parts</th>
                                    <th>CBO Value</th>
                                    <th>No of Days Since order</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                        </table>
                    </div>`;

        // Prepare columns
        var columns = [
            { "data": 'SlNo' },
            { "data": 'CustomerCode' },
            { "data": 'CustomerName' },
            { "data": 'CoNumber' },
            { "data": 'NumberOfParts' },
            { "data": 'CboPrice' },
            { "data": 'NumberOfDaysSinceOrder' },
            {
                "render": function (data, type, row, meta)
                {
                    return `<a href="javascript:void(0);" onclick="dbCbo.fetchCustomerParts('${subGroup}','${row.CoNumber}')">View</a>`;
                }
            }
        ];

        switch (subGroup)
        {
            case 'RO':
                $('#divRowCustomerDetail').empty();
                $('#divRowCustomerDetail').html(table);
                dbCommon.bindDataToTable('/dashboard/FetchRoWiseCustomerDetail', dbFilter, columns, '#customerDetail', '#divRowCustomerDetail');
                break;

            case 'SE':
                $('#divSewCustomerDetail').empty();
                $('#divSewCustomerDetail').html(table);
                dbCommon.bindDataToTable('/dashboard/FetchSeWiseCustomerDetail', dbFilter, columns, '#customerDetail', '#divSewCustomerDetail');
                break;
        }
    },

    fetchCustomerParts: (subGroup, coNumber) =>
    {
        console.log(`Fetch ${subGroup} wise customer parts detail for '${coNumber}' customer number`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            CoNumber: coNumber
        };

        switch (subGroup)
        {
            case 'RO':
                $('#divRowCustomerPartsDetail').empty();
                dbCbo.showLoader(roWiseCustomerPartsDetail);
                dbCommon.request(dashboardController + 'FetchRoWiseCustomerPartsDetail', dbFilter, function (response)
                {
                    $('#divRowCustomerPartsDetail').html(response);
                });
                break;

            case 'SE':
                $('#divSewCustomerPartsDetail').empty();
                dbCbo.showLoader(seWiseCustomerPartsDetail);
                dbCommon.request(dashboardController + 'FetchSeWiseCustomerPartsDetail', dbFilter, function (response)
                {
                    $('#divSewCustomerPartsDetail').html(response);
                });
                break;
        }
    }
};