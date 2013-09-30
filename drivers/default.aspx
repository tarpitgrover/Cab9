<%@ Page Title="" Language="C#" MasterPageFile="~/master/Cab9.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Cab9.drivers._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="" ng-controller="DriversController">
        <div class="row">
            <div class="pageBanner">
                <div class="pull-left">
                    <h4 class="pageName">Driver Management
                    </h4>
                </div>
                <div class="pull-left pageViewOptions">
                    <ul>
                        <li ng-class="(currentView == 'CardsView') ? 'active' : ''"><a href="#/cards-view" data-toggle="tooltip" data-placement="bottom" title="Cards View"><span class="glyphicon glyphicon-th-large"></span><span class="optionsName"></span></a></li>
                        <li ng-class="(currentView == 'TableView') ? 'active' : ''"><a href="#/table-view" data-toggle="tooltip" data-placement="bottom" title="Table View"><span class="glyphicon glyphicon-align-justify"></span></a></li>
                        <li ng-class="(currentView == 'MapView') ? 'active' : ''"><a href="#/map-view" data-toggle="tooltip" data-placement="bottom" title="Map View"><span class="glyphicon glyphicon-globe"></span></a></li>
                         <li class="dropdown" ng-class="(grouping.key != 'All') ? 'active' : ''" ng-hide="(currentView=='DriverView')" data-toggle="tooltip" data-placement="bottom" title="Group By">
                            <span class="dropdown-toggle uppercase" data-toggle="dropdown"><span class="glyphicon glyphicon-list"></span></span>
                            <ul class="dropdown-menu" role="menu" aria-labelledby="dLabel">
                                <li>
                                    <a ng-click="sorting = { key: 'Forename', reverse: false }; grouping = { key: 'Forename',  initials: true };">Name
                                    </a>
                                </li>
                                <li>
                                    <a ng-click="sorting = { key: 'Status', reverse: false }; grouping = { key: 'Status', initials: false };">Status
                                    </a>
                                </li>
                                <li>
                                    <a ng-click="sorting = { key: 'DriverType.Name', reverse: false }; grouping = { key: 'DriverType.Name',  initials: false };">Type
                                    </a>
                                </li>
                                <li>
                                    <a ng-click="sorting = { key: 'Forename', reverse: false }; grouping = { key: 'All', initials: false };">None
                                    </a>
                                </li>
                            </ul>
                        </li>
                    </ul>
                </div>
                <div class="addNew">
                    <a href="#/driver-view"><span class="glyphicon glyphicon-plus-sign" data-toggle="tooltip" data-placement="bottom" title="Add New Driver" ng-hide="(currentView=='DriverView' || currentView == 'MapView')"  ></span></a>
                </div>
                <div class="pull-right">
                    <input type="text" ng-model="searchTerm" class="bannerSearchTextBox" placeholder="Search..." />
                </div>
                <br class="clear" />
            </div>
        </div>
        <div ng-view>
        </div>

    </div>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="customModuleScripts" runat="server">
    <script src="driverModule.js" type="text/javascript"></script>
</asp:Content>
