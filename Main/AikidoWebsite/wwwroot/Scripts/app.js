angular.module('aikidoApp', [])
    .filter('moment', function ()
    {
        return function (input, format)
        {
            return moment(input).format(format);
        };
    })
    .directive('loginWindow', function ()
    {
        return {
            restrict: 'E',
            scope: {},
            controller: function ()
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
            link: ["$scope", "$element", "$attrs", function ($scope, $element, $attrs)
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
            }]
        };
    })
    .directive('datum', function ()
    {
        return {
            restrict: 'E',
            scope: {
                datum: '=value'
            },
            controller: ["$scope", "$element", "$timeout", function ($scope, $element, $timeout)
            {
                var datum = moment($scope.datum);
                $scope.dateString = datum.format("DD.MM.YYYY HH:mm");
                $scope.agoString = datum.fromNow();

                $timeout(function ()
                {
                    $scope.agoString = datum.fromNow();
                }, 600000);
            }],
            templateUrl: '/Content/component/datumComponent.html',
            replace: true
        };
    })
    .directive('mitteilungen', function ()
    {
        return {
            restrict: 'E',
            scope: {},
            controller: ["$scope", "$element", "$http", "$sce", function ($scope, $element, $http, $sce)
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
            }],
            templateUrl: '/Content/component/mitteilungenComponent.html',
            replace: true
        };
    })
    .directive('termine', function ()
    {
        return {
            restrict: 'E',
            scope: {},
            controller: ["$scope", "$element", "$http", "$sce", function ($scope, $element, $http, $sce)
            {
                getTermine = function ()
                {
                    $http.get('/Aktuelles/GetTermine').then(function (resp) 
                    {
                        $scope.data = resp.data;
                    });
                }

                $scope.getDatumString = function (termin)
                {
                    if (termin.endDatum)
                    {
                        if (moment(termin.startDatum).isSame(termin.endDatum, "day"))
                        {
                            return moment(termin.startDatum).format("DD.MM.YYYY HH:mm") + " bis " + moment(termin.endDatum).format("HH:mm");
                        }
                        else
                        {
                            return moment(termin.startDatum).format("DD.MM.YYYY") + " bis " + moment(termin.endDatum).format("DD.MM.YYYY");
                        }
                    }
                    else
                    {
                        return moment(termin.startDatum).format("DD.MM.YYYY");
                    }
                }

                getTermine();
            }],
            templateUrl: '/Content/component/termineComponent.html',
            replace: true
        };
    })
    .directive('hinweis', function ()
    {
        return {
            restrict: 'E',
            scope: {},
            controller: ["$scope", "$http", "$timeout", "$sce", function ($scope, $http, $timeout, $sce)
            {
                function updateHinweis()
                {
                    $http.get('/Aktuelles/Hinweis').then(function (resp) 
                    {
                        $scope.data = resp.data;
                        $scope.data.html = $sce.trustAsHtml($scope.data.html);
                        if (window.localStorage["hinweisKey"] !== resp.data.dateModified)
                        {
                            $scope.showHinweis = true;
                        }
                    });
                }

                $scope.closeHinweis = function ()
                {
                    window.localStorage.setItem("hinweisKey", $scope.data.dateModified);
                    $scope.showHinweis = false;
                }

                updateHinweis();
            }],
            templateUrl: '/Content/component/hinweisComponent.html',
            replace: true
        };
    });