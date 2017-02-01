
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