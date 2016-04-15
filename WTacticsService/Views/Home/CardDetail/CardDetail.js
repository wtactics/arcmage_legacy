
/*global angular*/  // JSHint directive

(function () {
    "use strict";

    // get the application
    var wtacticsApp = angular.module("wtacticsApp");

    // define the controller for projects
    wtacticsApp.controller("CardDetailController",
    [
        "$scope", "$timeout", "$state", "$stateParams", "services", "fileUpload", "$sce",
        function($scope, $timeout, $state, $stateParams, services, fileUpload, $sce) {

            $scope.cardGuid = $stateParams.cardGuid;
            $scope.isUploading = false;
            $scope.isPngAvailble = false;
            $scope.layoutTextInfo = $sce.trustAsHtml(
                'The layout text, shown on the card. <br/><br/>' +
                'All xml-tags must be in a <b>p</b> tag.<br>' +
                'All text must be in a xml-tag.<br/>' +
                'Tags are not nestable.<br/><br/>' +

                '<b>Xml markup options :</b><br/>' +
                '<b>p</b> - new paragraph <br />' +
                '<b>n</b> - normal text <br />' +
                '<b>b</b> - bold text <br />' +
                '<b>i</b> - italic text <br />' +
                '<b>bi</b> - bold italic text <br />' +
                '<b>br</b> - force line break <br />' +
                '<b>mis</b> - mark arrow small<br />' +
                '<b>g0-g9,gx,gt</b> - cypher symbol 0-9, x<br /><br />' +

                '<b>Only valid on start of paragraph</b><br />' +
                '<b>c</b> - capital<br />' +
                '<b>m</b> - mark<br />' +
                '<b>mi</b> - mark arrow<br />' +
                '<b>m0-m9,mx,ma,mt</b> - mark 0-9, x, t, a<br />' +
                '<b>mc</b> - mark, align column<br />' +
                '<b>mic</b> - mark arrow, align column<br />'
                );


            function checkPngAvailability() {

                if (!$scope.card.isPngAvailble) {

                    services.Cards.get({ guid: $scope.card.guid }, function (card) {
                        $scope.card.isPngAvailble = card.isPngAvailble;
                        $timeout(checkPngAvailability, 3*60*1000);
                    }, function (error) {
                        services.ErrorHandler(error);
                    });
                }
            }


            function setCard(card) {
                card.type = services.Find($scope.cardOptions.cardTypes, function (e) { return e.guid === card.type.guid });
                $scope.card = card;
                checkPngAvailability();
            }

            
            $scope.selectRuleSet = function (ruleSet) {
                $scope.card.ruleSet = ruleSet;
            }


            $scope.selectCardType = function(cardType) {
                $scope.card.type = cardType;
            }

            $scope.selectSerie = function (serie) {
                $scope.card.serie = serie;
            }

            $scope.selectLoyalty = function (loyalty) {
                $scope.card.loyalty = loyalty;
            }

            $scope.selectStatus = function (status) {
                $scope.card.status = status;
            }
            
            $scope.selectFaction = function (faction) {
                $scope.card.faction = faction;
            }

            $scope.uploadFile = function() {
                var file = $scope.artFile;
                var uploadUrl = "api/FileUpload/" + $scope.card.guid;
                $scope.isUploading = true;
                fileUpload.uploadFileToUrl(file, uploadUrl, function () {
                    $scope.artFile = null;
                    $scope.isUploading = false;
                });
                
            }

            $scope.generateCard = function () {

                $scope.card.lowResolutionPng = null;
                $scope.card.highResolutionPng = null;
                $scope.isGenerating = true;

                services.Cards.patch({ guid: $scope.cardGuid }, $scope.card, function (card) {
                    setCard(card);
                    $scope.isGenerating = false;
                }, function (error) {
                    services.ErrorHandler(error);
                    $scope.isGenerating = false;
                });
            }

            $scope.isGenerating = false;
            $scope.isLoading = true;

            services.CardOptions.get({ guid: $scope.cardGuid }, function (result) {

                $scope.cardOptions = result;
             
                services.Cards.get({ guid: $scope.cardGuid }, function (card) {
                    setCard(card);
                    $scope.isLoading = false;
                }, function (error) {
                    services.ErrorHandler(error);
                    $scope.isLoading = false;
                });

            }, function (error) {
                services.ErrorHandler(error);
                $scope.isLoading = false;
            });

           

        }]);
})();