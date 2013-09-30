<%@ Page Title="" Language="C#" MasterPageFile="~/master/Cab9.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Cab9.clients._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="" ng-controller="ClientsController">
        <div class="row">
            <div class="span12">
                <div class="pageBanner" >
                    <div class="pull-left">
                        <h4 class="pageName">Client Management
                        </h4>
                    </div>
                    <div class="pull-left pageViewOptions">
                        <ul>
                            <li ng-class="(currentView == 'CardsView') ? 'active' : ''"><a href="#/cards-view" data-toggle="tooltip" data-placement="bottom" title="Cards View"><span class="glyphicon glyphicon-th-large"></span><span class="optionsName"></span></a></li>
                            <li ng-class="(currentView == 'TableView') ? 'active' : ''"><a href="#/table-view" data-toggle="tooltip" data-placement="bottom" title="Table View"><span class="glyphicon glyphicon-align-justify"></span></a></li>
                            <li class="dropdown" ng-class="(grouping.key != 'All') ? 'active' : ''" ng-hide="(currentView=='ClientView')" data-toggle="tooltip" data-placement="bottom" title="Group By">
                                <span class="dropdown-toggle uppercase" data-toggle="dropdown"><span class="glyphicon glyphicon-list"></span></span>
                                <ul class="dropdown-menu" role="menu" aria-labelledby="dLabel">
                                    <li>
                                        <a ng-click="sorting = { key: 'Name', reverse: false }; grouping = { key: 'Name',  initials: true };">Name
                                        </a>
                                    </li>
                                    <li>
                                        <a ng-click="sorting = { key: 'ClientType.Name', reverse: false }; grouping = { key: 'ClientType.Name',  initials: false };">Type
                                        </a>
                                    </li>
                                    <li>
                                        <a ng-click="sorting = { key: 'Name', reverse: false }; grouping = { key: 'All', initials: false };">None
                                        </a>
                                    </li>
                                </ul>
                            </li>
                        </ul>
                    </div>
                    <div class="addNew">
                        <a href="#/client-view"><span class="glyphicon glyphicon-plus-sign" data-toggle="tooltip" data-placement="bottom" title="Add New Client"></span></a>
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
    <script src="clientModule.js" type="text/javascript"></script>
</asp:Content>
