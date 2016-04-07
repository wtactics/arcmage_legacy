
/*global angular*/  // JSHint directive

(function () {
    "use strict";

    // get the application
    var wtacticsApp = angular.module("wtacticsApp");

    // define the controller for projects
    wtacticsApp.controller("SignUpController",
        ["$scope", "$state", "$stateParams", "services", "md5",
        function ($scope, $state, $stateParams, services, md5) {

            $scope.isSingingUp = false;

            $scope.user = {
                name: null,
                email: null,
                password: null,
                password2: null,
            }

            $scope.signup = function () {

                $scope.isSingingUp = true;
                $scope.user.password = md5.createHash($scope.user.password || '');
                $scope.user.password2 = md5.createHash($scope.user.password2 || '');

                var data = $scope.user;
                services.Users.save(data, function (response) {
                    
                    $scope.isSingingUp = false;
                    $scope.$parent.isLogedIn = true;
                    services.Users.get({ id: "me" }, function (user) {
                        $scope.$parent.user = user;
                        $state.go("cards");
                    }, services.ErrorHandler);

                }, function (error) {
                    $scope.user = {
                        name: null,
                        email: null,
                        password: null,
                        password2: null,
                    }
                    $scope.isSingingUp = false;
                    services.ErrorHandler(error);
                    
                });
            }

          

           

        }]);
})();