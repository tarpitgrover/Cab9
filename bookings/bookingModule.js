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


