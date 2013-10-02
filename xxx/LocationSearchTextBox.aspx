<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="LocationSearchTextBox.aspx.cs" Inherits="Cab9.xxx.LocationSearchTextBox" %>
<!DOCTYPE html>
<html ng-app="Cab9">
<head>
    <title></title>
    <style type="text/css">
        .Selected {
            background: #26AE90;
        }
    </style>
    <link href="../includes/css/bootstrap.css" rel="stylesheet" />
    <link href="../includes/stylesheets/style.css" rel="stylesheet" />
    <script src="../includes/js/jquery-1.8.3.min.js"></script>
    <script src="../includes/js/jquery.signalR-2.0.0-rc1.js"></script>
    <script src="../includes/js/angular.js"></script>
    <script src="../includes/js/angular-resource.js"></script>
    <script src="../includes/js/angular.bootstrap.js"></script>
    <script src="../master/angular/masterModule.js"></script>
</head>
<body>
    <div ng-controller="TestController">
        <div journey-builder></div>
    </div>
    <script src="LocationSearchScript.js"></script>
</body>
</html>
    
    
