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
            controller: 'VehiclesCardsViewController',
            templateUrl: '/vehicles/partials/vehicles-cards.html'
        })
        .when('/table-view',
        {
            controller: 'VehiclesTableViewController',
            templateUrl: '/vehicles/partials/vehicles-table.html'
        })
        .when('/vehicle-view',
        {
            controller: 'VehicleEditViewController',
            templateUrl: '/vehicles/partials/vehicle-editor.html'
        })
        .when('/vehicle-view/:vehicleId',
        {
            controller: 'VehicleEditViewController',
            templateUrl: '/vehicles/partials/vehicle-editor.html'
        })
        .otherwise({ redirectTo: '/cards-view' });
});

app.controller('VehiclesController', function ($scope, $timeout, $filter) {
    $scope.$parent.appStatus = 'Loading';

    $scope.vehicles = Vehicle.query(function () {
    });
    $scope.$watch('vehicles', function (newvalue, oldvalue, scope) {
            scope.filteredVehicles = $filter('SearchFilter')(newvalue, scope.searchFields, scope.searchTerm);
    }, true);

    $scope.filteredVehicles = [];
    $scope.$watch('filteredVehicles', function (newvalue, oldvalue, scope) {
        scope.groups = $filter('GetGroupsForData')(newvalue, scope.grouping, scope.sorting);
        scope.$broadcast('Data-Changed', newvalue);
    }, true);

    $scope.selectedVehicle = {};

    $scope.sorting = { key: 'Make', reverse: false };
    $scope.$watch('sorting', function (newvalue, oldvalue, scope) { }, true);

    $scope.grouping = { key: 'VehicleType.Name', initials: false };
    $scope.$watch('grouping', function (newvalue, oldvalue, scope) {
        scope.groups = $filter('GetGroupsForData')(scope.filteredVehicles, newvalue, scope.sorting);
    }, true);

    $scope.searchFields = ['Make', 'Model', 'Colour', 'Registration'];
    $scope.searchTerm = '';
    $scope.$watch('searchTerm', function (newvalue, oldvalue, scope) {
        scope.filteredVehicles = $filter('SearchFilter')(scope.vehicles, scope.searchFields, newvalue);
    }, true);


    $scope.ChangeSorting = function (key, subkey, sorting) {
        if (sorting.key == key) {
            $scope.sorting.reverse = !($scope.sorting.reverse);
        } else {
            $scope.sorting.key = key;
            $scope.sorting.subkey = subkey;
        }
    };

    $scope.$on('RequestDataRefresh', function () {
        $scope.vehicles = Vehicle.query();
    });

    $scope.$parent.appStatus = 'Ready';
});

app.controller('VehiclesCardsViewController', function ($scope, $q) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'CardsView';

    $scope.fetchVehicleInModal = function (e) {
        $scope.$parent.selectedVehicle = this.vehicle;
        $scope.$emit('RequestModal', { name: 'VehicleDetails', url: '/vehicles/partials/vehicle-details.html', vehicleid: this.vehicle.ID });
    };


    $scope.$parent.$parent.appStatus = 'Ready';
});

app.controller('VehiclesTableViewController', function ($scope, $q) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'TableView';


    $scope.$parent.$parent.appStatus = 'Ready';
});

app.controller('VehiclesBirdsEyeViewController', function ($scope) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'DetailsView';

    $scope.SubmitFile = function () {
        $scope.documentInformation.OwnerType = 'Vehicle';
        $scope.documentInformation.OwnerID = $scope.vehicleId;
        if ($scope.documentmode == "New") {
            Document.save({ model: $scope.documentInformation, file: $scope.documentFile }, function (data) {
                alert("Document Added");
                $scope.selectedDocuments.push(data);
                $scope.documentInformation = {};
            });
        } else {
            Document.update({ model: $scope.documentInformation, file: $scope.documentFile }, function (data) {
                alert("Document Updated");
                $scope.selectedDocuments = Document.query({ ownertype: 'Vehicle', ownerid: $scope.vehicleId });
                $scope.documentInformation = {};
            });
        }

    };

    $scope.setSelectedDocument = function (path) {
        $scope.selectedDocumentUrl = path;
    }

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
        $scope.rawData.averagestats = Vehicle.stats({
            vehicleid: -1,
            from: $scope.range.from.toJSON(),
            to: $scope.range.to.toJSON(),
        }, function (result) {
            $scope.graphdata[1].data = result.Result;
        });

        $scope.rawData.vehiclestats = Driver.stats({
            vehicleid: $scope.driverId,
            from: $scope.range.from.toJSON(),
            to: $scope.range.to.toJSON(),
        }, function (result) {
            $scope.graphdata[0].data = result.Result;
        });
    };

    $scope.vehicleId = $scope.$parent.modal.vehicleid;
    $scope.selectedVehicle = Driver.get({ vehicleid: $scope.vehicleId }, function (vehicle) {
        $scope.selectedShifts = $scope.selectedVehicle.$shifts(vehicle.ID, new Date().addPeriod(-1, "months").toJSON(), new Date().toJSON());
    });

    $scope.selectedDocuments = Document.query({ ownertype: 'Vehicle', ownerid: $scope.vehicleId });

    //Document Uploading
    $scope.documentFile = {};
    $scope.documentInformation = {};
    $scope.dates = {};


    $scope.range = {};
    $scope.graphdata = [
        {
            label: 'This Vehicle',
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
            selected: Vehicle.stats({
                driverid: $scope.vehicleId,
                grouping: 'none'
            }),
            max: Vehicle.stats({
                driverid: -2,
                grouping: 'none'
            }),
        },
        month: {
            selected: Vehicle.stats({
                driverid: $scope.vehicleId,
                from: new Date().addPeriod(-1, 'months').toJSON(),
                to: new Date().toJSON(),
                grouping: 'none'
            }),
            max: Vehicle.stats({
                vehicleid: -2,
                from: new Date().addPeriod(-1, 'months').toJSON(),
                to: new Date().toJSON(),
                grouping: 'none'
            }),
        }
    };
    $scope.graphDataFetch();
});

app.controller('VehicleEditViewController', function ($scope, $routeParams, $location) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'VehicleView';

    $scope.vehicle = { Active: true, DriverTypeID: 0, ImageUrl: '' };
    
    $scope.dates = {};
    $scope.image = {};
    $scope.mode = {};

    $scope.boolValues =
    [
        true,
        false
    ];

    $scope.mode = { main: "New" };
    if ($routeParams.vehicleId) {
        $scope.mode.main = "Edit";
        $scope.vehicle = Vehicle.get({ vehicleId: $routeParams.vehicleId }, function (vehicle) {
            
            var dates = {
                StartDate: new Date(vehicle.StartDate),
                FinishDate: new Date(vehicle.FinishDate)
            };
            $scope.dates = dates;
        });
    } else {
        $scope.mode.main = "New";
        $scope.vehicle = new Vehicle();
    }

    $scope.$watch('dates', function (newvalue, oldvalue, scope) {
        scope.vehicle.StartDate = newvalue.StartDate;
        scope.vehicle.FinishDate = newvalue.FinishDate;
    }, true);


    $scope.Save = function () {
        if ($scope.mode.main == "New") {
            Vehicle.saveWithImage({
                model: $scope.driver,
                file: $scope.image
            }, function () {
                alert('Vehicle Added');
                $scope.$emit('RequestDataRefresh');
                $location.path('/cards-view');
            });
        } else {
            Vehicle.updateWithImage({
                model: $scope.vehicle,
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