﻿angular.module('aikidoApp', [])
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
    .filter('emojione', ['$sce', function ($sce)
    {
        return function (html)
        {
            return $sce.trustAsHtml(emojione.toImage(html.toString()));
        };
    }])
    .factory('creoleService', ['$http', function ($http)
    {
        return {
            generatePreview: function (text)
            {
                return $http.post('/Content/ParseCreole', { text: text }).then(function (resp)
                {
                    return resp.data;
                });
            }
        };
    }])
    .directive("selectNgFiles", function ()
    {
        return {
            require: "ngModel",
            link: function (scope, elem, attrs, ngModel)
            {
                elem.on("change", function (e)
                {
                    var files = elem[0].files;
                    ngModel.$setViewValue(files);
                });
            }
        };
    })
    .directive('dateTimePicker', function ()
    {
        return {
            restrict: 'E',
            scope: {
                datum: '=datum'
            },
            controller: ["$scope", "$element", "$timeout", function ($scope, $element, $timeout)
            {
                function removeTimezone(isoDateString) // TODO: in Service auslagern
                {
                    if (isoDateString && isoDateString.endsWith("Z"))
                    {
                        return isoDateString.substring(0, isoDateString.length - 2);
                    }

                    return isoDateString;
                }

                var parsedDate = moment($scope.datum);
                $scope.datumInternal = parsedDate.isValid() ? parsedDate.format("DD.MM.YYYY HH:mm:ss") : undefined;

                var pickerDomElement = $($element[0].children[0]);
                pickerDomElement.datetimepicker({
                    locale: 'de-ch',
                    format: 'dd.MM.yyyy hh:mm:ss',
                    useCurrent: false
                });
                var picker = pickerDomElement.data('datetimepicker');
                picker.setLocalDate(parsedDate.toDate());

                pickerDomElement.on('changeDate', function (evt)
                {
                    $timeout(function ()
                    {
                        $scope.datum = removeTimezone(evt.date.toISOString());
                    });
                });

                $scope.$watch('datumInternal', function (newValue, oldValue)
                {
                    if (newValue !== oldValue)
                    {
                        var newDate = moment(newValue, "DD.MM.YYYY HH:mm:ss");
                        if (newDate.isValid())
                        {
                            $scope.datum = removeTimezone(newDate.toISOString());
                        } else
                        {
                            $scope.datum = undefined;
                        }
                    }
                });
            }],
            templateUrl: '/Content/component/dateTimePicker.html',
            replace: true
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
                    $http.delete('/Aktuelles/DeleteMitteilung/' + mitteilung.id.split('/')[1]).then(
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
            controller: ["$scope", "$http", "$location", "$sce", "creoleService", function ($scope, $http, $location, $sce, creoleService)
            {
                var textArea = document.getElementById("mitteilung-text");
                $scope.uploadFiles = [];
                $scope.uploadFileBezeichnung = undefined;

                function loadMitteilung(mitteilungId)
                {
                    $http.get('/Aktuelles/LoadMitteilungEditModel', { params: { id: mitteilungId } }).then(function (resp) 
                    {
                        $scope.data = resp.data;
                        $scope.originalHtml = $sce.trustAsHtml(resp.data.mitteilung.html);
                    });
                }

                function removeTimezone(isoDateString) // TODO: in Service auslagern
                {
                    if (isoDateString && isoDateString.endsWith("Z"))
                    {
                        return isoDateString.substring(0, isoDateString.length - 2);
                    }

                    return isoDateString;
                }

                $scope.generatePreview = function ()
                {
                    creoleService.generatePreview($scope.data.mitteilung.text).then(function (text)
                    {
                        $scope.preview = $sce.trustAsHtml(emojione.toImage(text));
                    });
                };

                $scope.uploadFile = function ()
                {
                    if ($scope.uploadFiles.length === 0)
                    {
                        return;
                    }

                    var fd = new FormData();
                    fd.append("file", $scope.uploadFiles[0]);
                    fd.append("bezeichnung", $scope.uploadFileBezeichnung);
                    $http.post('/Content/UploadFile', fd, {
                        transformRequest: angular.identity,
                        headers: { 'Content-Type': undefined }
                    }).then(function (resp)
                    {
                        $scope.data.dateien.push({
                            id: resp.data,
                            dateiName: $scope.uploadFiles[0].name,
                            bezeichnung: $scope.uploadFileBezeichnung,
                            contentType: $scope.uploadFiles[0].type,
                            size: $scope.uploadFiles[0].size
                        });
                        $scope.data.mitteilung.dateiIds.push(resp.data);
                        $scope.uploadFiles = [];
                        $scope.uploadFileBezeichnung = undefined;
                    });
                };

                $scope.save = function ()
                {
                    $http.post('/Aktuelles/SaveMitteilung', $scope.data).then(function (resp)
                    {
                        $scope.data.mitteilung.id = resp.data;

                        // Update Original
                        creoleService.generatePreview($scope.data.mitteilung.text).then(function (text)
                        {
                            $scope.originalHtml = $sce.trustAsHtml(text);
                        });
                    });
                };

                $scope.discard = function ()
                {
                    loadMitteilung($scope.data.mitteilung.id ? $scope.data.mitteilung.id.split('/')[1] : null);
                };

                $scope.delete = function ()
                {
                    $http.delete('/Aktuelles/DeleteMitteilung/' + $scope.data.mitteilung.id.split('/')[1]).then(function ()
                    {
                        document.location.href = "/Aktuelles";
                    });
                };

                $scope.deleteFile = function (datei)
                {
                    $scope.data.mitteilung.dateiIds = $scope.data.mitteilung.dateiIds.filter(function (element)
                    {
                        return element !== datei.id;
                    });
                    $scope.data.dateien = $scope.data.dateien.filter(function (element)
                    {
                        return element.id !== datei.id;
                    });
                    $scope.data.deletedDateiIds.push(datei.id);
                };

                $scope.addTermin = function ()
                {
                    $scope.data.termine.push({
                        titel: $scope.data.mitteilung.titel,
                        startDatum: removeTimezone(new Date().toISOString()),
                        endDatum: null,
                        text: $scope.data.mitteilung.titel
                    });
                };

                $scope.deleteTermin = function (termin)
                {
                    $scope.data.mitteilung.terminIds = $scope.data.mitteilung.terminIds.filter(function (element)
                    {
                        return element !== termin.id;
                    });
                    $scope.data.termine = $scope.data.termine.filter(function (element)
                    {
                        return element.id !== termin.id;
                    });
                    $scope.data.deletedTerminIds.push(termin.id);
                };

                // TODO: In Service auslagern
                $scope.insert = function (text)
                {
                    // https://stackoverflow.com/questions/1064089/inserting-a-text-where-cursor-is-using-javascript-jquery
                    var strPos = 0;
                    var br = ((textArea.selectionStart || textArea.selectionStart === '0') ?
                        "ff" : (document.selection ? "ie" : false));
                    if (br === "ie")
                    {
                        textArea.focus();
                        var range = document.selection.createRange();
                        range.moveStart('character', -textArea.value.length);
                        strPos = range.text.length;
                    } else if (br === "ff")
                    {
                        strPos = textArea.selectionStart;
                    }

                    var front = (textArea.value).substring(0, strPos);
                    var back = (textArea.value).substring(strPos, textArea.value.length);
                    textArea.value = front + text + back;
                    strPos = strPos + text.length;
                    if (br === "ie")
                    {
                        textArea.focus();
                        var ieRange = document.selection.createRange();
                        ieRange.moveStart('character', -textArea.value.length);
                        ieRange.moveStart('character', strPos);
                        ieRange.moveEnd('character', 0);
                        ieRange.select();
                    } else if (br === "ff")
                    {
                        textArea.selectionStart = strPos;
                        textArea.selectionEnd = strPos;
                        textArea.focus();
                    }
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
                // Perload zum schnelleren Anzeigen
                $scope.albums = [{
                    thumbnailUrl: "/Content/images/dojo/dojo3p.jpg",
                    beschreibung: "Bilder vom Dojo Sursee",
                    titel: "Das Dojo",
                    photosetId: "sursee",
                    creationDate: "2012-11-17T00:00:00",
                    link: "/Dojo/Bilder"
                }];
                $scope.currentAlbum = $scope.albums[0];

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
    })
.directive('seiteEdit', function ()
{
    return {
        restrict: 'E',
        scope: {},
        controller: ["$scope", "$http", "$location", "$sce", "creoleService", function ($scope, $http, $location, $sce, creoleService)
        {
            var textArea = document.getElementById("site-text");

            function loadSeite(seiteId)
            {
                $http.get('/Content/LoadSeiteEditModel', { params: { id: seiteId } }).then(function (resp)
                {
                    $scope.seite = resp.data;
                    $scope.html = $sce.trustAsHtml(resp.data.html);
                    $scope.generatePreview();
                });
            }

            $scope.save = function ()
            {
                $http.post('/Content/Edit', $scope.seite).then(function (resp)
                {
                    $scope.seite.revision = resp.data;
                    creoleService.generatePreview($scope.seite.text).then(function (text)
                    {
                        $scope.html = $sce.trustAsHtml(text);
                    });
                });
            };

            $scope.generatePreview = function ()
            {
                creoleService.generatePreview($scope.seite.text).then(function (text)
                {
                    $scope.preview = $sce.trustAsHtml(text);
                });
            };

            // TODO: In Service auslagern
            $scope.insert = function (text)
            {
                // https://stackoverflow.com/questions/1064089/inserting-a-text-where-cursor-is-using-javascript-jquery
                var strPos = 0;
                var br = ((textArea.selectionStart || textArea.selectionStart === '0') ?
                    "ff" : (document.selection ? "ie" : false));
                if (br === "ie")
                {
                    textArea.focus();
                    var range = document.selection.createRange();
                    range.moveStart('character', -textArea.value.length);
                    strPos = range.text.length;
                } else if (br === "ff")
                {
                    strPos = textArea.selectionStart;
                }

                var front = (textArea.value).substring(0, strPos);
                var back = (textArea.value).substring(strPos, textArea.value.length);
                textArea.value = front + text + back;
                strPos = strPos + text.length;
                if (br === "ie")
                {
                    textArea.focus();
                    var ieRange = document.selection.createRange();
                    ieRange.moveStart('character', -textArea.value.length);
                    ieRange.moveStart('character', strPos);
                    ieRange.moveEnd('character', 0);
                    ieRange.select();
                } else if (br === "ff")
                {
                    textArea.selectionStart = strPos;
                    textArea.selectionEnd = strPos;
                    textArea.focus();
                }
            };

            var id = $location.search()['id'];
            loadSeite(id);

        }],
        templateUrl: '/Content/component/siteEditComponent.html',
        replace: true
    };
})
.directive('dateien', function ()
{
    return {
        restrict: 'E',
        scope: {
            onLinkInsert: '&'
        },
        controller: ["$scope", "$http", function ($scope, $http)
        {
            $scope.uploadFiles = [];
            $scope.uploadFileBezeichnung = undefined;

            $scope.getDateien = function (start, filter)
            {
                $scope.data = null;
                $http.get('/Content/ListFiles', { params: { start: start || 0, search: filter } }).then(function (resp) 
                {
                    $scope.data = resp.data;
                    $scope.numberAdditional = resp.data.totalCount - resp.data.count;
                });
            };

            $scope.createLink = function (datei)
            {
                return datei.mimeType.startsWith("image") ?
                    "{{/Content/File/" + datei.id + "|" + (datei.beschreibung || datei.name) + "}}" :
                    "[[/Content/File/" + datei.id + "|" + (datei.beschreibung || datei.name) + "]]";
            };

            $scope.insertLink = function (datei)
            {
                var link = $scope.createLink(datei);
                $scope.onLinkInsert({ link: link });
            };

            $scope.filter = function ()
            {
                $scope.getDateien(0, $scope.search);
            };

            $scope.uploadFile = function ()
            {
                if ($scope.uploadFiles.length === 0)
                {
                    return;
                }

                var fd = new FormData();
                fd.append("file", $scope.uploadFiles[0]);
                fd.append("bezeichnung", $scope.uploadFileBezeichnung);
                $http.post('/Content/UploadFile', fd, {
                    transformRequest: angular.identity,
                    headers: { 'Content-Type': undefined }
                }).then(function (resp)
                {
                    $scope.getDateien(0);
                    $scope.uploadFiles = [];
                    $scope.uploadFileBezeichnung = undefined;
                });
            };

            $scope.getDateien();
        }],
        templateUrl: '/Content/component/dateienComponent.html',
        replace: true
    };
})
.directive('dateienAdminListe', function ()
{
    return {
        restrict: 'E',
        scope: {},
        controller: ["$scope", "$http", function ($scope, $http)
        {
            var perPage = 10;
            $scope.uploadFiles = [];
            $scope.uploadFileBezeichnung = undefined;
            $scope.currentDatei = undefined;

            $scope.setCurrentDatei = function (datei)
            {
                $scope.currentDatei = datei;
            };

            $scope.getDateien = function (start, filter)
            {
                $scope.data = null;
                $http.get('/Content/ListFiles', { params: { start: start || 0, search: filter, perPage: perPage } }).then(function (resp) 
                {
                    $scope.data = resp.data;
                });
            };

            $scope.createLink = function (datei)
            {
                if (!datei)
                {
                    return null;
                }

                return datei.mimeType.startsWith("image") ?
                    "{{/Content/File/" + datei.id + "|" + (datei.beschreibung || datei.name) + "}}" :
                    "[[/Content/File/" + datei.id + "|" + (datei.beschreibung || datei.name) + "]]";
            };

            $scope.filter = function ()
            {
                $scope.getDateien(0, $scope.search);
            };

            $scope.showUsage = function (datei)
            {
                $scope.currentUsage = null;
                $scope.setCurrentDatei(datei);
                $http.get('/Content/GetFileUsage', { params: { id: datei.id } }).then(function (resp) 
                {
                    $scope.currentUsage = resp.data;
                });
            };

            $scope.getSiteDescription = function (usage)
            {
                switch (usage.documentType)
                {
                    case "seiteReference":
                        return "Seite '" + usage.documentName.charAt(0).toUpperCase() + usage.documentName.slice(1) + "'";
                    case "mitteilungAttachment":
                    case "mitteilungReference":
                        return "Mitteilung '" + usage.documentName + "'";
                }
            };

            $scope.deleteDatei = function (datei)
            {
                $scope.setCurrentDatei(datei);
            };

            $scope.deleteDateiConfirmed = function (datei)
            {
                $http.delete('/Content/DeleteConfirmed', { params: { id: datei.id } }).then(function (resp)
                {
                    $scope.getDateien();
                });
            };

            $scope.uploadFile = function ()
            {
                if ($scope.uploadFiles.length === 0)
                {
                    return;
                }

                var fd = new FormData();
                fd.append("file", $scope.uploadFiles[0]);
                fd.append("bezeichnung", $scope.uploadFileBezeichnung);
                $http.post('/Content/UploadFile', fd, {
                    transformRequest: angular.identity,
                    headers: { 'Content-Type': undefined }
                }).then(function (resp)
                {
                    $scope.getDateien(0);
                    $scope.uploadFiles = [];
                    $scope.uploadFileBezeichnung = undefined;
                });
            };

            $scope.goTo = function (to)
            {
                var start;

                if (to === 'BEGIN')
                {
                    $scope.getDateien(0);
                } else if (to === 'END')
                {
                    start = (($scope.data.totalCount / perPage) | 0) * $scope.data.count;
                    if ($scope.data.totalCount % perPage === 0)
                    {
                        start -= 1;
                    }
                    $scope.getDateien(start);
                } else if (to === 'BACK')
                {
                    start = $scope.data.start -= perPage;
                    start = (start < 0) ? 0 : start;
                    $scope.getDateien(start);
                } else if (to === 'FORWARD')
                {
                    start = $scope.data.start += perPage;
                    start = (start > $scope.data.totalCount) ? start - perPage : start;
                    $scope.getDateien(start);
                }
            };

            $scope.getDateien();
        }],
        templateUrl: '/Content/component/dateienAdminListeComponent.html',
        replace: true
    };
});