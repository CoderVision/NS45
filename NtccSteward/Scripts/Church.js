
/////// *************************************************************** Church List Loader
var chuLstLdr = {

    grid: null
    , dataView: null

    , dispose: function () {
        this.grid = null;
        this.dataView = null;
    }

    , loadgrid: function (data) {

        function filter(item, args) {

            var fullName = (item["ChurchName"] + "").toLowerCase();
            var ss = (args.searchString + "").toLowerCase();

            return (fullName.indexOf(ss + "") > -1);
        };



        ///* grid */
        //function alertFormatter(row, cell, value, columnDef, dataContext) {
        //    if (value) {
        //        return "<img class='smallImg' src='/Content/images/placeholder.gif' style='background:url(/Content/images/residentListImages16.png) 0 0;' />";
        //    }
        //    else
        //        return null;
        //}

        //function genderFormatter(row, cell, value, columnDef, dataContext) {

        //    if (value == null || value.trim() == "")
        //        return null;
        //    else {
        //        if (value == "F")
        //            return "<img class='smallImg' src='/Content/images/placeholder.gif' style='background:url(/Content/images/residentListImages16.png) -48px 0;' />";
        //        else
        //            return "<img class='smallImg' src='/Content/images/placeholder.gif' style='background:url(/Content/images/residentListImages16.png) -33px 0;' />";
        //    }
        //}


        function nameFormatter(row, cell, value, columnDef, dataContext) {
            if (value) {
                //return "<img class='smallImg' src='/Content/images/placeholder.gif' style='background:url(/Content/images/residentListImages16.png) 0 0;' />";
                return "<a href='/Church/Church?id=" + dataContext.id + "'>" + value + "</a>"
                //alert(dataContext.id);
            }
            else
                return null;
        }

        // Add a Formatter 
        var columns = [
            { id: "fullName", name: "Church Name", field: "ChurchName", minWidth: 150, sortable: true, headerCssClass: "cellTextHeader", cssClass: "cellText", formatter: nameFormatter },
            { id: "status", name: "Status", field: "Status", sortable: true, width: 125, headerCssClass: "cellTextHeader", cssClass: "cellText" },
            { id: "preferredCity", name: "Preferred City", sortable: true, width: 125, headerCssClass: "cellTextHeader", cssClass: "cellText" },
            { id: "preferredState", name: "Preferred State", sortable: true, width: 125, headerCssClass: "cellTextHeader", cssClass: "cellText" },
            { id: "preferredZip", name: "Preferred Zip", sortable: true, width: 125, headerCssClass: "cellTextHeader", cssClass: "cellText" },
            { id: "preferredPhone", name: "Preferred Phone", sortable: true, width: 125, headerCssClass: "cellTextHeader", cssClass: "cellText" },
            { id: "preferredEmail", name: "Preferred Email", sortable: true, width: 125, headerCssClass: "cellTextHeader", cssClass: "cellText" },
        ];


        var options = {
            enableCellNavigation: true
            , enableColumnReorder: false
            , fullWidthRows: true
            , rowHeight: 22
            , headerRowHeight: 22
        };


        var sortAsc = true;
        var sortColumn;

        function sortComparer(a, b) {

            var x = a[sortColumn];
            var y = b[sortColumn];

            if (typeof x === "string" && typeof y === "string") {
                // convert to lower case so that it the list gets sorted correctly.
                x = x.toLowerCase();
                y = y.toLowerCase();
            }

            if (x === y) // compare values & nulls
                return 0;
            if (x === null)
                return -1;
            else if (y === null)
                return 1;
            else
                return (x > y ? 1 : -1);
        };


        this.dataView = new Slick.Data.DataView({ inlineFilters: true });
        this.dataView.beginUpdate();
        this.dataView.setItems(data);
        this.dataView.setFilterArgs({
            searchString: ""
        });
        this.dataView.setFilter(filter);
        this.dataView.endUpdate();

        this.grid = new Slick.Grid("#churchGrid", this.dataView, columns, options);
        $("#churchGrid").data("gridInstance", this.grid);

        this.grid.onSort.subscribe(function (e, args) {

            sortColumn = args.sortCol.field;
            sortAsc = args.sortAsc;

            var dataView = this.getData();
            dataView.sort(sortComparer, sortAsc);

            this.invalidateAllRows();
            this.render();
        });

        $(window).resize(function () {

            var grid = $("#churchGrid").data("gridInstance");
            if (grid !== undefined)
                grid.resizeCanvas();
        });

        $("#loader").css('display', 'none');
    }

    , load: function () {

        $.ajax({
            type: "POST",
            url: "/api/church/GetList",
            contentType: "application/json",
            success: function (data) {
                // this refers to the "context: this" that is passed in as the context
                this.loadgrid(data);
            },
            fail: function (data) {
                alert('failed to load church list');
            }
            , context: this
        });
    }
};

/////// *************************************************************** Church List Loader


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
