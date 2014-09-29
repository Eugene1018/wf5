<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FlowPLApplyStart.aspx.cs" Inherits="Wf5.WebDemo.FlowPLApplyStart" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
     <link href="Skin/css.css" rel="stylesheet" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="text-align:center;margin-top:100px;">
            <H2>新建流程</H2><br />
            <table width="100%" border="0px" cellpadding="0" cellspacing="0" class="tb" id="flowApplyInfo" class="tblist">
                <tr>
                    <td align="right" class="bgcolor1 fsize12 fbold">自定义流程标题：</td>
                    <td>
                        <input id="txtApplyTitle" name="txtApplyTitle" runat="server" class="input wd300" obj="not-null" /></td>

                </tr>
                <tr>
                    <td align="right" class="bgcolor1 fsize12 fbold">流程:</td>
                    <td>
                        <input type="text" id="selectSplitFlow" runat="server" value="e3c8830d-290b-4c1f-bc6d-0e0e78eb0bbf" readonly="readonly" class="input wd300" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:Button ID="btnAdd" runat="server" class="btn" Text="新建" OnClick="btnAdd_Click" />
        </div>
    </form>
</body>
</html>
