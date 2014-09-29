<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NextStep.aspx.cs" Inherits="Wf5.WebDemo.NextStep" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="Skin/css.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table class="tblist">
                <tr>
                    <td>步骤名称</td>

                    <td>接收人</td>
                    <td>步骤ID</td>
                </tr>

                <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td><%#Eval("Text") %></td>

                            <td>
                                <asp:Repeater ID="Repeater2" runat="server">
                                    <ItemTemplate>
                                        <input type="hidden" id="hiduid<%#Container.ItemIndex %>" value="<%#Eval("uName") %>" />
                                        <input type="checkbox" id="chk<%#Container.ItemIndex %>" value="<%#Eval("uid") %>" />&nbsp;<%#Eval("uName") %>
                                        <br />
                                    </ItemTemplate>
                                </asp:Repeater>

                            </td>
                            <td><%#Eval("id") %>
                                <input type="hidden" id="hidActivityGuid<%#Container.ItemIndex %>" name="flowClass" value="<%#Eval("id") %>" />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
        <div id="form_control">
            <table class="small control_tbl" width="100%">
                <tr>
                    <td align="center" valign="middle">
                        <input type="hidden" id="formId" name="formId" value="" runat="server" />
                        <input type="hidden" id="flowGuid" name="flowGuid" value="" runat="server" />
                        <input type="hidden" id="hidUserCount" name="flowClass" value="" runat="server" />

                        <input type="hidden" id="hiddenStepGuid" value="" runat="server" />
                        <input type="hidden" id="hiddenStepMember" value="" runat="server" />
                       <%-- <asp:Button ID="btnAdd" runat="server" class="btn" Text="新建" OnClick="btnAdd_Click" />
                        <input type="submit" value="转交下一步" class="btn" onclick="SubmitWork();" />&nbsp;--%>
                             <asp:Button ID="Button1" runat="server" class="btn" Text="同意" OnClick="Button1_Click" />
                        <%--<input type="button" value="保留意见" class="btn" onclick="SaveFlowApproval(2);">&nbsp;
                           <asp:Button ID="btnCancel" runat="server" class="btn" Text="不同意"/>&nbsp;
                            <asp:Button ID="SendBack" runat="server" class="btn" Text="退回"/>&nbsp;--%>
                            <%--<input type="button" value="流程步骤" class="btn" onclick="ShowOpinion(<%=instanceId%>);">&nbsp;--%>
                        <%--<input type="button" value="流程事件" class="btn" onclick="javascript: alert('敬请期待！')">&nbsp;
                            <input type="button" value="流程图" class="btn" onclick="javascript: alert('敬请期待！');">&nbsp;--%>
                            <input type="text" id="selectFlowGuid" runat="server" value="072af8c3-482a-4b1c-890b-685ce2fcc75d" readonly="readonly" class="input wd300" />
                                 <input type="text" id="selectSplitFlow" runat="server" value="e3c8830d-290b-4c1f-bc6d-0e0e78eb0bbf" readonly="readonly" class="input wd300" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
