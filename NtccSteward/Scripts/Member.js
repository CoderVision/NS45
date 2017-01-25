
/// <reference path="jquery.validate.js" />
/// <reference path="jquery-1.12.4.intellisense.js" />



function SaveMemberProfile() {

    // validate first!

    var maleChecked = document.getElementById("maleRadio").checked;
   
    // get which radio is checked
    var memberProfile = {
        MemberId: $("#identityId").val()
        , FirstName: $("#FirstName").val()
        , MiddleName: $("#MiddleName").val()
        , LastName: $("#LastName").val()
        , ChurchId : 3 // default to Graham, but change later
        , ChurchName: $("#ChurchName").val()
        , StatusId: $("#Status").val()
        , PreferredName: $("#PreferredName").val()
        , Gender: maleChecked === true ? "M" : "F"
        , BirthDate: $("#BirthDate").val()
        , Married: false // hook up later
        , Veteran: false // hook up later
        , SponsorId: $("#Sponsor").val()
        , Comments: $("#Comments").val()
        , DateSaved: null
        , DateBaptizedWater: null
        , DateBaptizedHolyGhost: null
        , AddressList: []
        , EmailList: []
        , PhoneList: []
    };

    var addys = GetAddresses("addressList_" + memberProfile.MemberId);
    for (var i = 0; i < addys.length; i++) {

        if (addys[i].Changed === true)
        {
            addys[i].IdentityId = memberProfile.MemberId;
            memberProfile.AddressList.push(addys[i]);
        }
    }

    var phones = GetPhoneNumbers("phoneList_" + memberProfile.MemberId);
    for (var i2 = 0; i2 < phones.length; i2++) {

        if (phones[i2].Changed === true)
        {
            phones[i2].IdentityId = memberProfile.MemberId;
            memberProfile.PhoneList.push(phones[i2]);
        }
    }

    var emails = GetEmails("emailList_" + memberProfile.MemberId);
    for (var i3 = 0; i3 < emails.length; i3++) {

        if (emails[i3].Changed === true)
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

            //var node = document.getElementById(userId);
            //if (node) {
            //    $("#" + userId).remove();
            //}
            alert('Save Profile Success!');

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

    selectModuleLink(displayText);

    $(document).ajaxStart(function () {
        $("#profileLoader").css('visibility', 'visible');
    });
    $(document).ajaxStop(function () {
        $("#profileLoader").css('visibility', 'hidden');
    });

    //$("moduleContent").load("/Resident/GetModule" + displayText + "residentID=" + residentID);

    $.ajax({
        url: "/Member/GetView",
        type: "POST",
        datatype: "HTML",
        data: { viewName: displayText, memberId: id },
        success: function (data) {
            $('#moduleContent').html(data);

            wireEventHandlers("member");
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Status: " + textStatus); alert("Error: " + errorThrown);
        }
    });
}

function OpenNewMemberForm()
{
    ClearNewMemberForm();

    document.getElementById("dateCame").focus();
}

function ClearNewMemberForm()
{
    // clear previous values and open form
    $("#dateCame").val("");
    $("#isGroup").prop("checked", false);
    $("#prayed").prop("checked", false);
    $("#firstName").val("");
    $("#lastName").val("");
    $("#line1").val("");
    $("#city").val("");
    $("#state").val("");
    $("#zip").val("");
    $("#phone").val("");
    $("#email").val("");
    $("#sponsor").val("");

    var validator = $("#addMemberForm").validate();
    validator.resetForm();
}

function SaveNewMember(open) {

    //$("#addMemberForm").validate();

    var isvalid = $("#addMemberForm").valid();
    if (isvalid === false)
        return;

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
        Phone: $("#phone").val(),
        Phone2: $("#phone2").val(),
        Email: $("#email").val(),
        SponsorId: $("#sponsor").val(),

        // populate from session
        ChurchId: -1,
        CreatedByUserId: -1
    };

    var newId = 0;

    $.ajax({
        url: "/Member",
        type: "POST",
        datatype: "Json",
        data: newMember,
        success: function (newId) {

            if (open)
                OpenNewMember(newId)
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Status: " + textStatus + "\r\n" + "Error: " + errorThrown);
        }
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

    $('#addMemberForm').validate({
        rules: {
            dateCame: {
                date: true
            },
            firstName: {
                //required: true,
                required: function (element) {
                    var value = $("#lastName").val();
                    return value.trim() === "";
                }
            },
            lastName: {
                //required: true
                required: function (element) {
                    var value = $("#firstName").val();
                    return value.trim() === "";
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