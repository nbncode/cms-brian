'use strict';

// Global variables
var filterData = null;

const dashboardController = '/Dashboard/';
const dateTypeMtd = 'MTD', dateTypeCd = 'CustomDate';
const sale = 'sale', outstanding = 'outstanding', collection = 'collection', inventory = 'inventory', cbo = 'cbo', scheme = 'scheme', walletBal = 'walletBalance', customer = 'customer', loserAndGainer = 'loserAndGainer';

var dashboard = {

    init: () =>
    {
        dbCommon.bindDatepicker();

        // Fetch all info
        filterData = { DateType: dateTypeMtd };
        dashboard.fetchAllInfo();
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

                dashboard.fetchAllInfo();
            } else
            {
                common.LoadErrorMessage('End Date must be greater than Start Date');
            }

        } else
        {
            common.LoadErrorMessage('Start Date and End Date cannot be blank.');
        }
    },
    
    showLoader: (box) =>
    {
        let div = '';
        let loadMsg = '';

        switch (box)
        {
            case sale:
                div = $('#divSalesInfo');
                loadMsg = 'Loading sale info...';
                break;

            case outstanding:
                div = $('#divOutstandingInfo');
                loadMsg = 'Loading outstanding info...';
                break;

            case collection:
                div = $('#divCollectionInfo');
                loadMsg = 'Loading collection info...';
                break;

            case inventory:
                div = $('#divInventoryInfo');
                loadMsg = 'Loading inventory info...';
                break;

            case cbo:
                div = $('#divCboInfo');
                loadMsg = 'Loading cbo info...';
                break;

            case scheme:
                div = $('#divSchemeInfo');
                loadMsg = 'Loading scheme info...';
                break;

            case walletBal:
                div = $('#divWalletBalanceInfo');
                loadMsg = 'Loading wallet balance info...';
                break;

            case customer:
                div = $('#divCustomerInfo');
                loadMsg = 'Loading customers info...';
                break;

            case loserAndGainer:
                div = $('#divLoserAndGainerInfo');
                loadMsg = 'Loading loser and gainer info...';
                break;
        }

        const loader = `<div class="loader"><div class="kt-spinner home-page kt-spinner--v2 kt-spinner--md kt-spinner--info" style="display: block;"></div><span>${loadMsg}</span></div>`;
        div.html(loader);
    },

    fetchAllByDateFilter: () =>
    {
        const option = $('#ddlDateFilter').val();

        $('#divStartDate').toggle(option === dateTypeCd);
        $('#divEndDate').toggle(option === dateTypeCd);
        $('#divFilter').toggle(option === dateTypeCd);

        // Fetch all if selected option is not 'custom date'
        if (option !== dateTypeCd)
        {
            filterData = { DateType: option };
            dashboard.fetchAllInfo();
        }
    },

    fetchAllInfo: () =>
    {
        console.group('Fetching all info');
        dashboard.fetchSaleInfo();
        dashboard.fetchOutstandingInfo();
        dashboard.fetchCollectionInfo();
        dashboard.fetchInventoryInfo();
        dashboard.fetchCboInfo();
        dashboard.fetchSchemeInfo();
        dashboard.fetchWalletBalanceInfo();
        dashboard.fetchCustomerInfo();
        dashboard.fetchLoserAndGainerInfo();
        console.groupEnd();
    },

    fetchSaleInfo: () =>
    {
        console.log('Fetching sale info');

        dashboard.showLoader(sale);
        dbCommon.request(dashboardController + 'FetchSaleInfo', filterData, function (response)
        {
            $('#divSalesInfo').html(response);
        });
    },

    fetchOutstandingInfo: () =>
    {
        console.log('Fetching outstanding info');

        dashboard.showLoader(outstanding);
        dbCommon.request(dashboardController + 'FetchOutstandingInfo', filterData, function (response)
        {
            $('#divOutstandingInfo').html(response);
        });
    },

    fetchCollectionInfo: () =>
    {
        console.log('Fetching collection info');

        dashboard.showLoader(collection);
        dbCommon.request(dashboardController + 'FetchCollectionInfo', filterData, function (response)
        {
            $('#divCollectionInfo').html(response);
        });
    },

    fetchInventoryInfo: () =>
    {
        console.log('Fetching inventory info');

        dashboard.showLoader(inventory);
        dbCommon.request(dashboardController + 'FetchInventoryInfo', filterData, function (response)
        {
            $('#divInventoryInfo').html(response);
        });
    },

    fetchCboInfo: () =>
    {
        console.log('Fetching cbo info');

        dashboard.showLoader(cbo);
        dbCommon.request(dashboardController + 'FetchCboInfo', filterData, function (response)
        {
            $('#divCboInfo').html(response);
        });
    },

    fetchSchemeInfo: () =>
    {
        console.log('Fetching scheme info');

        dashboard.showLoader(scheme);
        dbCommon.request(dashboardController + 'FetchSchemeInfo', filterData, function (response)
        {
            $('#divSchemeInfo').html(response);
        });
    },

    fetchWalletBalanceInfo: () =>
    {
        console.log('Fetching wallet balance info');

        dashboard.showLoader(walletBal);
        dbCommon.request(dashboardController + 'FetchWalletBalanceInfo', filterData, function (response)
        {
            $('#divWalletBalanceInfo').html(response);
        });
    },

    fetchCustomerInfo: () =>
    {
        console.log('Fetching billing and non-billing customers info');

        dashboard.showLoader(customer);
        dbCommon.request(dashboardController + 'FetchCustomerInfo', filterData, function (response)
        {
            $('#divCustomerInfo').html(response);
        });
    },

    fetchLoserAndGainerInfo: () =>
    {
        console.log('Fetching losers and gainers info');

        dashboard.showLoader(loserAndGainer);
        dbCommon.request(dashboardController + 'FetchLoserAndGainerInfo', filterData, function (response)
        {
            $('#divLoserAndGainerInfo').html(response);
        });
    }
};