'use strict';

var filterData = null, partCategory = null;

const dashboardController = '/Dashboard/';
const dateTypeMtd = 'MTD', dateTypeCd = 'CustomDate';

const WalletSubGroup = 'WalletSubGroup';
const roWalletDetail = 'roWalletDetail', roWiseBranchWalletDetail = 'roWiseBranchWalletDetail', roWiseBranchCustomerWalletDetail = 'roWiseBranchCustomerWalletDetail', roWiseCustomerPartsDetail = 'roWiseCustomerPartsDetail';
const seWalletDetail = 'seWalletDetail', seWiseBranchWalletDetail = 'seWiseBranchWalletDetail', seWiseBranchCustomerWalletDetail = 'seWiseBranchCustomerWalletDetail',
    seWiseCustomerPartsDetail = 'seWiseCustomerPartsDetail';

var dbWallet = {

    initWallet: () =>
    {
        dbCommon.bindDatepicker();

        filterData = { DateType: dateTypeMtd };
        dbWallet.fetchWalletBySubGroup();
    },

    showLoader: (tag) =>
    {
        let div = '';
        let loadMsg = '';

        switch (tag)
        {
            case WalletSubGroup:
                div = $('#divWalletBySubGroup');
                loadMsg = 'Loading Wallet by sub-group...';
                break;

            case roWalletDetail:
                div = $('#divSubGroupWalletDetail');
                loadMsg = 'Loading RO Wallet detail...';
                break;

            case roWiseBranchWalletDetail:
                div = $('#divRowBranchWalletDetail');
                loadMsg = 'Loading RO Wallet detail by branch...';
                break;

            case roWiseBranchCustomerWalletDetail:
                div = $('#divRowBranchCustomerWalletDetail');
                loadMsg = `Loading RO wise branch's customer Wallet detail...`;
                break;

            case seWalletDetail:
                div = $('#divSubGroupWalletDetail');
                loadMsg = 'Loading SE Wallet detail...';
                break;

            case seWiseBranchWalletDetail:
                div = $('#divSewBranchWalletDetail');
                loadMsg = 'Loading SE Wallet detail by branch...';
                break;

            case seWiseBranchCustomerWalletDetail:
                div = $('#divSewBranchCustomerWalletDetail');
                loadMsg = `Loading SE wise branch's customer Wallet detail...`;
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

                dbWallet.fetchWalletBySubGroup();
            } else
            {
                common.LoadErrorMessage('End Date must be greater than Start Date');
            }

        } else
        {
            common.LoadErrorMessage('Start Date and End Date cannot be blank.');
        }
    },

    fetchWalletByDateFilter: () =>
    {
        const option = $('#ddlDateFilter').val();

        $('#divStartDate').toggle(option === dateTypeCd);
        $('#divEndDate').toggle(option === dateTypeCd);
        $('#divFilter').toggle(option === dateTypeCd);

        // Fetch all if selected option is not 'custom date'
        if (option !== dateTypeCd)
        {
            filterData = { DateType: option };
            dbWallet.fetchWalletBySubGroup();
        }
    },

    emptyContainers: () =>
    {
        $('#divSubGroupWalletDetail').empty();

        $('#divRowBranchWalletDetail').empty();
        $('#divRowBranchCustomerWalletDetail').empty();
        $('#divRowCustomerWalletDetail').empty();

        $('#divSewBranchWalletDetail').empty();
        $('#divSewBranchCustomerWalletDetail').empty();
        $('#divSewCustomerWalletDetail').empty();
    },

    fetchWalletBySubGroup: () =>
    {
        console.log('Fetch sub-group wallets');

        dbWallet.emptyContainers();

        dbWallet.showLoader(WalletSubGroup);
        dbCommon.request(dashboardController + 'FetchWalletBySubGroup', filterData, function (response)
        {
            $('#divWalletBySubGroup').html(response);
        });
    },

    fetchWalletDetailForSubGroup: (subGroup) =>
    {
        // 1st table
        console.log(`Fetch wallet detail for '${subGroup}' sub group`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate
        };

        dbWallet.emptyContainers();

        let subGroupWalletDetail = '';
        let actionMethod = '';

        $('#divRoContainer').toggle(subGroup === 'RO');
        $('#divSeContainer').toggle(subGroup === 'SE');

        switch (subGroup)
        {
            case 'RO':
                actionMethod = 'FetchRoWiseWalletDetail';
                subGroupWalletDetail = roWalletDetail;
                break;

            case 'SE':
                actionMethod = 'FetchSeWiseWalletDetail';
                subGroupWalletDetail = seWalletDetail;
                break;
        }

        dbWallet.showLoader(subGroupWalletDetail);
        dbCommon.request(dashboardController + actionMethod, dbFilter, function (response)
        {
            $('#divSubGroupWalletDetail').html(response);
        });
    },

    // #region RO wise wallet detail

    fetchRoWiseBranchWalletDetail: (customerType) => 
    {
        // 2nd table
        console.log(`Fetch RO wise branch wallet detail for '${customerType}' customer type`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            CustomerType: customerType
        };

        $('#divRowBranchCustomerWalletDetail').empty();
        $('#divRowCustomerWalletDetail').empty();

        dbWallet.showLoader(roWiseBranchWalletDetail);
        dbCommon.request(dashboardController + 'FetchRoWiseBranchWalletDetail', dbFilter, function (response)
        {
            $('#divRowBranchWalletDetail').html(response);
        });
    },

    fetchRoWiseBranchCustomerWalletDetail: (branchCode) =>
    {
        // 3rd table
        console.log(`Fetch RO wise branch customer wallet detail for '${branchCode}' branch code`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            BranchCode: branchCode
        };

        $('#divRowBranchCustomerWalletDetail').empty();
        $('#divRowCustomerWalletDetail').empty();

        dbWallet.showLoader(roWiseBranchCustomerWalletDetail);
        dbCommon.request(dashboardController + 'FetchRoWiseBranchCustomerWalletDetail', dbFilter, function (response)
        {
            $('#divRowBranchCustomerWalletDetail').html(response);
        });
    },

    // #endregion

    // #region SE wise wallet detail

    fetchSeWiseBranchWalletDetail: (customerType) => 
    {
        // 2nd table
        console.log(`Fetch SE wise branch wallet detail for '${customerType}' customer type`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            CustomerType: customerType
        };

        $('#divSewBranchCustomerWalletDetail').empty();
        $('#divSewCustomerWalletDetail').empty();

        dbWallet.showLoader(seWiseBranchWalletDetail);
        dbCommon.request(dashboardController + 'FetchSeWiseBranchWalletDetail', dbFilter, function (response)
        {
            $('#divSewBranchWalletDetail').html(response);
        });
    },

    fetchSeWiseBranchCustomerWalletDetail: (branchCode) =>
    {
        // 3rd table
        console.log(`Fetch SE wise branch customer wallet detail for '${branchCode}' branch code`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            BranchCode: branchCode
        };

        $('#divSewBranchCustomerWalletDetail').empty();
        $('#divSewCustomerWalletDetail').empty();

        dbWallet.showLoader(seWiseBranchCustomerWalletDetail);
        dbCommon.request(dashboardController + 'FetchSeWiseBranchCustomerWalletDetail', dbFilter, function (response)
        {
            $('#divSewBranchCustomerWalletDetail').html(response);
        });
    },

    // #endregion

    fetchCustomerWalletDetail: (subGroup, customerType, branchCode) => 
    {
        // 4th table
        console.log(`Fetch ${subGroup} wise customer wallet detail for '${customerType}' customer type and '${branchCode}' branch code`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            CustomerType: customerType,
            BranchCode: branchCode
        };

        // Create table
        var table = `<div class="table-responsive">
                        <table class="table table-bordered w-auto table-center" id="walletCustomerDetail">
                            <thead class="thead-light">
                                <tr>
                                    <th>Sr No</th>
                                    <th>Customer Code</th>
                                    <th>Customer Name</th>
                                    <th>Customer Type</th>
                                    <th>Avg Sale (3 Months)</th>
                                    <th>Wallet Balance</th>
                                    <th>Payout of Sales %</th>                                    
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
            { "data": 'AverageSale' },
            { "data": 'WalletBalance' },
            { "data": 'PayoutOfSalesPercentageTxt' }
        ];

        // Create set footer function
        const setFooter = function (api, intVal)
        {
            // api is DataTable method
            // intVal is method defined in db.common.js

            const totalAvgSale = api.column(4).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalWalletBalance = api.column(5).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
            const totalPayoutOfSalesPercentage = api.column(6).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);

            $(api.column(1).footer()).html('<b>Total</b>');
            $(api.column(4).footer()).html(`<b>${totalAvgSale.toFixed(2)}</b>`);
            $(api.column(5).footer()).html(`<b>${totalWalletBalance.toFixed(2)}</b>`);
            $(api.column(6).footer()).html(`<b>${totalPayoutOfSalesPercentage.toFixed(2)} %</b>`);
        };

        switch (subGroup)
        {
            case 'RO':
                $('#divRowCustomerDetail').empty();
                $('#divRowCustomerWalletDetail').html(table);
                dbCommon.bindDataToTable('/dashboard/FetchRoWiseCustomerWalletDetail', dbFilter, columns, '#walletCustomerDetail', '#divRowCustomerWalletDetail', setFooter);
                break;

            case 'SE':
                $('#divSewCustomerDetail').empty();
                $('#divSewCustomerWalletDetail').html(table);
                dbCommon.bindDataToTable('/dashboard/FetchSeWiseCustomerWalletDetail', dbFilter, columns, '#WalletCustomerDetail', '#divSewCustomerWalletDetail', setFooter);
                break;
        }
    }
};