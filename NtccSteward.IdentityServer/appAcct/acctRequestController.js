


'use strict';

angular.module('appAcct').controller('acctRequestController',
    ['$scope', '$location', '$log'
        , function ($scope, $location, $log) {

            var vm = this;
            vm.config = {};
            vm.user = {};

            vm.init = function () {

                $location.path("member");
            };


            return vm;
        }]);
