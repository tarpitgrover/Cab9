<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Cab9.DriverInterface._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" ng-app="DriverInterface">

<head runat="server">
    <title>Cab9 - Drivers</title>
    <link href="/includes/stylesheets/bootstrap.css" rel="stylesheet" />
    <link href='http://fonts.googleapis.com/css?family=Montserrat|Titillium+Web:400,300,600,700|Raleway:400,600' rel='stylesheet' type='text/css' />
    <style type="text/css">
        body {
            background: #3d3d3d !important;
        }

        h1, h2, h3, h4, p {
            color: white !important;
            font-family: "HelveticaNeue-Light", "Helvetica Neue Light", "Helvetica Neue", Helvetica, Arial, "Lucida Grande", sans-serif;
        }
    </style>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no">
</head>
<body ng-controller="DIController">
    <div id="content" style="padding-top: 50px;">
    <nav class="navbar navbar-default navbar-fixed-top" role="navigation" style="background-color: #26AE90; height: 30px;">
        <p class="navbar-brand" style="width: 35%; text-align: left;" ng-bind="backText" ng-click="BackClicked()"></p>
        <p class="navbar-brand" style="width: 30%; text-align: center;" ng-click="TestPush()">Cab9</p>
        <p class="navbar-brand" style="width: 35%; text-align: right;"><span class="label" ng-class="statusColors" ng-bind="current.driver.Status"></span></p>
    </nav>
    <div ng-view>
    </div>
    </div>
    <!-- Include for popovers -->
    <div ng-include="include">
    </div>

    <!-- Late Loaded Scripts -->
    <script src="/includes/js/jquery-1.8.3.min.js"></script>
    <script src="/includes/js/angular.js"></script>
    <script src="/includes/js/angular-route.js"></script>
    <script src="/includes/js/angular-resource.js"></script>
    <script src="/includes/js/angular-sanitize.js"></script>
    <script src="/includes/js/jquery.signalR-2.0.0-rc1.js"></script>
    <script src="/signalr/hubs"></script>
    <script src="driverInterfaceModule.js"></script>
</body>
</html>
