<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FlowOpinion.aspx.cs" Inherits="Wf5.WebDemo.FlowOpinion" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="Skin/css.css" rel="stylesheet" />

    <script src="js/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="js/layer/layer.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="content">
            <table width="100%" border="0" cellpadding="0" cellspacing="0" class="tb">
                <tr>
                    <td>
                        <asp:Repeater ID="Repeater1" runat="server">
                            <HeaderTemplate>
                                <table width='100%' border='0' cellpadding='0' cellspacing='0' class='tblist'>
                                    <thead>
                                        <tr>
                                            <th align="center">序号</th>
                                            <th align="center">步骤名称</th>
                                            <th align="center">状态</th>
                                            <th align="center">经办人/开始时间</th>
                                            <th align="center">持续时间</th>
                                            <th align="center">办理理由</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td align="center"><%#  Container.ItemIndex+1 %></td>
                                    <td align="left"><%# Eval("ActivityName")%></td>
                                    <td align="left"><%# Eval("StateName")%></td>
                                    <td align="left"><%# Eval("CreatedByUserName")%> / <%# Eval("CreatedDateTime")%></td>
                                    <td align="left">-</td>
                                    <td align="center"><%# Eval("frDealAdvice")%></td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </tbody> 
                                <footer></footer>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
            </table>

        </div>
    </form>
</body>
</html>
