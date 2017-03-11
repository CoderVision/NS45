
angular.module('App').factory('appService', ['$http', function ($http) {

    var svc = {};

    svc.apiRoot = "";

    svc.createEnum = function (e) {

        var enm = {};

        if (e != undefined) {
            enm.enumID = e.enumID;
            enm.enumDesc = e.enumDesc;
            enm.enumTypeID = e.enumTypeID; // selector:  EnumTypeId = 16
            enm.sortOrder = e.sortOrder;
        }
        else {
            enm.name = "";
            enm.typeDesc = "";
            enm.typeId = "";
            enm.typeSortOrder = "";
        }

        return enm;
    }

    return menu;
}]);