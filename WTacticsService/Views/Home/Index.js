/*global angular*/  // JSHint directive

(function () { 
    "use strict";

    var wtacticsApp = angular.module("wtacticsApp");

    wtacticsApp.config(["$stateProvider", "$urlRouterProvider", function ($stateProvider, $urlRouterProvider) {
    
        $urlRouterProvider.otherwise("/cards");

        $stateProvider

            // list of cards
            .state("cards", {
                url: "/cards",
                templateUrl: "/Views/Home/Cards/Cards.html",
                controller: "CardsController"
            })
            // card detail
            .state("carddetail", {
                url: "/cards/:cardGuid",
                templateUrl: "/Views/Home/CardDetail/CardDetail.html",
                controller: "CardDetailController"
            })
            .state("decks", {
                url: "/decks",
                templateUrl: "/Views/Home/Decks/Decks.html",
                controller: "DecksController"
            })
            .state("deckdetail", {
                url: "/decks/:deckGuid",
                templateUrl: "/Views/Home/DeckDetail/DeckDetail.html",
                controller: "DeckDetailController"
            })
            .state("games", {
                url: "/games",
                templateUrl: "/Views/Home/Games/Games.html",
                controller: "GamesController"
            })
            .state("signup", {
                url: "/signup",
                templateUrl: "/Views/Home/SignUp/SignUp.html",
                controller: "SignUpController"
            });


    }]).run(["editableOptions",function(editableOptions) {
        editableOptions.theme = 'bs3';
    }]);

    wtacticsApp.controller("ApplicationController",
        ["$scope", "$state", "md5", "services",
        function ($scope, $state, md5, services) {

            $scope.isLogedIn = false;
            $scope.login = { email : "", password : "" }
            $scope.password = null;

            $scope.logout = function() {
                services.Logout.save(null, function (response) {
                    $scope.isLogedIn = false;
                    $scope.user = null;
                    $state.reload();

                }, services.ErrorHandler);
            }

            $scope.tryLogin = function (login) {

                if (login != null) {
                    login.password = md5.createHash(login.password || '');
                }
                services.Login.save(login, function (response) {
                    $scope.isLogedIn = true;
                    $scope.login.email = "";
                    $scope.login.password = "";

                    services.Users.get({ id: "me" }, function (user) {
                        $scope.user = user;
                        $state.reload();
                    }, services.ErrorHandler);

                }, function (error) {
                    $scope.login.email = "";
                    $scope.login.password = "";
                });
            }

            // auto login
            $scope.tryLogin(null);

            
            $scope.year = new Date().getFullYear();
        }]);


    wtacticsApp.filter("format", function () {
       return function (input) {
           var args = arguments;
           return input.replace(/\{(\d+)\}/g, function (match, capture) {
               return args[1 * capture + 1];
           });
       };
   });

   
    wtacticsApp.directive("compareTo", function () {
        return {
            require: "ngModel",
            scope: {
                otherModelValue: "=compareTo"
            },
            link: function (scope, element, attributes, ngModel) {

                ngModel.$validators.compareTo = function (modelValue) {
                    return modelValue == scope.otherModelValue;
                };

                scope.$watch("otherModelValue", function () {
                    ngModel.$validate();
                });
            }
        };
    });

})();