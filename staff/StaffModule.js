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
            controller: 'StaffCardsViewController',
            templateUrl: '/staff/partials/staff-cards.html'
        })
        .when('/table-view',
        {
            controller: 'StaffTableViewController',
            templateUrl: '/staff/partials/staff-table.html'
        })
        .when('/staff-view',
        {
	        controller: 'StaffEditViewController',
            templateUrl: '/staff/partials/staff-editor.html'
        })
        .when('/staff-view/:staffId',
        {
            controller: 'StaffEditViewController',
            templateUrl: '/staff/partials/staff-editor.html'
        })
        .otherwise({ redirectTo: '/cards-view' });
});

/////////////////////////////////////////////////////////////
//               Controller Registration                   //
/////////////////////////////////////////////////////////////
app.controller('StaffController', function ($scope, $timeout, $filter) {
    $scope.$parent.appStatus = 'Loading';

    $scope.staff = Staff.query();

    $scope.$watch('staff', function (newvalue, oldvalue, scope) {
        scope.filteredStaff = $filter('SearchFilter')(newvalue, scope.searchFields, scope.searchTerm);
    }, true);
    
    $scope.filteredStaff = [];
    
    $scope.$watch('filteredStaff', function (newvalue, oldvalue, scope) {
        scope.groups = $filter('GetGroupsForData')(newvalue, scope.grouping, scope.sorting);
        scope.$broadcast('Data-Changed', newvalue);
    }, true);
   
    $scope.selectedStaff = {};

    $scope.sorting = { key: 'Forename', reverse: false };
    $scope.$watch('sorting', function (newvalue, oldvalue, scope) {}, true);
    
    $scope.grouping = { key: 'Position', initials: false };
     $scope.$watch('grouping', function (newvalue, oldvalue, scope) {
        scope.groups = $filter('GetGroupsForData')(scope.filteredStaff, newvalue, scope.sorting);
    }, true);
    
    $scope.searchFields = ['Forename', 'Surname', 'Position'];
    $scope.searchTerm = '';
        $scope.$watch('searchTerm', function (newvalue, oldvalue, scope) {    
        scope.filteredStaff = $filter('SearchFilter')(scope.staff, scope.searchFields, newvalue);
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
        $scope.staff = Staff.query();
    });

    $scope.$parent.appStatus = 'Ready';
});

app.controller('StaffCardsViewController', function ($scope, $q) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'CardsView';

    $scope.popOut = function (e) {
        $scope.$parent.selectedStaff = this.staff;
        $scope.$parent.poppedCard = angular.element(e.srcElement).parents('.card');
        $scope.$emit('RequestPopover', { name: 'StaffDetails', url: '/staff/partials/staff-details.html' });
        $('#popoverContainer').addClass("popOverShow");
        $('#popoverContainer').toggle();

    };

    $scope.$parent.$parent.appStatus = 'Ready';
});

app.controller('StaffTableViewController', function ($scope, $q) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'TableView';


    $scope.$parent.$parent.appStatus = 'Ready';
});

app.controller('StaffBirdsEyeViewController', function ($scope) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'DetailsView';

    $scope.staffId = $scope.$parent.$parent.selectedStaff.ID;
    $scope.selectedStaff = Staff.get({ staffid: $scope.staffId }, function (staff) {
        $scope.selectedShifts = StaffShift.query({staffid: $scope.staffId});
        $scope.selectedDocuments = Document.query({ ownertype: 'Staff', ownerid: $scope.staffId });
        $scope.selectedNotes = Note.query({ ownertype: 'Staff', ownerid: $scope.staffId });
    });
    
    //Document Uploading
    $scope.documentFile = {};
    $scope.documentInformation = {};

    $scope.SubmitFile = function () {
        $scope.documentInformation.OwnerType = 'Staff';
        $scope.documentInformation.OwnerID = $scope.staffId;
        Document.save({ model: $scope.documentInformation, file: $scope.documentFile }, function (data) {
            $scope.selectedDocuments.push(data);
            $scope.documentInformation = {};
        });
    };

    $scope.CancelFile = function () {
        $scope.documentInformation = {};
    };

    $scope.$on("fileSelected", function (event, args) {
        $scope.$apply(function () {
            $scope.documentFile = { type: args.type, file: args.file };
        });
    });
    
    $scope.dates = {};
    $scope.$watch('dates', function (newvalue, oldvalue, scope) {
        scope.documentInformation.ExpiryDate = newvalue.ExpiryDate;
    }, true);   
    
        
    $scope.range = {};
    $scope.graphdata = [
        {
            label: ' Staff',
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
        $scope.rawData.staffstats = Staff.stats({
            staffid: -1,
            from: $scope.range.from.toJSON(),
            to: $scope.range.to.toJSON(),
        }, function (result) {
            $scope.graphdata[1].data = result.Result;
        });

        $scope.rawData.averagestats = Staff.stats({
            staffid: $scope.staffId,
            from: $scope.range.from.toJSON(),
            to: $scope.range.to.toJSON(),
        }, function (result) {
            $scope.graphdata[0].data = result.Result;
        });
    };

    $scope.stats = {
        alltime: {
            selected: Staff.stats({
                staffid: $scope.staffId,
                grouping: 'none'
            }),
            max: Staff.stats({
                staffid: -2,
                grouping: 'none'
            }),
        },
        month: {
            selected: Staff.stats({
                staffid: $scope.staffId,
                from: new Date().addPeriod(-1, 'months').toJSON(),
                to: new Date().toJSON(),
                grouping: 'none'
            }),
            max: Staff.stats({
                staffid: -2,
                from: new Date().addPeriod(-1, 'months').toJSON(),
                to: new Date().toJSON(),
                grouping: 'none'
            }),
        }
    };

    $scope.NewRange('1yr');

    $scope.newNote = new Note();

    $scope.SubmitNote = function () {
        $scope.newNote.OwnerType = 'Staff';
        $scope.newNote.OwnerID = $scope.staffId;
        $scope.newNote.PostedDateTime = new Date().toJSON();
        $scope.newNote.$save({}, function (data) {
            $scope.selectedNotes.push(data);
            $scope.newNote = new Note();
        });
    };

    $scope.CancelNote = function () {
        $scope.newNote = new Note();
    };

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
});

app.controller('StaffEditViewController', function ($scope, $routeParams, $location) {
    $scope.$parent.$parent.appStatus = 'Loading';
    $scope.$parent.$parent.currentView = 'EditorView';

    $scope.staff = { Active: true, ImageUrl: 'http://s3.amazonaws.com/37assets/svn/765-default-avatar.png'};
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
    if ($routeParams.staffId) {
        $scope.mode.main = "Edit";
        $scope.staff = Staff.get({ staffid: $routeParams.staffId },function(staff){
            $scope.Fullname = staff.Forename + " " + staff.Middlenames + " " + staff.Surname;
            var dates = {
                DateOfBirth: new Date(staff.DateOfBirth),
                StartDate: new Date(staff.StartDate),
                FinishDate: new Date(staff.FinishDate)
            };
            $scope.dates = dates; 
        });
    } else {
        $scope.mode.main = "New";
        $scope.staff = new Staff();
    }
        	
    $scope.$watch('dates', function (newvalue, oldvalue, scope) {
        scope.staff.DateOfBirth = newvalue.DateOfBirth;
        scope.staff.StartDate = newvalue.StartDate;
        scope.staff.FinishDate = newvalue.FinishDate;
    }, true);

    $scope.$watch('Fullname', function (newvalue, oldvalue, scope) {
        var names = newvalue.split(" ");
        scope.staff.Forename = names[0];
        scope.staff.Surname = (names.length > 1) ? names[names.length - 1] : '';
        var middlenames = names.splice(1, names.length - 2);
        scope.staff.Middlenames = middlenames.join(" ");
    });

    $scope.Save = function () {
        if ($scope.mode.main == "New") {
            Staff.saveWithImage({
                model: $scope.staff,
                file: $scope.image
            }, function () {
                alert('Staff Added');
                $scope.$emit('RequestDataRefresh');
                $location.path('/cards-view');
            });
        } else {
            Staff.updateWithImage({
                model: $scope.staff,
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
