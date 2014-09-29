<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Wf5.WebDemo.Login" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="Skin/css.css" rel="stylesheet" />
</head>
<body>

    <form id="form1" runat="server">

        <table height="100%" cellspacing="0" cellpadding="0" width="100%" border="0" bgcolor="#EBEBEB">
            <tr>
                <td align="center">
                    <table border="0px" cellpadding="0" cellspacing="0" class="tb" style="width: 350px;">
                        <tr>
                            <td width="80" align="right">角色：</td>
                            <td>
                                <asp:DropDownList ID="ddlRole" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlRole_SelectedIndexChanged" Width="200"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td width="80" align="right">用户名：</td>
                            <td>
                                <asp:DropDownList ID="ddlUser" runat="server" Width="200"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td align="right"></td>
                            <td>
                                <asp:Button ID="btnLogin" runat="server" Text="登录" OnClick="btnLogin_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>


    </form>



</body>
</html>
