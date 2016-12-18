
/*global angular*/  // JSHint directive

(function () {
    "use strict";

    // get the application
    var wtacticsApp = angular.module("wtacticsApp");

    // define a template for game items
    wtacticsApp.directive("gameListItem", function () {
        return {
            restrict: "A",
            templateUrl: "/Views/Home/Games/GameListItem.html"
        };
    });

    // define the controller for games
    wtacticsApp.controller("GamesController",
        ["$scope", "$state", "$stateParams", "$location", "$uibModal", "$timeout", "services",
        function ($scope, $state, $stateParams, $location, $uibModal, $timeout, services) {

            $scope.isAddOpen = false;
            $scope.isCreating = false;
            $scope.game = { name: "" };
            $scope.isLoading = false;
            $scope.games = [];
            $scope.decks = [];
            $scope.selecteddeck = { name: "" };
            $scope.totalItems = null;

            function guid() {
                function s4() { return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1); }
                return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
            }

            function createGameFailed(error) {
                $scope.isCreating = false;
                services.ErrorHandler(error);
            }

            function createGame(data, onSuccessAction, onFailedAction) {
                $scope.isCreating = true;
                services.Games.save(data, function (game) {
                    $scope.game = game;
                    onSuccessAction();
                }, onFailedAction);
            }

            function refreshGameList() {
                $scope.isAddOpen = false;
                $scope.isCreating = false;
                $scope.searchClicked();
            }

            $scope.createClicked = function () {
                var data = { name: $scope.game.name };
                createGame(data, refreshGameList, createGameFailed);
            };

            $scope.gameSearchOptions = {
                search : "",
                pageSize : 30,
                pageNumber: 1,
                reverseOrder: true,
                orderBy: "CreateTime"
            };

            $scope.searchClicked = function () {
                $scope.isLoading = true;
                $scope.games = [];
                $scope.decks = [];

                services.Decks.get({},function (response) {
                    $scope.decks = response.items;
                    $scope.selecteddeck = { name: "" };

                    services.GameSearch.save($scope.gameSearchOptions, function (response) {
                        $scope.games = response.items;
                        $scope.totalItems = response.totalItems;
                        $scope.gameSearchOptions = response.searchOptions;
                        $scope.isLoading = false;
                    }, services.ErrorHandler);
                    
                }, services.ErrorHandler);
            }

            $scope.sortOrderClicked = function (orderBy) {

                if ($scope.gameSearchOptions.orderBy === orderBy) {
                    $scope.gameSearchOptions.reverseOrder = !$scope.gameSearchOptions.reverseOrder;
                }
                else
                {
                    $scope.gameSearchOptions.reverseOrder = false;
                }
                $scope.gameSearchOptions.orderBy = orderBy;
                $scope.searchClicked();
            }

           
            $scope.addClicked = function () {
                $scope.isAddOpen = true;
                $('.add-new .form-wrapper input.first').focus();
            };

            $scope.addCancelClicked = function () {
                $scope.isAddOpen = false;
            };

            function joinGame(game) {

                var url = "/WTactics/Game/index.html?gameGuid=" + game.guid + "&playerGuid=" + guid() + "&deckGuid=" + $scope.selecteddeck.guid;
                window.open(url, '_blank');
            };

            function selectDeck(deck) {
                $scope.selecteddeck = deck;
            }
            
            $scope.joinGame = joinGame;
            $scope.selectDeck = selectDeck;

            $scope.searchClicked();

        }]);
})();