angular.module('App').controller('teamController', ['$scope', '$http', function ($scope, $http) {

    $scope.init = function (churchId, title, apiUrl) {
        $scope.churchId = churchId;
        $scope.title = title;
        $scope.apiUrl = apiUrl;
        $scope.EditTeam = new $scope.Team("", "", "", "", "", "");

        // automatically load list
        $scope.loadList();
    }

    $scope.churchId;

    $scope.title = "Teams";

    $scope.apiUrl;

    $scope.teamList = [];

    $scope.memberList = [];

    $scope.EditTeam = null;

    $scope.Team = function (id, name, desc, churchId, teamTypeEnumId, teamPositionEnumTypeId) {
        this.id = id;
        this.name = name;
        this.desc = desc;
        this.churchId = churchId;
        this.teamTypeEnumId = teamTypeEnumId;
        this.teamPositionEnumTypeId = teamPositionEnumTypeId;
    }

    $scope.loadList = function () {
        //http://localhost:62428/church/3/team
        var uri = $scope.apiUrl + "church/" + $scope.churchId + "/team";
        $http.get(uri)
            .then(
                function (response) {

                    var list = $.map(response.data, function (item) { return new $scope.Team(item.id, item.name, item.desc, item.churchId, item.teamTypeEnumId, item.teamPositionEnumTypeId) })

                    $scope.teamList = list;

                    $("#teamTblBody").show();
                }
                , function (error) {
                    console.log("loadList error: " + error);
                }
        );
    }

    $scope.deleteTeam = function (id) {
        //http://localhost:62428/church/3/team
        var uri = $scope.apiUrl + "team/" + id;
        $http.delete(uri)
            .then(
                function (response) {
                    // remove element from list
                    var t = $scope.teamList.find(function (team) {
                        if (team.id == id)
                            return team;
                        else
                            return null;
                    });
                    if (t) {
                        var idx = $scope.teamList.indexOf(t);
                        $scope.teamList.splice(idx, 1);
                    }
                }
                 , function (error) {
                     console.log("deleteTeam error: " + error);
                 }
            );
    }

    $scope.addTeam = function () {

        var newTeam = $scope.EditTeam;
        newTeam.churchId = $scope.churchId;
        newTeam.teamTypeEnumId = 69;  // default to evangelist until we get other team types
        newTeam.teamPositionEnumTypeId = 18; // EvangelicalTeamPositionType (team leader, team member)

        var uri = $scope.apiUrl + "team";

        $http.post(
            uri,
            JSON.stringify(newTeam)
        )
            .then(function (success) {

                var t = success.data;
                var team = new $scope.Team(t.id, t.name, t.desc, t.churchId, t.teamTypeEnumId, t.teamPositionEnumTypeId);

                $scope.teamList.push(team);

                $scope.EditTeam.name = "";
                $scope.EditTeam.desc = "";

                $("#newTeam").modal('hide');
            },
            function (error) {

                alert(error);

            });
    }
}]);