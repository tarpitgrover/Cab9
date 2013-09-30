<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Cab9.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="Scripts/jquery-1.6.4.js"></script>
    <script src="Scripts/jquery.signalR-2.0.0-rc1.js"></script>
    <script src="signalr/hubs"></script>
    <script type="text/javascript">

        var hub = (function () {
            var testHub = $.connection.driverHub;
            testHub.client.Test = function (msg) {
                alert(msg);
            };
            $.connection.hub.start(function () { alert('Ready'); }).fail(function () { alert('Connection Not Authorised Please Log In'); window.location.replace('/Login.aspx'); });
            return testHub;
        })();
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        MAIN INDEX
    </div>
    </form>
</body>
</html>
