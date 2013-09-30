<%@ Page Title="" Language="C#" MasterPageFile="~/master/Cab9.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Cab9.clients._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <link href="../includes/css/angular-bootstrap-timepicker.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="" ng-controller="BookingController">
        <div class="row">
            <div class="span12">
                <div class="pageBanner" >
                    <div class="pull-left">
                        <h4 class="pageName">Dispatch
                        </h4>
                    </div>
                    <div class="pull-left pageViewOptions">
                        <ul>
                            <li ng-class="(currentView == 'TableView') ? 'active' : ''"><a href="#/table-view" data-toggle="tooltip" data-placement="bottom" title="Table View"><span class="glyphicon glyphicon-align-justify"></span></a></li>
                            <li ng-class="(currentView == 'MapView') ? 'active' : ''"><a href="#/map-view" data-toggle="tooltip" data-placement="bottom" title="Map View"><span class="glyphicon glyphicon-globe"></span></a></li>
                        </ul>
                    </div>
                    <div class="addNew">
                        <a href="#/add-booking"><span class="glyphicon glyphicon-plus-sign" data-toggle="tooltip" data-placement="bottom" title="Add New Booking"></span></a>
                    </div>
                    <div class="pull-right">
                        <input type="text" ng-model="searchTerm" class="bannerSearchTextBox" placeholder="Search..." />
                    </div>
                    <br class="clear" />
                </div>
            </div>
        </div>
        <div ng-view>
        </div>
    </div>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="customModuleScripts" runat="server">
    <script src="bookingModule.js" type="text/javascript"></script>
    <script src="../includes/js/angular.bootstrap.timepicker.js" type="text/javascript"></script>
</asp:Content>
