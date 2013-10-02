var app = angular.module('Cab9', ['$strap.directives', 'ngResource']);

app.value('signalR', $);
/////////////////////////////////////////////////////////////
//                   Master Controller                     //
/////////////////////////////////////////////////////////////
app.controller('MasterController', function ($scope, $q) {
    $scope.appStatus = 'Loading';
    $scope.modal = { name: '', url: null };
    $scope.currentView = '';
    $scope.company = {
        ID: 1,
        Name: 'Cab9 Cars',
        Latitude: 51.5072,
        Longitude: 0.1275,
        DefaultPricingModelID: 11
    };
    //$scope.company = Company.get();

    $scope.dismiss = function (e) {
        if ($(e.srcElement)[0] == $('#birdsEyeModal')[0] || $(e.srcElement)[0] == $('.closePopOver')[0]) {
            $scope.modal = { name: '', url: null };
        };
    };

    $scope.safeApply = function (fn) {
        var phase = this.$root.$$phase;
        if (phase == '$apply' || phase == '$digest') {
            if (fn && (typeof (fn) === 'function')) {
                fn();
            }
        } else {
            this.$apply(fn);
        }
    };

    $scope.$on('RequestModal', function (eventArgs, modal) {
        $scope.modal = modal;
    });
});

app.factory('httpAuthInterceptor', function ($q) {
    return {
        request: function (config) {
            config.headers.Authorization = "Hash " + userName + ":" + ComputeHash(config.method, config.url, config.data, config.params);
            return config || $q.when(config);
        }
    };
});

app.factory('signalRLocationHub', function (signalR, $rootScope) {
    var service = {
        hub: null,
        running: false,
        initalise: function (callback) {
            this.initialiseCallback = callback;
            this.hub = signalR.connection.driverHub;
            this.hub.client.PositionUpdate = function (location) {
                $rootScope.$broadcast('NewLocation', location);
            };
            signalR.connection.hub.start(this.initialiseCompletion);
        },
        initialiseCallback: function () { },
        initialiseCompletion: function () {
            service.running = true;
            service.initialiseCallback();
        },
        stop: function (callback) {
            this.stopCallback = callback;
            signalR.connection.hub.stop(this.stopCompletion);
        },
        stopCallback: function () { },
        stopCompletion: function () {
            service.running = false;
            service.stopCallback();
        }
    }
    return service;
});

/////////////////////////////////////////////////////////////
//                   Factories 				               //
/////////////////////////////////////////////////////////////

var Position;
var Driver;
var Staff;
var Client;
var Vehicle;
var StaffShift;
var Booking;
var Shift;
var Document;
var Note;
var PricingModel;
var PricingZone;
var PricingFixed;
//var apiEndPoint = "http://cab9.testpad.e9server.com/api/";
var apiEndPoint = "/api/";
var userName = "david";
var passWord = "beech";
var credentialHash = encode64('david:beech');
var Company;

app.run(function ($rootScope, $resource) {
    $rootScope.BoolValues = [true, false];



    Company = $resource(apiEndPoint + 'company/:action', {}, {
        update: {
            method: 'PUT'
        }
    })

    Position = $resource(apiEndPoint + 'position/:action', {}, {
        getForDriver: {
            method: 'GET',
            isArray: true,
            params: {
                action: 'getForDriver'
            }
        }
    });

    Note = $resource(apiEndPoint + 'note/:action', {}, {
        update: {
            method: 'PUT'
        }
    });

    Driver = $resource(apiEndPoint + 'driver/:action', {}, {
        get: {
            method: 'GET',
            params: {
                action: 'GetByID'
            }
        },
        update: { method: 'PUT' },
        updateWithImage: {
            method: 'PUT',
            params: { action: 'PutWithImage' },
            transformRequest: function (data) {
                var formData = new FormData();
                formData.append("model", angular.toJson(data.model));
                if (data.file) {
                    formData.append("newpicture", 'true');
                    formData.append(data.file.type, data.file.file);
                } else {
                    formData.append("newpicture", 'false');
                }
                return formData;
            },
            headers: {
                'Content-Type': false
            }
        },
        saveWithImage: {
            method: 'POST',
            params: { action: 'PostWithImage' },
            transformRequest: function (data) {
                var formData = new FormData();
                formData.append("model", angular.toJson(data.model));
                if (data.file) {
                    formData.append("newpicture", 'true');
                    formData.append(data.file.type, data.file.file);
                } else {
                    formData.append("newpicture", 'false');
                }

                return formData;
            },
            headers: {
                'Content-Type': false
            }
        },
        stats: { method: 'GET', params: { action: 'stats', timeformat: 'javascript' } }
    });

    Driver.prototype.$shifts = function (id, from, to) {
        return Shift.query({ driverid: id, from: from, to: to });
    };



    Vehicle = $resource(apiEndPoint + 'vehicle/:action', {}, {
        get: {
            method: 'GET',
            params: {
                action: 'GetByID'
            }
        },
        update: { method: 'PUT' },
        updateWithImage: {
            method: 'PUT',
            params: { action: 'PutWithImage' },
            transformRequest: function (data) {
                var formData = new FormData();
                formData.append("model", angular.toJson(data.model));
                if (data.file) {
                    formData.append("newpicture", 'true');
                    formData.append(data.file.type, data.file.file);
                } else {
                    formData.append("newpicture", 'false');
                }
                return formData;
            },
            headers: {
                'Content-Type': false
            }
        },
        saveWithImage: {
            method: 'POST',
            params: { action: 'PostWithImage' },
            transformRequest: function (data) {
                var formData = new FormData();
                formData.append("model", angular.toJson(data.model));
                if (data.file) {
                    formData.append("newpicture", 'true');
                    formData.append(data.file.type, data.file.file);
                } else {
                    formData.append("newpicture", 'false');
                }

                return formData;
            },
            headers: {
                'Content-Type': false
            }
        },
        stats: { method: 'GET', params: { action: 'stats', timeformat: 'javascript' } }
    });

    Vehicle.prototype.$shifts = function (id, from, to) {
        return Shift.query({ vehicleid: id, from: from, to: to });
    };



    Client = $resource(apiEndPoint + 'client/:action', {}, {
        update: {
            method: 'PUT',
            params: {},
            transformRequest: function (data) {
                var formData = new FormData();
                formData.append("model", angular.toJson(data.model));
                if (data.file) {
                    formData.append("newfile", 'true');
                    formData.append(data.file.type, data.file.file);
                } else {
                    formData.append("newfile", 'false');
                }
                return formData;
            },
            headers: {
                'Content-Type': false
            }
        },
        save: {
            method: 'POST',
            params: {},
            transformRequest: function (data) {
                var formData = new FormData();
                formData.append("model", angular.toJson(data.model));
                if (data.file) {
                    formData.append("newfile", 'true');
                    formData.append(data.file.type, data.file.file);
                } else {
                    formData.append("newfile", 'false');
                }

                return formData;
            },
            headers: {
                'Content-Type': false
            }
        },
        stats: {
            method: 'GET',
            params: {
                action: 'stats',
                timeformat: 'javascript'
            }
        },
        getById: {
            method: 'GET',
            isArray: false,
            params: {
                action: 'GetByID'
            }
        }
    });

    Client.prototype.$bookings = function (from, to) {
        return Booking.query({ clientid: this.ID, from: from, to: to });
    };

    ClientType = $resource(apiEndPoint + 'clienttype/:action', {}, {
        update: {
            method: 'PUT'
        },
        get: {
            method: 'GET',
            isArray: false,
            params: {
                action: 'GetByID'
            }
        }
    });

    Booking = $resource(apiEndPoint + 'booking/:action', {}, {
        update: { method: 'PUT' },
        getDriverOrder: {
            method: 'GET',
            isArray: true,
            params: {
                action: 'DriverOrderForBookingID'
            }
        },
        getDriverOrderForQuote: {
            method: 'GET',
            isArray: true,
            params: {
                action: 'DriverOrderForQuote'
            }
        },
        getPrevious: {
            method: 'GET',
            isArray: true,
            params: {
                action: 'SearchPrevious'
            }
        },
        getQuote: {
            method: 'GET',
            isArray: false,
            params: {
                action: 'Quote'
            }
        }
    });

    Shift = $resource(apiEndPoint + 'drivershift/:action', {}, {
        update: { method: 'PUT' }
    });

    Staff = $resource(apiEndPoint + 'staff/:action', {}, {
        update: { method: 'PUT' },
        updateWithImage: {
            method: 'PUT',
            params: { action: 'PutWithImage' },
            transformRequest: function (data) {
                var formData = new FormData();
                formData.append("model", angular.toJson(data.model));
                if (data.file) {
                    formData.append("newpicture", 'true');
                    formData.append(data.file.type, data.file.file);
                } else {
                    formData.append("newpicture", 'false');
                }
                return formData;
            },
            headers: {
                'Content-Type': false
            }
        },
        saveWithImage: {
            method: 'POST',
            params: { action: 'PostWithImage' },
            transformRequest: function (data) {
                var formData = new FormData();
                formData.append("model", angular.toJson(data.model));
                if (data.file) {
                    formData.append("newpicture", 'true');
                    formData.append(data.file.type, data.file.file);
                } else {
                    formData.append("newpicture", 'false');
                }

                return formData;
            },
            headers: {
                'Content-Type': false
            }
        },
        stats: { method: 'GET', params: { action: 'stats', timeformat: 'javascript' } }
    });

    StaffShift = $resource(apiEndPoint + 'staffshift/:action', {}, {
        update: { method: 'PUT' }
    });

    Document = $resource(apiEndPoint + 'document/:action', {}, {
        save: {
            method: 'POST',
            params: {},
            transformRequest: function (data) {
                var formData = new FormData();
                formData.append("model", angular.toJson(data.model));
                if (data.file) {
                    formData.append("newfile", 'true');
                    formData.append(data.file.type, data.file.file);
                } else {
                    formData.append("newfile", 'false');
                }

                return formData;
            },
            headers: {
                'Content-Type': false
            }
        },
        update: {
            method: 'PUT',
            params: {},
            transformRequest: function (data) {
                var formData = new FormData();
                formData.append("model", angular.toJson(data.model));
                if (data.file) {
                    formData.append("newfile", 'true');
                    formData.append(data.file.type, data.file.file);
                } else {
                    formData.append("newfile", 'false');
                }
                return formData;
            },
            headers: {
                'Content-Type': false
            }
        }
    });

    PricingModel = $resource(apiEndPoint + "pricingmodel/:action", {}, {
        update: { method: 'PUT' },
        get: {
            method: 'GET',
            isArray: false,
            params: {
                action: 'GetByID'
            }
        }
    });

    PricingZone = $resource(apiEndPoint + "pricingzone/:action", {}, {
        update: { method: 'PUT' }
    });

    PricingFixed = $resource(apiEndPoint + "pricingfixed/:action", {}, {
        get: {
            method: 'GET',
            isArray: false,
        },
        update: {
            method: 'PUT',
            params: {
            }
        },
    });

});

/////////////////////////////////////////////////////////////
//                     Common Filters                      //
/////////////////////////////////////////////////////////////
app.filter('RemoveSpaces', function () {
    return function (text) {
        return text.replace(/\s/g, '');
    }
});

app.filter('Truncate', function () {
    return function (text, length, end) {
        if (isNaN(length))
            length = 10;
        if (end === undefined)
            end = "...";
        if (text.length <= length || text.length - end.length <= length)
        { return text; }
        else
        { return String(text).substring(0, length - end.length) + end; }
    }
});

app.filter('BooleanValueFilter', function () {
    return function (val) {
        if (val)
            return "Yes";
        else
            return "No";
    }

});

app.filter('AbbrValue', function () {
    return function (val) {
        if (val < 999)
            return val;
        else if (val > 999 && val < 999999)
            return parseInt(val / 100) / 10 + 'K';
        else if (val > 999999 && val < 999999999)
            return parseInt(val / 100000) / 10 + 'M';
        else
            return parseInt(val / 100000000) / 10 + 'B';
    }
});

app.filter('Countdown', function () {
    return function (input) {
        var substitute = function (stringOrFunction, number, strings) {
            var string = $.isFunction(stringOrFunction) ? stringOrFunction(number, dateDifference) : stringOrFunction;
            var value = (strings.numbers && strings.numbers[number]) || number;
            return string.replace(/%d/i, value);
        },
            nowTime = (new Date()).getTime(),
            date = (new Date(input)).getTime(),
            //refreshMillis= 6e4, //A minute
            allowFuture = true,
            strings = {
                prefixAgo: null,
                prefixFromNow: null,
                suffixAgo: "ago",
                suffixFromNow: "",//"from now"
                seconds: "less than a minute",
                minute: "a minute",
                minutes: "%d minutes",
                hour: "an hour",
                hours: "%d hours",
                day: "a day",
                days: "%d days",
                month: "a month",
                months: "%d months",
                year: "a year",
                years: "%d years"
            },
            dateDifference = nowTime - date,
            words,
            seconds = Math.abs(dateDifference) / 1000,
            minutes = seconds / 60,
            hours = minutes / 60,
            days = hours / 24,
            years = days / 365,
            separator = strings.wordSeparator === undefined ? " " : strings.wordSeparator,

            // var strings = this.settings.strings;
            prefix = strings.prefixAgo,
            suffix = strings.suffixAgo;

        if (allowFuture) {
            if (dateDifference < 0) {
                prefix = strings.prefixFromNow;
                suffix = strings.suffixFromNow;
            }
        }

        words = seconds < 45 && substitute(strings.seconds, Math.round(seconds), strings) ||
        seconds < 90 && substitute(strings.minute, 1, strings) ||
        minutes < 45 && substitute(strings.minutes, Math.round(minutes), strings) ||
        minutes < 90 && substitute(strings.hour, 1, strings) ||
        hours < 24 && substitute(strings.hours, Math.round(hours), strings) ||
        hours < 42 && substitute(strings.day, 1, strings) ||
        days < 30 && substitute(strings.days, Math.round(days), strings) ||
        days < 45 && substitute(strings.month, 1, strings) ||
        days < 365 && substitute(strings.months, Math.round(days / 30), strings) ||
        years < 1.5 && substitute(strings.year, 1, strings) ||
        substitute(strings.years, Math.round(years), strings);

        return $.trim([prefix, words, suffix].join(separator));
        // conditional based on optional argument
        // if (somethingElse) {
        //     out = out.toUpperCase();
        // }
        // return out;
    }
});

app.filter('GetGroupObjectsForData', function () {
    return function (data, grouping, sorting) {
        var results = [];
        angular.forEach(data, function (entry) {
            var value = {};
            var keySplit = grouping.key.split(".");
            if (keySplit.length == 1) {
                value = entry[keySplit[0]];
            }
            if (keySplit.length == 2) {
                value = entry[keySplit[0]][keySplit[1]];
            }
            var included = false;
            angular.forEach(results, function (result) {
                if (result.ID == value.ID) included = true;
            });
            if (!included) {
                results.push(value);
            }
        });
        return results;
    };
});

app.filter('GetGroupsForData', function () {
    return function (data, grouping, sorting) {
        var result = [];
        if (grouping.key != 'All') {
            angular.forEach(data, function (entry) {
                var value = '';
                var keySplit = grouping.key.split(".");
                if (keySplit.length == 1) {
                    value = (grouping.initials) ? entry[keySplit[0]].charAt(0) : entry[keySplit[0]];
                }
                if (keySplit.length == 2) {
                    value = (grouping.initials) ? entry[keySplit[0]][keySplit[1]].charAt(0) : entry[keySplit[0]][keySplit[1]];
                }
                if (result.indexOf(value) == -1) {
                    result.push(value);
                }
            });
        } else {
            result.push('All');
        }
        result.sort();
        return result;
    };
});

app.filter('ItemsForGroup', function () {
    return function (data, groupValue, grouping, sorting) {

        var result = [];
        var keySplit = grouping.key.split(".");

        if (grouping.key == 'All') {
            angular.forEach(data, function (item) {
                result.push(item);
            });
        } else {
            angular.forEach(data, function (item) {
                if (keySplit.length == 1) {
                    var val = (grouping.initials) ? item[keySplit[0]].charAt(0) : item[keySplit[0]];
                    if (val == groupValue) {
                        result.push(item);
                    }
                }
                if (keySplit.length == 2) {
                    var val = (grouping.initials) ? item[keySplit[0]][keySplit[1]].charAt(0) : item[keySplit[0]][keySplit[1]];
                    if (val == groupValue) {
                        result.push(item);
                    }
                }
            });
        }
        result.sort(SortBy(sorting.key, sorting.reverse, null));
        return result;
    };
});

app.filter('SearchFilter', function () {
    return function (data, fields, term) {
        if (term != '') {
            var result = [];
            angular.forEach(data, function (value) {
                angular.forEach(fields, function (field) {
                    var fieldParts = field.split(".");
                    if (fieldParts.length == 1) {
                        if (value[fieldParts[0]].toLowerCase().indexOf(term.toLowerCase()) != -1) {
                            if (result.indexOf(value) == -1) {
                                result.push(value);
                            }
                        }
                    } else if (fieldParts.length == 2) {
                        if (value[fieldParts[0]][fieldParts[1]].toLowerCase().indexOf(term.toLowerCase()) != -1) {
                            if (result.indexOf(value) == -1) {
                                result.push(value);
                            }
                        }
                    }
                });
            });
            return result;
        } else {
            return data;
        }
    };
});


/////////////////////////////////////////////////////////////
//                     Directives                          //
/////////////////////////////////////////////////////////////

app.directive('chart', function () {
    return {
        restrict: 'A',
        scope: {
            graphdata: '=',
            xfield: '=',
            yfield: '=',
            range: '=',
            nullzeros: '=',
            datasets: '='
        },
        link: function (scope, elem) {
            var chart;

            scope.createOpts = function () {
                return {
                    lines: {
                        show: true
                    },
                    points: {
                        show: true
                    },
                    xaxis: {
                        mode: "time",
                        minTickSize: scope.range.tick,
                        timeformat: scope.range.format,
                        monthNames: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
                        min: scope.range.from,
                        max: scope.range.to,
                        font: { color: '#f9f9f9' },
                        tickColor: '#888'
                    },
                    yaxis: {
                        font: { color: '#f9f9f9' },
                        tickColor: '#888'
                    },
                    grid: {
                        backgroundColor: { colors: ["#3d3d3d", "#3d3d3d"] },
                        borderWidth: {
                            top: 0,
                            right: 0,
                            bottom: 0,
                            left: 0
                        }
                    },
                    legend: {
                        backgroundColor: '#26AE90',
                        backgroundOpacity: 0.3,
                        labelFormatter: function (label, series) { return '<span class="white">' + label + '</span>'; }
                    }
                };
            };

            scope.$watch('graphdata', function (value) {
                scope.processedDataSets = [];
                var count = 0;
                angular.forEach(value, function (dataset) {
                    if (dataset.data.length > 0) count++;
                });

                if (count == scope.datasets) {
                    angular.forEach(value, function (dataset) {
                        var processedData = scope.ProcessGraphData(dataset.data, scope.xfield, scope.yfield, scope.nullzeros);
                        scope.processedDataSets.push({
                            label: dataset.label,
                            data: processedData,
                            color: dataset.color
                        });
                    });

                    scope.draw();
                }
            }, true);

            scope.$watch('range', function (value) {
                scope.processedDataSets = [];
            });

            scope.draw = function () {
                chart = $.plot(elem, scope.processedDataSets, scope.createOpts());
                elem.show();
            };

            scope.ProcessGraphData = function (rawData, xField, yField, nullZeros) {
                var result = [];
                var xSplit = xField.split(".");
                var ySplit = yField.split(".");
                if (rawData.length > 0) {
                    angular.forEach(rawData, function (value) {
                        var x = (xSplit.length == 1) ? value[xSplit[0]] : value[xSplit[0]][xSplit[1]];

                        var y = (ySplit.length == 1) ? value[ySplit[0]] : value[ySplit[0]][ySplit[1]];

                        if (nullZeros && y == 0) y = null;

                        result.push([x, y]);
                    });
                }
                return result;
            };
        }
    };
});

app.directive('doughnut', function () {
    return {
        restrict: 'A',
        scope: true,
        link: function (scope, elem, attrs) {
            attrs.$observe('percent', function (value) {
                if (value != 0) {
                    var barcolor = attrs.barcolor;
                    if (attrs.barcolor == 'auto') {
                        if (value <= 25) {
                            barcolor = '#d21d1d'
                        }
                        else if (value > 25 && value <= 50) {
                            barcolor = '#ffb642'
                        }
                        else if (value > 50) {
                            barcolor = '#26AE90'
                        }
                    }
                    if (scope.chart) {
                        scope.chart.data('easyPieChart').options.barColor = barcolor;
                        scope.chart.data('easyPieChart').update(value);
                    } else {
                        scope.chart = elem.easyPieChart({
                            animate: isMobile() ? false : '700',
                            lineWidth: (attrs.linewidth === undefined) ? '12' : attrs.linewidth,
                            trackColor: attrs.trackcolor,
                            scaleColor: attrs.scalecolor,
                            barColor: barcolor,
                            size: (attrs.size === undefined) ? '330' : attrs.size,
                            lineCap: 'butt'
                        });
                    }
                }
                else {
                    scope.chart = elem.easyPieChart({
                        animate: isMobile() ? false : '700',
                        lineWidth: (attrs.linewidth === undefined) ? '12' : attrs.linewidth,
                        trackColor: attrs.trackcolor,
                        scaleColor: attrs.scalecolor,
                        size: (attrs.size === undefined) ? '330' : attrs.size,
                        lineCap: 'butt'
                    });

                }
            });
        }
    }
});

app.directive('fileUpload', function () {
    return {
        scope: true,        //create a new scope
        link: function (scope, el, attrs) {
            el.bind('change', function (event) {
                var files = event.target.files;
                //iterate files since 'multiple' may be specified on the element
                for (var i = 0; i < files.length; i++) {
                    //emit event upward
                    scope.$emit("fileSelected", { type: event.srcElement.name, file: files[i] });
                }
            });
        }
    };
});

app.directive('profileHero', function () {
    return {
        restrict: 'A',
        scope: true,
        link: function (scope, elem, attrs) {
            attrs.$observe('url', function (value) {
                scope.setValues();
            });
            attrs.$observe('cardtext', function (value) {
                scope.setValues();
            });
            scope.setValues = function () {
                if (attrs.url) {
                    elem.css("background-image", "url(/includes/img/glare.png), url(" + attrs.url + ")");
                    elem.text("");
                }
                else {

                    elem.css("background-color", colourFromText(attrs.cardtext));
                    elem.css("background-image", "-webkit-linear-gradient(top, rgba(255, 255, 255, 0.1) 1%, rgba(0, 0, 0, 0.1) 100%)");
                    elem.text(initialsFromText(attrs.cardtext));
                }
            }
        }
    };
});

app.directive('googleMap', function () {
    return {
        restrict: 'AE',
        scope: {
            mapObject: '=',
            mapOptions: '=',
            start: '='
        },
        replace: true,
        template: '<div></div>',
        link: function (scope, elem, attrs) {
            defaults = {
                center: new google.maps.LatLng(51.4872, -0.1275),
                zoom: 10,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };

            if (!scope.mapOptions) scope.mapOptions = {};

            scope.mapOptions = mergeOptions(defaults, scope.mapOptions);

            scope.startZones = [];

            scope.initialize = function () {
                scope.mapObject = new google.maps.Map(document.getElementById(attrs.id), scope.mapOptions);

                scope.drawingManager = new google.maps.drawing.DrawingManager({
                    drawingMode: null,
                    drawingControl: true,
                    drawingControlOptions: {
                        position: google.maps.ControlPosition.TOP_CENTER,
                        drawingModes: [
                        ]
                    },
                    polygonOptions: {
                        draggable: true,
                        editable: true,
                        strokeWeight: 2,
                        fillColor: '#26AE90',
                        fillOpacity: 0.6,
                        strokeColor: '#D21D1D',
                        strokeOpacity: 0.8,
                    }
                });

                scope.drawingManager.setMap(scope.mapObject);

                google.maps.event.addListener(scope.drawingManager, 'polygoncomplete', function (polygon) {
                    scope.drawingManager.setDrawingMode(null);
                    var paths = polygon.getPaths();
                    var encoded = google.maps.geometry.encoding.encodePath(paths.getArray()[0])
                    polygon.setOptions({
                        zIndex: (100000000 - (google.maps.geometry.spherical.computeArea(polygon.getPath()) / 10000))
                    });
                    scope.$emit('ZoneComplete', { polygon: polygon, encoded: encoded });
                });
            };

            scope.$watch('start', function (newval, oldval, scope) {
                if (!scope.mapObject) {
                    if (newval == 'Zones') {
                        scope.initialize();
                        scope.infoWindow = new google.maps.InfoWindow();
                    }
                }
            });

            scope.$on('StartZone', function () {
                scope.drawingManager.setDrawingMode(google.maps.drawing.OverlayType.POLYGON);
            });

            scope.$on('CancelZone', function () {
                scope.drawingManager.setDrawingMode(null);
            });
        }
    }
});

app.directive('journeyQuote', function () {
    return {
        restrict: 'AE',
        scope: {
            //options: '='
        },
        replace: true,
        templateUrl: '/pricing/partials/journeyQuote.html',
        link: function (scope, elem, attrs) {
            defaults = {
                center: new google.maps.LatLng(54, -2),
                zoom: 6,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };

            if (!scope.options) scope.options = {};

            scope.options = mergeOptions(defaults, scope.options);

            scope.map = new google.maps.Map(document.getElementById('journeyMap'), scope.options);

            scope.directionsService = new google.maps.DirectionsService();
            scope.directionsDisplay = new google.maps.DirectionsRenderer({
                suppressMarkers: true,
                suppressInfoWindows: true,
                draggable: false
            });
            scope.directionsDisplay.setMap(scope.map);

            scope.journey = {};

            scope.from = {};
            scope.from.input = document.getElementById('from');
            scope.from.autocomplete = new google.maps.places.Autocomplete(scope.from.input);
            scope.from.marker = new google.maps.Marker({
                map: scope.map
            });
            google.maps.event.addListener(scope.from.autocomplete, 'place_changed', function () {
                scope.from.marker.setVisible(false);
                scope.from.input.className = '';
                scope.from.place = scope.from.autocomplete.getPlace();
                if (!scope.from.place.geometry) {
                    // Inform the user that the place was not found and return.
                    scope.from.input.className = 'notfound';
                    return;
                }

                // If the place has a geometry, then present it on a map.
                if (scope.from.place.geometry.viewport) {
                    scope.map.fitBounds(scope.from.place.geometry.viewport);
                } else {
                    scope.map.setCenter(scope.from.place.geometry.location);
                    scope.map.setZoom(11);  // Why 17? Because it looks good.
                }
                scope.from.marker.setIcon(/** @type {google.maps.Icon} */({
                    url: scope.from.place.icon,
                    size: new google.maps.Size(71, 71),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(17, 34),
                    scaledSize: new google.maps.Size(35, 35)
                }));
                scope.from.marker.setPosition(scope.from.place.geometry.location);
                scope.from.marker.setVisible(true);
            });
            scope.from.autocomplete.bindTo('bounds', scope.map)

            scope.to = {};
            scope.to.input = document.getElementById('to');
            scope.to.autocomplete = new google.maps.places.Autocomplete(scope.to.input);
            scope.to.marker = new google.maps.Marker({
                map: scope.map
            });
            google.maps.event.addListener(scope.to.autocomplete, 'place_changed', function () {
                scope.to.marker.setVisible(false);
                scope.to.input.className = '';
                scope.to.place = scope.to.autocomplete.getPlace();
                if (!scope.to.place.geometry) {
                    // Inform the user that the place was not found and return.
                    scope.to.input.className = 'notfound';
                    return;
                }

                // If the place has a geometry, then present it on a map.
                if (scope.to.place.geometry.viewport) {
                    scope.map.fitBounds(scope.to.place.geometry.viewport);
                } else {
                    scope.map.setCenter(scope.to.place.geometry.location);
                    scope.map.setZoom(11);  // Why 17? Because it looks good.
                }
                scope.to.marker.setIcon(/** @type {google.maps.Icon} */({
                    url: scope.to.place.icon,
                    size: new google.maps.Size(71, 71),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(17, 34),
                    scaledSize: new google.maps.Size(35, 35)
                }));
                scope.to.marker.setPosition(scope.to.place.geometry.location);
                scope.to.marker.setVisible(true);
            });

            scope.to.autocomplete.bindTo('bounds', scope.map)

            scope.MapAndQuote = function () {
                if (scope.from.place && scope.to.place) {
                    //scope.bounds = new google.maps.LatLngBounds();
                    //if (scope.from.place.geometry.viewport) {
                    //    scope.bounds.extend(scope.from.place.geometry.location);
                    //}
                    //if (scope.to.place.geometry.viewport) {
                    //    scope.bounds.extend(scope.to.place.geometry.location);
                    //}
                    //console.log(scope.from.place.geometry.location)
                    //scope.map.fitBounds(scope.bounds);

                    scope.request = {
                        origin: scope.from.place.geometry.location,
                        destination: scope.to.place.geometry.location,
                        travelMode: google.maps.DirectionsTravelMode.DRIVING
                    };
                    scope.directionsService.route(scope.request, function (response, status) {
                        if (status == google.maps.DirectionsStatus.OK) {
                            scope.$apply(function () {
                                scope.directionsDisplay.setDirections(response);
                            })
                        }
                    });

                } else {
                    alert('absent')
                }
            };
        }
    }
});

app.directive('locationSearch', function () {
    return {
        restrict: 'A',
        scope: {
            model: '=',
            map: '='
        },
        replace: false,
        link: function (scope, elem, attrs) {
            scope.model.autocomplete = new google.maps.places.Autocomplete(elem[0]);
            google.maps.event.addListener(scope.model.autocomplete, 'place_changed', function () {
                elem.removeClass('notfound');

                scope.$apply(function () {
                    scope.model.place = scope.model.autocomplete.getPlace();
                    if (!scope.model.place.geometry) {
                        elem.addClass('notfound');
                        if (scope.model.marker) {
                            scope.model.marker.setMap(null);
                            delete scope.model.marker;
                        }
                    };
                    var marker = new google.maps.Marker({
                        map: scope.map,
                        icon: {
                            url: scope.model.place.icon,
                            size: new google.maps.Size(71, 71),
                            origin: new google.maps.Point(0, 0),
                            anchor: new google.maps.Point(17, 34),
                            scaledSize: new google.maps.Size(35, 35)
                        },
                        position: scope.model.place.geometry.location
                    });
                    if (scope.model.marker) {
                        scope.model.marker.setMap(null);
                        delete scope.model.marker;
                    }
                    scope.model.marker = marker;
                });
            });
        }
    }
});

app.directive('basicMap', function () {
    return {
        restrict: 'A',
        scope: {
            options: '=',
            map: '=',
            click: '=',
            route: '='
        },
        link: function (scope, elem, attrs) {
            var defaults = {
                center: new google.maps.LatLng(54, -2),
                zoom: 6,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };
            scope.options = mergeOptions(defaults, scope.options);
            scope.map = new google.maps.Map(elem[0], scope.options);
            if (!window.maps)
                window.maps = [scope.map];
            else
                window.maps.push(scope.map);

            scope.$emit("MapReady");

            //$('#map').css({ 'height': (($(window).height())) - $('#map').offset().top - 10 + 'px' });

            //$(window).resize(function () {
            //    if ($('#map').length > 0)
            //        $('#map').css({ 'height': (($(window).height())) - $('#map').offset().top - 10 + 'px' });
            //});

            if (scope.click)
                google.maps.event.addListener(scope.map, 'click', scope.click());
        }
    }
});

app.directive('routeRender', function () {
    return {
        restrict: 'A',
        transclude: true,
        link: function (scope, elem, attrs) {
            scope.directionsDisplay = new google.maps.DirectionsRenderer({
                suppressMarkers: true,
                suppressInfoWindows: true,
                draggable: false,
                map: scope.map
            });

            scope.setDirections = function () {
                if (scope.route.routes) {
                    scope.directionsDisplay.setDirections(scope.route);
                }
            }
            scope.$watch('route', function (a, b, scope) {
                (scope.route) ? scope.setDirections() : angular.noop()
            }, true);
        }
    }
});

/////////////////////////////////////////////////////////////
//                     Extension Methods                   //
/////////////////////////////////////////////////////////////

function isMobile() {
    var browser = navigator.userAgent;
    var browserRegex = /(Android|BlackBerry|IEMobile|Nokia|iP(ad|hone|od)|Opera M(obi|ini))/;
    if (browser.match(browserRegex)) {
        return true;
    }
    return false;
}

$(function () {
    $("#tabViewSelect").live("change", function () {
        var divToShow = $('#tabViewSelect').val();
        $('.tab-pane').removeClass('active');
        $('#' + divToShow).addClass('active');
    });
});

Date.prototype.addPeriod = function (value, period) {
    var ms;
    switch (period.toLowerCase()) {
        case 'minutes':
            ms = (value * 60 * 1000);
            break;
        case 'hours':
            ms = (value * 60 * 60 * 1000);
            break;
        case 'days':
            ms = (value * 24 * 60 * 60 * 1000);
            break;
        case 'weeks':
            ms = (value * 7 * 24 * 60 * 60 * 1000);
            break;
        case 'months':
            ms = (value * 30 * 24 * 60 * 60 * 1000);
            break;
        case 'years':
            ms = (value * 365 * 24 * 60 * 60 * 1000);
            break;
        default:
            ms = 0;
            break;
    }
    this.setTime(this.getTime() + ms);
    return this;
};

//IE8 and below doesn't have an array indexOf function.
if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function (elt /*, from*/) {
        var len = this.length >>> 0;

        var from = Number(arguments[1]) || 0;
        from = (from < 0)
             ? Math.ceil(from)
             : Math.floor(from);
        if (from < 0)
            from += len;

        for (; from < len; from++) {
            if (from in this &&
                this[from] === elt)
                return from;
        }
        return -1;
    };
}
//IE8 and below doesn't have an array indexOf function.

if (!Array.prototype.where) {
    Array.prototype.where = function (filter) {
        var result = [];
        this.forEach(function (value) {
            if (filter(value)) result.push(value);
        });
        return result;
    };
}

if (!Array.prototype.replace) {
    Array.prototype.replace = function (filter, replace) {
        this.forEach(function (value) {
            if (filter(value)) value = replace;
        });
    };
}

function SortBy(field, reverse, primer) {
    var keySplit = (field) ? field.split(".") : null;

    if (field != "Unsorted" || !field) {
        var key = function (x) {
            if (!field) {
                return primer ? primer(x) : x;
            }
            else if (keySplit.length == 1) {
                return primer ? primer(x[keySplit[0]]) : x[keySplit[0]];
            } else if (keySplit.length == 2) {
                return primer ? primer(x[keySplit[0]][keySplit[1]]) : x[keySplit[0]][keySplit[1]];
            }
        };

        return function (a, b) {
            var A = key(a), B = key(b);
            return (A < B ? -1 : (A > B ? 1 : 0)) * [1, -1][+!!reverse];
        };
    } else {
        return function (a, b) {
            return 0;
        };
    }
};

function colourFromText(text) {
    var sum = text.toUpperCase().charCodeAt(0) + text.toUpperCase().charCodeAt(text.length - 1);
    if (sum < 130) {
        sum = ((sum * 50) / 129) + 130;
    }
    else if (sum > 180) {
        sum = (sum % 50) + 130;
    }
    var colour = ((sum - 130) * 360) / 50;
    return "hsl(" + colour + "," + "45%,45%)";
}

function initialsFromText(text) {
    var parts = text.trim().split(' ');
    if (parts.length > 1) {
        return (parts[0][0] + parts[parts.length - 1][0]);
    }
    else {
        return text[0] + (text[1] || '');
    }
}

function encode64(input) {
    //input = escape(input);
    var output = "";
    var chr1, chr2, chr3 = "";
    var enc1, enc2, enc3, enc4 = "";
    var i = 0;
    var keyStr = "ABCDEFGHIJKLMNOP" +
               "QRSTUVWXYZabcdef" +
               "ghijklmnopqrstuv" +
               "wxyz0123456789+/" +
               "=";

    do {
        chr1 = input.charCodeAt(i++);
        chr2 = input.charCodeAt(i++);
        chr3 = input.charCodeAt(i++);

        enc1 = chr1 >> 2;
        enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
        enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
        enc4 = chr3 & 63;

        if (isNaN(chr2)) {
            enc3 = enc4 = 64;
        } else if (isNaN(chr3)) {
            enc4 = 64;
        }

        output = output +
           keyStr.charAt(enc1) +
           keyStr.charAt(enc2) +
           keyStr.charAt(enc3) +
           keyStr.charAt(enc4);
        chr1 = chr2 = chr3 = "";
        enc1 = enc2 = enc3 = enc4 = "";
    } while (i < input.length);

    return output;
}

function ComputeHash(verb, resource, data, params) {
    var paramString = constructQueryString(params);
    message = verb + '&' + resource + paramString + '&' + (data || '');
    console.log(message);
    hash = CryptoJS.HmacSHA256(message, passWord);

    return hash.toString(CryptoJS.enc.Base64);
}

function constructQueryString(params) {
    var result = '';
    if (params) {
        var arr = [];
        result += '?';
        angular.forEach(params, function (value, key) {
            arr.push(key + '=' + value + '&')
        });

        arr.sort();

        angular.forEach(arr, function (value, key) {
            result += value;
        });

        result = result.slice(0, -1);
    }
    return result;
}

function mergeOptions(defaults, overrides) {
    if (!overrides) return defaults;
    var result = {};
    if (defaults) for (var attr in defaults) { result[attr] = defaults[attr]; }
    if (overrides) for (var attr in overrides) { result[attr] = overrides[attr]; }
    return result;
}



//////////////////NEW BOOKING CONTROLLER/////////////////////
app.controller("NewBookingController", function ($scope, signalRLocationHub) {
    $scope.mapObject = {};
    $scope.mapOptions = {
        center: new google.maps.LatLng(51.4775, -0.4614),
        zoom: 13
    };
    $scope.clients = Client.query();

    signalRLocationHub.initalise(function () { });

    var activeBooking = function () {
        this.booking = new Booking();
        this.booking.CarType = 1;
        this.booking.date = new Date();
        this.booking.time = ((new Date().getHours() > 9) ? new Date().getHours() : '0' + new Date().getHours()) + ':' + ((new Date().getMinutes() > 9) ? new Date().getMinutes() : '0' + new Date().getMinutes());
        this.booking.PAX = 1;
        this.booking.BAX = 0;
        this.booking.PaymentMethod = 1;
        this.booking.Priority = 1;

        this.from = null;
        this.to = null;

        this.fromMarker = null;
        this.toMarker = null;

        this.route = null;

        this.selectedNumber = null;
    }

    $scope.activeBookings = [];
    $scope.activeActiveBooking = null;
    $scope.previous = [];

    $scope.$watch('activeActiveBooking.from', function (newValue) {
        if (newValue) {
            $scope.activeActiveBooking.booking.From = newValue.Name;
            if ($scope.activeActiveBooking.fromMarker) {
                $scope.activeActiveBooking.fromMarker.setPosition(new google.maps.LatLng(newValue.Latitude, newValue.Longitude));
                $scope.activeActiveBooking.fromMarker.setMap($scope.mapObject);
            } else {
                $scope.activeActiveBooking.fromMarker = new google.maps.Marker({
                    map: $scope.mapObject,
                    position: new google.maps.LatLng(newValue.Latitude, newValue.Longitude)
                })
            }
        } else {
            $scope.activeActiveBooking.booking.From = '';
            if ($scope.activeActiveBooking.fromMarker) {
                $scope.activeActiveBooking.fromMarker.setMap(null);
            }
        }
        $scope.GetRouteAndQuote();
    }, true);

    $scope.$watch('activeActiveBooking.to', function (newValue) {
        if (newValue) {
            $scope.activeActiveBooking.booking.To = newValue.Name;
            if ($scope.activeActiveBooking.toMarker) {
                $scope.activeActiveBooking.toMarker.setPosition(new google.maps.LatLng(newValue.Latitude, newValue.Longitude));
                $scope.activeActiveBooking.toMarker.setMap($scope.mapObject);
            } else {
                $scope.activeActiveBooking.toMarker = new google.maps.Marker({
                    map: $scope.mapObject,
                    position: new google.maps.LatLng(newValue.Latitude, newValue.Longitude)
                })
            }
        } else {
            $scope.activeActiveBooking.booking.To = '';
            if ($scope.activeActiveBooking.toMarker) {
                $scope.activeActiveBooking.toMarker.setMap(null);
            }
        }
        $scope.GetRouteAndQuote();
    }, true);

    $scope.$watch('activeActiveBooking.selectedNumber', function (newValue) {
<<<<<<< HEAD
        if (newValue && newValue.ContactNumber.length>3) {
=======
        if (newValue) {
>>>>>>> 9a1a08c4ba3015204fff15d8ac742cc8ab53926a
            if (!$scope.activeActiveBooking.booking.PassengerName) $scope.activeActiveBooking.booking.PassengerName = newValue.PassengerName;
            $scope.activeActiveBooking.booking.ContactNumber = newValue.ContactNumber;
            $scope.previous = Booking.getPrevious({ name: null, number: newValue.ContactNumber })
            $scope.bookingsFor = newValue.ContactNumber;

        } else {
<<<<<<< HEAD
            $scope.previous = [];
=======
>>>>>>> 9a1a08c4ba3015204fff15d8ac742cc8ab53926a
            $scope.activeActiveBooking.booking.ContactNumber = '';
        }
    });

    $scope.CopyPrevious = function (previous) {

        var directionsService = new google.maps.DirectionsService();

        var request = {
            origin: previous.From,
            destination: previous.To,
            optimizeWaypoints: false,
            travelMode: google.maps.DirectionsTravelMode.DRIVING
        };

        $('#fromTextbox').children('input')[0].value = previous.From;
        $('#toTextbox').children('input')[0].value = previous.To;
        $scope.activeActiveBooking.booking.From = previous.From;
        $scope.activeActiveBooking.booking.To = previous.To;

        directionsService.route(request, function (response, status) {
            if (status == google.maps.DirectionsStatus.OK) {
                $scope.safeApply(function () {
                    var start = response.routes[0].legs[0].start_location;
                    var end = response.routes[0].legs[0].end_location;
                    $scope.activeActiveBooking.from = {
                        Name: previous.From,
                        Latitude: start.lat(),
                        Longitude: start.lng()
                    };
                    $scope.activeActiveBooking.to = {
                        Name: previous.To,
                        Latitude: end.lat(),
                        Longitude: end.lng()
                    };
                })

            }
        });
    };

    $scope.GetRouteAndQuote = function () {
        if ($scope.activeActiveBooking.from && $scope.activeActiveBooking.to) {
            var time = $scope.activeActiveBooking.booking.time; // HH:MM
            var date = new Date();
            date = new Date(date.setUTCHours(new Number(time[0].toString() + time[1].toString())));
            date = new Date(date.setUTCMinutes(new Number(time[3].toString() + time[4].toString())));
            $scope.activeActiveBooking.booking.BookedDateTime = date.toJSON();

            var directionsService = new google.maps.DirectionsService();

            var request = {
                origin: new google.maps.LatLng($scope.activeActiveBooking.from.Latitude, $scope.activeActiveBooking.from.Longitude),
                destination: new google.maps.LatLng($scope.activeActiveBooking.to.Latitude, $scope.activeActiveBooking.to.Longitude),
                optimizeWaypoints: false,
                travelMode: google.maps.DirectionsTravelMode.DRIVING
            };

            directionsService.route(request, function (response, status) {
                if (status == google.maps.DirectionsStatus.OK) {
                    $scope.safeApply(function () {
                        $scope.activeActiveBooking.route = response;

                        var points = [
                            new google.maps.LatLng($scope.activeActiveBooking.from.Latitude, $scope.activeActiveBooking.from.Longitude),
                            new google.maps.LatLng($scope.activeActiveBooking.to.Latitude, $scope.activeActiveBooking.to.Longitude)
                        ]

                        var line = new google.maps.Polyline({
                            path: points,
                            map: null
                        });

                        var quote = {
                            encodedRoute: google.maps.geometry.encoding.encodePath(line.getPath()),
                            waitingTimes: null,
                            carType: $scope.activeActiveBooking.booking.CarType,
                            clientId: $scope.activeActiveBooking.booking.ClientID,
                            bookingtime: $scope.activeActiveBooking.booking.BookedDateTime
                        }

                        $scope.activeActiveBooking.quote = Booking.getQuote(quote, function (data) {
                            $scope.activeActiveBooking.booking.CalculatedFare = data.TotalFare;
                            $scope.activeActiveBooking.booking.ActualFare = data.TotalFare
                        });

                        angular.forEach($scope.driverOrder, function (driver) {
                            driver.marker.setMap(null);
                            delete driver.marker;
                        });

                        $scope.driverOrder = Booking.getDriverOrderForQuote({
                            latitude: $scope.activeActiveBooking.from.Latitude,
                            longitude: $scope.activeActiveBooking.from.Longitude,
                            pax: $scope.activeActiveBooking.booking.PAX
                        }, function () {
                            angular.forEach($scope.driverOrder, function (d) {
                                d.marker = new google.maps.Marker({
                                    map: $scope.mapObject,
                                    position: new google.maps.LatLng(d.driver.LastKnownPosition.latitude, d.driver.LastKnownPosition.longitude),
                                    icon: {
                                        url: '/api/image?imagetype=Google&ownertype=driver&ownerid=' + d.driver.ID,
                                        scaledSize: new google.maps.Size(40, 40)
                                    }
                                });
                            });
                        })

                    })
                }
            });

        }
    };

    $scope.AddBooking = function (auto) {
        var time = $scope.activeActiveBooking.booking.time; // HH:MM
        var date = new Date();
        date = new Date(date.setUTCHours(new Number(time[0].toString() + time[1].toString())));
        date = new Date(date.setUTCMinutes(new Number(time[3].toString() + time[4].toString())));
        $scope.activeActiveBooking.booking.BookedDateTime = date.toJSON();
        $scope.activeActiveBooking.booking.AutoDispatch = auto;
        Booking.save($scope.activeActiveBooking.booking, function () {
            alert('Booking Added');
        })
    }

    $scope.TimeClicked = function (period) {
        var date = new Date();
        if (period == 'Asap') {
        } else if (period == '15m') {
            date = new Date().addPeriod(15, 'minutes');
        }
        else if (period == '30m') {
            date = new Date().addPeriod(30, 'minutes');
        } else if (period == '1hr') {
            date = new Date().addPeriod(1, 'hours');
        }
        $scope.activeActiveBooking.booking.date = date;
        $scope.activeActiveBooking.booking.time = ((date.getHours() > 9) ? date.getHours() : '0' + date.getHours()) + ':' + ((date.getMinutes() > 9) ? date.getMinutes() : '0' + date.getMinutes());

        $scope.activeActiveBooking.booking.BookedDateTime = date;
    };

    $scope.DriversMaster = Driver.query({ active: true }, function () {
        angular.forEach($scope.DriversMaster, function (driver) {
            driver.$marker = new google.maps.Marker({
                position: new google.maps.LatLng(driver.LastKnownPosition.latitude, driver.LastKnownPosition.longitude),
                icon: {
                    url: '/api/image?imagetype=Google&ownertype=driver&ownerid=' + driver.ID,
                    scaledSize: new google.maps.Size(50, 50)
                },
                title: driver.CallSign,
                map: $scope.mapObject
            });
        });
    });

    $scope.$on('NewLocation', function (evt, item) {
        $scope.DriversMaster.forEach(function (driver) {
            if (driver.ID == item.DriverID) {
                driver.$marker.setPosition(new google.maps.LatLng(item.Point.latitude, item.Point.longitude));
            }
        });
    });

    $scope.addNewBooking = function () {
        $scope.driverOrder = [];
        $scope.previous = [];
        var booking = new activeBooking();
        $scope.activeBookings.push(booking);
        $scope.activeActiveBooking = booking;
    };

    $scope.swapBooking = function (activeBooking) {
        $scope.driverOrder = [];
        $scope.previous = [];
        var old = $scope.activeActiveBooking;

        if (old.fromMarker) {
            old.fromMarker.setMap(null);
        }
        if (old.toMarker) {
            old.toMarker.setMap(null);
        }

        if (activeBooking.fromMarker) {
            activeBooking.fromMarker.setMap($scope.mapObject);
        }
        if (activeBooking.toMarker) {
            activeBooking.toMarker.setMap($scope.mapObject);
        }
        $scope.activeActiveBooking = activeBooking;
    };

    $scope.addNewBooking();

});

app.directive('locationSearchAdv', function (Location, $filter) {
    return {
        restrict: 'A',
        scope: {
            selected: '='
        },
        template:
              ' <input type="text" ng-model="searchText" placeholder="Postcodes, Stations, Airports etc" style="width:100%;display:inline-block;" class="textBox location" />'
            + ' <div></div>'
            + ' <div class="popover bottom" style="position:absolute; top: 25px; left:15px; right:15px; max-width: none;border-radius:0px;border-radius:0px;background-color:#2C3432;opacity:0.98;border: 3px solid #1D2826;">'
            + '     <div class="arrow"></div>'
            + '     <div class="popover-content" style="padding:0px;border-radius:0px;">'
            + '         <div ng-hide="locations">Searching...</div>'
            + '         <div ng-show="locations" ng-repeat="t in types | customOrder:order">'
            + '             <div class="addressList {{t | limitTo:3}}" style="height:30px; padding:0 5px; font-weight:600; width:100%;color:#f9f9f9;text-shadow: 0px 1px 2px rgba(0,0,0,0.5);line-height:30px;margin-bottom:3px;font-family: Montserrat, sans-serif;text-transform:uppercase;">{{ t }} <img class="pull-right" ng-src="{{GetTypeImage(t)}}" /></div>'
            + '             <div ng-repeat="l in locations | ItemsForGroup:t:grouping:sorting" ng-click="Select(l)" class="repeaterItemCount bold" style="padding:2px 5px;margin:2px 0;">'
            + '             {{ l.Name }}'
            + '             </div>'
            + '         </div>'
            + '     </div>'
            + ' </div>',
        link: function (scope, elem, attrs) {
            elem.css("position", "relative");
            scope.gService = new google.maps.places.AutocompleteService();
            scope.gDetails = new google.maps.places.PlacesService(elem.children('div')[0]);

            scope.locations = [];
            scope.types = [];
            scope.grouping = { key: 'Type', initials: false };
            scope.sorting = { key: 'Match', reverse: true };
            scope.order = {
                'Airport': 1,
                'Train Station': 2,
                'Underground': 3,
                'Company': 4,
                'Google Results': 10
            }

            scope.GetOrderIndex = function (type) {
                return scope.order[type] ? scope.order[type] : 20;
            };

            scope.GetTypeImage = function (type) {
                var lookup = {
                    'Airport': '/includes/img/airports.png',
                    'Train Station': '/includes/img/trainStations.png',
                    'Company': '/includes/img/powered-by-google-on-white2.png',
                    'Google Results': '/includes/img/powered-by-google-on-white2.png'
                }
                return lookup[type];
            }

            scope.searchText = "";
            scope.$watch("searchText", function (newValue, oldValue) {
                if (newValue && newValue.length > 2) {
                    scope.highlighted = -1;
                    elem.children('.popover').show();
                    Location.search({ q: newValue }, function (data) {
                        scope.locations = scope.locations.where(function (value) { return (value.Type == "Google Results") });
                        angular.forEach(data, function (result) {
                            scope.locations.push(result);
                        });
                        scope.types = $filter('GetGroupsForData')(scope.locations, scope.grouping, scope.sorting);
                    });
                    scope.gService.getPlacePredictions({ input: newValue, componentRestrictions: { country: 'gb' } }, function (data, status) {
                        if (status = google.maps.places.PlacesServiceStatus.OK) {
                            scope.locations = scope.locations.where(function (value) { return (value.Type != "Google Results") });
                            angular.forEach(data, function (result) {
                                scope.locations.push({
                                    Name: result.description,
                                    Type: "Google Results",
                                    Reference: result.reference
                                });
                            });
                            scope.types = $filter('GetGroupsForData')(scope.locations, scope.grouping, scope.sorting);
                            scope.$digest();
                        }
                    })
                } else {
                    elem.children('.popover').hide();
                    scope.locations = [];
                    scope.selected = null;
                }
            });

            scope.$watch('selected', function () {
                if (scope.selected && scope.selected.Name) {
                    searchText = scope.selected.Name;
                    elem.children('input')[0].value = scope.selected.Name;
                }
            });

            scope.Select = function (location) {
                if (location.Type == "Google Results") {
                    scope.gDetails.getDetails({
                        reference: location.Reference,
                        sensor: false,
                        key: 'AIzaSyBG7ERZfOCGfUxgrB1aA7mdgt5aY3HzZvY'
                    }, function (details) {
                        elem.children('input')[0].value = location.Name;
                        location.Latitude = details.geometry.location.lat();
                        location.Longitude = details.geometry.location.lng();
                        elem.children('.popover').hide();
                        if (scope.$root.$$phase == "$apply" || scope.$root.$$phase == "$digest") {
                            scope.locations = [];
                            scope.selected = location;
                        } else {
                            scope.$root.$apply(function () {
                                scope.locations = [];
                                scope.selected = location;
                            });
                        }
                    });
                } else {
                    elem.children('input')[0].value = location.Name;
                    elem.children('.popover').hide();
                    if (scope.$root.$$phase == "$apply" || scope.$root.$$phase == "$digest") {
                        scope.locations = [];
                        scope.selected = location;
                    } else {
                        scope.$root.$apply(function () {
                            scope.locations = [];
                            scope.selected = location;
                        });
                    }
                }

            }

            scope.highlighted = -1;
            elem.children('input')[0].onkeydown = function (event) {
                if (event.keyIdentifier == "Down") {
                    scope.highlighted += 1;
                    if (scope.highlighted > scope.locations.length - 1)
                        scope.highlighted = scope.locations.length - 1;
                    $('.repeaterItemCount.Selected').removeClass('Selected');
                    $('.repeaterItemCount').addClass(function (index) {
                        if (index == scope.highlighted)
                            return 'Selected';
                    });
                    event.preventDefault();
                }
                if (event.keyIdentifier == "Up") {
                    scope.highlighted -= 1;
                    if (scope.highlighted < 0)
                        scope.highlighted = 0;
                    $('.repeaterItemCount.Selected').removeClass('Selected');
                    $('.repeaterItemCount').addClass(function (index) {
                        if (index == scope.highlighted)
                            return 'Selected';
                    });
                    event.preventDefault();
                }
                if (event.keyIdentifier == "Enter") {
                    scope.Select($('.repeaterItemCount.Selected').scope().l);
                }
                if (event.keyIdentifier == "U+001B") {
                    elem.children('.popover').hide();
                    if (scope.$root.$$phase == "$apply" || scope.$root.$$phase == "$digest") {
                        scope.locations = [];
                        scope.selected = {
                            Name: elem.children('input')[0].value
                        };
                    } else {
                        scope.$root.$apply(function () {
                            scope.locations = [];
                            scope.selected = {
                                Name: elem.children('input')[0].value
                            };
                        });
                    }
                }
                if (event.keyIdentifier == "U+0009") {
                    elem.children('.popover').hide();
                    if (scope.$root.$$phase == "$apply" || scope.$root.$$phase == "$digest") {
                        scope.locations = [];
                        if (scope.selected && scope.selected.Name == elem.children('input')[0].value) {
                        } else {
                            scope.selected = {
                                Name: elem.children('input')[0].value
                            };
                        }
                    } else {
                        scope.$root.$apply(function () {
                            scope.locations = [];
                            if (scope.selected && scope.selected.Name == elem.children('input')[0].value) {
                            } else {
                                scope.selected = {
                                    Name: elem.children('input')[0].value
                                };
                            }
                        });
                    }
                }
            };
        }
    };
});

app.directive('phoneSearch', function ($filter) {
    return {
        restrict: 'A',
        scope: {
            selected: '='
        },
        template:
              ' <input type="text" ng-model="searchText" placeholder="07720.." style="width:100%" class="textBox mobile" />'
            + ' <div></div>'
            + ' <div class="popover bottom" style="position:absolute; top: 20px; left:15px; right:15px; max-width: none;">'
            + '     <div class="arrow"></div>'
            + '     <div class="popover-content">'
            + '         <div ng-show="searching">Searching...</div>'
            + '         <div ng-show="!searching && !numbers.length">No Results Found</div>'
            + '         <div ng-repeat="n in numbers" ng-click="Select(n)" class="repeaterItemCount black">'
            + '             {{ n.ContactNumber }} <span class="cyan">({{ n.PassengerName | Truncate:10}})</span>'
            + '         </div>'
            + '     </div>'
            + ' </div>',
        link: function (scope, elem, attrs) {
            elem.css("position", "relative");

            scope.numbers = [];

            scope.searchText = "";
            scope.$watch("searchText", function (newValue, oldValue) {
                if (newValue && newValue.length > 2) {
                    scope.highlighted = -1;
                    elem.children('.popover').show();
                    scope.searching = true;
                    Booking.getPrevious({ number: newValue }, function (data) {
                        scope.searching = false;
                        scope.numbers = data;
                    })
                    scope.selected = null;
                } else {
                    elem.children('.popover').hide();
                    scope.numbers = [];
                    scope.selected = null;
                }
            });

            scope.$watch('selected', function () {
                elem.children('input')[0].value = (scope.selected && scope.selected.ContactNumber) ? scope.selected.ContactNumber : '';
            }, true);

            scope.Select = function (number) {
                elem.children('input')[0].value = number.ContactNumber;
                elem.children('.popover').hide();
                if (scope.$root.$$phase == "$apply" || scope.$root.$$phase == "$digest") {
                    scope.numbers = [];
                    scope.selected = {
                        PassengerName: number.PassengerName,
                        ContactNumber: number.ContactNumber
                    };
                } else {
                    scope.$root.$apply(function () {
                        scope.numbers = [];
                        scope.selected = {
                            PassengerName: number.PassengerName,
                            ContactNumber: number.ContactNumber
                        };
                    });
                }
            }

            scope.highlighted = -1;
            elem.children('input')[0].onkeydown = function (event) {
                if (event.keyIdentifier == "Down") {
                    scope.highlighted += 1;
                    if (scope.highlighted > scope.numbers.length - 1)
                        scope.highlighted = scope.numbers.length - 1;
                    $('.repeaterItemCount.Selected').removeClass('Selected');
                    $('.repeaterItemCount').addClass(function (index) {
                        if (index == scope.highlighted)
                            return 'Selected';
                    });
                    event.preventDefault();
                }
                if (event.keyIdentifier == "Up") {
                    scope.highlighted -= 1;
                    if (scope.highlighted < 0)
                        scope.highlighted = 0;
                    $('.repeaterItemCount.Selected').removeClass('Selected');
                    $('.repeaterItemCount').addClass(function (index) {
                        if (index == scope.highlighted)
                            return 'Selected';
                    });
                    event.preventDefault();
                }
                if (event.keyIdentifier == "Enter") {
                    scope.Select($('.repeaterItemCount.Selected').scope().n);
                }
                if (event.keyIdentifier == "U+001B") {
                    elem.children('.popover').hide();
                    if (scope.$root.$$phase == "$apply" || scope.$root.$$phase == "$digest") {
                        scope.numbers = [];
                        scope.selected = {
                            ContactNumber: elem.children('input')[0].value
                        };
                    } else {
                        scope.$root.$apply(function () {
                            scope.numbers = [];
                            scope.selected = {
                                ContactNumber: elem.children('input')[0].value
                            };
                        });
                    }
                }
                if (event.keyIdentifier == "U+0009") {
                    elem.children('.popover').hide();
                    if (scope.$root.$$phase == "$apply" || scope.$root.$$phase == "$digest") {
                        scope.numbers = [];
                        scope.selected = {
                            ContactNumber: elem.children('input')[0].value
                        };
                    } else {
                        scope.$root.$apply(function () {
                            scope.numbers = [];
                            if (scope.selected && scope.selected.ContactNumber == elem.children('input')[0].value) {
                            } else {
                                scope.selected = {
                                    ContactNumber: elem.children('input')[0].value
                                };
                            }
                        });
                    }
                }
            };
        }
    };
});

app.directive('nameSearch', function ($filter) {
    return {
        restrict: 'A',
        scope: {
            selected: '='
        },
        template:
              ' <input type="text" ng-model="searchText" placeholder="Search previous passengers.." style="width:100%"/>'
            + ' <div></div>'
            + ' <div class="popover bottom" style="position:absolute; top: 20px; left:15px; right:15px; max-width: none">'
            + '     <div class="arrow"></div>'
            + '     <div class="popover-content">'
            + '         <div ng-show="searching">Searching...</div>'
            + '         <div ng-show="!searching && !names.length">No Results Found</div>'
            + '         <div ng-repeat="n in names" ng-click="Select(n)" class="repeaterItemCount">'
            + '             {{ n.PassengerName }} ({{ n.ContactNumber }})'
            + '         </div>'
            + '     </div>'
            + ' </div>',
        link: function (scope, elem, attrs) {
            elem.css("position", "relative");

            scope.names = [];

            scope.searchText = "";
            scope.$watch("searchText", function (newValue, oldValue) {
                if (newValue && newValue.length > 2) {
                    scope.highlighted = -1;
                    elem.children('.popover').show();
                    scope.searching = true;
                    Booking.getPrevious({ name: newValue }, function (data) {
                        scope.searching = false;
                        scope.names = data;
                    })
                    scope.selected = null;
                } else {
                    elem.children('.popover').hide();
                    scope.names = [];
                    scope.selected = null;
                }
            });

            scope.Select = function (name) {
                elem.children('input')[0].value = name.PassengerName;
                elem.children('.popover').hide();
                if (scope.$root.$$phase == "$apply" || scope.$root.$$phase == "$digest") {
                    scope.names = [];
                    scope.selected = name;
                } else {
                    scope.$root.$apply(function () {
                        scope.names = [];
                        scope.selected = name;
                    });
                }
            }

            scope.highlighted = -1;
            elem.children('input')[0].onkeydown = function (event) {
                if (event.keyIdentifier == "Down") {
                    scope.highlighted += 1;
                    if (scope.highlighted > scope.names.length - 1)
                        scope.highlighted = scope.names.length - 1;
                    $('.repeaterItemCount.Selected').removeClass('Selected');
                    $('.repeaterItemCount').addClass(function (index) {
                        if (index == scope.highlighted)
                            return 'Selected';
                    });
                    event.preventDefault();
                }
                if (event.keyIdentifier == "Up") {
                    scope.highlighted -= 1;
                    if (scope.highlighted < 0)
                        scope.highlighted = 0;
                    $('.repeaterItemCount.Selected').removeClass('Selected');
                    $('.repeaterItemCount').addClass(function (index) {
                        if (index == scope.highlighted)
                            return 'Selected';
                    });
                    event.preventDefault();
                }
                if (event.keyIdentifier == "Enter") {
                    scope.Select($('.repeaterItemCount.Selected').scope().n);
                }
                if (event.keyIdentifier == "U+001B") {
                    elem.children('.popover').hide();
                    if (scope.$root.$$phase == "$apply" || scope.$root.$$phase == "$digest") {
                        scope.names = [];
                        scope.selected = {
                            PassengerName: elem.children('input')[0].value
                        };
                    } else {
                        scope.$root.$apply(function () {
                            scope.names = [];
                            scope.selected = {
                                PassengerName: elem.children('input')[0].value
                            };
                        });
                    }
                }
                if (event.keyIdentifier == "U+0009") {
                    elem.children('.popover').hide();
                    if (scope.$root.$$phase == "$apply" || scope.$root.$$phase == "$digest") {
                        scope.names = [];
                        scope.selected = {
                            PassengerName: elem.children('input')[0].value
                        };
                    } else {
                        scope.$root.$apply(function () {
                            scope.names = [];
                            scope.selected = {
                                PassengerName: elem.children('input')[0].value
                            };
                        });
                    }
                }
            };
        }
    };
});

app.factory("Location", function ($resource) {
    return $resource(apiEndPoint + 'location/:action', {}, {
        get: {
            method: 'GET',
            isArray: false,
            params: {
                action: 'GetByID'
            }
        },
        search: {
            method: 'GET',
            isArray: true,
            params: {
                action: 'Search'
            }
        },
        update: {
            method: 'PUT'
        }
    });
});

app.filter("customOrder", function () {
    return function (array, order) {
        return array.sort(SortBy(null, false, function (value) { return order[value]; }));
    };
});

//////////////////NEW BOOKING CONTROLLER/////////////////////
