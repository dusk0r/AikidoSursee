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
    .directive('tooltip', function ()
    {
        return {
            restrict: 'A',
            link: function ($scope, $element, $attrs)
            {
                $element[0].onmouseover = function ()
                {
                    // on mouseenter
                    $($element[0]).tooltip('show');
                };
                $element[0].onmouseout = function ()
                {
                    // on mouseleave
                    $($element[0]).tooltip('hide');
                };
            }
        };
    })
    .directive('datum', function ()
    {
        return {
            restrict: 'E',
            scope: {
                datum: '=value'
            },
            controller: function ($scope, $element, $timeout)
            {
                var datum = moment($scope.datum);
                $scope.dateString = datum.format("DD.MM.YYYY HH:mm");
                $scope.agoString = datum.fromNow();

                $timeout(function ()
                {
                    $scope.agoString = datum.fromNow();
                }, 600000);
            },
            templateUrl: '/Content/component/datumComponent.html',
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