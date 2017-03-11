
angular.module('App').controller('appController', ['appService', function (appService) {

    $scope.init = function (apiUrl) {

        appService.apiRoot = apiUrl;

    }
}]);