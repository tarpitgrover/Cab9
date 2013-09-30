/////////////////////////////////////////////////////////////
//                   Route Registration                    //
/////////////////////////////////////////////////////////////
app.config(function ($routeProvider, $httpProvider) {
    $httpProvider.defaults.useXDomain = true;
    delete $httpProvider.defaults.headers.common['X-Requested-With'];
    $httpProvider.interceptors.push('NetworkActivityInterceptor');
    $httpProvider.interceptors.push('httpAuthInterceptor');

    $routeProvider
         .when('/table-view',
        {
            controller: 'PricingModelsController',
            templateUrl: '/pricing/partials/model-table.html'
        })
        .when('/editor-view',
        {
            controller: 'PricingEditorController',
            templateUrl: '/pricing/partials/model-editor.html'
        })
        .when('/editor-view/:modelId',
        {
            controller: 'PricingEditorController',
            templateUrl: '/pricing/partials/model-editor.html'
        })
        .when('/quote-test',
        {
            controller: 'PricingQuoteTest',
            templateUrl: '/pricing/partials/quote-test.html'
        })
        .when('/assignments',
        {
            controller: 'PricingAssignmentsController',
            templateUrl: '/pricing/partials/model-assignments.html'
        })
        .otherwise({ redirectTo: '/editor-view' });
});

/////////////////////////////////////////////////////////////
//               Controller Registration                   //
/////////////////////////////////////////////////////////////
app.controller('PricingController', function ($scope) {
    $scope.$parent.appStatus = 'Loading';

    $scope.pricingModels = PricingModel.query(function () {

    });

    $scope.allZones = PricingZone.query(function () {
    });

    $scope.$parent.appStatus = 'Ready';
});

app.controller('PricingModelsController', function ($scope) {
    $scope.$parent.appStatus = 'Loading';

    $scope.$parent.appStatus = 'Ready';
});

app.controller('PricingAssignmentsController', function ($scope, $filter) {
    $scope.$parent.appStatus = 'Loading';

    $scope.grouping = { key: 'AccountType', initials: false };
    $scope.groupingVal = { key: 'AccountType.Name', initials: false };
    $scope.sorting = { key: 'AccountType', initials: false }

    $scope.customers = Customer.query(function (data) {
        $scope.customerTypes = $filter('GetGroupObjectsForData')(data, $scope.grouping, $scope.sorting)
    });

    $scope.GetCurrentModel = function (customer) {
        var result = 'Company Default';
        if (customer.DefaultPricingModel) {
            angular.forEach($scope.pricingModels, function (value) {
                if (value.ID == customer.DefaultPricingModel) result = value.Name;
            });
        } else if (customer.AccountType) {
            angular.forEach($scope.customerTypes, function (custType) {
                if (custType.ID == customer.AccountType.ID)
                    angular.forEach($scope.pricingModels, function (value) {
                        if (value.ID == custType.DefaultPricingModel) result = 'Group Default: ('+value.Name+')';
                    });
            });
        }
        return result;
    };

    $scope.UpdateGroup = function (grp) {
        CustomerType.update(grp, function (data) {
            grp = data;
        });
    };

    $scope.UpdateCustomer = function (cust) {
        cust.$update();
    };

    $scope.$parent.appStatus = 'Ready';
});

app.controller('PricingQuoteTest', function ($scope) {
    $scope.$parent.appStatus = 'Loading';

    $scope.mapOptions = {};
    $scope.mapObject = {};

    $scope.waypoints = [{}, {}];
    $scope.AddWaypoint = function () {
        $scope.waypoints.push({});
    };

    $scope.directions = {};

    $scope.quote = {
        $$reorder: true,
        cartype: 1,
        customerid: null,
        pricingmodel: 11,
        encodedRoute: '',
        waitingTimes: ''
    };

    $scope.GetRoute = function () {
        var directionsService = new google.maps.DirectionsService();
        var waypoints = [];
        if ($scope.waypoints.length > 2) {
            angular.forEach($scope.waypoints.slice(1, -1), function (way) {
                waypoints.push({
                    location: way.place.geometry.location,
                    stopover: true
                });
            });
        }

        var request = {
            origin: $scope.waypoints[0].place.geometry.location,
            destination: $scope.waypoints[$scope.waypoints.length - 1].place.geometry.location,
            waypoints: waypoints,
            optimizeWaypoints: $scope.quote.$$reorder,
            travelMode: google.maps.DirectionsTravelMode.DRIVING
        };
        directionsService.route(request, function (response, status) {
            if (status == google.maps.DirectionsStatus.OK) {
                $scope.$apply(function () {
                    $scope.OrderWaypoints(response.routes[0].waypoint_order);
                    $scope.directions = response;
                    var route = [];
                    angular.forEach($scope.waypoints, function (way) {
                        route.push(way.place.geometry.location);
                    });
                    $scope.quote.encodedRoute = google.maps.geometry.encoding.encodePath(route);
                    $scope.GetQuote();
                })
            }
        });
    };

    $scope.GetQuote = function () {
        if ($scope.waypoints.length > 2) {
            var waitingTimes = '';
            angular.forEach($scope.waypoints.slice(1, -1), function (way) {
                waitingTimes += way.waitingTime + ',';
            });
            $scope.quote.waitingTimes = waitingTimes.slice(0, -1);
        }
        $scope.response = Quote.get($scope.quote);
    };

    $scope.RemoveWaypoint = function (waypoint) {

        $scope.waypoints.splice($scope.waypoints.indexOf(waypoint), 1);
        if (waypoint.marker) {
            waypoint.marker.setMap(null);
            delete waypoint;
        }
    };

    $scope.MoveWaypoint = function (direction, index, end) {
        if (end) return;
        if (direction == 'UP') {
            var temp = $scope.waypoints[index - 1];
            $scope.waypoints[index - 1] = $scope.waypoints[index];
            $scope.waypoints[index] = temp;
        }
        else if (direction == 'DOWN') {
            var temp = $scope.waypoints[index + 1];
            $scope.waypoints[index + 1] = $scope.waypoints[index];
            $scope.waypoints[index] = temp;
        }
    };

    $scope.OrderWaypoints = function (order) {
        var newOrder = [];        
        newOrder.push($scope.waypoints[0]);
        for (var i = 0; i < order.length; i++)
        {
            newOrder.push($scope.waypoints[order.indexOf(i) + 1]);

        }
        newOrder.push($scope.waypoints[$scope.waypoints.length - 1]);
        $scope.waypoints = newOrder;
    };

    $scope.$parent.appStatus = 'Ready';
});

app.controller('PricingEditorController', function ($scope, $location, $routeParams) {
    $scope.$parent.$parent.appStatus = 'Loading';

    if ($routeParams.modelId) {
        $scope.selectedModel = PricingModel.get({ id: $routeParams.modelId }, function () {
            $scope.selectedModel.$saved = true;
        });

        $scope.currentZones = PricingZone.query({ pricingmodelid: $routeParams.modelId }, function (result) {
            $scope.$broadcast('InitialiseWithZones', result);
        })

        $scope.selectedPricings = PricingFixed.query({ modelid: $routeParams.modelId });
    } else {
        $scope.selectedModel = new PricingModel();

        $scope.currentZones = [];

        $scope.selectedPricings = [];
    }



    $scope.PricingModes = [{
        value: 'HighestEntered',
        display: 'Highest Entered'
    }, {
        value: 'LastEntered',
        display: 'Last Entered'
    }, {
        value: 'AverageEntered',
        display: 'Average Entered'
    }
    ];
    ///////////// START: Model Tab ///////////////

    $scope.SavePricingModel = function () {
        if ($scope.selectedModel.$saved != true) {
            $scope.selectedModel.$save(function () {
                alert('Pricing Model Saved: You may now customise further with zones and fixed pricings');
                $scope.selectedModel.$saved = true;
            });
        } else {
            $scope.selectedModel.$update(function () {
                alert('Pricing Model Updated');
                $scope.selectedModel.$saved = true;
            });
        }
    }

    $scope.CancelPricingModel = function () {
        $scope.selectedModel = null;
        $location.path('/table-view');
    }

    ///////////// END: Model Tab ///////////////


    ///////////// START: Fixed Price Tab ///////////////

    $scope.GetValue = function(to, from) {
        var result = $scope.CheckForValue(to, from);
        if (result)
            return result.Price;
        else
            return "";
    }

    $scope.GetClass = function (to, from) {
        var result = $scope.CheckForValue(to, from);
        if (result)
            return 'btn-danger';
        else
            return 'btn-success';
    }

    $scope.UpdateValue = function (to, from) {
        var newval = document.getElementById('t' + to + 'f' + from).value;
        var result = $scope.CheckForValue(to, from);
        if (newval == "" || newval == null) {
            if (result) {
                result.$remove({
                    pricingmodelid: result.PricingModelID,
                    fromid: result.PricingZoneFrom,
                    toid: result.PricingZoneTo
                });
                delete result;
            }
        } else if (result && result.Price == newval) {
            result.$remove({
                pricingmodelid: result.PricingModelID,
                fromid: result.PricingZoneFrom,
                toid: result.PricingZoneTo
            });
            delete result;
        } else {
            if (result) {
                result.Price = newval;
                result.$update();
            } else {
                result = new PricingFixed();
                result.Price = newval;
                result.PricingZoneTo = to;
                result.PricingZoneFrom = from;
                result.PricingModelID = $scope.selectedModel.ID;
                result.$save();
                $scope.selectedPricings.push(result);
            }
        }
    }

    $scope.CheckForValue = function (to, from) {
        var search = $.grep($scope.selectedPricings, function (e) { return (e.PricingZoneFrom == from && e.PricingZoneTo == to); });
        if (search.length > 0)
        {
            return search[0];
        }
        return null;
    }

    ///////////// END: Model Tab ///////////////


    ///////////// START: Map Tab ///////////////
    $scope.mode = "Default";

    $scope.options = {};

    $scope.selectedZone = null;
    $scope.selectedMapZone = null;

    $scope.mapZones = [];

    $scope.SelectZone = function (zone) {
        $scope.ClearSelection();

        $scope.selectedZone = zone;
        $scope.selectedMapZone = $scope.mapZones[$scope.currentZones.indexOf(zone)]

        alert(google.maps.geometry.spherical.computeArea($scope.selectedMapZone.getPath()));

        $scope.selectedMapZone.setEditable(true)
        $scope.selectedMapZone.setDraggable(true)
        $scope.selectedMapZone.setOptions({
            strokeWeight: 3.0,
            fillColor: '#D21D1D',
            fillOpacity: 0.6,
            strokeColor: '#26AE90',
            strokeOpacity: 0.7,
            zIndex: (100000000 - (google.maps.geometry.spherical.computeArea($scope.selectedMapZone.getPath()) / 10000))
        });

        $scope.mode = 'ConfigZone';
    };

    $scope.ClearSelection = function () {
        if ($scope.selectedZone && $scope.selectedMapZone) {
            $scope.selectedMapZone.setEditable(false)
            $scope.selectedMapZone.setDraggable(false)
            $scope.selectedMapZone.setOptions({
                strokeWeight: 2,
                fillColor: '#26AE90',
                fillOpacity: 0.6,
                strokeColor: '#D21D1D',
                strokeOpacity: 0.7
            });
            $scope.selectedZone = null;
            $scope.selectedMapZone = null;
        };
        $scope.mode = "Default";
    };

    $scope.StartNewZone = function () {
        $scope.$broadcast('StartZone');
        $scope.mode = "DrawingZone";
        $scope.newZone = new PricingZone();
        $scope.newMapZone = {};
    };

    $scope.SaveNewZone = function () {
        $scope.newZone.PricingModelID = $scope.selectedModel.ID;

        $scope.newMapZone.setEditable(false),
        $scope.newMapZone.setDraggable(false);

        $scope.currentZones.push($scope.newZone);
        $scope.mapZones.push($scope.newMapZone);

        var paths = $scope.newMapZone.getPaths();
        $scope.newZone.EncodedZone = google.maps.geometry.encoding.encodePath(paths.getArray()[0]);
        $scope.newZone.$save();
        $scope.mode = "Default";
    };

    $scope.CancelNewZone = function () {
        $scope.newZone = new PricingZone();
        $scope.newMapZone = null;

        $scope.newZone.PricingModelID = $scope.selectedModel.ID;
        $scope.mode = "Default";
        $scope.$broadcast('CancelZone');
    };

    $scope.UpdateZone = function () {
        $scope.selectedMapZone.setEditable(false),
        $scope.selectedMapZone.setDraggable(false);
        var paths = $scope.selectedMapZone.getPaths();
        $scope.selectedZone.EncodedZone = google.maps.geometry.encoding.encodePath(paths.getArray()[0]);
        $scope.selectedZone.$update();
        $scope.ClearSelection();
        $scope.mode = "Default";
    };

    $scope.CancelUpdate = function () {
        if ($scope.newMapZone) {
            $scope.newMapZone.setEditable(false),
            $scope.newMapZone.setDraggable(false);
        }

        $scope.ClearSelection();
        $scope.mode = "Default";
    };

    $scope.RemoveZone = function () {
        if ($scope.selectedMapZone)
            $scope.selectedMapZone.setMap(null);

        $scope.selectedZone.$delete({ id: $scope.selectedZone.ID });

        $scope.currentZones.splice($scope.currentZones.indexOf($scope.selectedZone), 1);
        $scope.mapZones.splice($scope.mapZones.indexOf($scope.selectedMapZone), 1);

        $scope.selectedMapZone = null;
        $scope.selectedZone = null;

        $scope.ClearSelection();
        $scope.mode = "Default";
    };

    $scope.$on('ZoneComplete', function (event, params) {
        $scope.$apply(function () {
            $scope.mode = "AddingZone";
            $scope.newZone.EncodedZone = params.encoded;
            $scope.newMapZone = params.polygon;
        })
    });

    $scope.CloneZone = function (zone) {
        if (zone) {
            $scope.newZone = zone;
            $scope.$broadcast('AddThisZone', zone);
        }
    }

    $scope.$on('ZoneAdded', function (event, params) {
        $scope.newMapZone = params.polygon;
        $scope.mode = "AddingZone";
    });

    $scope.$on('StartingZoneAdded', function (event, params) {
        $scope.mapZones.push(params.polygon);
    });
    ///////////// END: Map Tab ///////////////



    // Toggle modal
    $scope.$parent.appStatus = 'Ready';
});
