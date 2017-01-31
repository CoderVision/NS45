
/// <reference path="jquery.validate.js" />
/// <reference path="jquery-1.12.4.intellisense.js" />



function SaveMemberProfile() {

    var maleChecked = document.getElementById("maleRadio").checked;

    var statusId = $("#StatusId").val();
    var status = $("#status").val();
    if (statusId != status)
    {
        // status has changed, make sure reason has changed
        var statusChangeTypeId = $("#StatusChangeTypeId").val();
        var reason = $("#reason").val();
        if (statusChangeTypeId == reason)
        {
            $("#reason").closest('.form-group').addClass('has-error');
            alert("You must select a different reason when the status changes");
            return;
        }
        else
            $("#reason").closest('.form-group').removeClass('has-error');
    }
   
    // get which radio is checked
    var memberProfile = {
        MemberId: $("#identityId").val()
        , FirstName: $("#FirstName").val()
        , MiddleName: $("#MiddleName").val()
        , LastName: $("#LastName").val()
        , PreferredName: $("#PreferredName").val()
        , ChurchId: $("#ChurchId").val()
        , ChurchName: $("#ChurchName").val()
        , StatusId: $("#status").val()
        //, StatusDesc: $("#StatusDesc").val()
        //, StatusChangeTypeDesc: $("#StatusChangeTypeDesc").val()
        , StatusChangeTypeId: $("#reasonId").val()
        , Gender: maleChecked == true ? "M" : "F"
        , BirthDate: $("#BirthDate").val()
        , Married: document.getElementById("Married").checked 
        , Veteran: document.getElementById("Veteran").checked
        , SponsorId: $("#SponsorId").val()
        //, Sponsor: $("#Sponsor").val()
        , Comments: $("#Comments").val()
        , DateSaved: $("#DateSaved").val()
        , DateBaptizedWater: $("#DateBaptizedWater").val()
        , DateBaptizedHolyGhost: $("#DateBaptizedHolyGhost").val()
        , AddressList: []
        , EmailList: []
        , PhoneList: []
    };

    var addys = GetAddresses("addressList_" + memberProfile.MemberId);
    for (var i = 0; i < addys.length; i++) {

        if (addys[i].Changed == true)
        {
            addys[i].IdentityId = memberProfile.MemberId;
            memberProfile.AddressList.push(addys[i]);
        }
    }

    var phones = GetPhoneNumbers("phoneList_" + memberProfile.MemberId);
    for (var i2 = 0; i2 < phones.length; i2++) {

        if (phones[i2].Changed == true)
        {
            phones[i2].IdentityId = memberProfile.MemberId;
            memberProfile.PhoneList.push(phones[i2]);
        }
    }

    var emails = GetEmails("emailList_" + memberProfile.MemberId);
    for (var i3 = 0; i3 < emails.length; i3++) {

        if (emails[i3].Changed == true)
        {
            emails[i3].IdentityId = memberProfile.MemberId;
            memberProfile.EmailList.push(emails[i3]);
        }
    }

    $.ajax({
        type: "POST",
        datatype: "JSON",
        data: memberProfile,
        url: "/Member/SaveProfile",
        success: function (data) {

            document.location.reload(true);

        },
        error: function (xhr, asaxOptions, thrownError) {

            alert(xhr.responseText);

            //$("#errMsg").append(xhr.responseText);
            //$("#errMsg").css('visibility', 'visible');
        }
        , context: this
    });



    // use Ajax to submit a request and stay on the same page.
    //http://stackoverflow.com/questions/5382728/asp-net-mvc-multiple-forms-staying-on-same-page

    //var form = $("#_memberProfileForm");
    ////form.submit(function () { $.post($(this).attr('action'), $(this).serialize(), function (response) { return; }, 'json'); return false; });
    //form.submit();

    // load form data
    // loop through each addy and send those that have changed.

    //var forms = document.getElementsByTagName("form");
    //for (var i = 0; i < forms; i++)
    //{
    //    forms[i].submit(submitForm())
    //}

    // get all forms on page and submit them

    //alert("saveMemberProfile Saved!");

    enableSave(false); // after saving
}


function memberModuleLinkClick(displayText, id) {

    $(document).ajaxStart(function () {
        $("#profileLoader").css('visibility', 'visible');
    });
    $(document).ajaxStop(function () {
        $("#profileLoader").css('visibility', 'hidden');
    });

    //$("moduleContent").load("/Resident/GetModule" + displayText + "residentID=" + residentID);

    //$.ajax({
    //    url: "/Member/GetView",
    //    type: "POST",
    //    datatype: "HTML",
    //    data: { viewName: displayText, memberId: id },
    //    success: function (data) {
    //        $('#moduleContent').html(data);

    //        wireEventHandlers("member");
    //    },
    //    error: function (XMLHttpRequest, textStatus, errorThrown) {
    //        alert("Status: " + textStatus); alert("Error: " + errorThrown);
    //    }
    //});
}

function OpenNewMemberForm()
{
    ClearNewMemberForm();

    document.getElementById("dateCame").focus();
}

function ClearNewMemberForm()
{
    var today = new Date();
    var todayFormatted = (today.getMonth() + 1) + "/" + today.getDate() + "/" + today.getFullYear();
    $("#dateCame").val(todayFormatted);

    // clear previous values and open form
    $("#isGroup").prop("checked", false);
    $("#prayed").prop("checked", false);
    $("#firstName").val("");
    $("#lastName").val("");
    $("#line1").val("");
    $("#city").val("");
    $("#state").val("");
    $("#zip").val("");
    $("#phone").val("(___) ___-____");
    $("#phone2").val("(___) ___-____");
    $("#email").val("");
    $("#sponsor").val("");
    //$("#svdMsg").css("visibility", "hidden");
    $("#addMbrTitle").val("Add Member");

    var validator = $("#addMemberForm").validate();
    validator.resetForm();
}


function SaveNewMember(open) {

    var isvalid = $("#addMemberForm").valid();
    if (isvalid == false)
        return;

    var regX = /_|\(|\)|\s|-/g;
    var ph = $("#phone").val().replace(regX, "");
    var ph2 = $("#phone2").val().replace(regX, "");

    var newMember = {
        id:-1,
        DateCame: $("#dateCame").val(),
        IsGroup: $("#isGroup").prop("checked"),
        Prayed: $("#prayed").prop("checked"),
        FirstName: $("#firstName").val(),
        LastName: $("#lastName").val(),
        Line1: $("#line1").val(),
        City: $("#city").val(),
        State: $("#state").val(),
        Zip: $("#zip").val(),
        Phone: ph,
        Phone2: ph2,
        Email: $("#email").val(),
        SponsorId: $("#sponsor").val(),

        // populate from session
        ChurchId: -1,
        CreatedByUserId: -1
    };

    var newId = 0;

    $.ajax({
        url: "/Member/CreateMember",
        type: "POST",
        datatype: "Json",
        data: newMember,
        success: function (newId) {

            //$("#svdMsg").val("Saved");
            //$("#svdMsg").css("visibility", "visible");

            $("#addMbrTitle").val("Add Member - Saved");
            var cnt = $("#addCount").val();
            $("#addCount").val((cnt+1));
            
            setTimeout(OpenNewMemberForm, 2000);
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

// save the new member then clear form and add another
function SaveNewMemberAdd()
{
    SaveNewMember(false);

    ClearNewMemberForm();

    document.getElementById("dateCame").focus();
}

function CloseNewMember() {

    ClearNewMemberForm();

    $("#newMember").modal("hide"); // close
}

// close form & open new member
function OpenNewMember(id) {

    $("#newMember").modal("hide"); // close

    // open member
    $.ajax({
        url: "/Member/Member",
        type: "POST",
        datatype: "Json",
        data: { id: id },
        success: function (data) {

            // should automatically redirect to member form

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Status: " + textStatus + "\r\n" + "Error: " + errorThrown);
        }
    });
}

function initializeNewMember() {

    var today = new Date();
    var todayFormatted = (today.getMonth()+1) + "/" + today.getDate() + "/" + today.getFullYear();
    $("#dateCame").val(todayFormatted);

    $("#newMember").on("hidden.bs.modal", function () {

        var cnt = $("#addCount").val();
        if (cnt > 0)
        {
            window.location.reload();
        }
    });

    $('#addMemberForm').validate({
        rules: {
            dateCame: {
                date: true
            },
            firstName: {
                //required: true,
                required: function (element) {
                    var value = $("#lastName").val();
                    return value.trim() == "";
                }
            },
            lastName: {
                //required: true
                required: function (element) {
                    var value = $("#firstName").val();
                    return value.trim() == "";
                }
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
            firstName: "First or Last name is required",
            lastName: "First or Last name is required"
        }
    });
}