angular.module('aikidoApp', [])
    .directive('loginWindow', function ()
    {
        return {
            restrict: 'E',
            scope: {},
            controller: function ($scope, $element)
            {

            },
            templateUrl: '/Content/component/loginComponent.html',
            replace: true
        };
    });