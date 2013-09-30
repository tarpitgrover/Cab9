var resourceModule = angular.module('Cab9Resources', ['ngResource']);

//Please set apiEndPoint for your app if xdomain
//Example:
//app.value('cab9-apiEndPoint', VALUE_HERE);

resourceModule.factory('Booking', ['$resource', 'cab9-apiEndPoint', function (ngResource, apiEndPoint) {
    var result = ngResource(apiEndPoint + '/api/booking/:action', {}, {
        update: {
            method: 'PUT'
        },
        getDriverOrder: {
            method: 'GET',
            isArray: true,
            params: {
                action: 'DriverOrderForBookingID'
            }
        },
        get: {
            method: 'GET',
            isArray: false,
            params: {
                action: 'GetByID'
            }
        }
    });

    //result.prototype.getRecommendedDrivers = function () {

    //}

    return result;
}]);

resourceModule.factory('Client', ['$resource', 'cab9-apiEndPoint', 'Booking', function (ngResource, apiEndPoint, Booking) {
    var result = ngResource(apiEndPoint + '/api/client/:action', {}, {
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
        },
        get: {
            method: 'GET',
            isArray: false,
            params: {
            action: 'GetByID'
        }
    }  
    });

    result.prototype.getBookings = function (from, to) {
        return Booking.query({ clientid: this.id, from: from, to: to });
    }

    return result;
}]);

resourceModule.factory('ClientType', ['$resource', 'cab9-apiEndPoint', function (ngResource, apiEndPoint) {
    var result = ngResource(apiEndPoint + '/api/clienttype/:action', {}, {
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
    return result;
}]);

resourceModule.factory('Company', ['$resource', 'cab9-apiEndPoint', function (ngResource, apiEndPoint) {
    var result = ngResource(apiEndPoint + '/api/company/:action', {}, {
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
    return result;
}]);

resourceModule.factory('Document', ['$resource', 'cab9-apiEndPoint', function (ngResource, apiEndPoint) {
    var result = ngResource(apiEndPoint + '/api/document/:action', {}, {
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
        },
        get: {
            method: 'GET',
            isArray: false,
            params: {
                action: 'GetByID'
            }
        }
    });

    return result;
}]);

resourceModule.factory('Driver', ['$resource', 'cab9-apiEndPoint', 'Shift', 'Note', 'Document', function (ngResource, apiEndPoint, Shift, Note, Document) {
    var result = ngResource(apiEndPoint + '/api/driver/:action', {}, {
        get: {
            method: 'GET',
            params: {
                action: 'GetByID'
            }
        },
        update: {
            method: 'PUT'
        },
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
        stats: {
            method: 'GET',
            params: {
                action: 'stats',
                timeformat: 'javascript'
            }
        }
    });

    result.prototype.getShifts = function (from, to) {
        return Shift.query({ driverid: this.ID, from: from, to: to });
    }

    result.prototype.getNotes = function () {
        return Note.query({ ownerType: 'Driver', ownerId: this.id });
    }

    result.prototype.getDocuments = function () {
        return Document.query({ ownerType: 'Driver', ownerId: this.id });
    }

    return result;
}]);

resourceModule.factory('Note', ['$resource', 'cab9-apiEndPoint', function (ngResource, apiEndPoint) {
    var result = ngResource(apiEndPoint + '/api/note/:action', {}, {
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
    return result;
}]);

resourceModule.factory('PricingFixed', ['$resource', 'cab9-apiEndPoint', function (ngResource, apiEndPoint) {
    var result = ngResource(apiEndPoint + '/api/pricingfixed/:action', {}, {
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
    return result;
}]);

resourceModule.factory('PricingModel', ['$resource', 'cab9-apiEndPoint', function (ngResource, apiEndPoint) {
    var result = ngResource(apiEndPoint + '/api/pricingmodel/:action', {}, {
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
    return result;
}]);

resourceModule.factory('PricingZone', ['$resource', 'cab9-apiEndPoint', function (ngResource, apiEndPoint) {
    var result = ngResource(apiEndPoint + '/api/pricingzone/:action', {}, {
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
    return result;
}]);

resourceModule.factory('Shift', ['$resource', 'cab9-apiEndPoint', 'Booking', function (ngResource, apiEndPoint, Booking) {
    var result = ngResource(apiEndPoint + '/api/drivershift/:action', {}, {
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

    result.prototype.getBookings = function () {
        return Booking.query({ shiftid: this.ID });
    }

    return result;
}]);

resourceModule.factory('Staff', ['$resource', 'cab9-apiEndPoint', function (ngResource, apiEndPoint) {
    var result = ngResource(apiEndPoint + '/api/staff/:action', {}, {
        update: {
            method: 'PUT'
        }
    });
    return result;
}]);


