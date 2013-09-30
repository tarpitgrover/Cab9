<%@ Page Title="" Language="C#" MasterPageFile="~/master/Cab9.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Cab9.pricing._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div ng-controller="PricingController">
        <div class="row">
            <div class="pageBanner">
                <div class="pull-left">
                    <h4 class="pageName">Pricing Management
                    </h4>
                </div>
                <div class="pull-left pageViewOptions">
                    <ul>
                        <li ng-class="(currentView == 'TableView') ? 'active' : ''"><a href="#/table-view" data-toggle="tooltip" data-placement="bottom" title="Table View"><span class="glyphicon glyphicon-align-justify"></span></a></li>
                    </ul>
                </div>
                <div class="addNew">
                    <a href="#/editor-view"><span class="glyphicon glyphicon-plus-sign" data-toggle="tooltip" data-placement="bottom" title="Add New Pricing Model"></span></a>
                </div>
                <br class="clear" />
            </div>
        </div>
        <div ng-view>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="customModuleScripts" runat="server">
    <script src="pricingModule.js" type="text/javascript"></script>
</asp:Content>
