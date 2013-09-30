<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Cab9.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    
</head>
<body>
    <form id="form1" runat="server" >
    <div>
        <asp:TextBox ID="Username" runat="server"></asp:TextBox><asp:TextBox ID="Password" runat="server"></asp:TextBox>
        <asp:Button ID="Submit" runat="server" Text="Button" OnClick="Submit_Click"/>
    </div>
    </form>
</body>
</html>
