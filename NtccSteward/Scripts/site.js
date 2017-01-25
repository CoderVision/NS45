﻿/// <reference path="jquery-1.12.4.intellisense.js" />
// Write your Javascript code.
///// <reference path="../lib/jquery/dist/jquery.js" />


//function parseDate(dateString) {
//    if (dateString != null) {
//        var patt = new RegExp("[0-9]+", "i");
//        if (patt.test(dateString)) {
//            var match = patt.exec(dateString);
//            var d = new Date();
//            d.setTime(match);

//            var y = d.getFullYear();
//            var m = d.getMonth() + 1;
//            var d = d.getDate();

//            return m + "/" + d + "/" + y
//        }
//        else
//            return null;
//    }
//    return dateString;
//}


function FilterTable(tableId,criteria,fieldsToSearch)
{
    var fieldAry = fieldsToSearch.split(",");
    var fieldIdxAry = [];
    var th = "#" + tableId + " th";
    var list = $(th);
    list.each(function () {
        // loop through all fields to search for th matches
        var fieldidx = 0;
        for (var i = 0; i < fieldAry.length; i++)
        {
            if ($(this).text().trim() == fieldAry[i].trim()) {
                fieldIdxAry[fieldidx++] = list.index(this);
            }
        }
    });

    var table = document.getElementById(tableId);
    var rows = table.getElementsByTagName("tr");

    if (criteria.length > 0)
    {
        // hide rows that do not meet the criteria
        criteria = criteria.toLowerCase();

        for (var i = 0; i < rows.length; i++) {
            var td = rows[i].getElementsByTagName("td");
            if (td.length > 0) {
                for (var idx = 0; idx < fieldIdxAry.length; idx++) {

                    if (td[fieldIdxAry[idx]].innerText.trim().toLowerCase().indexOf(criteria) > -1) {
                        rows[i].style.display = "";
                    }
                    else
                        rows[i].style.display = "none";
                }
            }
        }
    }
    else
    {
        // show all rows
        for (var i = 0; i < rows.length; i++) {
            rows[i].style.display = "";
        }
    }
}


function selectModuleLink(displayText) {

    // loop through all links and update them
    // get all links in 'moduleMenu' whose text does not equal display text and reset their background color
    var links = $('#moduleMenu a');

    for (var i = 0; i < links.length; i++) {
        var link = links[i];
        console.log(link);

        var text = link.innerHTML;
        if (text === displayText) {
            link.style.backgroundColor = '#B8DEC5';
            link.style.margin = '0px -3.8px 4px 0px';
            link.style.borderRightWidth = '0px';
        }
        else {
            link.style.backgroundColor = '';
            link.style.margin = '';
            link.style.borderRightWidth = '';
        }
    }
}







function AddAddress(id, addressType, listName) {
    $.ajax({
        url: "/Member/CreateAddress",
        type: "POST",
        datatype: "HTML",
        data: { addyType: addressType, memberId: id },
        success: function (data) {

            $("#" + listName).append(data);

            // get form contrls & wire handlers
            var elementId = getIdFromString(data.toString());

            wireEventHandlers(elementId);

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Status: " + textStatus); alert("Error: " + errorThrown);
        }
    });
}

function getIdFromString(elementString) {
    var regEx = /id=\"(.+)\"/
    var ary = regEx.exec(elementString)
    var elementId = ary[1]
    return elementId;
}


function RemoveAddress(id, addressType) {

    if (confirm("Delete address?") === false) {
        return;
    }

    $.ajax({
        url: "/Member/RemoveAddress",
        type: "POST",
        datatype: "HTML",
        data: { addressId: id },
        success: function (data) {

            // remove address from list
            var elementName = addressType + "_" + id;
            var node = document.getElementById(elementName);
            if (node) {
                $("#" + elementName).remove();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Status: " + textStatus); alert("Error: " + errorThrown);
        }
    });
}

function enableSave(enable) {
    var img = document.getElementById("memberSaveBtn");
    if (img) {
        if (enable)
            img.src = "../images/save-32.png";
        else
            img.src = "../images/save-32-gray.png";
    }
}

function getTarget(e) {
    var targ;
    if (!e) e = window.event;
    if (e.target) targ = e.target;
    else if (e.srcElement) targ = e.srcElement;
    if (targ.nodeType === 3) // defeat Safari bug
        targ = targ.parentNode;

    return targ;
}


function keydown(e) {

    enableSave(true);

    e.currentTarget.setAttribute("data-changed", "true");
}

//possibly change "keydown" to "change"
function wireEventHandlers(entityName) {
    var mp = document.getElementById(entityName);
    if (mp) {
        var inputs = mp.getElementsByTagName('input');
        for (var i = 0; i < inputs.length; i++) {
            if (inputs[i].type === 'radio' || inputs[i].type === 'checkbox')
                inputs[i].addEventListener("click", keydown, true);
            else
                inputs[i].addEventListener("change", keydown, true);
        }
    }
}

function GetAddresses(id) {
    var addresses = [];
    var addys = document.getElementById(id);
    if (addys === null) return addresses;
    for (
    var i = 0; i < addys.childElementCount; i++) {
        var addy = { IdentityId: 0, ContactInfoId: 0, Line1: "", Line2: "", Line3: "", City: "", State: "", Zip: "", Preferred: 0, Verified: 0, Changed: false };
        var child = addys.children[i];

        if (child.id && child.id.indexOf("address_") > -1) {

            addy.ContactInfoId = child.id.replace("address_", "");
            var fields = child.querySelectorAll(".form-control,.nscheckbox:checked");
            for (var n = 0; n < fields.length; n++) {

                var changed = fields[n].getAttribute("data-changed");
                if (changed !== null && changed === "true")
                    addy.Changed = true;

                var value = fields[n].value;

                if (fields[n].id === "line1")
                    addy.Line1 = value;
                else if (fields[n].id === "line2")
                    addy.Line2 = value;
                else if (fields[n].id === "line3")
                    addy.Line3 = value;
                else if (fields[n].id === "city")
                    addy.City = value;
                else if (fields[n].id === "state")
                    addy.State = value;
                else if (fields[n].id === "zip")
                    addy.Zip = value;
                else if (fields[n].id === "preferred")
                    addy.Preferred = value;
                else if (fields[n].id === "verified")
                    addy.Verified = value;
            }

            addresses.push(addy);
        }

    }
    return addresses;
}

function GetPhoneNumbers(id) {
    var addresses = [];
    var addys = document.getElementById(id);
    if (addys === null) return addresses;
    for (var i = 0; i < addys.childElementCount; i++) {
        var addy = { IdentityId: 0, ContactInfoId: 0, PhoneNumber: "", Preferred: 0, Verified: 0, Changed: false };
        var child = addys.children[i];

        if (child.id && child.id.indexOf("phone_") === -1)
            continue;

        addy.ContactInfoId = child.id.replace("phone_", "");
        var fields = child.querySelectorAll(".form-control,.nscheckbox:checked");
        for (var n = 0; n < fields.length; n++) {

            var changed = fields[n].getAttribute("data-changed");
            if (changed !== null && changed === "true")
                addy.Changed = true;

            var value = fields[n].value;

            if (fields[n].id === "phoneNumber")
                addy.PhoneNumber = value;
            else if (fields[n].id === "preferred")
                addy.Preferred = value;
            else if (fields[n].id === "verified")
                addy.Verified = value;
        }
        addresses.push(addy);

    }
    return addresses;
}

function GetEmails(id) {
    var addresses = [];
    var addys = document.getElementById(id);
    if (addys === null) return addresses;
    for (var i = 0; i < addys.childElementCount; i++) {
        var addy = { IdentityId: 0, ContactInfoId: 0, EmailAddress: "", Preferred: 0, Verified: 0, Changed: false };
        var child = addys.children[i];

        if (child.id && child.id.indexOf("email_") === -1)
            continue;

        addy.ContactInfoId = child.id.replace("email_", "");
        var fields = child.querySelectorAll(".form-control,.nscheckbox:checked");
        for (var n = 0; n < fields.length; n++) {

            var changed = fields[n].getAttribute("data-changed");
            if (changed !== null && changed === "true")
                addy.Changed = true;

            var value = fields[n].value;

            if (fields[n].id === "emailAddress")
                addy.EmailAddress = value;
            else if (fields[n].id === "preferred")
                addy.Preferred = value;
            else if (fields[n].id === "verified")
                addy.Verified = value;
        }
        addresses.push(addy);
    }
    return addresses;
}




function OpenMap(id) {

    var addy = {};

    var identityId = $("#identityId").val()
    var addys = GetAddresses("addressList_" + identityId);
    for (var i = 0; i < addys.length; i++) {
        if (addys[i].ContactInfoId === id)
            addy = addys[i];
    }

    if (!addy || !addy.Id)
        return;

    // get Address inbput boxes
    var mapAddy = "";

    if (addy.Line1.trim() !== "")
        mapAddy = addy.Line1.trim();

    if (addy.Line2.trim() !== "")
        mapAddy += (mapAddy !== "" ? "," : "") + addy.Line2.trim();

    if (addy.City.trim() !== "")
        mapAddy += (mapAddy !== "" ? "," : "") + addy.City.trim();

    if (addy.State.trim() !== "")
        mapAddy += (mapAddy !== "" ? "," : "") + addy.State.trim();

    if (addy.Zip.trim() !== "")
        mapAddy += (mapAddy !== "" ? " " : "") + addy.Zip.trim();

    var map = "https://www.google.com/maps/place/" + mapAddy.replace(" ", "+");

    window.open(map, '_blank'); //, 'location=yes,height=570,width=520,scrollbars=yes,status=yes'
}

function OpenText(id) {

    var phone = { PhoneNumber: "" };

    var identityId = $("#identityId").val()
    var phones = GetPhoneNumbers("phoneList_" + identityId);
    for (var i = 0; i < phones.length; i++) {
        if (phones[i].ContactInfoId === id)
            phone = phones[i];
    }

    if (phone && phone.PhoneNumber) {
        // get EmailAddress inbput box
        if (phone.PhoneNumber.trim() !== "") {

            $("#msgType").val("sms");
            $("#recipient").val(phone.PhoneNumber);
            $("#body").val("");
            $("#subjectGrp").addClass("hidden");
            $("#sendMsg").modal();
        }
    }
}

function OpenEmail(id) {

    var email = { EmailAddress: "" };

    var identityId = $("#identityId").val()
    var emails = GetEmails("emailList_" + identityId);
    for (var i = 0; i < emails.length; i++) {
        if (emails[i].ContactInfoId === id)
            email = emails[i];
    }

    if (email && email.EmailAddress) {
        // get EmailAddress inbput box
        if (email.EmailAddress.trim() !== "") {

            $("#msgType").val("email");
            $("#recipient").val(email.EmailAddress);
            $("#subject").val("");
            $("#body").val("");
            $("#subjectGrp").removeClass("hidden");
            $("#sendMsg").modal();
        }
    }
}


function SendMsg() {
    var msgType = $("#msgType").val();
    var recipient = $("#recipient").val();
    var subject = $("#subject").val();
    var body = $("#body").val();

    var apiurl = "/api/message/" + (msgType === "sms" ? "SendSms" : "SendEmail");
    var params = { churchId: 0, recipient: recipient.trim(), subject: subject.trim(), body: body.trim() };

    $.ajax({
        type: "POST",
        datatype: "JSON",
        data: params,
        url: apiurl,
        success: function (data) {

            $("#sendMsg").modal("hide"); // close
        },
        error: function (xhr, asaxOptions, thrownError) {
            alert(xhr.responseText);
        }
        , context: this
    });
}



///////  Login Page

function initializeRequestAccount() {

    $("#requestAcct").validate({
        rules: {
            FirstName: {
                required: true
            },
            LastName: {
                required: true
            },
            Email: {
                required: true,
                email: true,
            },
            Password: {
                required: true,
                minlength: 7,
            },
            password2: {
                required: true,
                equalTo: "#Password"
            },
            ChurchName: {
                required: true
            },
            PastorName: {
                required: true
            },
            City: {
                required: true
            },
            State: {
                required: true
            }
        },

        // called when error occurs
        highlight: function (element, errorClass) {
            // closest walks up the dom and finds the nearest element
            $(element).closest('.form-group').addClass('has-error');
        },

        // called when error is removed
        unhighlight: function (element, errorClass) {
            // closest walks up the dom and finds the nearest element
            $(element).closest('.form-group').removeClass('has-error');
        },

        // this method will prevent the form from being cleared automatically and let you process the fields before submitting to server.
        //  this is the standard way of handling submis
        submitHandler: function (form) {

            form.submit();
        }
    });
}

function initializeLogin() {
    $('#loginForm').validate({
        rules: {
            Email: {
                required: true,
                email: true
            },
            Password: {
                required: true
            },
            ChurchId: {
                required: true
            }
        },
        highlight: function (element, errorClass) {
            $(element).closest('.form-group').addClass('has-error');
        },
        unhighlight: function (element, errorClass) {
            $(element).closest('.form-group').removeClass('has-error');
        },
        submitHandler: function (form) {
            form.submit();
        }
    });
}

////// End Login Page