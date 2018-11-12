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
            controller: ["$scope", "$element", "$timeout", function ($scope, $element, $timeout)
            {
                var datum = moment($scope.datum);
                $scope.dateString = datum.format("DD.MM.YYYY HH:mm");
                $scope.agoString = datum.fromNow();

                function refresh()
                {
                    $timeout(function ()
                    {
                        $scope.agoString = datum.fromNow();
                        refresh();
                    }, 600000 /* 10 Min */);
                }

                refresh();
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
                    $http.get('/Aktuelles/GetMitteilungen', { params: { start: start || 0 } }).then(function (resp) 
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
                };

                $scope.getOlder = function ()
                {
                    $scope.getMitteilungen($scope.data.start + $scope.data.perPage);
                };
                $scope.getNewer = function ()
                {
                    $scope.getMitteilungen(Math.max($scope.data.start - $scope.data.perPage, 0));
                };
                $scope.deleteMitteilung = function (mitteilung)
                {
                    $http.delete('/Aktuelles/DeleteMitteilung/' + encodeURIComponent(mitteilung.id)).then(
                        function () { $scope.getMitteilungen($scope.data.start); },
                        function () { alert("Konnte Mitteilung nicht löschen"); }
                    );
                };

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
                };

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
                };

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
                        console.log(window.localStorage["hinweisKey"] + " !== " + resp.data.dateModified);
                        if (window.localStorage["hinweisKey"] !== resp.data.dateModified)
                        {
                            $scope.showHinweis = true;
                        }

                        $timeout(function ()
                        {
                            updateHinweis();
                        }, 600000 /* 10 Min */);
                    });
                }

                $scope.closeHinweis = function ()
                {
                    window.localStorage.setItem("hinweisKey", $scope.data.dateModified);
                    $scope.showHinweis = false;
                };

                updateHinweis();
            }],
            templateUrl: '/Content/component/hinweisComponent.html',
            replace: true
        };
    })
    .directive('albums', function ()
    {
        return {
            restrict: 'E',
            scope: {},
            controller: ["$scope", "$http", function ($scope, $http)
            {
                function loadAlbums()
                {
                    $http.get('/Dojo/ListAlbums').then(function (resp) 
                    {
                        $scope.albums = resp.data;
                        $scope.currentAlbum = resp.data[0];
                    });
                }

                $scope.setAlbum = function (album)
                {
                    $scope.currentAlbum = album;
                };

                loadAlbums();

            }],
            templateUrl: '/Content/component/albumsComponent.html',
            replace: true
        };
    })
    .directive('album', function ()
    {
        return {
            restrict: 'E',
            scope: {
                album: '<album'
            },
            controller: ["$scope", "$http", function ($scope, $http)
            {
                $scope.$watch('album', function (newValue)
                {
                    if (newValue)
                    {
                        $http.get('/Dojo/ListBilder/' + newValue.photosetId).then(function (resp) 
                        {
                            $scope.images = resp.data;
                            $scope.currentImageIndex = 0;
                            $scope.currentImage = resp.data[$scope.currentImageIndex];
                        });
                    }
                }, true);

                $scope.nextImage = function ()
                {
                    $scope.currentImageIndex = ($scope.currentImageIndex + 1) % $scope.images.length;
                    $scope.currentImage = $scope.images[$scope.currentImageIndex];
                };

                $scope.prevImage = function ()
                {
                    $scope.currentImageIndex = $scope.currentImageIndex === 0 ?
                        $scope.images.length - 1 :
                        $scope.currentImageIndex - 1;
                    $scope.currentImage = $scope.images[$scope.currentImageIndex];
                };
            }],
            templateUrl: '/Content/component/albumComponent.html',
            replace: true
        };
    });