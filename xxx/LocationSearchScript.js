/// Sample Change
/// <reference path="../master/angular/masterModule.js" />
/// <reference path="https://maps.googleapis.com/maps/api/js?key=AIzaSyBSjg13rvyaC145WmGcf1FInYT3dUebzOg&sensor=false&libraries=drawing,geometry,places" />
app.directive('locationSearchAdv', function (Location, $filter) {
    return {
        restrict: 'A',
        scope: {
            selected: '='
        },
        template:
              ' <input type="text" ng-model="searchTerm" placeholder="Search Postcodes, Stations, Airports.." style="width:100%"/>'
            + ' <div></div>'
            + ' <div class="popover bottom" style="position:absolute; top: 20px; left:30px; right:30px; max-width: none">'
            + '     <div class="arrow"></div>'
            + '     <div class="popover-content">'
            + '         <div ng-hide="locations">Searching...</div>'
            + '         <div ng-show="locations" ng-repeat="t in types | customOrder:order">'
            + '             <div style="height:20px; padding:4px; font-weight:600; border-bottom:1px solid #3d3d3d; width:100%">{{ t }} <img class="pull-right" ng-src="{{GetTypeImage(t)}}" /></div>'
            + '             <div ng-repeat="l in locations | ItemsForGroup:t:grouping:sorting" ng-click="Select(l)" class="repeaterItemCount">'
            + '             {{ l.Name }}'
            + '             </div>'
            + '         </div>'
            + '     </div>'
            + ' </div>',
        link: function (scope, elem, attrs) {
            elem.css("position", "relative");
            scope.gService = new google.maps.places.AutocompleteService();
            scope.gDetails = new google.maps.places.PlacesService(elem.children('div')[0]);

            scope.locations = [];
            scope.types = [];
            scope.grouping = { key: 'Type', initials: false };
            scope.sorting = { key: 'Match', reverse: true };
            scope.order = {
                'Airport': 1,
                'Train Station': 2,
                'Company': 3,
                'Google Results':10
            }

            scope.GetOrderIndex = function (type) {
                return scope.order[type] ? scope.order[type] : 20;
            };

            scope.GetTypeImage = function (type) {
                var lookup = {
                    'Airport': null,
                    'Train Station': null,
                    'Company': null,
                    'Google Results': '/includes/img/powered-by-google-on-white2.png'
                }
                return lookup[type];
            }

            scope.searchTerm = "";
            scope.$watch("searchTerm", function (newValue, oldValue) {
                if (newValue && newValue.length > 2)
                {
                    scope.highlighted = -1;
                    elem.children('.popover').show();
                    Location.search({ q: newValue }, function (data) {
                        scope.locations = scope.locations.where(function (value) { return (value.Type == "Google Results") });
                        angular.forEach(data, function (result) {
                            scope.locations.push(result);
                        });
                        scope.types = $filter('GetGroupsForData')(scope.locations, scope.grouping, scope.sorting);
                    });
                    scope.gService.getPlacePredictions({ input: newValue }, function (data, status) {
                        if (status = google.maps.places.PlacesServiceStatus.OK) {
                            scope.locations = scope.locations.where(function (value) { return (value.Type != "Google Results") });
                            angular.forEach(data, function (result) {
                                scope.locations.push({
                                    Name: result.description,
                                    Type: "Google Results",
                                    Reference: result.reference
                                });
                            });
                            scope.types = $filter('GetGroupsForData')(scope.locations, scope.grouping, scope.sorting);
                            scope.$digest();
                        }
                    })
                } else {
                    elem.children('.popover').hide();
                    scope.locations = [];
                    scope.selected = null;
                }
            });

            scope.Select = function (location) {
                if (location.Type == "Google Results") {
                    scope.gDetails.getDetails({
                        reference: location.Reference,
                        sensor: false,
                        key: 'AIzaSyBG7ERZfOCGfUxgrB1aA7mdgt5aY3HzZvY'
                    }, function (details) {
                        elem.children('input')[0].value = location.Name;
                        location.Latitude = details.geometry.location.lat();
                        location.Longitude = details.geometry.location.lng();
                        elem.children('.popover').hide();
                        scope.$apply(function () {
                            scope.locations = [];
                            scope.selected = location;
                        });
                    });
                } else {
                    elem.children('input')[0].value = location.Name;
                    elem.children('.popover').hide();
                    scope.$apply(function () {
                        scope.locations = [];
                        scope.selected = location;
                    });
                }
            }

            scope.highlighted = -1;
            elem.children('input')[0].onkeydown = function (event) {
                if (event.keyIdentifier == "Down") {
                    scope.highlighted += 1;
                    if (scope.highlighted > scope.locations.length - 1)
                        scope.highlighted = scope.locations.length - 1;
                    $('.repeaterItemCount.Selected').removeClass('Selected');
                    $('.repeaterItemCount').addClass(function (index) {
                        if (index == scope.highlighted)
                            return 'Selected';
                    });
                    event.preventDefault();
                }
                if (event.keyIdentifier == "Up") {
                    scope.highlighted -= 1;
                    if (scope.highlighted < 0)
                        scope.highlighted = 0;
                    $('.repeaterItemCount.Selected').removeClass('Selected');
                    $('.repeaterItemCount').addClass(function (index) {
                        if (index == scope.highlighted)
                            return 'Selected';
                    });
                    event.preventDefault();
                }
                if (event.keyIdentifier == "Enter") {
                    console.log($('.repeaterItemCount.Selected').scope().l);
                    scope.Select($('.repeaterItemCount.Selected').scope().l);
                }
            };
        }
    };
});

app.factory("Location", function ($resource) {
    return $resource(apiEndPoint + 'location/:action', {}, {
        get: {
            method: 'GET',
            isArray: false,
            params: {
                action: 'GetByID'
            }
        },
        search: {
            method: 'GET',
            isArray: true,
            params: {
                action: 'Search'
            }
        },
        update: {
            method: 'PUT'
        }
    });
});

app.filter("customOrder", function () {
    return function (array, order) {
        return array.sort(SortBy(null, false, function (value) { return order[value]; }));
    };
});

app.controller("TestController", function ($scope) {
    $scope.from = null;
    $scope.to = null;
    $scope.fromMarker = null;
    $scope.toMarker = null;
    $scope.mapOptions = {};
    $scope.mapObject = {};

    $scope.$watch('from', function (newValue) {
        if (newValue) {
            if ($scope.fromMarker) {
                $scope.fromMarker.setPosition(new google.maps.LatLng(newValue.Latitude, newValue.Longitude));
                $scope.fromMarker.setMap($scope.mapObject);
            } else {
                $scope.fromMarker = new google.maps.Marker({
                    map: $scope.mapObject,
                    position: new google.maps.LatLng(newValue.Latitude, newValue.Longitude)
                })
            }
        } else {
            if ($scope.fromMarker) {
                $scope.fromMarker.setMap(null);
            }
        }
    });

    $scope.$watch('to', function (newValue) {
        if (newValue) {
            if ($scope.toMarker) {
                $scope.toMarker.setPosition(new google.maps.LatLng(newValue.Latitude, newValue.Longitude));
                $scope.toMarker.setMap($scope.mapObject);
            } else {
                $scope.toMarker = new google.maps.Marker({
                    map: $scope.mapObject,
                    position: new google.maps.LatLng(newValue.Latitude, newValue.Longitude)
                })
            }
        } else {
            if ($scope.toMarker) {
                $scope.toMarker.setMap(null);
            }
        }
    });
});