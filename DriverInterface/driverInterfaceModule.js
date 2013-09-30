/// <reference path='../includes/js/angular.js' />

var app = angular.module('DriverInterface', ['ngResource', 'ngRoute', 'ngSanitize']);

app.value('signalR', $);

app.config(['$routeProvider', '$httpProvider', function (Routing, Http) {
    Routing
        .when('/menu',
        {
            controller: 'MenuViewController',
            templateUrl: '/driverinterface/partials/menu.html',
            backText: false
        })
        .when('/menu-shifts',
        {
            controller: 'ShiftMenuController',
            templateUrl: '/driverinterface/partials/menu-shifts.html',
            backText: 'Back'
        })
        .when('/shift-start',
        {
            controller: 'StartShiftController',
            templateUrl: '/driverinterface/partials/shift-start.html',
            backText: 'Back'
        })
        .when('/booking-offer',
        {
            controller: 'BookingOfferViewController',
            templateUrl: '/driverinterface/partials/offer.html',
            backText: false
        })
        .when('/login',
        {
            controller: 'LoginViewController',
            templateUrl: '/driverinterface/partials/login.html',
            backText: false
        })
        .otherwise({ redirectTo: '/login' });
}]);

app.run(function () {

});

app.controller('DIController', ['$rootScope', '$scope', '$location', '$timeout', 'HubService', function (root, scope, Location, Delay, Hub) {
    root.current = {
        offers: [],
        booking: null,
        user: null,
        driver: null,
        shift: null,
        location: null
    };

    /// Debugging 
    if (DEBUG = true) {
        scope.TestPush = function () {
            root.$broadcast('BookingOffered', {
                ID: 20,
                DriverID: 14,
                BookingID: 2001,
                Booking: {
                    From: '29 Diamedes Avenue, Stanwell, TW19 7JE',
                    To: '88 Peatey Court, High Wycombe, HP13 7AZ',
                    PassengerName: 'David',
                    AccountID: 12,
                    Fare: 32.00,
                    PaymentMethod: 'Account',
                    BookedDateTime: '2013-09-27T15:00:22'
                }
            })
            root.$broadcast('BookingOffered', {
                ID: 21,
                DriverID: 14,
                BookingID: 2001,
                Booking: {
                    From: '88 Peatey Court, High Wycombe, HP13 7AZ',
                    To: 'High Wycombe Train Station',
                    PassengerName: 'David',
                    AccountID: 12,
                    Fare: 6.00,
                    PaymentMethod: 'Cash',
                    BookedDateTime: '2013-09-27T15:30:22'
                }
            })
            root.$broadcast('BookingOffered', {
                ID: 22,
                DriverID: 14,
                BookingID: 2001,
                Booking: {
                    From: 'Heathrow Terminal 4',
                    To: 'Hotel IBIS Bath Road',
                    PassengerName: 'David',
                    AccountID: 12,
                    Fare: 15.00,
                    PaymentMethod: 'Cash',
                    BookedDateTime: '2013-09-27T15:00:22'
                }
            })
        }
    }

    /// Route Change Events

    scope.$on('$routeChangeStart', function (event, newRoute) {
        if (!root.user && newRoute && newRoute.$$route && newRoute.$$route.originalPath != '/login') Location.path('/login');
    });

    scope.$on('$routeChangeSuccess', function (event, newRoute) {
        if (newRoute && newRoute.$$route && newRoute.$$route.backText)
            scope.backText = newRoute.$$route.backText;
        else
            scope.backText = '';
    });

    /// Hub
    var LocationPromise = null;
    var SendLocation = function () {
            navigator.geolocation.getCurrentPosition(function (location) {
                root.current.location = location;
                Hub.SendUpdate(location)
                LocationPromise = Delay(SendLocation, 8000);
            });        
    };

    scope.$on('ShiftStarted', function (event) {
        Hub.initalise(function () { SendLocation(); });
    });
   
    scope.$on('BookingOffered', function (event, offer) {
        if (root.current.driver.Status == 'Available') {
            scope.$apply(function(){
                root.current.offers.push(offer);
                if (root.include != 'partials/offer.html') {
                    $('#content').css('-webkit-filter', 'blur(2px)')
                    root.include = 'partials/offer.html';
                }
            })
        } else if (root.current.driver.Status == 'Clearing') {

        } else {
            Hub.rejectBooking(offer.ID, 'UNAVAILABLE');
        }
    });

    root.$watch('include', function (newValue) {
        if ((!newValue) || (newValue == ''))
        {
            $('#content').css('-webkit-filter', 'none');
        }
    })

    /// Menubar Management

    scope.BackClicked = function () {
        root.$broadcast('BackClicked');
    };

    scope.statusColors = {
        'label-danger': (root.current.driver && root.current.driver.Status == 'OffDuty') ? true : false,
        'label-warning': (root.current.driver && root.current.driver.Status == 'Unavailable') ? true : false,
        'label-info': (root.current.driver && (root.current.driver.Status == 'Clearing' || root.current.driver.Status == 'OnBreak')) ? true : false,
        'label-success': (root.current.driver && root.current.driver.Status == 'Available') ? true : false
    };

    root.$watch('current.driver', function (newValue, oldValue) {
        if (newValue && newValue.$resolved) {
            if (!oldValue || newValue.Status != oldValue.Status) {
                scope.statusColors = {
                    'label-danger': (root.current.driver && root.current.driver.Status == 'OffDuty') ? true : false,
                    'label-warning': (root.current.driver && root.current.driver.Status == 'Unavailable') ? true : false,
                    'label-info': (root.current.driver && (root.current.driver.Status == 'Clearing' || root.current.driver.Status == 'OnBreak')) ? true : false,
                    'label-success': (root.current.driver && root.current.driver.Status == 'Available') ? true : false
                };
            }
        }
    }, true);

    scope.Logout = function () {
        if (LocationPromise) Delay.cancel(LocationPromise);
        Hub.stop(function () { });
        root.current = {
            offers: [],
            booking: null,
            user: null,
            driver: null,
            shift: null,
            location: null
        };
        Location.path('/login');
    }
}]);

app.controller('LoginViewController', ['$rootScope', '$scope', '$location', 'User', 'Driver', 'DriverShift', function (root, scope, Location, User, Driver, DriverShift) {
    scope.credentials = {};
    scope.feedback = '';

    scope.SubmitLogin = function () {
        User.AttemptLogin(
            scope.credentials,
            function (user) {
                //Success
                root.user = user;
                if (user.ObjectType == "Driver" && user.ObjectID)
                    root.current.driver = Driver.get(
                        { driverid: user.ObjectID },
                        function (data) {
                            if (data.CurrentShiftID)
                            {
                                root.current.shift = DriverShift.get({ id: data.CurrentShiftID }, function () { root.$broadcast('ShiftStarted'); });
                            }
                            Location.path('/menu');
                        }
                    );
                else {
                    root.current.driver = null;
                    scope.feedback = '<div class="alert alert-warning">Login Failed: User is not a Driver</div>'
                    scope.credentials = {};
                }
                
            },
            function (data) {
                //Fail
                scope.feedback = '<div class="alert alert-warning">Login Failed: ' + data.Reason + '</div>'
                scope.credentials = {};
            }
        );
    };

    scope.ForgotDetails = function () {
        alert('Not Implemented Yet');
    }
}]);

app.controller('MenuViewController', ['$rootScope', '$scope', '$location', function (root, scope, Location) {
    scope.$on('BackClicked', function () {
        Location.path('/menu');
    });

    scope.SubMenu = function (sub) {
        Location.path('/menu-' + sub);
    };
}]);

app.controller('ShiftMenuController', ['$rootScope', '$scope', '$location', 'DriverShift', function (root, scope, Location, DriverShift) {
    scope.$on('BackClicked', function () {
        Location.path('/menu');
    });

    scope.StartShift = function () {
        Location.path('/shift-start');
    };

    scope.EndShift = function () {
        if (root.current.booking)
        {
            alert('Complete current booking before ending shift.');
        }
        else
        {
            try {
                var miles = Number(prompt('Please enter finishing mileage.'));
            }
            catch (error) {
                var miles = null;
            }
            root.current.shift.MileageEnd = miles;
            root.current.shift.$end(
                function () {
                    root.current.driver.Status = 'OffDuty'
                    root.current.shift = null;
                    root.$broadcast("ShiftStarted");
                }
            );
        }
    };
}]);

app.controller('StartShiftController', ['$rootScope', '$scope', '$location', 'DriverShift', 'Vehicle', function (root, scope, Location, DriverShift, Vehicle) {
    scope.$on('BackClicked', function () {
        Location.path('/menu-shift');
    });

    scope.vehicles = Vehicle.query();

    scope.shift = new DriverShift();
    scope.shift.DriverID = root.current.driver.ID;

    scope.Submit = function () {
        scope.shift.$start(
            function (data) {
                root.current.driver.Status = 'Available';
                root.current.shift = data;
                root.$broadcast("ShiftStarted");
                Location.path('/menu-shifts');
            }
        );
    };

    scope.Cancel = function () {
        Location.path('/menu-shifts');
    }
}]);

app.controller('BookingOfferController', ['$rootScope', '$scope', '$timeout', 'HubService', function (root, scope, Delay, Hub) {
    scope.time = 30;
    scope.currentdelay = null;

    scope.offer = root.current.offers[0];

    scope.FlashTime = function () {
        $('#acceptBtn').html((scope.time--).toString());
        if (scope.time > 0) {
            scope.currentdelay = Delay(scope.ResetFlash, 500);
        } else {
            scope.currentdelay = Delay(scope.Timeout, 500);
        }
    };

    scope.ResetFlash = function () {
        $('#acceptBtn').html('Accept');
        scope.currentdelay = Delay(scope.FlashTime, 500);
    };

    Delay(scope.FlashTime, 1000);

    scope.Accept = function () {
        Delay.cancel(scope.currentdelay);
        Hub.acceptBooking(scope.offer.ID);
        angular.forEach(root.current.offers, function (offer) {
            if (offer.ID != scope.offer.ID)
                Hub.rejectBooking(offer.ID, 'NO LONGER AVAILABLE');
        });
        root.current.booking = Booking.get({ id: scope.offer.BookingID });
        root.current.offers = [];
    };

    scope.Reject = function () {
        Delay.cancel(scope.currentdelay);
        var Reason = prompt('Reject Reason:')
        Hub.rejectBooking(scope.offer.ID, Reason);
        root.current.offers.splice(0, 1);
        if (root.current.offers.length > 0) {
            scope.time = 30;
            scope.currentdelay = null;
            scope.offer = root.current.offers[0];
            Delay(scope.FlashTime, 1000);
        } else {
            root.include = '';
        }
    };

    scope.Timeout = function () {
        var Reason = 'TIMEOUT';
        Hub.rejectBooking(scope.offer.ID, Reason);
        root.current.offers.splice(0, 1);
        if (root.current.offers.length > 0) {
            scope.time = 30;
            scope.currentdelay = null;
            scope.offer = root.current.offers[0];
            scope.FlashTime();
        } else {
            root.include = '';
        }
    };
}]);

app.factory('User', [function () {
    var result = {};
    result.AttemptLogin = function (credentials, success, fail) {
        if (credentials.email == 'test' && credentials.pwd == 'test') {
            success({
                ID: 1,
                CompanyID: 1,
                Name: 'David',
                Email: 'davidscfc@gmail.com',
                ObjectType: 'Driver',
                ObjectID: '14'
            })
        } else {
            fail({
                Reason: 'Invalid Details'
            })
        }
        
    }
    return result;
}]);

app.factory('DriverShift', ['$resource', function (Resource) {
    var result = Resource('/api/drivershift/:action', {}, {
        get: {
            method: 'GET',
            isArray: false,
            params: {
                action: 'GetByID'
            }
        },
        start: {
            method: 'POST',
            params: {
                action: 'StartShift'
            }
        },
        end: {
            method: 'PUT',
            params: {
                action: 'EndShift'
            }
        },
        sendUpdate: {
            method: 'POST',
            params: {
                action: 'PostPoint'
            }
        }
    })
    return result;
}]);

app.factory('Driver', ['$resource', function (Resource) {
    var result = Resource('/api/driver/:action', {}, {
        get: {
            method: 'GET',
            params: {
                action: 'GetByID'
            }
        }
    })
    return result;
}]);

app.factory('Vehicle', ['$resource', function (Resource) {
    var result = Resource('/api/vehicle/:action', {}, {
        get: {
            method: 'GET',
            params: {
                action: 'GetByID'
            }
        }
    })
    return result;
}]);

app.factory('APIBackup', ['$resource', function (Resource) {
    var result = Resource('/api/:controller/:action', {}, {
        sendUpdate: {
            method: 'POST',
            params: {
                controller: 'drivershift',
                action: 'PostPoint'
            }
        }
    })
    return result;
}]);

//app.factory('DispatchService', ['signalR', '$rootScope', function (signalR, root) {
//    var service = {
//        hub: null,
//        running: false,
//        initalise: function (callback) {
//            this.initialiseCallback = callback;
//            this.hub = signalR.connection.dispatchHub;
//            this.hub.client.offerBooking = function (driverid, booking) {
//                if (driverid == root.current.driver.ID)
//                    root.$broadcast('BookingOffered', booking);
//            };
//            signalR.connection.hub.disconnected(function () {
//                service.running = false;
//            });
//            signalR.connection.hub.reconnected(function () {
//                service.running = true;
//            });
//            signalR.connection.hub.start(this.initialiseCompletion);
//        },
//        initialiseCallback: function () { },
//        initialiseCompletion: function () {
//            service.running = true;
//            service.initialiseCallback();
//        },
//        stop: function (callback) {
//            this.stopCallback = callback;
//            signalR.connection.hub.stop();
//            service.stopCompletion();
//        },
//        stopCallback: function () { },
//        stopCompletion: function () {
//            service.running = false;
//            service.stopCallback();
//        },
//        acceptBooking: function (offerId) {
//            this.hub.server.acceptBookingOffer(offerId)
//        },
//        rejectBooking: function (offerId, reason) {
//            this.hub.server.rejectBookingOffer(offerId, reason)
//        }
//    }
//    return service;
//}]);

app.factory('HubService', ['signalR', '$rootScope', function (signalR, root) {
    var service = {
        hub: null,
        running: false,
        initalise: function (callback) {
            this.initialiseCallback = callback;
            this.hub = signalR.connection.driverHub;
            this.hub.client.offerBooking = function (driverid, booking) {
                if (driverid == root.current.driver.ID)
                    root.$broadcast('BookingOffered', booking);
            };
            signalR.connection.hub.disconnected(function () {
                service.running = false;
            });
            signalR.connection.hub.reconnected(function () {
                service.running = true;
            });
            signalR.connection.hub.start(this.initialiseCompletion);
        },
        initialiseCallback: function () { },
        initialiseCompletion: function () {
            service.running = true;
            service.initialiseCallback();
        },
        stop: function (callback) {
            this.stopCallback = callback;
            signalR.connection.hub.stop(this.stopCompletion());
        },
        stopCallback: function () { },
        stopCompletion: function () {
            service.running = false;
            service.stopCallback();
        },
        acceptBooking: function (offerId) {
            this.hub.server.acceptBookingOffer(offerId)
        },
        rejectBooking: function (offerId, reason) {
            this.hub.server.rejectBookingOffer(offerId, reason)
        },
        SendUpdate: function (location) {
            service.hub.server.sendUpdate(root.current.shift.ID, location.coords.latitude, location.coords.longitude, location.coords.accuracy, location.coords.speed, location.coords.heading);
        }
    }
    return service;
}]);
