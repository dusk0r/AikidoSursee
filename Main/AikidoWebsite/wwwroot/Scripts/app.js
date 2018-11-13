angular.module('aikidoApp', [])
    .config(function ($locationProvider)
    {
        $locationProvider.html5Mode({
            enabled: true,
            rewriteLinks: false
        });
    })
    .filter('moment', function ()
    {
        return function (input, format)
        {
            return moment(input).format(format);
        };
    })
    .filter('urlencode', function ()
    {
        return function (input)
        {
            return window.encodeURIComponent(input);
        };
    })
    .filter('documentIdPart', function ()
    {
        return function (input)
        {
            return input.split('/')[1];
        };
    })
    .filter('bytes', function ()
    {
        return function (bytes, precision)
        {
            if (isNaN(parseFloat(bytes)) || !isFinite(bytes)) return '-';
            if (typeof precision === 'undefined') precision = 1;
            var units = ['bytes', 'kB', 'MB', 'GB', 'TB', 'PB'],
                number = Math.floor(Math.log(bytes) / Math.log(1024));
            return (bytes / Math.pow(1024, Math.floor(number))).toFixed(precision) + ' ' + units[number];
        };
    })
    .directive("selectNgFiles", function ()
    {
        return {
            require: "ngModel",
            link: function postLink(scope, elem, attrs, ngModel)
            {
                elem.on("change", function (e)
                {
                    var files = elem[0].files;
                    ngModel.$setViewValue(files);
                });
            }
        };
    })
    .directive('syntaxHelp', function ()
    {
        return {
            restrict: 'E',
            templateUrl: '/Content/component/syntaxHelpCompoment.html',
            replace: true
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
            controller: ["$scope", "$timeout", "$http", "$sce", function ($scope, $timeout, $http, $sce)
            {
                $scope.getMitteilungen = function (start)
                {
                    $scope.data = null;
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
                $scope.editMitteilung = function (mitteilung)
                {
                    window.location.href = "/Aktuelles/EditMitteilung?id=" + mitteilung.id.split('/')[1];
                };

                $(document).keydown(function (e)
                {
                    if (e.which === 37) // <-
                    {
                        $timeout(function () { $scope.getNewer(); }, 0);
                    }
                    if (e.which === 39) // ->
                    {
                        $timeout(function () { $scope.getOlder(); }, 0);
                    }
                });

                $scope.getMitteilungen(0);
            }],
            templateUrl: '/Content/component/mitteilungenComponent.html',
            replace: true
        };
    })
    .directive('mitteilungEdit', function ()
    {
        return {
            restrict: 'E',
            scope: {},
            controller: ["$scope", "$http", "$location", "$sce", function ($scope, $http, $location, $sce)
            {
                $scope.uploadFiles = [];
                $scope.uploadFileBezeichnung = undefined;

                function loadMitteilung(mitteilungId)
                {
                    $http.get('/Aktuelles/LoadMitteilungEditModel', { params: { id: mitteilungId } }).then(function (resp) 
                    {
                        $scope.data = resp.data;
                        $scope.data.mitteilung.html = $sce.trustAsHtml(resp.data.mitteilung.html);
                    });
                }

                $scope.generatePreview = function () {
                    $http.post('/Aktuelles/ParseCreole', { text: $scope.data.mitteilung.text }).then(function (resp)
                    {
                        $scope.preview = $sce.trustAsHtml(resp.data);
                    });
                };

                $scope.uploadFile = function ()
                {
                    var fd = new FormData();
                    fd.append("file", $scope.uploadFiles[0]);
                    fd.append("bezeichnung", $scope.uploadFileBezeichnung);
                    $http.post('/Aktuelles/UploadFile', fd, {
                        transformRequest: angular.identity,
                        headers: { 'Content-Type': undefined }
                    }).then(function (resp)
                    {
                        console.log($scope.uploadFiles[0]);
                        $scope.data.dateien.push({
                            id: resp.data,
                            dateiName: $scope.uploadFiles[0].name,
                            bezeichnung: $scope.uploadFileBezeichnung,
                            contentType: $scope.uploadFiles[0].type,
                            size: $scope.uploadFiles[0].size
                        });
                        $scope.data.mitteilung.dateiIds.push(resp.data);
                    });
                };

                var id = $location.search()['id'];
                loadMitteilung(id);
            }],
            templateUrl: '/Content/component/mitteilungEditComponent.html',
            replace: true
        };
    })
    .directive('termine', function ()
    {
        return {
            restrict: 'E',
            scope: {},
            controller: ["$scope", "$http", function ($scope, $http)
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
                        if (resp.data.hasHinweis && window.localStorage["hinweisKey"] !== resp.data.dateModified)
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
            controller: ["$scope", "$http", "$timeout", function ($scope, $http, $timeout)
            {
                $scope.$watch('album', function (newValue)
                {
                    if (newValue)
                    {
                        $scope.currentImage = null;
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

                $(document).keydown(function (e)
                {
                    if (e.which === 37) // <-
                    {
                        $timeout(function () { $scope.prevImage(); }, 0);
                    }
                    if (e.which === 39) // ->
                    {
                        $timeout(function () { $scope.nextImage(); }, 0);
                    }
                });
            }],
            templateUrl: '/Content/component/albumComponent.html',
            replace: true
        };
    });