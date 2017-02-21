/// <reference path="jquery-3.1.1.intellisense.js" />


function SaveNewChurch() {

    var isvalid = $("#addChurchForm").valid();
    if (isvalid == false)
        return;

    var regX = /_|\(|\)|\s|-/g;
    var ph = $("#phone").val().replace(regX, "");

    var newChurch = {
        id: -1,
        Name: $("#churchName").val(),
        PastorId: $("#pastorId").val(),
        Line1: $("#line1").val(),
        City: $("#city").val(),
        State: $("#state").val(),
        Zip: $("#zip").val(),
        Phone: ph,
        Email: $("#email").val(),
    };

    var newId = 0;

    $.ajax({
        url: "/Church/CreateChurch",
        type: "POST",
        datatype: "Json",
        data: newChurch,
        success: function (newId) {

            $("#addChurchTitle").val("Add Church - Saved");
            var cnt = $("#addCount").val();
            $("#addCount").val((cnt + 1));

            setTimeout(OpenNewChurchForm, 2000);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {

            //$("#svdMsg").val("Error");
            //$("#svdMsg").css("visibility", "visible");

            alert("Status: " + textStatus + "\r\n" + "Error: " + errorThrown);
        },
        context: this
    });

    if (!open)
        return newId;
}

function SaveNewChurchAdd() {

    SaveNewChurch(false);

    ClearNewChurchForm();

    document.getElementById("addChurch").focus();
}

function CloseNewChurch() {

    ClearNewChurchForm();

    $("#addChurch").modal("hide"); // close
}

function OpenNewChurchForm() {

    ClearNewChurchForm();

    document.getElementById("churchName").focus();
}

function ClearNewChurchForm() {

    // clear previous values and open form
    $("#churchName").val("");
    $("#pastorId").val("");
    $("#line1").val("");
    $("#city").val("");
    $("#state").val("");
    $("#zip").val("");
    $("#phone").val("(___) ___-____");
    $("#email").val("");
    $("#addChurchTitle").val("Add Church");

    var validator = $("#addChurchForm").validate();
    validator.resetForm();
}

function initializeNewChurch() {

    $("#addChurch").on("hidden.bs.modal", function () {

        var cnt = $("#addCount").val();
        if (cnt > 0) {
            window.location.reload();
        }
    });

    $('#addChurchForm').validate({
        rules: {
            churchName: {
                //required: true,
                required: function (element) {
                    var value = $("#churchName").val();
                    return value.trim() == "";
                }
            },
            phone: {
                phone: true
            },
            // validate on if it's entered, but do not require it
            email: {
                email: true
            }
        },
        highlight: function (element, errorClass) {
            $(element).closest('.form-group').addClass('has-error');
        },
        unhighlight: function (element, errorClass) {
            $(element).closest('.form-group').removeClass('has-error');
        },
        messages: {
            churchName: "Church name is required",
        }
    });
}


////AddTeammate('teamTbl')
function AddTeammate(teamTblId)
{
    var table = document.getElementById(teamTblId);

    var template = document.getElementById("teammateRowTemplate");
    var newRow = template.cloneNode(true);

    var newId = $('#' + teamTblId + ' tr').length * -1;
    newRow.id += newId;

    var cells = newRow.cells;
    for (var i = 0; i < cells.length; i++) {
        cells[i].style.padding = "8px";
        cells[i].style.border = "1px solid #ddd";
    }

    $(newRow).removeClass("hidden");

    newRow.setAttribute("data-changed", "true");
   
    table.appendChild(newRow);

    wireEventHandlers("church");
}

////RemoveTeammate('teamTbl', 'teammateRow-@teammate.Id')
function RemoveTeammate(element, teammateId) {

    if (element == "teamTbl")
    {
        $("table#teamTbl tr#" + teammateId).remove();
    }
    else
    {
        $(element).closest('tr').remove();
    }

    if (teammateId == undefined)
        return;

    var teammateId = teammateId.substring(12); //teammateRow-@teammate.Id
    var teamId = $("#teamId").val();

    $.ajax({
        type: "POST",
        url: "/Church/DeletePastoralTeammate?teamId=" + teamId + "&teammateId=" + teammateId,
        datatype: "JSON",
        success: function (data) {

            return;

        },
        error: function (xhr, asaxOptions, thrownError) {

            console.log(xhr.responseText);

        }
         , context: this
    });
}


function SaveChurchProfile() {

    //public int StatusId { get; set; }

    //var statusId = $("#StatusId").val();
    //var status = $("#status").val();

    // get which radio is checked
    var profile = {
        Id: $("#identityId").val()
        , Name: $("#churchName").val()
        , StatusId: $("#status").val()
        , Comment: $("#Comment").val()
        , AddressList: []
        , EmailList: []
        , PhoneList: []
        , CustomAttributeList: []
        , MetaDataList: []
        , PastoralTeam: GetPastoralTeam()
    };

    var addys = GetAddresses("addressList_" + profile.Id);
    for (var i = 0; i < addys.length; i++) {

        if (addys[i].Changed == true) {
            addys[i].IdentityId = profile.Id;
            profile.AddressList.push(addys[i]);
        }
    }

    var phones = GetPhoneNumbers("phoneList_" + profile.Id);
    for (var i2 = 0; i2 < phones.length; i2++) {

        if (phones[i2].Changed == true) {
            phones[i2].IdentityId = profile.Id;
            profile.PhoneList.push(phones[i2]);
        }
    }

    var emails = GetEmails("emailList_" + profile.Id);
    for (var i3 = 0; i3 < emails.length; i3++) {

        if (emails[i3].Changed == true) {
            emails[i3].IdentityId = profile.Id;
            profile.EmailList.push(emails[i3]);
        }
    }

    
    $.ajax({
        type: "POST",
        datatype: "JSON",
        data: profile,
        url: "/Church/SaveProfile",
        success: function (data) {

            document.location.reload(true);

        },
        error: function (xhr, asaxOptions, thrownError) {

            console.log(xhr.responseText);

            //$("#errMsg").append(xhr.responseText);
            //$("#errMsg").css('visibility', 'visible');
        }
        , context: this
    });

    function GetPastoralTeam()
    {
        var pastoralTeam = {
            Id: $("#teamId").val(),
            Name: $("#teamname").val(),
            TeamTypeEnumId: $("#teamTypeEnumId").val(),
            TeamPositionEnumTypeId: $("#teamPositionEnumTypeId").val(),
            ChurchId: $("#identityId").val(),
            teammates: []
        };

        var tableId = "teamTbl";
        var table = document.getElementById(tableId);

        var rows = $("#teamTbl tr").not(".hidden");


        for (var i = 0; i < rows.length; i++) {
            var td = rows[i].getElementsByTagName("td");
            if (td.length > 0) {
                
                var teammate = {
                    Id: $("[type='hidden']", td[0]).val(),
                    TeamId: pastoralTeam.Id,
                    PersonId: $(td[1]).find("select").val(),
                    TeamPositionEnumId: $(td[2]).find("select").val(),
                }

                pastoralTeam.teammates.push(teammate);
            }
        }

        return pastoralTeam;
    }
}


