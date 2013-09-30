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
            controller: 'CustomersCardsViewController',
            templateUrl: '/customers/partials/customers-cards.html'
        })
        .when('/table-view',
        {
            controller: 'CustomersTableViewController',
            templateUrl: '/customers/partials/customers-table.html'
        })
        .when('/customer-view',
        {
            controller: 'CustomersEditViewController',
            templateUrl: '/customers/partials/customer-editor.html'
        })
        .when('/customer-view/:customerId',
        {
            controller: 'CustomersEditViewController',
            templateUrl: '/customers/partials/customer-editor.html'
        })
        .otherwise({ redirectTo: '/cards-view' });
});

/////////////////////////////////////////////////////////////
//                   Filter Registration                   //
/////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////
//               Controller Registration                   //
/////////////////////////////////////////////////////////////
app.controller('CustomersController', function ($scope, $filter) {
    $scope.$parent.appStatus = 'Loading';

    //Customers
    $scope.customers = Customer.query();
    $scope.$watch('customers', function (newvalue, oldvalue, scope) {
        scope.filteredCustomers = $filter('SearchFilter')(newvalue, scope.searchFields, scope.searchTerm);
    },true);

    //FilteredCustomers
    $scope.filteredCustomers = [];
    $scope.$watch('filteredCustomers', function (newvalue, oldvalue, scope) {
        scope.groups = $filter('GetGroupsForData')(newvalue, scope.grouping, scope.sorting);
        scope.$broadcast('Data-Changed', newvalue);
    },true);

    $scope.selectedCustomer = {};

    //Sorting
    $scope.sorting = { key: 'Name', reverse: false };
    $scope.$watch('sorting', function (newvalue, oldvalue, scope) {}, true);

    //Grouping
    $scope.grouping = { key: 'AccountType.Name', initials: false };
    $scope.$watch('grouping', function (newvalue, oldvalue, scope) {
        scope.groups = $filter('GetGroupsForData')(scope.filteredCustomers, newvalue, scope.sorting);
    }, true);

    //SearchTerm
    $scope.searchFields = ['Name', 'ContactName'];
    $scope.searchTerm = '';
    $scope.$watch('searchTerm', function (newvalue, oldvalue, scope) {
        
        scope.filteredCustomers = $filter('SearchFilter')(scope.customers, scope.searchFields, newvalue);
    }, true);


    //Scope Functions
    $scope.ChangeSorting = function (key, subkey, sorting) {
        if (sorting.key == key) {
            $scope.sorting.reverse = !($scope.sorting.reverse);
        } else {
            $scope.sorting.key = key;
            $scope.sorting.subkey = subkey;
        }
    };

    $scope.$on('RequestDataRefresh', function() {
        $scope.customers = Customer.query();
    });

    $scope.$parent.appStatus = 'Ready';
});

app.controller('CustomersTableViewController', function ($scope) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'TableView';



    $scope.$parent.$parent.appStatus = 'Ready';
});

app.controller('CustomersBirdsEyeViewController', function ($scope) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'DetailsView';
    //Current Customer
    
    $scope.customerId = $scope.$parent.$parent.selectedCustomer.ID;
    $scope.selectedCustomer = Customer.get({ customerid: $scope.customerId }, function(cust) {
        $scope.selectedBookings = $scope.selectedCustomer.$bookings(cust.ID, new Date().addPeriod(-1, "months").toJSON(), new Date().toJSON());
        $scope.selectedNotes = Note.query({ ownertype: 'Customer', ownerid: cust.ID });
        $scope.selectedDocuments = Document.query({ ownertype: 'Customer', ownerid: cust.ID });
    });


    //Document Uploading
    $scope.documentFile = {};
    $scope.documentInformation = {};

    $scope.SubmitFile = function () {
        $scope.documentInformation.OwnerType = 'Customer';
        $scope.documentInformation.OwnerID = $scope.customerId;
        Document.save({ model: $scope.documentInformation, file: $scope.documentFile }, function (data) {
            $scope.selectedDocuments.push(data);
            $scope.documentInformation = {};
        });
    };

    $scope.CancelFile = function () {
        $scope.documentInformation = {};
    };
    
    $scope.newNote = new Note();
    
    $scope.SubmitNote = function () {
        $scope.newNote.OwnerType = 'Customer';
        $scope.newNote.OwnerID = $scope.customerId;
        $scope.newNote.PostedDateTime = new Date().toJSON();
        $scope.newNote.$save({}, function (data) {
            $scope.newNote = new Note();
        });
        $scope.selectedNotes = Note.query({ ownertype: 'Customer', ownerid: cust.ID });
    };

    $scope.CancelNote = function () {
        $scope.newNote = new Note();
    };

    $scope.$on("fileSelected", function (event, args) {
        $scope.$apply(function () {
            $scope.documentFile = { type: args.type, file: args.file };
        });
    });


    //DatePicker Handling
    $scope.dates = {};
    $scope.$watch('dates', function (newvalue, oldvalue, scope) {
        scope.documentInformation.ExpiryDate = newvalue.ExpiryDate;
    }, true);

    //GraphData
    $scope.range = {};
    $scope.graphdata = [
        {
            label: ' Customer',
            data: [],
            color: "#28AE90"
        },
        {
            label: ' Average',
            data: [],
            color: "#EE7951"
        }
    ];

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
        $scope.rawData.customerstats = Customer.stats({
            customerid: -1,
            from: $scope.range.from.toJSON(),
            to: $scope.range.to.toJSON(),
        }, function (result) {
            $scope.graphdata[1].data = result.Result;
        });

        $scope.rawData.averagestats = Customer.stats({
            customerid: $scope.customerId,
            from: $scope.range.from.toJSON(),
            to: $scope.range.to.toJSON(),
        }, function (result) {
            $scope.graphdata[0].data = result.Result;
        });
    };

    $scope.stats = {
        alltime: {
            selected: Customer.stats({
                customerid: $scope.customerId,
                grouping: 'none'
            }),
            max: Customer.stats({
                customerid: -2,
                grouping: 'none'
            }),
        },
        month: {
            selected: Customer.stats({
                customerid: $scope.customerId,
                from: new Date().addPeriod(-1, 'months').toJSON(),
                to: new Date().toJSON(),
                grouping: 'none'
            }),
            max: Customer.stats({
                customerid: -2,
                from: new Date().addPeriod(-1, 'months').toJSON(),
                to: new Date().toJSON(),
                grouping: 'none'
            }),
        }
    };


    $scope.NewRange('1yr');
    
    //Card Popping
    var chosenCard = $scope.$parent.poppedCard;
    var vartop;
    var varleft;
    vartop = $(chosenCard).offset().top - $(window).scrollTop();
    varleft = $(chosenCard).position().left;
    height = $(chosenCard).height();
    width = $(chosenCard).width();

    var popoutCard = $('#popOver');
    popoutCard.addClass("cardShow");
    popoutCard.height(height);
    popoutCard.width(width);
    popoutCard.css("top", vartop);
    popoutCard.css("left", varleft + 25);
    popoutCard.toggle();
    popoutCard.toggleClass('animated');
    popoutCard.toggleClass('growToCenter');
    $scope.$parent.$parent.appStatus = 'Ready';
    $scope.$parent.$parent.appStatus = 'Ready';
});

app.controller('CustomersCardsViewController', function ($scope) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'CardsView';

    $scope.popOut = function (e) {
        $scope.$parent.selectedCustomer = this.customer;
        $scope.$parent.poppedCard = angular.element(e.srcElement).parents('.card');
        $scope.$emit('RequestPopover', { name: 'CustomerDetails', url: '/customers/partials/customer-details.html' });
        $('#popoverContainer').addClass("popOverShow");
        $('#popoverContainer').toggle();

    };


    $scope.$parent.$parent.appStatus = 'Ready';
});

app.controller('CustomersEditViewController', function ($scope, $routeParams, $location) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'CustomerView';

    $scope.mode = { main: "New" };
    if ($routeParams.customerId) {
        $scope.mode.main = "Edit";
        $scope.customer = Customer.get({ customerId: $routeParams.customerId });
    } else {
        $scope.mode.main = "New";
        $scope.customer = new Customer();
    }

    $scope.Save = function() {
        if ($scope.mode.main == "New") {
            Customer.saveWithImage({
                model: $scope.customer,
                file: $scope.image
                }, function () {
                alert('Customer Added');
                $scope.$emit('RequestDataRefresh');
                $location.path('/cards-view');
            });
        } else {
            Customer.updateWithImage({
                model: $scope.customer,
                file: $scope.image
            }, function () {
                alert('Changes Saved');
                $scope.$emit('RequestDataRefresh');
                $location.path('/cards-view');
            });
        }
    };
    
    $scope.image = {};
    $scope.$on("fileSelected", function (event, args) {
        $scope.$apply(function () {
            $scope.image = { type: args.type, file: args.file };
        });
    });

    $scope.$parent.$parent.appStatus = 'Ready';
});
