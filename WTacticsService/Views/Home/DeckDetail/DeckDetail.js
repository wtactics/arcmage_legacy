
/*global angular*/  // JSHint directive

(function () {
    "use strict";

    // get the application
    var wtacticsApp = angular.module("wtacticsApp");

    // define a template for card items
    wtacticsApp.directive("deckCardListItem", function () {
        return {
            restrict: "A",
            templateUrl: "/Views/Home/DeckDetail/DeckCardListItem.html"
        };
    });

    // define a template for card items
    wtacticsApp.directive("deckCardSearchListItem", function () {
        return {
            restrict: "A",
            templateUrl: "/Views/Home/DeckDetail/DeckCardSearchListItem.html"
        };
    });


    // define the controller for projects
    wtacticsApp.controller("DeckDetailController",
        ["$scope", "$state", "$stateParams", "services",
        function ($scope, $state, $stateParams, services) {

            $scope.deckGuid = $stateParams.deckGuid;
            $scope.isLoading = true;
            $scope.isGenerating = false;
            $scope.deck = {};

            $scope.searchClicked = function () {

                $scope.isLoading = true;
                $scope.cards = [];
             
                services.Cards.query({ search: $scope.search}, function (response) {
                    $scope.cards = response.items;
                    $scope.isLoading = false;
                }, services.ErrorHandler);
            };

            $scope.generateDeck = function () {

                var data = {
                    guid: $scope.deck.guid,
                    name: $scope.deck.name
                };
                $scope.deck.isPdfAvailable = false;

                services.Decks.patch({ guid: $scope.deck.guid }, data, function (deck) {
                    $scope.deck.name = deck.name,
                    $scope.deck.isPdfAvailable = deck.isPdfAvailable;
                    $scope.isGenerating = false;
                }, function (error) {
                    services.ErrorHandler(error);
                    $scope.deck.isPdfAvailable = false;
                    $scope.isGenerating = false;
                });
            }

            function saveDeckCard (deckCard, index) {
                $scope.isLoading = true;
                deckCard.deck = { guid: $scope.deck.guid };
                services.DeckCards.save(deckCard , function (response) {
                    if (index && deckCard.quantity <= 0) {
                        $scope.deck.deckCards.splice(index, 1);    
                    }
                    deckCard.deck = $scope.deck;
                    $scope.isLoading = false;
                }, function (error) {
                    services.ErrorHandler(error);
                    deckCard.deck = $scope.deck;
                    $scope.isLoading = false;
                });
            }

            $scope.increaseDeckCard = function(card, deck) {
              
                var deckCard = services.Find($scope.deck.deckCards, function(dc) {
                    return dc.card.guid === card.guid;
                });

                if (!deckCard) {
                    deckCard = { deck: deck, card: card, quantity: 0 }
                    $scope.deck.deckCards.push(deckCard);
                }
                deckCard.quantity++;
                saveDeckCard(deckCard);
            }

            $scope.decreaseDeckCard = function(card, deck, index) {
                var deckCard = services.Find($scope.deck.deckCards, function (dc) {
                    return dc.card.guid === card.guid;
                });
                if (deckCard) {
                    deckCard.quantity--;
                    saveDeckCard(deckCard, index);
                }
            }


            services.Decks.get({ guid: $scope.deckGuid }, function (deck) {
                $scope.deck = deck;
                $scope.isLoading = false;
            }, function (error) {
                services.ErrorHandler(error);
                $scope.isLoading = false;
            });

        }]);
})();