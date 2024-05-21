var sales, schemeId, controller;
var workshopid, admin = {
    LoadAdminPartialView: function (partail) {
        console.log("partail", partail);
        doAjax(adminController + partail, null, function (data) {
            $('#main-content').html(data);
        });
    },
    LoadDistributorPartialView: function (partail) {
        doAjax(distributorController + partail, null, function (data) {
            $('#main-content').html(data);
        });
    },
    LoadHomePartialView: function (partail) {
        doAjax(homeController + partail, null, function (data) {
                $('#main-content').html(data);
        });
    }, 
    LoadAdminPartialView: function (partail,divid) {
        console.log("partail", partail);
        doAjax(adminController + partail, null, function (data) {
            $(divid).html(data);
        });
    },
    LoadPartialView: function (partail) {
        doAjax(partail, null, function (data) {
            $('#main-content').html(data);
        });
    },
    ApproveUser: function (id, Approved) {
        common.showLoader();
        var data = {
            UserId: $(id).parents("tr").attr("id"),
            Approved: Approved
        };
        doAjaxPost(apiController + "ApproveUser", data, function (d) {
            common.hideLoader();
            common.ShowMessage(d);
            if (Approved) {
                $(id).parents("td").find("a.confirm").hide();
                $(id).parents("td").find("a.not-confirm").show();
            } else {
                $(id).parents("td").find("a.confirm").show();
                $(id).parents("td").find("a.not-confirm").hide();
            }
            if (d.ResultFlag) {
                admin.LoadPartialView($("#PageView").val());
            }
        });
    },
    DeleteUser: function (id, from) {
        if (confirm('Do you want to Delete ?')) {
            common.showLoader();
            var data = {
                UserId: $(id).parents("tr").attr("id")
            };
            doAjaxPost(apiController + "DeleteUser", data, function (d) {
                common.hideLoader();
                common.ShowMessage(d);
                admin.LoadPartialView($("#PageView").val());
                //if ($("#FromDis").length > 0) {
                //    admin.LoadDistributorPartialView($("#PageView").val());
                //} else {
                //    admin.LoadAdminPartialView($("#PageView").val());
                //}

            });
        }
    },


    LockUser: function (id, Locked) {
        common.showLoader();
        var data = {
            UserId: $(id).parents("tr").attr("id"),
            Locked: Locked
        };
        doAjaxPost(apiController + "LockUser", data, function (d) {
            common.hideLoader();
            common.ShowMessage(d);
            if (Locked) {
                $(id).parents("td").find("a.lock").hide();
                $(id).parents("td").find("a.un-lock").show();
            } else {
                $(id).parents("td").find("a.lock").show();
                $(id).parents("td").find("a.un-lock").hide();
            }
        });
    },

    saveDistributors: function () {
        var id = $(".modal-dialog").attr("id");
        var checked = $(".modal-body .input-chk:checked");

        var Classes = "";
        $.each($(".modal-body .input-chk:checked"), function () {
            if (Classes === "") {
                Classes = $(this).val();
            } else {
                Classes = Classes + "," + $(this).val();
            }
        });

        if (Classes == "") {
            alert("Please select atleast one distributor");
        } else {
            common.showLoader();
            var data = {
                WorkShopId: id,
                Distributors: Classes
            };
            doAjaxPost(adminController + "saveDistributorsList", data, function (d) {
                common.hideLoader();
                $("#myDistributors").modal('hide');
                common.ShowMessage(d);
            });
        }
    },
    SetValueToInput: function () {
        var auth = $("#access_token").val();
        $("#tags").autocomplete({
            minLength: 0,
            source: function (request, response) {
                $.ajax({
                    type: "POST",
                    data: {
                        searchText: request.term
                    },
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader('Authorization', 'bearer ' + auth);
                    },
                    url: apiController + "WorkShopList",
                    success: function (data) {
                        response(data.Data);
                    }
                });
            },
            focus: function (event, ui) {

                $("#tags").val(ui.item.Name);
                return false;
            },
            select: function (event, ui) {

                workshopid = ui.item.value;
                $("#tags").val(ui.item.Name);
                $("#tags-id").val(ui.item.value);

                return false;
            }
        }).data("ui-autocomplete")._renderItem = function (ul, item) {
                return $("<li>").data("ui-autocomplete-item", item).append("<span>" + item.Name + "</span>").appendTo(ul);
            };

    },

    ApproveNewUser: function (id, userId, Approved) {
        common.showLoader();
        var data = {
            UserId: userId,
            Approved: Approved
        };
        doAjaxPost(apiController + "ApproveUser", data, function (d) {
            common.hideLoader();
            common.ShowMessage(d);
            if (Approved) {
                $(id).parents("div").find("a.confirm").hide();
                $(id).parents("div").find("a.not-confirm").show();
            } else {
                $(id).parents("div").find("a.confirm").show();
                $(id).parents("div").find("a.not-confirm").hide();
            }
        });
    },


    ActiveDeactiveClass: function (id, status) {
        common.showLoader();
        var data = {
            ClassId: id,
            Status: status
        };
        doAjaxPost(apiController + "ClassStatusChange", data, function (d) {
            common.hideLoader();
            common.ShowMessage(d);
            admin.LoadPartialView($("#PageView").val());
            location.reload();
        });
    },

    SubmitClientClassId: function (clientId) {

        var classIds = "";
        $(".icheck_n:checked").each(function (index) {
            var val = $(this).val();
            classIds += classIds.length > 0 ? "," + val : val;
        });
        console.log("ClassIds: ",classIds);
        var data = {
            clientId: clientId,
            ClassIds: classIds
        };
        doAjaxPost("/Admin/SaveClientsClass", data, function (d) {
            common.hideLoader();
            common.ShowMessage(d);
        });
    },
    DeleteClient: function (id, from) {
        if (confirm('Do you want to Delete ?')) {
            common.showLoader();
            var data = {
                ClientId: $(id).parents("tr").attr("id")
            };
            doAjaxPost(apiController + "DeleteClient", data, function (d) {
                common.hideLoader();
                common.ShowMessage(d);
                admin.LoadPartialView($("#PageView").val());
            });
        }
    },
    SaveClientsForClass: function (classId) {

        var clientIds = "";
        $(".icheck_n:checked").each(function (index) {
            var val = $(this).val();
            clientIds += clientIds.length > 0 ? "," + val : val;
        });
        console.log("ClientId: ", clientIds);
        var data = {
            ClassId: classId,
            ClientIds: clientIds
        };
        doAjaxPost("/Admin/SaveClientForClass", data, function (d) {
            common.hideLoader();
            common.ShowMessage(d);
        });
    },

    AssignClassToUser: function (userId) {

        var classIds = "";
        $(".icheck_n:checked").each(function (index) {
            var val = $(this).val();
            classIds += classIds.length > 0 ? "," + val : val;
        });
        console.log("ClassIds: ", classIds);
        var data = {
            userId: userId,
            assignClassIds: classIds
        };
        doAjaxPost("/Admin/SaveUserClasses", data, function (d) {
            common.hideLoader();
            common.ShowMessage(d);
        });
    },

    AssignClientsToJudge: function (judgeId) {

        var clientsIds = "";
        $(".icheck_n:checked").each(function (index) {
            var val = $(this).val();
            clientsIds += clientsIds.length > 0 ? "," + val : val;
        });
        console.log("clientsIds: ", clientsIds);
        var data = {
            JudgeId: judgeId,
            assignClientIds: clientsIds
        };
        doAjaxPost("/Admin/SaveJudgeClients", data, function (d) {
            common.hideLoader();
            common.ShowMessage(d);
        });
    },
};

