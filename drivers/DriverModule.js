/// <reference path="../master/angular/masterModule.js" />
/// <reference path="../includes/js/jquery-1.8.3.min.js" />
/// <reference path="../includes/js/angular.min.js" />
/// <reference path="../master/angular/masterModule.js" />
/////////////////////////////////////////////////////////////
//                   Route Registration                    //
/////////////////////////////////////////////////////////////
app.config(function ($routeProvider, $httpProvider) {
    $httpProvider.defaults.useXDomain = true;
    delete $httpProvider.defaults.headers.common['X-Requested-With'];
    $httpProvider.interceptors.push('httpAuthInterceptor');


    $routeProvider
        .when('/cards-view',
        {
            controller: 'DriversCardsViewController',
            templateUrl: '/drivers/partials/drivers-cards.html'
        })
        .when('/table-view',
        {
            controller: 'DriversTableViewController',
            templateUrl: '/drivers/partials/drivers-table.html'
        })
        .when('/driver-view',
        {
            controller: 'DriverEditViewController',
            templateUrl: '/drivers/partials/driver-editor.html'
        })
        .when('/driver-view/:driverId',
        {
            controller: 'DriverEditViewController',
            templateUrl: '/drivers/partials/driver-editor.html'
        })
        .when('/map-view',
        {
            controller: 'DriverTrackingController',
            templateUrl: '/drivers/partials/driver-tracking.html'
        })
        .when('/export',
        {
            controller: 'DriversExportViewController',
            templateUrl: '/drivers/partials/drivers-export.html'
        })
        .otherwise({ redirectTo: '/cards-view' });
});

/////////////////////////////////////////////////////////////
//               Controller Registration                   //
/////////////////////////////////////////////////////////////
app.controller('DriversController', function ($scope, $timeout, $filter) {
    $scope.$parent.appStatus = 'Loading';

    $scope.drivers = Driver.query(function (data) {
        $scope.filteredDrivers = $filter('SearchFilter')(data, $scope.searchFields, $scope.searchTerm);
        $scope.groups = $filter('GetGroupsForData')($scope.filteredDrivers, $scope.grouping, $scope.sorting);
        $scope.$broadcast('Data-Changed', $scope.filteredDrivers);
    });

    //$scope.$watch('drivers', function (newvalue, oldvalue, scope) {
        
    //}, true);

    $scope.filteredDrivers = [];
    //$scope.$watch('filteredDrivers', function (newvalue, oldvalue, scope) {

    //}, true);

    $scope.selectedDriver = {};

    $scope.sorting = { key: 'Forename', reverse: false };
    $scope.$watch('sorting', function (newvalue, oldvalue, scope) { }, true);

    $scope.grouping = { key: 'Status', initials: false };
    $scope.$watch('grouping', function (newvalue, oldvalue, scope) {
        scope.groups = $filter('GetGroupsForData')(scope.filteredDrivers, newvalue, scope.sorting);
    }, true);

    $scope.searchFields = ['Forename', 'Surname'];
    $scope.searchTerm = '';
    $scope.$watch('searchTerm', function (newvalue, oldvalue, scope) {
        scope.filteredDrivers = $filter('SearchFilter')(scope.drivers, scope.searchFields, newvalue);
        scope.groups = $filter('GetGroupsForData')(scope.filteredDrivers, scope.grouping, scope.sorting);
        scope.$broadcast('Data-Changed', scope.filteredDrivers);
    }, true);

    //$scope.timedUpdate = function () {
    //    $timeout(function () {
    //        var status = ['OffDuty', 'Unavailable', 'PickingUp', 'OnJob', 'Clearing', 'OnBreak', 'Available']
    //        var randomdriver = Math.floor(Math.random() * $scope.drivers.length);
    //        var randomstatus = status[Math.floor(Math.random() * 7)];
    //        $scope.drivers[randomdriver].Status = randomstatus;
    //        randomdriver = Math.floor(Math.random() * $scope.drivers.length);
    //        randomstatus = status[Math.floor(Math.random() * 7)];
    //        $scope.drivers[randomdriver].Status = randomstatus;
    //        randomdriver = Math.floor(Math.random() * $scope.drivers.length);
    //        randomstatus = status[Math.floor(Math.random() * 7)];
    //        $scope.drivers[randomdriver].Status = randomstatus;
    //        $scope.timedUpdate();
    //    }, 6000);
    //}

    //Scope Functions
    $scope.ChangeSorting = function (key, subkey, sorting) {
        if (sorting.key == key) {
            $scope.sorting.reverse = !($scope.sorting.reverse);
        } else {
            $scope.sorting.key = key;
            $scope.sorting.subkey = subkey;
        }
    };

    $scope.$on('RequestDataRefresh', function () {
        $scope.drivers = Driver.query();
    });

    $scope.$parent.appStatus = 'Ready';
});

app.controller('DriversCardsViewController', function ($scope, $q) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'CardsView';

    $scope.fetchDriverInModal = function (e) {
        $scope.$parent.selectedDriver = this.driver;
        $scope.$emit('RequestModal', { name: 'DriverDetails', url: '/drivers/partials/driver-details.html', driverid: this.driver.ID });
    };


    $scope.$parent.$parent.appStatus = 'Ready';
});

app.controller('DriversTableViewController', function ($scope, $q) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'TableView';


    $scope.$parent.$parent.appStatus = 'Ready';
});

app.controller('DriversBirdsEyeViewController', function ($scope) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'DetailsView';

    $scope.SubmitFile = function () {
        $scope.documentInformation.OwnerType = 'Driver';
        $scope.documentInformation.OwnerID = $scope.driverId;
        if ($scope.documentmode == "New") {
            Document.save({ model: $scope.documentInformation, file: $scope.documentFile }, function (data) {
                alert("Document Added");
                $scope.selectedDocuments.push(data);
                $scope.documentInformation = {};
            });
        } else {
            Document.update({ model: $scope.documentInformation, file: $scope.documentFile }, function (data) {
                alert("Document Updated");
                $scope.selectedDocuments = Document.query({ ownertype: 'Driver', ownerid: $scope.driverId });
                $scope.documentInformation = {};
            });
        }

    };

    $scope.setSelectedDocument = function (path) {
        $scope.selectedDocumentUrl = path;
    };

    $scope.deleteDocument = function (id) {
        if (confirm("Are you sure you wish to delete this document?")) {
            Document.delete({ id: id });
            $scope.selectedDocuments = $scope.selectedDocuments.where(function (value) { return (value.ID != id); });
            $scope.selectedDocumentUrl = null;
        }
    };


    $scope.editDocument = function (document) {
        $scope.documentInformation = document;
        $scope.documentmode = "Edit";
        $scope.dates.ExpiryDate = new Date(document.ExpiryDate);
    };

    $scope.newDocument = function () {
        $scope.documentInformation = {};
        $scope.documentmode = "New";
    };

    $scope.CancelFile = function () {
        $scope.documentInformation = {};
    };

    $scope.$on("fileSelected", function (event, args) {
        $scope.$apply(function () {
            $scope.documentFile = { type: args.type, file: args.file };
        });
    });

    $scope.$watch('dates', function (newvalue, oldvalue, scope) {
        scope.documentInformation.ExpiryDate = newvalue.ExpiryDate;
    }, true);

    $scope.BookingsForShiftFetch = function (shiftid) {
        $scope.bookingsDataForShift = Booking.query({ shiftid: shiftid });
    };

    $scope.NewRange = function (type) {
        var today = new Date();
        switch (type) {
            case '1wk':
                from = new Date().addPeriod(-7, 'days');
                $scope.range = { from: from, to: today, tick: [1, "day"], format: "%a" };
                break;
            case '1m':
                from = new Date().addPeriod(-1, 'months');
                $scope.range = { from: from, to: today, tick: [7, "day"], format: "%d %b" };
                break;
            case '3m':
                from = new Date().addPeriod(-3, 'months');
                $scope.range = { from: from, to: today, tick: [1, "month"], format: "%b %y" };
                break;
            case '6m':
                from = new Date().addPeriod(-6, 'months');
                $scope.range = { from: from, to: today, tick: [1, "month"], format: "%b %y" };
                break;
            case '1yr':
                from = new Date().addPeriod(-1, 'years');
                $scope.range = { from: from, to: today, tick: [1, "quarter"], format: "%b %y" };
                break;
            default:
                break;
        }
        $scope.graphDataFetch();
    };

    $scope.graphDataFetch = function () {
        $scope.rawData = {};
        $scope.rawData.averagestats = Driver.stats({
            driverid: -1,
            from: $scope.range.from.toJSON(),
            to: $scope.range.to.toJSON(),
        }, function (result) {
            $scope.graphdata[1].data = result.Result;
        });

        $scope.rawData.driverstats = Driver.stats({
            driverid: $scope.driverId,
            from: $scope.range.from.toJSON(),
            to: $scope.range.to.toJSON(),
        }, function (result) {
            $scope.graphdata[0].data = result.Result;
        });
    };

    $scope.driverId = $scope.$parent.modal.driverid;
    $scope.selectedDriver = Driver.get({ driverid: $scope.driverId }, function (driver) {
        $scope.selectedShifts = $scope.selectedDriver.$shifts(driver.ID, new Date().addPeriod(-1, "months").toJSON(), new Date().toJSON());
    });

    $scope.selectedDocuments = Document.query({ ownertype: 'Driver', ownerid: $scope.driverId });

    //Document Uploading
    $scope.documentFile = {};
    $scope.documentInformation = {};
    $scope.dates = {};


    $scope.range = {};
    $scope.graphdata = [
        {
            label: ' Driver',
            data: [],
            color: "#28AE90"
        },
        {
            label: ' Average',
            data: [],
            color: "#EE7951"
        }
    ];
    $scope.NewRange('1yr');

    $scope.stats = {
        alltime: {
            selected: Driver.stats({
                driverid: $scope.driverId,
                grouping: 'none'
            }),
            max: Driver.stats({
                driverid: -2,
                grouping: 'none'
            }),
        },
        month: {
            selected: Driver.stats({
                driverid: $scope.driverId,
                from: new Date().addPeriod(-1, 'months').toJSON(),
                to: new Date().toJSON(),
                grouping: 'none'
            }),
            max: Driver.stats({
                driverid: -2,
                from: new Date().addPeriod(-1, 'months').toJSON(),
                to: new Date().toJSON(),
                grouping: 'none'
            }),
        }
    };
    $scope.graphDataFetch();
});

app.controller('DriverEditViewController', function ($scope, $routeParams, $location) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'DriverView';

    $scope.driver = { Active: true, DriverTypeID: 0, ImageUrl: 'http://s3.amazonaws.com/37assets/svn/765-default-avatar.png' };
    $scope.Fullname = '';
    $scope.dates = {};
    $scope.image = {};
    $scope.mode = {};

    $scope.boolValues =
    [
        true,
        false
    ];

    $scope.mode = { main: "New" };
    if ($routeParams.driverId) {
        $scope.mode.main = "Edit";
        $scope.driver = Driver.get({ driverId: $routeParams.driverId }, function (driver) {
            $scope.Fullname = driver.Forename + " " + driver.Surname;
            var dates = {
                DateOfBirth: new Date(driver.DateOfBirth),
                StartDate: new Date(driver.StartDate),
                FinishDate: new Date(driver.FinishDate)
            };
            $scope.dates = dates;
        });
    } else {
        $scope.mode.main = "New";
        $scope.driver = new Driver();
    }

    $scope.$watch('dates', function (newvalue, oldvalue, scope) {
        scope.driver.DateOfBirth = newvalue.DateOfBirth;
        scope.driver.StartDate = newvalue.StartDate;
        scope.driver.FinishDate = newvalue.FinishDate;
    }, true);

    $scope.$watch('Fullname', function (newvalue, oldvalue, scope) {
        var names = newvalue.split(" ");
        scope.driver.Forename = (names.length > 0) ? names[0] : '';
        var lastNames = names.splice(1, names.length - 1);
        scope.driver.Surname = (lastNames.length > 0) ? lastNames.join(" ") : '';
    });

    $scope.Save = function () {
        if ($scope.mode.main == "New") {
            Driver.saveWithImage({
                model: $scope.driver,
                file: $scope.image
            }, function () {
                alert('Driver Added');
                $scope.$emit('RequestDataRefresh');
                $location.path('/cards-view');
            });
        } else {
            Driver.updateWithImage({
                model: $scope.driver,
                file: $scope.image
            }, function () {
                alert('Changes Saved');
                $scope.$emit('RequestDataRefresh');
                $location.path('/cards-view');
            });
        }
    };


    $scope.$on("fileSelected", function (event, args) {
        $scope.$apply(function () {
            $scope.image = { type: args.type, file: args.file };
        });
    });

    $scope.$parent.$parent.appStatus = 'Ready';
});

app.controller('DriverTrackingController', function (signalRLocationHub, $scope, $q) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'MapView';

    var styles = [
          {
              "featureType": "poi.park",
              "stylers": [
                { "visibility": "off" }
              ]
          }, {
              "featureType": "road.highway",
              "elementType": "geometry",
              "stylers": [
                { "visibility": "on" },
                { "color": "#24b090" },
                { "weight": 0.9 }
              ]
          }, {
              "featureType": "landscape.man_made",
              "elementType": "geometry",
              "stylers": [
                { "visibility": "off" }
              ]
          }, {
              "featureType": "transit.line",
              "stylers": [
                { "visibility": "off" }
              ]
          }, {
              "featureType": "road.arterial",
              "elementType": "geometry.fill",
              "stylers": [
                { "color": "#7fe5d0" },
                { "visibility": "on" }
              ]
          }, {
              "featureType": "road.arterial",
              "elementType": "geometry.stroke",
              "stylers": [
                { "color": "#d6804b" },
                { "weight": 0.3 }
              ]
          }, {
              "elementType": "labels.icon"
          }
    ];

    $scope.mapObject = {};
    $scope.mapOptions = {
        center: new google.maps.LatLng(51.4775, -0.4614),
        zoom: 13,
        //styles: styles
    };

    $scope.$on("Map-Ready", function () {
        angular.forEach($scope.$parent.drivers, function (driver) {
            driver.visible = true;
            if (driver.LastKnownPosition) {
                driver.marker = new google.maps.Marker({
                    position: new google.maps.LatLng(driver.LastKnownPosition.latitude, driver.LastKnownPosition.longitude),
                    icon: {
                        url: '/api/image?imagetype=Google&ownertype=driver&ownerid=' + driver.ID,
                        scaledSize: new google.maps.Size(50, 50)
                    },
                    title: driver.CallSign,
                    map: this.mapObject,
                });
            }
        }, $scope);

        $scope.$on("Data-Changed", function (eve, data) {
            angular.forEach($scope.$parent.drivers, function (driver) {
                driver.visible = false;
                if (driver.marker) {
                    driver.marker.setMap(null);
                }
            }, $scope);

            angular.forEach($scope.$parent.filteredDrivers, function (driver) {
                driver.visible = true;
                if (driver.LastKnownPosition) {
                    if (driver.marker) {
                        driver.marker.setMap(this.mapObject);
                    } else {
                        driver.marker = new google.maps.Marker({
                            position: new google.maps.LatLng(driver.LastKnownPosition.latitude, driver.LastKnownPosition.longitude),
                            icon: {
                                url: '/api/image?imagetype=Google&ownertype=driver&ownerid=' + driver.ID,
                                scaledSize: new google.maps.Size(50, 50)
                            },
                            title: driver.CallSign,
                            map: this.mapObject,
                        });
                    }
                }
            }, $scope);
        });
    });

    $scope.$on('NewLocation', function (evt, item) {
            $scope.$parent.filteredDrivers.forEach(function (driver) {
                if (driver.ID == item.DriverID) {
                    if (driver.marker) {
                        driver.marker.setPosition(new google.maps.LatLng(item.Point.latitude, item.Point.longitude));
                        if (driver.visible) {
                            if (!driver.marker.map)
                                driver.marker.setMap($scope.mapObject);
                        }
                    }
                    else {
                        driver.marker = new google.maps.Marker({
                            position: new google.maps.LatLng(item.Point.latitude, item.Point.longitude),
                            icon: {
                                url: '/api/image?imagetype=Google&ownertype=driver&ownerid=' + driver.ID,
                                scaledSize: new google.maps.Size(50, 50)
                            },
                            title: driver.CallSign,
                            map: $scope.mapObject,

                        });
                        if (driver.visible) {
                            if (!driver.marker.map)
                                driver.marker.setMap($scope.mapObject);
                        }
                    }
                    if (driver.ID == $scope.TrackedDriver) {
                        $scope.mapObject.setCenter(driver.marker.getPosition());
                        $scope.mapObject.setZoom(16);
                    }
                }
            });
        });

  
    $scope.$on('$destroy', function (evt) {
        signalRLocationHub.stop(function () { });
    });

    $scope.UpdateMap = function (driver) {
        if (!driver.visible) {
            if (driver.marker)
            driver.marker.setMap(null);
        } else {
            if (driver.marker)
            driver.marker.setMap($scope.mapObject);
        }
    };

    $scope.TrackedDriver = null;
    $scope.ToggleTracking = function (driver) {
        if ($scope.TrackedDriver != driver.ID) {
            $scope.TrackedDriver = driver.ID;
            if (driver.marker) {
                $scope.mapObject.setCenter(driver.marker.getPosition());
                $scope.mapObject.setZoom(16);
            }
        } else {
            $scope.TrackedDriver = null;
            $scope.mapObject.setCenter(new google.maps.LatLng(51.4775, -0.4614));
            $scope.mapObject.setZoom(13);
        }
    };

    $scope.Route = null;
    $scope.DrawnShift = null;
    $scope.ToggleShift = function (shift) {
        if ($scope.DrawnShift != shift.ID) {
            if ($scope.TrackedDriver > 0)
                $scope.TrackedDriver *= -1;
            $scope.DrawnShift = shift.ID;
            if (shift.EncodedRoute) {
                var decodedPath = google.maps.geometry.encoding.decodePath(shift.EncodedRoute);
                if (!$scope.Route) {
                    $scope.Route = new google.maps.Polyline({
                        path: decodedPath,
                        strokeWeight: 3.0,
                        fillColor: '#D21D1D',
                        fillOpacity: 0.6,
                        strokeColor: '#D21D1D',
                        strokeOpacity: 0.7,
                        zIndex: 100000000,
                        map: $scope.mapObject
                    });
                } else {
                    $scope.Route.setPath(decodedPath);
                    $scope.Route.setMap($scope.mapObject);
                }
                var bounds = new google.maps.LatLngBounds();
                var points = $scope.Route.getPath().getArray();
                for (var n = 0; n < points.length ; n++) {
                    bounds.extend(points[n]);
                }
                $scope.mapObject.fitBounds(bounds);
            }
        } else {
            $scope.DrawnShift = null;
            if ($scope.Route) {
                $scope.Route.setMap(null);
            }
            $scope.TrackedDriver *= -1;
            $scope.mapObject.setCenter(new google.maps.LatLng(51.4775, -0.4614));
            $scope.mapObject.setZoom(13);
        }
    };

    $scope.selectedDriver = null;
    $scope.ShowShifts = function (driver) {
        angular.forEach($scope.$parent.filteredDrivers, function (d) {
            d.visible = false;
            if (d.marker)
                d.marker.setMap(null);
        });
        driver.visible = true;
        if (driver.marker)
            driver.marker.setMap($scope.mapObject);
        $scope.selectedDriver = driver;
        $scope.selectedShifts = Shift.query({
            driverid: driver.ID
        }, function (data) {
        });
    };

    $scope.Back = function () {
        $scope.mapObject.setCenter(new google.maps.LatLng(51.4775, -0.4614));
        $scope.mapObject.setZoom(13);
        $scope.selectedDriver = null;
        $scope.DrawnShift = null;
        if ($scope.Route)
            $scope.Route.setMap(null);
        $scope.Route = null;
        $scope.TrackedDriver = null;
        angular.forEach($scope.$parent.filteredDrivers, function (d) {
            d.visible = true;
            if (d.marker)
                d.marker.setMap($scope.mapObject);
        });
    };

    signalRLocationHub.initalise(function () { });

    $scope.$parent.$parent.appStatus = 'Ready';
});

app.controller('DriversExportViewController', function ($scope, $q) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'TableView';

    $scope.myData = $scope.$parent.filteredDrivers;
    $scope.$on('Data-Change', function () {
        $scope.myData = $scope.$parent.filteredDrivers;
    });

    $scope.message = '';

    $scope.myCallback = function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
        $('td:eq(2)', nRow).bind('click', function () {
            $scope.$apply(function () {
                $scope.someClickHandler(aData);
            });
        });
        return nRow;
    };

    $scope.someClickHandler = function (info) {
        $scope.message = 'clicked: ' + info.price;
    };

    $scope.columnDefs = [
        { "mDataProp": "CallSign", "aTargets": [0], "sWidth": "10%" },
        { "mDataProp": "Forename", "aTargets": [1], "sWidth": "20%" },
        { "mDataProp": "Surname", "aTargets": [2], "sWidth": "20%" },
        { "mDataProp": "Mobile", "aTargets": [3], "sWidth": "20%" },
        { "mDataProp": "DriverType.Name", "aTargets": [4], "sWidth": "20%" },
        { "mDataProp": "Status", "aTargets": [5], "sWidth": "10%" }
    ];

    $scope.overrideOptions = {
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": false,
        "bInfo": false,
        "bDestroy": true
    };



    $scope.$parent.$parent.appStatus = 'Ready';
});

/////////////////////////////////////////////////////////////
//                 Controller Functions                    //
/////////////////////////////////////////////////////////////