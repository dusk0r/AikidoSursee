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
    })
    .directive('mitteilungen', function ()
    {
        return {
            restrict: 'E',
            scope: {},
            controller: function ($scope, $element, $http, $sce)
            {
                $scope.getMitteilungen = function (start)
                {
                    $http.get('/Aktuelles/GetMitteilungen', { params: { start: start || 0 }}).then(function (resp) 
                    {
                        $scope.data = resp.data;
                        for (var idx in $scope.data.mitteilungen)
                        {
                            var mitteilung = $scope.data.mitteilungen[idx];
                            mitteilung.html = $sce.trustAsHtml(mitteilung.html);
                        }
                        $scope.hasOlder = resp.data.mitteilungenCount >= (resp.data.start + resp.data.perPage);
                        $scope.hasNewer = resp.data.start !== 0;
                    });
                }

                $scope.getOlder = function ()
                { 
                    $scope.getMitteilungen($scope.data.start + $scope.data.perPage);
                }
                $scope.getNewer = function ()
                {
                    $scope.getMitteilungen(Math.max($scope.data.start - $scope.data.perPage, 0));
                }

                $scope.getMitteilungen(0);
            },
            templateUrl: '/Content/component/mitteilungenComponent.html',
            replace: true
        };
    });