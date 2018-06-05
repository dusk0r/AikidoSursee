angular.module('aikidoApp', [])
    .directive('loginWindow', function ()
    {
        return {
            restrict: 'E',
            scope: {},
            controller: function ($scope, $element)
            {

            },
            template: '<span>Hallo</span>',
            replace: true
        };
    });