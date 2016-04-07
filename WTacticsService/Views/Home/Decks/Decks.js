
/*global angular*/  // JSHint directive

(function () {
    "use strict";

    // get the application
    var wtacticsApp = angular.module("wtacticsApp");

    // define a template for deck items
    wtacticsApp.directive("deckListItem", function () {
        return {
            restrict: "A",
            templateUrl: "/Views/Home/Decks/DeckListItem.html"
        };
    });

    // define the controller for decks
    wtacticsApp.controller("DecksController",
        ["$scope", "$state", "$stateParams", "$location", "$uibModal", "$timeout", "services",
        function ($scope, $state, $stateParams, $location, $uibModal, $timeout, services) {

            $scope.isAddOpen = false;
            $scope.isCreating = false;
            $scope.deck = { name: "" };
            $scope.isLoading = false;
            $scope.decks = [];
            $scope.totalItems = null;

            function createDeckFailed(error) {
                $scope.isCreating = false;
                services.ErrorHandler(error);
            }

            function createDeck(data, onSuccessAction, onFailedAction) {
                $scope.isCreating = true;
                services.Decks.save(data, function (deck) {
                    $scope.deck = deck;
                    onSuccessAction();
                }, onFailedAction);
            }

            function navigateToDeckDetail() {
                $scope.isAddOpen = false;
                $scope.isCreating = false;
                $state.go("deckdetail", { deckGuid: $scope.deck.guid });
            }

            $scope.createClicked = function () {
                var data = { name: $scope.deck.name };
                createDeck(data, navigateToDeckDetail, createDeckFailed);
            };

            $scope.deckSearchOptions = {
                search : "",
                pageSize : 30,
                pageNumber : 1,
            };

            $scope.searchClicked = function () {
                $scope.isLoading = true;
                $scope.decks = [];
                services.DeckSearch.save($scope.deckSearchOptions, function (response) {
                    $scope.decks = response.items;
                    $scope.totalItems = response.totalItems;
                    $scope.deckSearchOptions = response.searchOptions;
                    $scope.isLoading = false;
                }, services.ErrorHandler);
            }

           
            $scope.addClicked = function () {
                $scope.isAddOpen = true;
                $('.add-new .form-wrapper input.first').focus();
            };

            $scope.addCancelClicked = function () {
                $scope.isAddOpen = false;
            };

            $scope.searchClicked();

        }]);
})();