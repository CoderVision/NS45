

function churchModuleLinkClick(displayText, id) {

    selectModuleLink(displayText);

    $(document).ajaxStart(function () {
        $("#profileLoader").css('visibility', 'visible');
    });
    $(document).ajaxStop(function () {
        $("#profileLoader").css('visibility', 'hidden');
    });

    //$("moduleContent").load("/Resident/GetModule" + displayText + "residentID=" + residentID);

    $.ajax({
        url: "/Church/GetView",
        type: "POST",
        datatype: "HTML",
        data: { viewName: displayText, churchId: id },
        success: function (data) {
            $('#moduleContent').html(data);

            wireEventHandlers("church");
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Status: " + textStatus); alert("Error: " + errorThrown);
        }
    });
}


$(document).ready(function () {

    //selectModuleLink('Personal Info');

    //wireEventHandlers();

    //$("#datepicker").datepicker({
    //    showOtherMonths: true,
    //    selectOtherMonths: true,
    //    changeMonth: true,
    //    changeYear: true,
    //    yearRange: "-100:+0"
    //});

    //var form = $("#_memberProfileForm");
    //form.submit(function () { $.post($(this).attr('action'), $(this).serialize(), function (response) { return; }, 'json'); return false; });
    //form.submit(submitForm(this));

    //var forms = document.getElementsByTagName("form");
    //for (var i = 0; i < forms; i++)
    //{
    //    forms[i].submit(function () { $.post($(this).attr('action'), $(this).serialize(), function (response) { return; }, 'json'); return false; });
    //}
});
