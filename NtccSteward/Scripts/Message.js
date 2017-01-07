
function ShowGroupMembers(groupId)
{
    if (groupId === undefined)
        return;

    var lbl = document.getElementById("lblGroupMember");
    lbl.innerText = "Group " + groupId + " Members";

    // create list
    var ul = document.createElement("ul");
    ul.className = "list-group";

    ul.appendChild(CreateGroupMemberLi(groupId === 1 ? "Gary" : groupId === 2 ? "Chavez" : "Connors"));
    ul.appendChild(CreateGroupMemberLi(groupId === 1 ? "Wei" : groupId === 2 ? "Shabana" : "Bro. Connel"));
    ul.appendChild(CreateGroupMemberLi(groupId === 1 ? "Curtis" : groupId === 2 ? "Foo" : "Bro. Bean"));

    // get div
    var dm = document.getElementById("groupMembers");

    for (var i = dm.childNodes.length - 1; i >= 0; i--) {
        var childNode = dm.childNodes[i];
        dm.removeChild(childNode);
    }

    dm.appendChild(ul);
}

function CreateGroupMemberLi(memberName)
{
    var li = document.createElement("li");
    li.className = "list-group-item";
    li.innerText = memberName;
    return li;
}


/////// *************************************************************** Member List Loader
var memLstLdr = {

    grid: null
    , dataView: null

    , dispose: function () {
        this.grid = null;
        this.dataView = null;
    }

    , loadgrid: function (data) {

        function filter(item, args) {

            var fullName = (item["FullName"] + "").toLowerCase();
            var ss = (args.searchString + "").toLowerCase();

            return (fullName.indexOf(ss + "") > -1);
        };


        function nameFormatter(row, cell, value, columnDef, dataContext) {
            if (value) {
                //return "<img class='smallImg' src='/Content/images/placeholder.gif' style='background:url(/Content/images/residentListImages16.png) 0 0;' />";
                return "<a href='/Member/Member?id=" + dataContext.id + "'>" + value + "</a>"
                //alert(dataContext.id);
            }
            else
                return null;
        }


        function isSelectedFormatter(row, cell, value, columnDef, dataContext) {
            //if (value) {
                
                //return "<a href='/Member/Member?id=" + dataContext.id + "'>" + value + "</a>"
            return "<input type='checkbox' class='nscenter' style='margin: 2px 0 2px 6px;' />";
            //}
            //else
            //    return null;
        }

        // Add a Formatter 
        var columns = [
            { id: "isSelected", name: "", field: "", sortable: true, width: 20, headerCssClass: "cellTextHeader", cssClass: "cellText", formatter: isSelectedFormatter },
            { id: "fullName", name: "Full Name", field: "FullName", minWidth: 150, sortable: true, headerCssClass: "cellTextHeader", cssClass: "cellText", formatter: nameFormatter },
            { id: "preferredPhone", name: "Preferred Phone", sortable: true, width: 125, headerCssClass: "cellTextHeader", cssClass: "cellText" },
            { id: "preferredEmail", name: "Preferred Email", sortable: true, width: 125, headerCssClass: "cellTextHeader", cssClass: "cellText" },
        ];


        //var checkboxSelector = new Slick.CheckboxSelectColumn({
        //    cssClass: "slick-cell-checkboxsel"
        //});

        //columns.push(checkboxSelector.getColumnDefinition());


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

        this.grid = new Slick.Grid("#membersGrid", this.dataView, columns, options);
        $("#membersGrid").data("gridInstance", this.grid);

        //this.grid.setSelectionModel(new Slick.RowSelectionModel());
        //this.grid.multiSelect = false;

        this.grid.onSort.subscribe(function (e, args) {

            sortColumn = args.sortCol.field;
            sortAsc = args.sortAsc;

            var dataView = this.getData();
            dataView.sort(sortComparer, sortAsc);

            this.invalidateAllRows();
            this.render();
        });

        //this.grid.onDblClick.subscribe(function (e, args) {

        //    var cell = this.getCellFromEvent(e);
        //    var row = cell.row;

        //    var dataView = this.getData();
        //    var item = dataView.getItem(row);

        //    if (item != undefined) {
        //        $("#memberId").attr("value", item.Id);

        //        var form = $("#__ResidentProfileAntiForgeryForm");
        //        form.submit();
        //    }
        //});

        $(window).resize(function () {

            var grid = $("#membersGrid").data("gridInstance");
            if (grid !== undefined)
                grid.resizeCanvas();
        });

        $("#loader").css('display', 'none');
    }

    , load: function () {

        var parameters = {
            ChurchID: 3,
            StatusID: 49
        }

        $.ajax({
            type: "POST",
            url: "/api/Member/GetByStatus",
            contentType: "application/json",
            data: JSON.stringify(parameters),
            success: function (data) {

                this.loadgrid(data);
            },
            fail: function (data) {
                alert('failed to load members');
            }
            , context: this
        });
    }
};

/////// *************************************************************** End Member List Loader


function Message_wireEvents()
{
    wireGoupList("groupList");
    wireGoupList("corrList");
}

function wireGoupList(name)
{
    // wire up active selection indicator
    var gl = document.getElementById(name);
    var liList = gl.getElementsByTagName('li');
    for (i = 0; i < liList.length; i++) {
        liList[i].addEventListener('click', groupListItemClick, false);
    }
}

function groupListItemClick(e) {
        e.preventDefault();

        $(this).parent().find('li').removeClass('active');
        $(this).addClass('active');

        var id = String($(this).attr('id'));
        var pid = id.substr(id.indexOf("_") + 1, id.length - 1);
        loadMessages(pid);
}

/// load the messages for the Active list item
function loadMessageList(list)
{
    var activeItemId = getActiveChildId(list)

    if (activeItemId !== undefined)
        loadMessages(activeItemId);
}


function loadMessages(groupId)
{
    $.ajax({
        url: "/Messages/MessageList",
        type: "POST",
        datatype: "HTML",
        data: { id: groupId },
        success: function (data) {

            $('#messageListContainer').html(data);

        },
        fail: function (data) {
            alert('failed to load messages');
        }
    , context: this
    });
}

function getActiveChildId(parentElementId)
{
    var l = document.getElementById(parentElementId);
    var items = l.getElementsByClassName('active');
    if (items.length > 0) {
        var id = items[0].id;
        var pid = id.substr(id.indexOf("_") + 1); //, id.length - 1
        id.substr()
        return pid;
    }
    else
        return undefined;
}

function sendMsg()
{
    var activeTabElementId = getActiveChildId("msgNav");

    var activeListElementId;
    if (activeTabElementId === "grpPnlTab")
        activeListElementId = "groupList";
    else
        activeListElementId = "corrList";

    var activeTabId = getActiveChildId(activeListElementId);

    var txtMsg = document.getElementById("msgTxt").value;

    if (txtMsg === undefined || txtMsg.toString().trim() === "")
        return;

    $.ajax({
        url: "/Messages/SendSmsMsg",
        type: "POST",
        datatype: "HTML",
        data: { id: activeTabId, msg: txtMsg },
        success: function (data) {

            var list = document.getElementById("messageList");
            var li = document.createElement('li');
            li.innerHTML = data;

            list.appendChild(li);

        },
        fail: function (data) {
            alert('failed to load messages');
        }
        , context: this
    });
}

function RemoveGroup(groupId, groupName) {
    var elementName = "grp_" + groupId;
    var node = document.getElementById(elementName);
    if (node) {
        $("#" + elementName).remove();

        // select previous element, or next element if there are not other elements before it.
        //$("#" + elementName).pre
    }
}