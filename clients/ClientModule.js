/////////////////////////////////////////////////////////////
//                   Route Registration                    //
/////////////////////////////////////////////////////////////
app.config(function ($routeProvider,$httpProvider) {
	$httpProvider.defaults.useXDomain = true;
	delete $httpProvider.defaults.headers.common['X-Requested-With'];
	$httpProvider.interceptors.push('httpAuthInterceptor');

    
    $routeProvider
        .when('/cards-view',
        {
            controller: 'ClientsCardsViewController',
            templateUrl: '/clients/partials/clients-cards.html'
        })
        .when('/table-view',
        {
            controller: 'ClientsTableViewController',
            templateUrl: '/clients/partials/clients-table.html'
        })
        .when('/client-view',
        {
	        controller: 'ClientEditViewController',
            templateUrl: '/clients/partials/client-editor.html'
        })
        .when('/client-view/:clientId',
        {
            controller: 'ClientEditViewController',
            templateUrl: '/clients/partials/client-editor.html'
        })
        .otherwise({ redirectTo: '/cards-view' });
});

/////////////////////////////////////////////////////////////
//               Controller Registration                   //
/////////////////////////////////////////////////////////////
app.controller('ClientsController', function ($scope, $timeout, $filter) {
    $scope.$parent.appStatus = 'Loading';

    //Data Fetching
    $scope.Clients = Client.query(function () {
    });

    //Data Processing
    $scope.$watch('Clients', function (newvalue, oldvalue, scope) {
        scope.filteredClients = $filter('SearchFilter')(newvalue, scope.searchFields, scope.searchTerm);
    }, true);
    
    $scope.filteredClients = [];
    $scope.$watch('filteredClients', function (newvalue, oldvalue, scope) {
        scope.groups = $filter('GetGroupsForData')(newvalue, scope.grouping, scope.sorting);
        scope.$broadcast('Data-Changed', newvalue);
    }, true);
   

    //Data Sorting / Grouping / Searching
    $scope.sorting = { key: 'Name', reverse: false };
    $scope.$watch('sorting', function (newvalue, oldvalue, scope) {}, true);
    
    $scope.grouping = { key: 'ClientType.Name', initials: false };
     $scope.$watch('grouping', function (newvalue, oldvalue, scope) {
        scope.groups = $filter('GetGroupsForData')(scope.filteredClients, newvalue, scope.sorting);
    }, true);
    
    $scope.searchFields = ['Name'];
    $scope.searchTerm = '';
    $scope.$watch('searchTerm', function (newvalue, oldvalue, scope) {
        scope.filteredClients = $filter('SearchFilter')(scope.Clients, scope.searchFields, newvalue);
    }, true);

    $scope.ChangeSorting = function (key, subkey, sorting) {
        if (sorting.key == key) {
            $scope.sorting.reverse = !($scope.sorting.reverse);
        } else {
            $scope.sorting.key = key;
            $scope.sorting.subkey = subkey;
        }
    };
    

    $scope.selectedClient = {};

    $scope.$on('RequestDataRefresh', function() {
        $scope.Clients = Client.query();
    });

    $scope.$parent.appStatus = 'Ready';
});

app.controller('ClientsCardsViewController', function ($scope, $q) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'CardsView';

    $scope.FetchSelectedEntityInModal = function (e) {
        $scope.$parent.selectedClient = this.client;
        $scope.$emit('RequestModal', { name: 'ClientDetails', url: '/clients/partials/client-details.html', clientid: this.client.ID });
    };

    $scope.$parent.$parent.appStatus = 'Ready';
});

app.controller('ClientsTableViewController', function ($scope, $q) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'TableView';


    $scope.$parent.$parent.appStatus = 'Ready';
});

app.controller('ClientsBirdsEyeViewController', function ($scope) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'DetailsView';

   
    //Document Tab
    $scope.SubmitFile = function () {
        $scope.documentInformation.OwnerType = 'Client';
        $scope.documentInformation.OwnerID = $scope.clientId;
        if ($scope.documentmode == "New") {
            Document.save({ model: $scope.documentInformation, file: $scope.documentFile }, function (data) {
                alert("Document Added");
                $scope.selectedDocuments.push(data);
                $scope.documentInformation = {};
            });
        } else {
            Document.update({ model: $scope.documentInformation, file: $scope.documentFile }, function (data) {
                alert("Document Updated");
                $scope.selectedDocuments = Document.query({ ownertype: 'Client', ownerid: $scope.clientId });
                $scope.documentInformation = {};
            });
        }

    };
    $scope.newNote = new Note();
    $scope.notemode = "New";
    $scope.SubmitNote = function () {
        $scope.newNote.OwnerType = 'Client';
        $scope.newNote.OwnerID = $scope.clientId;
        $scope.newNote.TimeStamp = new Date().toJSON();
        if ($scope.notemode == "New") {
            $scope.newNote.$save({}, function (data) {
                alert("Note Added");
                $scope.selectedNotes.push(data);
                $scope.newNote = {};
            });
        }
        else
        {
            $scope.newNote.TimeStamp = new Date().toJSON();
            $scope.newNote.$update({}, function (data) {
                alert("Note Updated");
                $scope.selectedNotes = Note.query({ ownertype: 'Client', ownerid: $scope.clientId });
                $scope.newNote = {};
                $scope.notemode = "New";
            });
        }
    };

    $scope.editNote = function (note) {
        $scope.notemode = "Edit";
        $scope.newNote = note;
        
    };
    
    $scope.setSelectedDocument = function(path) {
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

    //Bookings Tab


    //DashboardTab    
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
        $scope.rawData.clientstats = Client.stats({
            clientid: -1,
            from: $scope.range.from.toJSON(),
            to: $scope.range.to.toJSON(),
        }, function (result) {
            $scope.graphdata[1].data = result.Result;
        });

        $scope.rawData.averagestats = Client.stats({
            clientid: $scope.clientId,
            from: $scope.range.from.toJSON(),
            to: $scope.range.to.toJSON(),
        }, function (result) {
            $scope.graphdata[0].data = result.Result;
        });
    };

    $scope.clientId = $scope.$parent.modal.clientid;
    $scope.selectedClient = Client.getById({ clientid: $scope.clientId }, function (client) {
        $scope.selectedBookings = $scope.selectedClient.$bookings(new Date().addPeriod(-1, "months").toJSON(), new Date().toJSON());
        $scope.selectedNotes = Note.query({ ownertype: 'Client', ownerid: client.ID });
    });

    $scope.selectedDocuments = Document.query({ ownertype: 'Client', ownerid: $scope.clientId });

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
            selected: Client.stats({
                clientid: $scope.clientId,
                grouping: 'none'
            }),
            max: Client.stats({
                clientid: -2,
                grouping: 'none'
            }),
        },
        month: {
            selected: Client.stats({
                clientid: $scope.clientId,
                from: new Date().addPeriod(-1, 'months').toJSON(),
                to: new Date().toJSON(),
                grouping: 'none'
            }),
            max: Client.stats({
                clientid: -2,
                from: new Date().addPeriod(-1, 'months').toJSON(),
                to: new Date().toJSON(),
                grouping: 'none'
            }),
        }
    };
    $scope.graphDataFetch();
});

app.controller('ClientEditViewController', function ($scope, $routeParams, $location) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'DriverView';

    $scope.client = { Active: true, ClientTypeID: 0, ClientSince: new Date()};
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
    if ($routeParams.clientId) {
        $scope.mode.main = "Edit";
        $scope.client = Client.getById({ clientid: $routeParams.clientId }, function (client) {
            $scope.dates = {
                ClientSince: new Date(client.ClientSince)
            };
        });
    } else {
        $scope.mode.main = "New";
        $scope.client = new Client();
        $scope.client.Active = true;
        $scope.client.ClientTypeID = 0;
        $scope.client.ClientSince = new Date();
    }
        	
    $scope.$watch('dates', function (newvalue, oldvalue, scope) {
        scope.client.ClientSince = newvalue.ClientSince;
    }, true);

    $scope.Save = function () {
        if ($scope.mode.main == "New") {
            Client.save({
                model: $scope.client,
                file: $scope.image
            }, function () {
                alert('Client Added');
                $scope.$emit('RequestDataRefresh');
                $location.path('/cards-view');
            });
        } else {
            Client.update({
                model: $scope.client,
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