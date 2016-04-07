/*global angular, alert*/  // JSLint directive

(function () {
    "use strict";

    var app = angular.module("wtacticsApp");

    app.factory("services", ["$resource", "$log", function ($resource, $log) {
        return {
            Login: $resource("/api/Login", {}, { query: { isArray: false } }),
            Logout: $resource("/api/Logout", {}, { query: { isArray: false } }),
            Users: $resource("/api/Users/:guid", {}, { query: { isArray: false } }),
            Cards: $resource("/api/Cards/:guid", {}, { query: { isArray: false }, patch: { method: "PATCH" } }),
            CardSearch: $resource("/api/CardSearch", {}, { query: { isArray: false } }),
            Decks: $resource("/api/Decks/:guid", {}, { query: { isArray: false }, patch: { method: "PATCH" } }),
            DeckSearch: $resource("/api/DeckSearch", {}, { query: { isArray: false } }),
            DeckCards: $resource("/api/DeckCards", {}, { query: { isArray: false } }),
            CardOptions: $resource("/api/CardOptions/:guid", {}, { query: { isArray: false } }),

            ErrorHandler: function (error) {
                // First log all technical details
                var technicalMessage = "An error occurred.";
                if (error.config) {
                    technicalMessage += "\n" + error.config.method + " " + error.config.url;
                }
                if (error.data) {
                    technicalMessage += "\n" + error.data.message + "\n" + error.data.exceptionType + " [" + error.data.exceptionMessage + "]\n" + error.data.stackTrace;
                }
                $log.error(technicalMessage);

                // Then show a dialog
                var userMessage;
                if (error.data) {
                    userMessage = error.data.message || error.data;
                }
                else {
                    userMessage = error.statusText || "An unknown error occurred";
                }

                alert(userMessage);
            },

            // library function : array findIndex
            FindIndex : function (array, predicateFunction) {
                for (var index in array) {
                    var element = array[index];
                    if (predicateFunction(element, index, array)) return index;
                }
                return -1;
            },
            Find : function (array, predicateFunction) {
                for (var index in array) {
                    var element = array[index];
                    if (predicateFunction(element, index, array)) return element;
                }
                return null;
            }
        }
    }]);


    // Directive to convert ',' in decimal input-fields to '.'. This allows the user to enter decimal using either notations.
    app.directive('numberinput', function () {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, element, attrs, ngModelController) {

                ngModelController.$parsers.push(function (data) {
                    //convert data from view format to model format
                    return data.replace(',', "."); //converted
                });
            }
        };
    });

    app.directive('fileModel', ['$parse', function ($parse) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var model = $parse(attrs.fileModel);
                var modelSetter = model.assign;

                element.bind('change', function () {
                    scope.$apply(function () {
                        modelSetter(scope, element[0].files[0]);
                    });
                });
            }
        };
    }]);

    app.service('fileUpload', ['$http', function ($http) {
        this.uploadFileToUrl = function (file, uploadUrl, onSucces, onError) {
            var fd = new FormData();
            fd.append('file', file);
            $http.post(uploadUrl, fd, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            })
            .success(function () {
                if(onSucces) onSucces();
                })
            .error(function () {
                if (onError) onError();
            });
        }
    }]);

}());