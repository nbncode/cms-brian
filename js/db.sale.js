'use strict';

// Global variables
var filterData = null, partCategory = null, partGroupName = null, customerType = null;

const dashboardController = '/Dashboard/';
const dateTypeMtd = 'MTD', dateTypeCd = 'CustomDate';
const salesDetail = 'salesDetail', salesSubGroup = 'salesSubGroup';
const roSalesDetail = 'roSalesDetail', seSalesDetail = 'seSalesDetail', csSalesDetail = 'csSalesDetail';
const roWiseBranchSaleDetail = 'roWiseBranchSaleDetail', roWiseCustomerSaleDetail = 'roWiseCustomerSaleDetail', roWiseBranchCustomerSaleDetail = 'roWiseBranchCustomerSaleDetail';
const seWiseCustomerSaleDetail = 'seWiseCustomerSaleDetail', seWiseBranchCustomerSaleDetail = 'seWiseBranchCustomerSaleDetail', seWiseCustomerSaleDetailByCustType = 'seWiseCustomerSaleDetailByCustType';
const csWiseCustomerSaleDetailByCustType = 'csWiseCustomerSaleDetailByCustType';

const pgSalesDetail = 'pgSalesDetail', pgSalesDetailByPartType = 'pgSalesDetailByPartType', pgSalesDetailByCust = 'pgSalesDetailByCustomerType', pgSalesDetailByBranch = 'pgSalesDetailByBranch',
    pgSalesDetailByCustomer = 'pgSalesDetailByCustomer';

var dbSale = {

    initSale: () =>
    {
        dbCommon.bindDatepicker();

        filterData = { DateType: dateTypeMtd };
        dbSale.fetchSalesByCategory();
    },

    showLoader: (box) =>
    {
        let div = $('#divContainer');
        let loadMsg = '';

        switch (box)
        {
            case salesDetail:
                loadMsg = 'Loading sales detail...';
                break;

            case salesSubGroup:
                loadMsg = 'Loading sales by sub-group...';
                break;

            case roSalesDetail:
                loadMsg = 'Loading RO sales detail...';
                break;

            case roWiseBranchSaleDetail:
                div = $('#divSubContainer');
                loadMsg = 'Loading RO sales detail by branch...';
                break;

            case roWiseBranchCustomerSaleDetail:
                loadMsg = 'Loading RO sales customer detail by branch...';
                break;

            case roWiseCustomerSaleDetail:
                div = $('#divSubContainer');
                loadMsg = 'Loading RO sales detail by customer...';
                break;

            case seSalesDetail:
                loadMsg = 'Loading SE sales detail...';
                break;

            case seWiseCustomerSaleDetail:
                div = $('#divSewCustomerSaleDetail');
                loadMsg = 'Loading SE sales detail by customer...';
                break;

            case seWiseBranchCustomerSaleDetail:
                div = $('#divSewBranchCustomerSaleDetail');
                loadMsg = 'Loading SE sales customer detail by branch...';
                break;

            case seWiseCustomerSaleDetailByCustType:
                div = $('#divSewCustomerSaleDetailByCustType');
                loadMsg = 'Loading SE sales detail by customer type...';
                break;

            case csSalesDetail:
                div = $('#divSubGroupSaleDetail');
                loadMsg = 'Loading CS sales detail...';
                break;

            case csWiseCustomerSaleDetailByCustType:
                div = $('#divCswCustomerSaleDetailByCustType');
                loadMsg = 'Loading CS sales detail by customer type...';
                break;

            case pgSalesDetail:
                div = $('#divSubGroupSaleDetail');
                loadMsg = 'Loading PG sales detail...';
                break;

            case pgSalesDetailByPartType:
                div = $('#divPgSalesDetailByPartType');
                loadMsg = 'Loading PG sales detail by part group type...';
                break;

            case pgSalesDetailByCust:
                div = $('#divpgSalesDetailCustomerType');
                loadMsg = 'Loading PG sales detail by customer type...';
                break;

            case pgSalesDetailByBranch:
                div = $('#divpgSalesDetailByBranch');
                loadMsg = 'Loading PG sales detail by branch...';
                break;

            case pgSalesDetailByCustomer:
                div = $('#divpgSalesDetailByCustomer');
                loadMsg = 'Loading PG sales detail by customer...';
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

                dbSale.fetchSalesByCategory();
            } else
            {
                common.LoadErrorMessage('End Date must be greater than Start Date');
            }

        } else
        {
            common.LoadErrorMessage('Start Date and End Date cannot be blank.');
        }
    },

    handleBack: () => 
    {
        var back = $('#back');
        var referer = back.data('referer');

        switch (referer)
        {
            case 'Main':
                dbSale.fetchSalesByCategory();
                break;

            case 'SubGroup':
                dbSale.fetchSalesBySubGroup(partCategory);
                break;

            case 'RO':
                dbSale.fetchRoWiseBranchSaleDetail();
                break;
        }
    },

    setBackLink: (referer, title, show) =>
    {
        $('#divContainer').empty();
        $('#divSubContainer').empty();

        var back = $('#back');
        back.data('referer', referer);
        back.prop('title', title);
        back.toggle(show);
    },

    fetchSalesByDateFilter: () =>
    {
        const option = $('#ddlDateFilter').val();

        $('#divStartDate').toggle(option === dateTypeCd);
        $('#divEndDate').toggle(option === dateTypeCd);
        $('#divFilter').toggle(option === dateTypeCd);

        // Fetch all if selected option is not 'custom date'
        if (option !== dateTypeCd)
        {
            filterData = { DateType: option };
            dbSale.fetchSalesByCategory();
        }
    },

    // TODO: Remove this
    emptyContainers: () =>
    {
        $('#divSubGroupSaleDetail').empty();

        $('#divRowBranchSaleDetail').empty();
        $('#divRowBranchCustomerSaleDetail').empty();
        $('#divRowCustomerSaleDetail').empty();

        $('#divSewCustomerSaleDetail').empty();
        $('#divSewBranchCustomerSaleDetail').empty();
        $('#divSewCustomerSaleDetailByCustType').empty();

        $('#divCswCustomerSaleDetailByCustType').empty();

        $('#divpgSalesDetail').empty();
        $('#divPgSalesDetailByPartType').empty();
        $('#divpgSalesDetailCustomerType').empty();
        $('#divpgSalesDetailByBranch').empty();
        $('#divpgSalesDetailByCustomer').empty();
    },

    fetchSalesByCategory: () =>
    {
        console.log('Fetch sales by category');

        dbSale.setBackLink('', '', false);
        dbSale.showLoader(salesDetail);
        dbCommon.request(dashboardController + 'FetchSalesByCategory', filterData, function (response) 
        {
            $('#divContainer').html(response);
        });
    },

    fetchSalesBySubGroup: (category) =>
    {
        console.log(`Fetch sub-group sales for '${category}' category`);

        partCategory = category;

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            Category: partCategory
        };

        dbSale.setBackLink('Main', 'Back to all sales', true);
        dbSale.showLoader(salesSubGroup);
        dbCommon.request(dashboardController + 'FetchSalesBySubGroup', dbFilter, function (response)
        {
            $('#divContainer').html(response);
        });
    },

    fetchSaleDetailForSubGroup: (subGroup) =>
    {
        console.log(`Table 1 - Fetch sale detail for '${subGroup}' sub group`);

        dbSale.setBackLink('SubGroup', 'Back to sub group sales', true);

        let subGroupSaleDetail = '';
        let actionMethod = '';

        switch (subGroup)
        {
            case 'RO':
                actionMethod = 'FetchRoWiseSaleDetail';
                subGroupSaleDetail = roSalesDetail;
                break;

            case 'SE':
                actionMethod = 'FetchSeWiseSaleDetail';
                subGroupSaleDetail = seSalesDetail;
                break;

            case 'CS':
                actionMethod = 'FetchCsWiseSaleDetail';
                subGroupSaleDetail = csSalesDetail;
                break;

            case 'PG':
                actionMethod = 'FetchPGWiseSaleDetail';
                subGroupSaleDetail = pgSalesDetail;
                break;
        }

        if (actionMethod)
        {
            const dbFilter = {
                DateType: filterData.DateType,
                StartDate: filterData.StartDate,
                EndDate: filterData.EndDate,
                Category: partCategory
            };

            dbSale.showLoader(subGroupSaleDetail);
            dbCommon.request(dashboardController + actionMethod, dbFilter, function (response)
            {
                $('#divContainer').html(response);

                switch (subGroup)
                {
                    case 'RO':
                        dbSale.fetchRoWiseBranchSaleDetail();
                        break;

                    case 'SE':
                        dbSale.fetchSeWiseCustomerSaleDetail();
                        break;
                }
            });
        } else
        {
            console.error('No action method was set.');
        }
    },

    // #region RO wise sale detail

    fetchRoWiseBranchSaleDetail: () =>
    {
        console.log(`Table 2 - Fetch RO wise branch sale detail`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            Category: partCategory
        };

        dbSale.showLoader(roWiseBranchSaleDetail);
        dbCommon.request(dashboardController + 'FetchRoWiseBranchSaleDetail', dbFilter, function (response) 
        {
            $('#divSubContainer').html(response);
            dbCommon.applyDataTable();
        });
    },

    fetchRoWiseBranchCustomerSaleDetail: (branchCode) =>
    {
        console.log(`Table 3 - Fetch RO wise branch customer sale detail for '${branchCode}' branch code`);

        dbSale.setBackLink('RO', 'Back to RO wise sales', true);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            Category: partCategory,
            BranchCode: branchCode
        };

        dbSale.showLoader(roWiseBranchCustomerSaleDetail);
        dbCommon.request(dashboardController + 'FetchRoWiseBranchCustomerSaleDetail', dbFilter, function (response)
        {
            $('#divContainer').html(response);

            dbSale.fetchRoWiseCustomerSaleDetail(branchCode, '');
        });
    },

    fetchRoWiseCustomerSaleDetail: (branchCode, customerType) =>
    {
        // TODO: Show customers in dynamic data table
        console.log(`Table 4 - Fetch RO wise customer sale detail for '${branchCode}' branch code and '${customerType}' customer type`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            Category: partCategory,
            BranchCode: branchCode,
            CustomerType: customerType
        };

        dbSale.showLoader(roWiseCustomerSaleDetail);
        dbSale.fetchCustomerDetail('RO', dbFilter);

        //dbCommon.request(dashboardController + 'FetchRoWiseCustomerSaleDetail', dbFilter, function (response)
        //{
        //    $('#divSubContainer').html(response);
        //});
    },

    // #endregion

    // #region SE wise sale detail

    fetchSeWiseBranchCustomerSaleDetail: (branchCode) =>
    {
        console.log(`Fetch SE wise branch customer sale detail for '${branchCode}' branch code`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            Category: partCategory,
            BranchCode: branchCode
        };

        dbSale.showLoader(seWiseBranchCustomerSaleDetail);
        dbCommon.request(dashboardController + 'FetchSeWiseBranchCustomerSaleDetail', dbFilter, function (response)
        {
            $('#divSewBranchCustomerSaleDetail').html(response);
        });
    },

    fetchSeWiseCustomerSaleDetail: (customerType) =>
    {
        console.log(customerType ? `Fetch SE wise customer sale detail for '${customerType}' customer type` : 'Fetch SE wise customer sale detail');

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            Category: partCategory,
            CustomerType: customerType
        };

        dbSale.showLoader(customerType ? seWiseCustomerSaleDetailByCustType : seWiseCustomerSaleDetail);
        dbCommon.request(dashboardController + 'FetchSeWiseCustomerSaleDetail', dbFilter, function (response)
        {
            if (customerType)
            {
                $('#divSewCustomerSaleDetailByCustType').html(response);
            } else
            {
                $('#divSewCustomerSaleDetail').html(response);
            }
        });
    },

    // #endregion

    // #region CS wise sale detail

    fetchCsWiseCustomerSaleDetail: (customerType) =>
    {
        console.log(`Fetch CS wise customer sale detail for '${customerType}' customer type`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            Category: partCategory,
            CustomerType: customerType
        };

        dbSale.showLoader(csWiseCustomerSaleDetailByCustType);
        dbCommon.request(dashboardController + 'FetchCsWiseCustomerSaleDetail', dbFilter, function (response)
        {
            $('#divCswCustomerSaleDetailByCustType').html(response);
        });
    },

    // #endregion

    // #region PG wise sale detail

    fetchPgWiseSaleDetail: () =>
    {
        console.log(`Fetch PG wise part group sale detail`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            Category: partCategory
        };

        dbSale.showLoader(pgSalesDetail);
        dbCommon.request(dashboardController + 'FetchPgWiseSaleDetail', dbFilter, function (response)
        {
            $('#divpgSalesDetail').html(response);
        });
    },

    fetchPgWisePartTypeSaleDetail: (partGroup) =>
    {

        console.log(`Fetch PG wise part group sale detail for '${partGroup}' part group`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            Category: partCategory,
            PartGroupName: partGroup
        };

        dbSale.showLoader(pgSalesDetailByPartType);
        dbCommon.request(dashboardController + 'FetchPgWisePartGroupSaleDetail', dbFilter, function (response)
        {

            $('#divPgSalesDetailByPartType').html(response);
        });
    },

    fetchPgWiseCustomerTypeSaleDetail: (partGroup) =>
    {
        partGroupName = null;
        partGroupName = partGroup;

        console.log(`Fetch PG wise customer type sale detail for '${partGroup}' part group`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            Category: partCategory,
            PartGroupName: partGroupName
        };

        dbSale.showLoader(pgSalesDetailByCust);
        dbCommon.request(dashboardController + 'FetchPgWiseCustomerTypeSaleDetail', dbFilter, function (response)
        {
            $('#divpgSalesDetailCustomerType').html(response);
        });
    },

    fetchPgWiseBranchCustomerSaleDetail: (customer) =>
    {
        customerType = null;
        customerType = customer;
        console.log(`Fetch PG wise branch sale detail for '${customer}' customer Type`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            Category: partCategory,
            PartGroupName: partGroupName,
            CustomerType: customerType
        };

        dbSale.showLoader(pgSalesDetailByBranch);
        dbCommon.request(dashboardController + 'FetchPgWiseBranchSaleDetail', dbFilter, function (response)
        {
            $('#divpgSalesDetailByBranch').html(response);
        });
    },

    fetchPgWiseCustomerSaleDetail: (branchCode) =>
    {
        console.log(`Fetch PG wise customer sale detail for '${branchCode}' branch code`);

        const dbFilter = {
            DateType: filterData.DateType,
            StartDate: filterData.StartDate,
            EndDate: filterData.EndDate,
            Category: partCategory,
            PartGroupName: partGroupName,
            CustomerType: customerType,
            BranchCode: branchCode
        };

        dbSale.showLoader(pgSalesDetailByCustomer);
        dbCommon.request(dashboardController + 'FetchPgWiseCustomerSaleDetail', dbFilter, function (response)
        {
            $('#divpgSalesDetailByCustomer').html(response);
        });
    },

    // #endregion

    fetchCustomerDetail: (subGroup, dbFilter) => 
    {
        // TODO: Test search feature

        // Create table
        var table = `<div class="table-responsive">
                        <table class="table table-bordered w-auto table-center" id="customerDetail">
                            <thead class="thead-light">
                                <tr>
                                    <th>Sr No</th>
                                    <th>Customer Code</th>
                                    <th>Customer Name</th>
                                    <th>Customer Type</th>
                                    <th>PY ${dbFilter.DateType}</th>                                                                      
                                </tr>
                            </thead>
                            <tfoot>
		                        <tr>
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
        const columns = [
            { "data": 'SlNo' },
            { "data": 'CustomerCode' },
            { "data": 'CustomerName' },
            { "data": 'CustomerType' },
            { "data": 'PrevAchieved' }
        ];

        switch (subGroup)
        {
            case 'RO':
                $('#divSubContainer').html(table);
                dbCommon.bindDataToTable('/dashboard/FetchRoWiseCustomerSaleDetail', dbFilter, columns, '#customerDetail', '#divSubContainer', null, true);
                break;

            case 'SE':
                $('#divSubContainer').html(table);
                dbCommon.bindDataToTable('/dashboard/FetchSeWiseCustomerSaleDetail', dbFilter, columns, '#customerDetail', '#divSubContainer', null, true);
                break;
        }
    }
};