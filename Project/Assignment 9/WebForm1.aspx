<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="Assignment_9.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            Web Application to Perform Automated MapReduce</div>
        <p>
&nbsp;<asp:FileUpload ID="FileUpload1" runat="server" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="UploadButton" runat="server" OnClick="Button3_Click" Text="Upload" />
        </p>
        <p>
            Choose the number of parallel threads</p>
        <p>
            <asp:TextBox ID="ThreadTextBox" runat="server" Width="43px">1</asp:TextBox>
        </p>
        <p>
            Web Service for Map function</p>
        <p>
            <asp:TextBox ID="MapTextBox" runat="server" Width="530px"></asp:TextBox>
        </p>
        <p>
            Web Service for Reduce function</p>
        <asp:TextBox ID="ReduceTextBox" runat="server" Width="530px"></asp:TextBox>
        <p>
            Web Service for Combiner function</p>
        <asp:TextBox ID="CombineTextBox" runat="server" Width="530px"></asp:TextBox>
        <p>
            <asp:Button ID="MapReduceButton" runat="server" OnClick="Button1_Click" Text="Perform MapReduce Computation" />
        </p>
        <p>
            <asp:Label ID="ResultLabel" runat="server" Text="Results:"></asp:Label>
        </p>
    </form>
</body>
</html>
