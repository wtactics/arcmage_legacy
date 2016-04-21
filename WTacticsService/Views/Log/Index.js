(function () {
    "use strict";

    var app = angular.module("logApp", ["ngResource"]);

    app.factory("services", ["$resource", function ($resource) {
        return {
            Log: $resource("/api/Log", {}, { query: { isArray: false } }),
        }
    }]);

    app.directive("logEventListItem", function () {
        return {
            restrict: "A",
            templateUrl: "/Views/Log/LogEventListItem.html"
        };
    });

    app.controller("LogController",
    ["$scope", "services",
    function ($scope, services) {

        $scope.logevents = [];   // Will be filled in with data from the server

        services.Log.query({}, function (response) {
            angular.forEach(response.items, function (item) {
                item.timestamp = Date.parse(item.timestamp);
            });
            $scope.logevents = response.items;
        });
    }]);
})();