
/// <reference path="../master/angular/masterModule.js" />
/// <reference path="../includes/js/angular.js" />


/////////////////////////////////////////////////////////////
//                   Route Registration                    //
/////////////////////////////////////////////////////////////
app.config(function ($routeProvider, $httpProvider) {
    $httpProvider.defaults.useXDomain = true;
    delete $httpProvider.defaults.headers.common['X-Requested-With'];

    $routeProvider
         .when('/table-view',
        {
            controller: 'PricingModelsController',
            templateUrl: '/pricing/partials/pricingmodels-table.html'
        })
        .when('/editor-view',
        {
            controller: 'PricingEditorController',
            templateUrl: '/pricing/partials/pricingmodel-editor.html'
        })
        .when('/editor-view/:modelId',
        {
            controller: 'PricingEditorController',
            templateUrl: '/pricing/partials/pricingmodel-editor.html'
        })
        .when('/quote-test',
        {
            controller: 'PricingQuoteTest',
            templateUrl: '/pricing/partials/quote-test.html'
        })
        .when('/assignments',
        {
            controller: 'PricingAssignmentsController',
            templateUrl: '/pricing/partials/pricingmodels-assignment.html'
        })
        .otherwise({ redirectTo: '/table-view' });
});

/////////////////////////////////////////////////////////////
//               Controller Registration                   //
/////////////////////////////////////////////////////////////
app.controller('PricingController', function ($scope) {
    $scope.$parent.appStatus = 'Loading';

    $scope.pricingModels = PricingModel.query(function () {

    });

    $scope.allZones = PricingZone.query(function () {
        angular.forEach($scope.allZones, function (value) {
            value.$PricingModel = value.PricingModel;
            value.PricingModel = null;
        });
    });

    $scope.$parent.appStatus = 'Ready';
});

app.controller('PricingModelsController', function ($scope) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'TableView';


    $scope.$parent.appStatus = 'Ready';
});

app.controller('PricingAssignmentsController', function ($scope, $filter) {
    $scope.$parent.appStatus = 'Loading';

    $scope.grouping = { key: 'ClientType', initials: false };
    $scope.groupingVal = { key: 'ClientType.Name', initials: false };
    $scope.sorting = { key: 'ClientType.Name', reverse: false }

    $scope.clients = Client.query(function (data) {
        $scope.clientTypes = $filter('GetGroupObjectsForData')(data, $scope.grouping, $scope.sorting)
    });

    $scope.GetCurrentModel = function (object) {
        var result = 'Company Default';
        if (object.DefaultPricingModelID) {
            angular.forEach($scope.pricingModels, function (value) {
                if (value.ID == object.DefaultPricingModelID) result = value.Name;
            });
        } else if (object.ClientType) {
            angular.forEach($scope.clientTypes, function (clientType) {
                if (clientType.ID == object.ClientType.ID)
                    angular.forEach($scope.pricingModels, function (value) {
                        if (value.ID == clientType.DefaultPricingModelID) result = 'Group Default: (' + value.Name + ')';
                    });
            });
        }
        return result;
    };

    $scope.UpdateCompany = function (company) {
        Company.update(company, function (data) {
            company = data;
        });
    };

    $scope.UpdateGroup = function (clientType) {
        ClientType.update(clientType, function (data) {
            clientType = data;
        });
    };

    $scope.UpdateClient = function (client) {
        client.$update();
    };

    $scope.$parent.appStatus = 'Ready';
});

app.controller('PricingQuoteTest', function ($scope) {
    $scope.$parent.appStatus = 'Loading';

    $scope.mapOptions = {
        center: new google.maps.LatLng($scope.$parent.$parent.company.Latitude, $scope.$parent.$parent.company.Longitude)
    };
    $scope.mapObject = {};

    $scope.waypoints = [{}, {}];
    $scope.AddWaypoint = function () {
        $scope.waypoints.push({});
    };

    $scope.directions = {};

    $scope.quote = {
        $$reorder: true,
        cartype: 1,
        clientid: null,
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

    if ($routeParams.modelId)
    {
        $scope.selectedModel = PricingModel.get({ pricingmodelid: $routeParams.modelId }, function () {
            $scope.selectedModel.$saved = true;
        });

        $scope.currentZones = PricingZone.query({ pricingmodelid: $routeParams.modelId }, function (result) {
            angular.forEach($scope.currentZones, function (value) {
                var decodedPath = google.maps.geometry.encoding.decodePath(value.EncodedZone);
                value.$PricingModel = value.PricingModel;
                value.PricingModel = null;
                value.$poly = new google.maps.Polygon({
                    paths: [decodedPath],
                    strokeWeight: 2,
                    fillColor: '#26AE90',
                    fillOpacity: 0.6,
                    strokeColor: '#D21D1D',
                    strokeOpacity: 0.7,
                    map: $scope.mapObject,
                    editable: false,
                    dragable: false,
                })
                value.$poly.setOptions({
                    zIndex: (100000000 - (google.maps.geometry.spherical.computeArea(value.$poly.getPath()) / 10000))
                })
            });
            $scope.FixedPrices = PricingFixed.query({ modelid: $routeParams.modelId });
        });

        $scope.fetchZoneNamefromId = function (zoneId) {
            var result = $scope.currentZones.where(function(value){
                return (value.ID == zoneId)
            });
            return result[0].Title;
        }
        $scope.UpdateFixedPrice = function (fixedPrice) {
            fixedPrice.$update(function () { alert('Price Updated'); $scope.fixedPriceEditRowId = null;});
        }
        $scope.SaveFixedPrice = function (newfixedPrice) {
            newfixedPrice.$save(function () { alert('Fixed Price Added'); $scope.fixedPriceEditRowId = null; $scope.FixedPrices.push(newfixedPrice); });
            
        }
        $scope.NewFixedPrice = function () {
            $scope.fixedPriceEditRowId = 0;
            $scope.newFixedPrice = new PricingFixed();
            $scope.newFixedPrice.PricingModelID = $scope.selectedModel.ID;
            
        }

        $scope.DeleteFixedPrice = function (fixedPrice) {
            if (confirm("Are you Sure?")) {
                PricingFixed.remove({ Id: fixedPrice.ID }, function () { alert('Fixed Price Removed'); $scope.FixedPrices = PricingFixed.query({ modelid: $scope.selectedModel.ID }); });
            }
        }
    }
    else {
        $scope.selectedModel = new PricingModel();
        $scope.selectedModel.$saved = false;

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
                    pricingmodelfromid: result.PricingZoneFromID,
                    pricingmodeltoid: result.PricingZoneToID
                }, function () {
                    $scope.selectedPricings = $scope.selectedPricings.where(function (value) { return (value.ID != result.ID) });
                    delete result;
                });
            }
        } else if (result && result.Price == newval) {
            result.$remove({
                pricingmodelid: result.PricingModelID,
                pricingmodelfromid: result.PricingZoneFromID,
                pricingmodeltoid: result.PricingZoneToID
            }, function () {
                $scope.selectedPricings = $scope.selectedPricings.where(function (value) { return (value.ID != result.ID) });
                delete result;
            });
            delete result;
        } else {
            if (result) {
                result.Price = newval;
                result.$update();
            } else {
                result = new PricingFixed();
                result.Price = newval;
                result.PricingZoneToID = to;
                result.PricingZoneFromID = from;
                result.PricingModelID = $scope.selectedModel.ID;
                result.$save();
                $scope.selectedPricings.push(result);
            }
        }
    }

    $scope.CheckForValue = function (to, from) {
        var search = $.grep($scope.selectedPricings, function (e) { return (e.PricingZoneFromID == from && e.PricingZoneToID == to); });
        if (search.length > 0)
        {
            return search[0];
        }
        return null;
    }

    ///////////// END: Model Tab ///////////////


    ///////////// START: Map Tab ///////////////
    $scope.mode = 'Default';
    $scope.zonemode = 'Default';

    $scope.mapOptions = {};
    $scope.mapObject = null;

    $scope.zoneToClone = null;
    $scope.selectedZone = null;

    $scope.Save = null;
    $scope.Cancel = null;
    $scope.Delete = null;

    $scope.$watch('mapObject', function (newvalue, oldvalue, scope) {
        if (newvalue) {
            if (!oldvalue) {
                angular.forEach($scope.currentZones, function (value) {
                    value.$poly.setMap(newvalue);
                })
            }
        }
    });

    $scope.CloneZone = function (zone) {
        if (!zone)
            alert("Please select a zone to clone.");
        else {
            $scope.mode = 'Editing';
            $scope.zonemode = 'Cloning';
            var decodedPath = google.maps.geometry.encoding.decodePath(zone.EncodedZone);
            zone.$poly = new google.maps.Polygon({
                paths: [decodedPath],
                strokeWeight: 3.0,
                fillColor: '#D21D1D',
                fillOpacity: 0.6,
                strokeColor: '#26AE90',
                strokeOpacity: 0.7,
                zIndex: 100000000,
                map: $scope.mapObject,
                editable: true,
                dragable: true
            })
            $scope.selectedZone = zone;
            $scope.Save = function () {
                $scope.selectedZone.PricingModelID = $scope.selectedModel.ID;
                $scope.selectedZone.PricingModel = null;
                var poly = $scope.selectedZone.$poly;
                $scope.selectedZone.$save(function (data) {
                    alert("Zone Saved");
                    $scope.selectedZone.$poly = poly;
                    $scope.selectedZone.$poly.setEditable(false)
                    $scope.selectedZone.$poly.setDraggable(false)
                    $scope.selectedZone.$poly.setOptions({
                        strokeWeight: 2,
                        fillColor: '#26AE90',
                        fillOpacity: 0.6,
                        strokeColor: '#D21D1D',
                        strokeOpacity: 0.7,
                        zIndex: 100000000 - (google.maps.geometry.spherical.computeArea($scope.selectedZone.$poly.getPath()) / 10000)
                    });
                    $scope.currentZones.push($scope.selectedZone);
                    $scope.mode = 'Default';
                    $scope.zonemode = 'Default';
                    $scope.Save = null;
                    $scope.Cancel = null;
                });
            }
            $scope.Cancel = function () {
                $scope.selectedZone.$poly.setMap(null);
                delete $scope.selectedZone.$poly;
                $scope.selectedZone = null;
                $scope.mode = 'Default';
                $scope.zonemode = 'Default';
                $scope.Save = null;
                $scope.Cancel = null;
            }
        }
    }

    $scope.StartNewZone = function () {
        $scope.$broadcast('StartZone');
        $scope.mode = 'Editing';
        $scope.zonemode = 'Drawing';
        $scope.selectedZone = new PricingZone();
        $scope.Cancel = function () {
            $scope.$broadcast('CancelZone');
            $scope.selectedZone = null;
            $scope.mode = 'Default';
            $scope.zonemode = 'Default';
            $scope.Cancel = null;
        }
    }

    $scope.$on('ZoneComplete', function (event, params) {
        if ($scope.zonemode = "Drawing") {
            $scope.$apply(function () {
                $scope.zonemode = 'Creating';
                $scope.selectedZone.EncodedZone = params.encoded;
                $scope.selectedZone.$poly = params.polygon;
                $scope.Save = function () {
                    $scope.selectedZone.PricingModelID = $scope.selectedModel.ID;
                    var poly = $scope.selectedZone.$poly;
                    $scope.selectedZone.$save(function (data) {
                        $scope.selectedZone.$poly = poly;
                        alert("Zone Saved");
                        $scope.selectedZone.$poly.setEditable(false)
                        $scope.selectedZone.$poly.setDraggable(false)
                        $scope.selectedZone.$poly.setOptions({
                            strokeWeight: 2,
                            fillColor: '#26AE90',
                            fillOpacity: 0.6,
                            strokeColor: '#D21D1D',
                            strokeOpacity: 0.7,
                            zIndex: 100000000 - (google.maps.geometry.spherical.computeArea($scope.selectedZone.$poly.getPath()) / 10000)
                        });
                        $scope.currentZones.push($scope.selectedZone);
                        $scope.mode = 'Default';
                        $scope.zonemode = 'Default';
                        $scope.Save = null;
                        $scope.Cancel = null;
                    });
                };
                $scope.Cancel = function () {
                    $scope.selectedZone.$poly.setMap(null);
                    delete $scope.selectedZone.$poly;
                    $scope.selectedZone = null;
                    $scope.mode = 'Default';
                    $scope.zonemode = 'Default';
                    $scope.Cancel = null;
                }
            })
        }
    });

    $scope.EditZone = function (zone) {
        if (!zone)
            alert("Please select a zone to clone.");
        else {
            $scope.mode = 'Editing';
            $scope.zonemode = 'Editing';
            $scope.selectedZone = zone;
            $scope.selectedZone.$poly.setEditable(true);
            $scope.selectedZone.$poly.setDraggable(true);
            $scope.selectedZone.$poly.setOptions({
                strokeWeight: 3.0,
                fillColor: '#D21D1D',
                fillOpacity: 0.6,
                strokeColor: '#26AE90',
                strokeOpacity: 0.7,
                zIndex: 100000000
            });
            $scope.Save = function () {
                $scope.selectedZone.PricingModelID = $scope.selectedModel.ID;
                var poly = $scope.selectedZone.$poly;
                $scope.selectedZone.$update(function (data) {
                    $scope.selectedZone.$poly = poly;
                    alert("Zone Saved");
                    $scope.selectedZone.$poly.setEditable(false)
                    $scope.selectedZone.$poly.setDraggable(false)
                    $scope.selectedZone.$poly.setOptions({
                        strokeWeight: 2,
                        fillColor: '#26AE90',
                        fillOpacity: 0.6,
                        strokeColor: '#D21D1D',
                        strokeOpacity: 0.7,
                        zIndex: 100000000 - (google.maps.geometry.spherical.computeArea($scope.selectedZone.$poly.getPath()) / 10000)
                    });
                    $scope.mode = 'Default';
                    $scope.zonemode = 'Default';
                    $scope.Save = null;
                    $scope.Cancel = null;
                    $scope.Delete = null;
                });
            };

            $scope.Cancel = function () {
                var decodedPath = google.maps.geometry.encoding.decodePath($scope.selectedZone.EncodedZone);
                $scope.selectedZone.$poly.setPaths([decodedPath]);
                $scope.selectedZone.$poly.setDraggable(false);
                $scope.selectedZone.$poly.setEditable(false);
                $scope.selectedZone.$poly.setOptions({
                    strokeWeight: 2,
                    fillColor: '#26AE90',
                    fillOpacity: 0.6,
                    strokeColor: '#D21D1D',
                    strokeOpacity: 0.7,
                    zIndex: 100000000 - (google.maps.geometry.spherical.computeArea($scope.selectedZone.$poly.getPath()) / 10000)
                });

                $scope.selectedZone = null;
                $scope.mode = 'Default';
                $scope.zonemode = 'Default';
                $scope.Save = null;
                $scope.Cancel = null;
                $scope.Delete = null;
            }

            $scope.Delete = function () {
                PricingZone.delete({id: $scope.selectedZone.ID}, function (data) {
                    alert("Zone Deleted");
                    $scope.selectedZone.$poly.setMap(null);
                    delete $scope.selectedZone.$poly;
                    $scope.currentZones.splice($scope.currentZones.indexOf($scope.selectedZone), 1);
                    $scope.selectedZone = null;
                    $scope.mode = 'Default';
                    $scope.zonemode = 'Default';
                    $scope.Save = null;
                    $scope.Cancel = null;
                    $scope.Delete = null;
                });
            }

        }
    }

    ///////////// END: Map Tab ///////////////

    // Toggle modal
    $scope.$parent.appStatus = 'Ready';
});
