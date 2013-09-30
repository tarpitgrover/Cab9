<%@ Page Title="" Language="C#" MasterPageFile="~/master/Cab9.Master" AutoEventWireup="true" CodeBehind="LocationSearchTextBox.aspx.cs" Inherits="Cab9.xxx.LocationSearchTextBox" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .Selected {
            background: #26AE90;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div ng-controller="TestController">
        <div location-search-adv selected="from" style="width:40%; display:inline-block;"></div>
        <div location-search-adv selected="to" style="width:40%; display:inline-block;"></div>
        <div basic-map map="mapObject" options="mapOptions" style="height:400px;"></div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="customModuleScripts" runat="server">
    <script src="LocationSearchScript.js"></script>
</asp:Content>
