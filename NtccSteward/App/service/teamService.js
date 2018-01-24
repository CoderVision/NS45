
// the menu service will allow us to update the side-bar menu by sharing data
angular.module('App').factory('teamService', ['$http','appService',function ($http,appService) {
    var svc = {};
    
    svc.getTeamProfile = function(teamId){
        return $http.get(appService.apiRoot + 'Team/GetTeamProfile/' + teamId);
    }

    svc.getTeamProfileMetadata = function (teamId) {
        return $http.get(appService.apiRoot + 'team/{' + teamId + '}/profileMetadata', { cache: true });
    }

    svc.createProfile = function (profile) {
        var p = {};

        if (profile != undefined)
        {
            p.teamId = profile.teamId;
            p.churchId = profile.churchId;
            p.name = profile.name;  // team name
            p.desc = profile.desc;
            p.teamTypeEnumId = profile.teamTypeEnumId;
            p.teamPositionEnumTypeId = profile.teamPositionEnumTypeId;
            p.teammates = $.map(profile.teammates, function (team) { return teamService.createTeamMember(team); })
        }

        return p;
    }

    svc.createTeamMember = function (team) {
        var member = {};

        if (team != undefined)
        {
            member.name = name;
            member.typeDesc = typeDesc;
            member.typeId = typeId; // selector:  EnumTypeId = 16
            member.typeSortOrder = -1;
        }
        else
        {
            member.name = "";
            member.typeDesc = "";
            member.typeId = "";
            member.typeSortOrder = "";
        }

        return member;
    }

    return menu;
}]);