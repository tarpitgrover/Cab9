/// <reference path="../master/angular/masterModule.js" />
/// <reference path="../includes/js/jquery-1.8.3.min.js" />
/// <reference path="../includes/js/angular.min.js" />
/////////////////////////////////////////////////////////////
//                   Route Registration                    //
/////////////////////////////////////////////////////////////
app.config(function ($routeProvider, $httpProvider) {
    $httpProvider.defaults.useXDomain = true;
    //delete $httpProvider.defaults.headers.common['X-Requested-With'];
    //$httpProvider.interceptors.push('httpAuthInterceptor');

    $routeProvider
        .when('/table-view',
        {
            controller: 'BookingTableViewController',
            templateUrl: '/bookings/partials/table-view.html'
        })
        .when('/map-view',
        {
            controller: 'BookingMapViewController',
            templateUrl: '/bookings/partials/map-view.html'
        })
        .when('/new-booking',
        {
            controller: 'NewBookingController',
            templateUrl: '/bookings/partials/new-booking.html'
        })
        .otherwise({ redirectTo: '/table-view' });
});

/////////////////////////////////////////////////////////////
//               Controller Registration                   //
/////////////////////////////////////////////////////////////
app.controller('BookingController', function ($scope) {
    $scope.$parent.appStatus = 'Loading';

    $scope.currentBookings = Booking.query({
        bookedFrom: new Date().addPeriod(-1, "days").toJSON(),
        bookedTo: new Date().addPeriod(1, "days").toJSON()
    });

    $scope.drivers = Driver.query({});

    $scope.$parent.appStatus = 'Ready';
});

app.controller('BookingTableViewController', function ($scope) {
    $scope.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'TableView';

    $scope.currentBookings = Booking.query({
        bookedFrom: new Date().addPeriod(-1, "days").toJSON(),
        bookedTo: new Date().addPeriod(1, "days").toJSON()
    });

    $scope.drivers = Driver.query({});

    $scope.BuildQueue = function (bookingid) {
        Booking.getDriverOrder({
            bookingid: bookingid
        }, function (data) {
            angular.forEach($scope.drivers, function (driver) {
                var record = data.where(function (record) { return (driver.ID == record.driverid); });
                if (record.length > 0)
                    driver.$points = record[0].total;
            });
        });
    };

    $scope.$parent.appStatus = 'Ready';
});

app.controller('BookingMapViewController', function ($scope) {
    $scope.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'MapView';

    $scope.mapOptions = {};
    $scope.mapObject = {};




    $scope.$parent.appStatus = 'Ready';
});

app.controller("NewBookingController", function ($scope) {
    $scope.booking = new Booking();
    $scope.booking.CarType = 1;
    $scope.booking.date = new Date();
    $scope.booking.time = new Date().getHours() + ':' + new Date().getMinutes() + ' ' + ((new Date().getHours() > 11) ? 'PM' : 'AM');
    $scope.booking.PAX = 1;
    $scope.booking.BAX = 0;
    $scope.booking.PaymentMethod = 1;
    $scope.booking.Priority = 1;

    $scope.from = null;
    $scope.to = null;

    $scope.fromMarker = null;
    $scope.toMarker = null;

    $scope.mapObject = {};
    $scope.mapOptions = {};
    $scope.route = {};

    $scope.sideTab = '';
    $scope.previous = [];

    $scope.clients = Client.query();

    $scope.$watch('sideTab', function (newValue) {
        if (newValue == 'from' && $scope.fromMarker) {
            $scope.mapObject.setCenter($scope.fromMarker.getPosition());
            $scope.mapObject.setZoom(17);
        }
        else if (newValue == 'to' && $scope.toMarker) {
            $scope.mapObject.setCenter($scope.toMarker.getPosition());
            $scope.mapObject.setZoom(17);
        }
        else if (newValue == 'journey' && $scope.route.routes) {
            var bounds = $scope.route.routes[0].bounds;
            $scope.mapObject.fitBounds(bounds);
        }
    });

    $scope.$watch('from', function (newValue) {
        if (newValue) {
            $scope.booking.From = newValue.Name;
            if ($scope.fromMarker) {
                $scope.fromMarker.setPosition(new google.maps.LatLng(newValue.Latitude, newValue.Longitude));
                $scope.fromMarker.setMap($scope.mapObject);
            } else {
                $scope.fromMarker = new google.maps.Marker({
                    map: $scope.mapObject,
                    position: new google.maps.LatLng(newValue.Latitude, newValue.Longitude)
                })
            }
            $scope.sideTab = 'from';
        } else {
            $scope.booking.From = '';
            if ($scope.fromMarker) {
                $scope.fromMarker.setMap(null);
            }
        }
        $scope.GetRouteAndQuote();
    }, true);

    $scope.$watch('to', function (newValue) {
        if (newValue) {
            $scope.booking.To = newValue.Name;
            if ($scope.toMarker) {
                $scope.toMarker.setPosition(new google.maps.LatLng(newValue.Latitude, newValue.Longitude));
                $scope.toMarker.setMap($scope.mapObject);
            } else {
                $scope.toMarker = new google.maps.Marker({
                    map: $scope.mapObject,
                    position: new google.maps.LatLng(newValue.Latitude, newValue.Longitude)
                })
            }
            $scope.sideTab = 'to';
        } else {
            $scope.booking.To = '';
            if ($scope.toMarker) {
                $scope.toMarker.setMap(null);
            }
        }
        $scope.GetRouteAndQuote();
    }, true);

    //$scope.$watch('selectedName', function (newValue) {
    //    if (newValue) {
    //        $scope.booking.PassengerName = newValue.PassengerName
    //        $scope.previous = Booking.getPrevious({ number: null, name: newValue.PassengerName })
    //        $scope.bookingsFor = newValue.PassengerName;

    //    } else {
    //        $scope.booking.PassengerName = '';
    //    }
    //});

    $scope.$watch('selectedNumber', function (newValue) {
        if (newValue) {
            if (!$scope.booking.PassengerName) $scope.booking.PassengerName = newValue.PassengerName;
            $scope.booking.ContactNumber = newValue.ContactNumber;
            $scope.previous = Booking.getPrevious({ name: null, number: newValue.ContactNumber })
            $scope.bookingsFor = newValue.ContactNumber;

        } else {
            $scope.booking.ContactNumber = '';
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
        $scope.booking.From = previous.From;
        $scope.booking.To = previous.To;

        directionsService.route(request, function (response, status) {
            if (status == google.maps.DirectionsStatus.OK) {
                $scope.safeApply(function () {
                    $scope.fromMarker = new google.maps.Marker({
                        position: response.routes[0].legs[0].start_location,
                        map: $scope.mapObject
                    });
                    $scope.toMarker = new google.maps.Marker({
                        position: response.routes[0].legs[0].end_location,
                        map: $scope.mapObject
                    });
                    $scope.sideTab = 'journey';
                    $scope.route = response;

                    var quote = {
                        encodedRoute: google.maps.geometry.encoding.encodePath([
                            $scope.fromMarker.getPosition(),
                            $scope.toMarker.getPosition()
                        ]),
                        waitingTimes: null,
                        carType: $scope.booking.CarType,
                        clientId: $scope.booking.ClientID,
                        bookingtime: $scope.booking.BookedDateTime
                    }

                    $scope.quote = Booking.getQuote(quote, function (data) {
                        $scope.booking.CalculatedFare = data.TotalFare;
                        $scope.booking.ActualFare = data.TotalFare
                    });

                    $scope.driverOrder = Booking.getDriverOrderForQuote({
                        latitude: $scope.fromMarker.getPosition().lat(),
                        longitude: $scope.fromMarker.getPosition().lng(),
                        pax: $scope.booking.PAX
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
    };

    $scope.GetRouteAndQuote = function () {
        if ($scope.from && $scope.to) {
            var directionsService = new google.maps.DirectionsService();

            var request = {
                origin: new google.maps.LatLng($scope.from.Latitude, $scope.from.Longitude),
                destination: new google.maps.LatLng($scope.to.Latitude, $scope.to.Longitude),
                optimizeWaypoints: false,
                travelMode: google.maps.DirectionsTravelMode.DRIVING
            };

            directionsService.route(request, function (response, status) {
                if (status == google.maps.DirectionsStatus.OK) {
                    $scope.safeApply(function () {
                        $scope.sideTab = 'journey';
                        $scope.route = response;

                        var points = [
                            new google.maps.LatLng($scope.from.Latitude, $scope.from.Longitude),
                            new google.maps.LatLng($scope.to.Latitude, $scope.to.Longitude)
                        ]

                        var line = new google.maps.Polyline({
                            path: points,
                            map: null
                        });

                        var quote = {
                            encodedRoute: google.maps.geometry.encoding.encodePath(line.getPath()),
                            waitingTimes: null,
                            carType: $scope.booking.CarType,
                            clientId: $scope.booking.ClientID,
                            bookingtime: $scope.booking.BookedDateTime
                        }

                        $scope.quote = Booking.getQuote(quote, function (data) {
                            $scope.booking.CalculatedFare = data.TotalFare;
                            $scope.booking.ActualFare = data.TotalFare
                        });

                        angular.forEach($scope.driverOrder, function (driver) {
                            driver.marker.setMap(null);
                            delete driver.marker;
                        });

                        $scope.driverOrder = Booking.getDriverOrderForQuote({
                            latitude: $scope.from.Latitude,
                            longitude: $scope.from.Longitude,
                            pax: $scope.booking.PAX
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
        $scope.booking.AutoDispatch = auto;
        Booking.save($scope.booking, function () {
            alert('Booking Added');
        })
    }

    $scope.TimeClicked = function (period) {
        var date = new Date();
        if (period == 'Asap') {
        } else if (period == '30m') {
            date = new Date().addPeriod(30, 'minutes');
        } else if (period == '1hr') {
            date = new Date().addPeriod(1, 'hours');
        }
        $scope.booking.date = date;
        $scope.booking.time = date.getHours() + ':' + ((date.getMinutes() > 9) ? date.getMinutes() : '0' + date.getMinutes()) + ' ' + ((date.getHours() > 11) ? 'PM' : 'AM');
        $scope.booking.BookedDateTime = date;

    };
});

app.directive('locationSearchAdv', function (Location, $filter) {
    return {
        restrict: 'A',
        scope: {
            selected: '='
        },
        template:
              ' <input type="text" ng-model="searchText" placeholder="Search Postcodes, Stations, Airports.." style="width:100%"/>'
            + ' <div></div>'
            + ' <div class="popover bottom" style="position:absolute; top: 20px; left:15px; right:15px; max-width: none">'
            + '     <div class="arrow"></div>'
            + '     <div class="popover-content">'
            + '         <div ng-hide="locations">Searching...</div>'
            + '         <div ng-show="locations" ng-repeat="t in types | customOrder:order">'
            + '             <div style="height:20px; padding:4px; font-weight:600; border-bottom:1px solid #3d3d3d; width:100%">{{ t }} <img class="pull-right" ng-src="{{GetTypeImage(t)}}" /></div>'
            + '             <div ng-repeat="l in locations | ItemsForGroup:t:grouping:sorting" ng-click="Select(l)" class="repeaterItemCount">'
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
                    'Airport': null,
                    'Train Station': null,
                    'Company': null,
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
                    scope.gService.getPlacePredictions({ input: newValue }, function (data, status) {
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

app.directive('datetimepicker', function () {
    return {
        restrict: 'A',
        template:
            ' <input type="text" ng-model="dateTime" style="width:100%"/>'
            + ' <div></div>'
            + ' <div class="popover bottom" style="position:absolute; top: 20px; left:15px; right:15px; max-width: none">'
            + '     <div class="arrow"></div>'
            + '     <div class="popover-content">'
            + '         <div>Test</div>'
            + '     </div>'
            + ' </div>',
        link: function(scope, elem, attrs) {
            elem.css("position", "relative");

            var PickerFocus = function () {
                elem.children('input').one('blur', PickerBlur);
                elem.children('.popover').show();
            }

            var PickerBlur = function () {
                elem.children('input').one('focus', PickerFocus);
                elem.children('.popover').hide();
            }

            elem.children('input').one('focus', PickerFocus);
        }
    }
});

app.directive('phoneSearch', function ($filter) {
    return {
        restrict: 'A',
        scope: {
            selected: '='
        },
        template:
              ' <input type="text" ng-model="searchText" placeholder="07720.." style="width:100%"/>'
            + ' <div></div>'
            + ' <div class="popover bottom" style="position:absolute; top: 20px; left:15px; right:15px; max-width: none">'
            + '     <div class="arrow"></div>'
            + '     <div class="popover-content">'
            + '         <div ng-show="searching">Searching...</div>'
            + '         <div ng-show="!searching && !numbers.length">No Results Found</div>'
            + '         <div ng-repeat="n in numbers" ng-click="Select(n)" class="repeaterItemCount">'
            + '             {{ n.ContactNumber }} ({{ n.PassengerName }})'
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

            scope.Select = function (number) {
                elem.children('input')[0].value = number.ContactNumber;
                elem.children('.popover').hide();
                if (scope.$root.$$phase == "$apply" || scope.$root.$$phase == "$digest") {
                    scope.numbers = [];
                    scope.selected = number;
                } else {
                    scope.$root.$apply(function () {
                        scope.numbers = [];
                        scope.selected = number;
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
                if (event.keyIdentifier == "U+001B"){
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
