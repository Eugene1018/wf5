<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FlowList.aspx.cs" Inherits="Wf5.WebDemo.FlowList" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="../../Skin/common/css.css" rel="stylesheet" />

    <script src="../../Js/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../../Js/common.js" type="text/javascript"></script>
    <link href="Skin/css.css" rel="stylesheet" />
    <script type="text/javascript">
        function HandleWork(taskid, proinid) {
            window.open("FlowPLApply.aspx?type=Handle&taskid=" + taskid + "&proid=" + proinid );
        }
        function StartWork()
        {
            window.open("FlowPLApplyStart.aspx");
        }
    </script>
</head>
<body>

    <form id="form1" runat="server">
        <div class="content">
            顺序流程示例：
            <table width="100%" border="0" cellpadding="0" cellspacing="0" class="tb">
                <tr>
                    <td align="center">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Literal ID="lLoginInfo" runat="server"></asp:Literal>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <a href="FlowApply.aspx" target="_blank">发起新流程</a>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <a href="Login.aspx">重新登录</a>
                    </td>
                </tr>
                <tr>
                    <td>

                        <%=flowInstance %>
                    </td>
                </tr>
            </table>
            <div style="margin-top:50px;">分支流程示例：</div>
            <table width="100%" border="0" cellpadding="0" cellspacing="0" class="tb">
                <tr>
                    <td align="center">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <a href="javascript:void(0);" onclick="StartWork();">发起分支新流程</a>
                    </td>
                </tr>
                <tr>
                    <td>待办任务
                        <table  class="tblist">
                            <thead>
                            <tr>
                                <th>实例名称</th>
                                <th>活动名称</th>
                                <th>活动状态</th>
                                <th>任务状态</th>
                                <th>操作</th>
                            </tr>
                        </thead>
                        <asp:Repeater ID="Repeater1" runat="server">
                            <ItemTemplate>
                                <tr>
                                <td><%#Eval("AppName") %></td>
                                <td><%#Eval("ActivityName") %></td>
                                <td><%#Eval("ActivityState") %></td>
                                <td><%#Eval("TaskState") %></td>
                                    <td><a href="FlowPLApply.aspx?type=show" onclick="">查看</a>
                                        |<a href="javascript:void(0);" onclick="HandleWork(<%#Eval("TaskID") %>,<%#Eval("ProcessInstanceID") %>)">办理</a>
                                        |<a href="FlowList.aspx?type=withdraw&taskid=<%#Eval("TaskID") %>&appname=<%#Eval("AppName") %>&proid=<%#Eval("processguid") %>" onclick="HandleWork()">撤销</a>
                                    </td>
                                    </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        </table>
                       
                    </td>
                </tr>
                <tr>
                    <td>办理中的任务
                        <table  class="tblist">
                            <thead>
                            <tr>
                                <th>实例名称</th>
                                <th>活动名称</th>
                                <th>活动状态</th>
                                <th>任务状态</th>
                                <th>操作</th>
                            </tr>
                        </thead>
                        <asp:Repeater ID="Repeater2" runat="server">
                            <ItemTemplate>
                                <tr>
                                <td><%#Eval("AppName") %></td>
                                <td><%#Eval("ActivityName") %></td>
                                <td><%#Eval("ActivityState") %></td>
                                <td><%#Eval("TaskState") %></td>
                                    <td><a href="FlowPLApply.aspx?type=show" onclick="">查看</a>
                                        |<a href="javascript:void(0);" onclick="HandleWork(<%#Eval("TaskID") %>,<%#Eval("ProcessInstanceID") %>)">办理</a>
                                        
                                    </td>
                                    </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        </table>
                       
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>

