


'use strict';

angular.module('appAcct').controller('acctRequestController',
    ['$scope', '$location', '$log',  '$http'
        , function ($scope, $location, $log, $http) {

            var vm = this;
            vm.config = {};
            vm.user = {};

            vm.init = function () {

               // $location.path("member");
                vm.loadConfig();
            };

            vm.reset = function (form) {
                if (form) {
                    form.$setPristine();
                    form.$setUntouched();
                }
                $scope.user = {};
            };

            vm.loadConfig = function () {

                var uri = "account/getconfig";
                $http.get(uri).then(function (success) {

                    vm.config = success.data;

                }, function (error) {
                    console.log(error | error.message);
                });
            }

            vm.getPostUrl = function()
            {
                //signin=d30cb231fac523dd6ccbf62a300b1b74
                return "Account?signin=" + $location.search().signin;
            }

            return vm;
        }]);
