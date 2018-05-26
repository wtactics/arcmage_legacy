
/*global angular*/  // JSHint directive

(function () {
    "use strict";

    // get the application
    var wtacticsApp = angular.module("wtacticsApp");

    // define a template for card items
    wtacticsApp.directive("cardListItem", function () {
        return {
            restrict: "A",
            templateUrl: "/Views/Home/Cards/CardListItem.html"
        };
    });

    // define the controller for cards
    wtacticsApp.controller("CardsController",
        ["$scope", "$state", "$stateParams", "$location", "$uibModal", "$timeout", "services",
        function ($scope, $state, $stateParams, $location, $uibModal, $timeout, services) {

            $scope.isAddOpen = false;
            $scope.isCreating = false;
            $scope.card = { name: "" };
            $scope.isLoading = false;
            $scope.cards = [];
            $scope.totalItems = null;

            function createCardFailed(error) {
                $scope.isCreating = false;
                services.ErrorHandler(error);
            }

            function createCard(data, onSuccessAction, onFailedAction) {
                $scope.isCreating = true;
                services.Cards.save(data, function (card) {
                    $scope.card = card;
                    onSuccessAction();
                }, onFailedAction);
            }

            function navigateToCardDetail() {
                $scope.isAddOpen = false;
                $scope.isCreating = false;
                $state.go("carddetail", { cardGuid: $scope.card.guid });
            }

            $scope.createClicked = function () {
                var data = { name: $scope.card.name };
                createCard(data, navigateToCardDetail, createCardFailed);
            };

            $scope.cardSearchOptions = {
                search : "",
                pageSize : 30,
                pageNumber: 1,
                reverseOrder: false,
                orderBy: "Name"
            };

            $scope.searchClicked = function () {
                $scope.isLoading = true;
                $scope.cards = [];
                services.CardSearch.save($scope.cardSearchOptions, function (response) {
                    $scope.cards = response.items;
                    $scope.totalItems = response.totalItems;
                    $scope.cardSearchOptions = response.searchOptions;
                    $scope.isLoading = false;
                }, services.ErrorHandler);
            }

            $scope.sortOrderClicked = function (orderBy) {

                if ($scope.cardSearchOptions.orderBy === orderBy) {
                    $scope.cardSearchOptions.reverseOrder = !$scope.cardSearchOptions.reverseOrder;
                }
                else
                {
                    $scope.cardSearchOptions.reverseOrder = false;
                }
                $scope.cardSearchOptions.orderBy = orderBy;
                $scope.searchClicked();
            }

           
            $scope.addClicked = function () {
                $scope.isAddOpen = true;
                $('.add-new .form-wrapper input.first').focus();
            };

            $scope.addCancelClicked = function () {
                $scope.isAddOpen = false;
            };

            $scope.clearSearch = function () {
                $scope.cardSearchOptions.cardType = null;
                $scope.cardSearchOptions.faction = null;
                $scope.cardSearchOptions.cost = null;
                $scope.cardSearchOptions.loyalty = null;
                $scope.cardSearchOptions.serie = null;
                if (!$scope.isLogedIn) {
                    $scope.cardSearchOptions.status = services
                        .Find($scope.cardOptions.statuses,
                            function (e) { return e.guid === "7dedc883-5dd2-5f17-b2a4-eaf04f7ad464" });
                } else {
                    $scope.cardSearchOptions.status = null;
                }
                
            }

            $scope.isLoading = true;

            services.CardOptions.get(function (result) {
                $scope.cardOptions = result;

                $scope.clearSearch();

                $scope.searchClicked();

            }, function (error) {
                services.ErrorHandler(error);
                $scope.isLoading = false;
            });
          

        }]);
})();