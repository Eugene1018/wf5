<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FlowPLApply.aspx.cs" Inherits="Wf5.WebDemo.FlowPLApply" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script type="text/javascript">
        function SubmitWork() {
            window.location.href="NextStep.aspx?days=" + document.getElementById("txtDays").value + "&roleid=" + document.getElementById("roleid").value + "&taskid=" + document.getElementById("taskid").value+"&appname="+document.getElementById("txtApplyTitle").value+"&proid="+document.getElementById("hidProId").value;
        }
    </script>
</head>
<body>
<form id="form1" runat="server">
        <div class="navTitle">当前位置：流程办理</div>
        <div style="text-align: right"><a href="Login.aspx">重新登录</a></div>
        <table width="100%" border="1px" cellpadding="0" cellspacing="0" class="tb">
            <tr style="background: #E4EEF8; color: #215181">
                <td><span class="fbold fsize16">流程信息</span></td>
            </tr>
            <tr>
                <td>
                    <table width="100%" border="0px" cellpadding="0" cellspacing="0" class="tb" id="flowApplyInfo">
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
                    <!--表单区域开始-->
                    <table height="100%" cellspacing="0" cellpadding="0" width="100%" border="0" class="tb">
                        <tr style="background: #E4EEF8; color: #215181">
                            <td>
                                <span id="Span1" runat="server" class="fbold fsize16">表单内容不用填写</span>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" style="background: #EDEDED">
                                <div id="form_body" align="left" runat="server" style="width: 100%; height: 100%; overflow: scroll; border: 1px solid #0094ff; background-color: #FFFFFF;">
                                    <p>&nbsp;</p>

                                    <table bgcolor="#ffffff" border="1" bordercolor="#000000" style="border-collapse: collapse" uetable="null" width="700">
                                        <tbody>
                                            <tr>
                                                <td style="text-align: center; width: 100%"><span style="font-family: '楷体','楷体_gb2312','simkai'; font-size: 24px">员工请假单</span></td>
                                            </tr>
                                        </tbody>
                                    </table>

                                    <p>&nbsp;</p>

                                    <table bgcolor="#ffffff" border="1" bordercolor="#000000" style="border-collapse: collapse" uetable="null" width="700">
                                        <tbody>
                                            <tr>
                                                <td align="center" bgcolor="#ffffff" height="40" style="width: 16%" valign="middle">姓名</td>
                                                <td align="center" bgcolor="#ffffff" height="40" style="width: 16%" valign="middle">
                                                    <asp:TextBox ID="leaveName" runat="server"></asp:TextBox></td>
                                                <td align="center" bgcolor="#ffffff" height="40" style="width: 16%" valign="middle">部门</td>
                                                <td align="center" bgcolor="#ffffff" height="40" style="width: 16%" valign="middle">
                                                    <asp:TextBox ID="txtDeptName" runat="server"></asp:TextBox></td>
                                                <td align="center" bgcolor="#ffffff" height="40" style="width: 16%" valign="middle">请假天数</td>
                                                <td align="center" bgcolor="#ffffff" height="40" style="width: 16%" valign="middle">
                                                    <asp:TextBox ID="txtDays" runat="server"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td align="center" bgcolor="#ffffff" height="40" style="width: 16%" valign="middle">请假类别</td>
                                                <td align="center" bgcolor="#ffffff" colspan="5" height="40" style="width: 16%" valign="middle">
                                                    <p style="text-align: left">
                                                        <select id="Select1" name="selectLeaveType" runat="server" >
                                                            <option selected="selected" value="1">事假</option>
                                                            <option value="2">病假</option>
                                                            <option value="3">婚假</option>
                                                            <option value="4">调休</option>
                                                            <option value="5">丧假</option>
                                                            <option value="6">其他</option>
                                                        </select>
                                                    </p>
                                                    <asp:DropDownList ID="ddlcount" runat="server">
                                                        <asp:ListItem Text="正常"></asp:ListItem>
                                                        <asp:ListItem Text="超量"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" bgcolor="#ffffff" height="40" style="width: 16%" valign="middle">请假时间</td>
                                                <td bgcolor="#ffffff" colspan="5" height="40" style="width: 16%" valign="middle">
                                                    <input class="Wdate" name="leaveBeginDate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" type="text" />&nbsp;&nbsp;&nbsp;&nbsp; 至&nbsp;&nbsp;
                                            <input class="Wdate" name="leaveEndDate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" type="text" /></td>
                                            </tr>
                                            <tr>
                                                <td align="center" bgcolor="#ffffff" height="40" style="width: 16%" valign="middle">事由</td>
                                                <td bgcolor="#ffffff" colspan="5" height="40" style="width: 16%" valign="middle">
                                                    <textarea cols="30" name="leaveReason" rows="3"></textarea></td>
                                            </tr>
                                            <tr>
                                                <td align="center" bgcolor="#ffffff" style="width: 16%" valign="middle"><span style="background-color: #ffffff; font-size: 14px">部门负责人审核</span></td>
                                                <td align="center" bgcolor="#ffffff" colspan="2" style="width: 16%" valign="middle">
                                                    <input name="leaveDepartmentManager" onclick="SelectDepartMember('', 'leaveDepartmentManager', 'checkMember', false)" type="text" /></td>
                                                <td align="center" bgcolor="#ffffff" style="width: 16%" valign="middle"><span style="background-color: #ffffff">主管总监</span><span style="background-color: #ffffff">审核</span></td>
                                                <td align="center" bgcolor="#ffffff" colspan="2" style="width: 16%" valign="middle">
                                                    <input name="leaveManager" onclick="SelectDepartMember('', 'leaveManager', 'checkMember', false)" type="text" /></td>
                                            </tr>
                                        </tbody>
                                    </table>

                                    <table bgcolor="#ffffff" border="0" bordercolor="#000000" style="border-collapse: collapse" uetable="null" width="700">
                                        <tbody>
                                            <tr>
                                                <td style="width: 100%">&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td style="width: 100%"><span style="white-space: normal">流程说明：</span><br style="white-space: normal" />
                                                    <br style="white-space: normal" />
                                                    <span style="white-space: normal">1、普通员工请假二天以内的（含二天），请假单由部门负责人批准,人事备档；&nbsp;</span><br style="white-space: normal" />
                                                    <br style="white-space: normal" />
                                                    <span style="white-space: normal">2、二天以上，五天以内的（含五天），经部门负责人同意后，再由主管总监批准后人事备档；</span><br style="white-space: normal" />
                                                    <br style="white-space: normal" />
                                                    <span style="white-space: normal">3、五天以上的，经部门负责人、主管总监确认后，再由副总经理和总经理批准后人事备档；</span><br style="white-space: normal" />
                                                    <br style="white-space: normal" />
                                                    <span style="white-space: normal">4、部门负责人请事假三天以内的，请假单由主管总监批准；三天以上的，经主管副总确认后，由总经理批准后人事备档；</span><br style="white-space: normal" />
                                                    <br style="white-space: normal" />
                                                    <span style="white-space: normal">5、总监和副总请假，由总经理审批后，交由人事备档；</span></td>
                                            </tr>
                                        </tbody>
                                    </table>

                                    <p>&nbsp;</p>

                                </div>
                            </td>
                        </tr>
                    </table>
                    <!--表单区域结束-->
                </td>
            </tr>
        </table>

        <!--操作区域开始-->
        <div id="form_control">
            <table class="small control_tbl" width="100%">
                <tr>
                    <td align="center" valign="middle">
                        <input type="hidden" id="formId" name="formId" value="" runat="server" />
                        <input type="hidden" id="flowGuid" name="flowGuid" value="" runat="server" />
                        <input type="hidden" id="hidUserCount" name="flowClass" value="" runat="server" />
                        <asp:HiddenField runat="server" ID="roleid" />
                        <asp:HiddenField runat="server" ID="taskid" />
                        <input type="hidden" id="hiddenStepGuid" value="" runat="server" />
                        <input type="hidden" id="hiddenStepMember" value="" runat="server" />
                        <input type="hidden" id="hidProId" value="" runat="server" />
                       <%-- <input type="submit" value="转交下一步" class="btn"  onclick="SubmitWork();"/>&nbsp;--%>
                             <asp:Button ID="btnSubmit" runat="server" class="btn" Text="转交下一步" OnClick="btnSubmit_Click" />
                        <input type="button" value="保留意见" class="btn" onclick="SaveFlowApproval(2);">&nbsp;
                           <asp:Button ID="btnCancel" runat="server" class="btn" Text="不同意" OnClick="btnCancel_Click" />&nbsp;
                            <asp:Button ID="SendBack" runat="server" class="btn" Text="退回" OnClick="SendBack_Click" />&nbsp;
                            <%--<input type="button" value="流程步骤" class="btn" onclick="ShowOpinion(<%=instanceId%>);">&nbsp;--%>
                    
                            
                    </td>
                </tr>
            </table>
        </div>
        <!--操作区域结束-->
    </form>
</body>
</html>
