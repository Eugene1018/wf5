<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FlowApply.aspx.cs" Inherits="Wf5.WebDemo.FlowApply" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="Skin/css.css" rel="stylesheet" />

    <link href="js/layer/skin/layer.css" rel="stylesheet" />
    <script src="js/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="js/layer/layer.js" type="text/javascript"></script>
    <script src="js/json2.js" type="text/javascript"></script>


    <script type="text/javascript">
        function SaveFlowApply() {
            //准备流程发起的基本数据
            var flowLevel = $("#selectLevel").val();//重要等级
            var applyTitle = $("#txtApplyTitle").val();//自定义标题
            var applyReason = $("#txtApplyReason").val();//申请理由
            var flowGuid = $("#selectFlowGuid").val();
            var flowNumber = $("#flowNumber").val();
            var fiNo = $("#txtFlowNo").val();

            if (applyTitle == null || applyTitle == "") { alert("请输入自定义标题"); return; }
            if (applyReason == null || applyReason == "") { alert("请输入申请理由"); return; }
            if (fiNo == null || fiNo == "") { alert("请输入流程编号"); return; }


            //开始选择流程的步骤及办理的人员
            var urlParameter = "flowGuid=" + flowGuid + "&&Step=start";
            $.layer({
                type: 2,
                closeBtn: [0, true],
                shadeClose: false,
                shade: [0],
                border: [5, 0.3, '#000', true],
                offset: ['10px', ''],
                area: ['420px', '410px'],
                title: '转下一步 - 选择步骤及办理人',
                iframe: { src: 'FlowStepSelect.aspx?' + urlParameter },
                close: function (index) {
                    layer.close(index);
                },
                beforeClose: function (index) {
                    var selectOK = layer.getChildFrame('#hiddenOK', index).val();
                    if (selectOK == "OK") {
                        var nextActivityGuid = layer.getChildFrame('#hiddenStepGuid', index).val();//选中的步骤ID
                        var nextActivityUserId = layer.getChildFrame('#hiddenStepUser', index).val();//步骤办理人员ID

                        if (nextActivityGuid != undefined && nextActivityGuid != null && nextActivityGuid != "" && nextActivityUserId != undefined && nextActivityUserId != null && nextActivityUserId != "") {

                            var FlowInstanceInfo = {
                                fiFlowDefinitionGuid: flowGuid,
                                fiObjectId: flowNumber,
                                fiNo: fiNo,
                                fiLevel: flowLevel,
                                fiTitle: applyTitle,
                                fiApplyReason: applyReason
                            };

                            $.ajax({
                                type: "post",
                                dataType: "json",
                                async: false,
                                url: "Handler/FlowApply.ashx",
                                data: { Action: "FlowApply", nextActivityGuid: nextActivityGuid, nextActivityUserId: nextActivityUserId, flowInstanceInfo: JSON.stringify(FlowInstanceInfo) },
                                error: function (errMsg) { alert("错误：" + errMsg); },
                                success: function (msg) {
                                    if (msg != null && msg != "" && msg == "success") {
                                        alert("提交成功！");
                                    } else {
                                        alert("提交失败！");
                                    }
                                }
                            });
                        } else {
                            ///  alert("未选择 转到步骤  或 办理人员,无法提交流程！");
                            //  return;
                        }
                    }
                },
                end: function () {
                    return true;
                }
            });
        }

    </script>

</head>
<body>
    <div class="content">
        <form id="form1" name="form1" runat="server">
            <div class="navTitle">当前位置：流程办理</div>

            <table width="100%" border="1px" cellpadding="0" cellspacing="0" class="tb">
                <tr style="background: #E4EEF8; color: #215181">
                    <td><span class="fbold fsize16">流程信息</span></td>
                </tr>
                <tr>
                    <td>
                        <table width="100%" border="0px" cellpadding="0" cellspacing="0" class="tb" id="flowApplyInfo">
                            <tr>
                                <td width="13%" align="right" class="bgcolor1 fsize12 fbold">流程编号：</td>
                                <td>
                                    <input type="text" id="txtFlowNo" runat="server" class="input wd300" />
                                </td>
                                <td width="10%" align="right" class="bgcolor1 fsize12 fbold">重要等级：</td>
                                <td>
                                    <select id="selectLevel" name="selectLevel" runat="server" class="select">
                                        <option value="0">普通</option>
                                        <option value="1">重要</option>
                                        <option value="2">非常重要</option>
                                    </select>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" class="bgcolor1 fsize12 fbold">自定义流程标题：</td>
                                <td>
                                    <input id="txtApplyTitle" name="txtApplyTitle" runat="server" class="input wd300" obj="not-null" /></td>
                                <td align="right" class="bgcolor1 fsize12 fbold">申请理由：</td>
                                <td>
                                    <input id="txtApplyReason" name="txtApplyReason" runat="server" class="input wd300" />
                                </td>
                            </tr>
                            <tr>
                                <td align="right" class="bgcolor1 fsize12 fbold">表单编号</td>
                                <td align="left">
                                    <input type="text" id="flowNumber" runat="server" readonly="readonly" class="input wd300" value="1" />
                                </td>
                                <td align="right" class="bgcolor1 fsize12 fbold">流程:</td>
                                <td>
                                    <input type="text" id="selectFlowGuid" runat="server" value="8593aa9c-1c0c-45d0-8ad2-e025a1c74fa6" readonly="readonly" class="input wd300" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>


            <!--表单区域开始-->
            <table height="100%" cellspacing="0" cellpadding="0" width="100%" border="0" class="tb">
                <tr style="background: #E4EEF8; color: #215181">
                    <td>
                        <span runat="server" class="fbold fsize16">表单内容不用填写</span>
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
                                            <input name="leaveName" type="text" /></td>
                                        <td align="center" bgcolor="#ffffff" height="40" style="width: 16%" valign="middle">部门</td>
                                        <td align="center" bgcolor="#ffffff" height="40" style="width: 16%" valign="middle">
                                            <input name="leaveDepartment" onclick="SelectDepartMember('', 'leaveDepartment', 'checkDepartment', false)" type="text" /></td>
                                        <td align="center" bgcolor="#ffffff" height="40" style="width: 16%" valign="middle">请假天数</td>
                                        <td align="center" bgcolor="#ffffff" height="40" style="width: 16%" valign="middle">
                                            <input name="leaveDays" type="text" /></td>
                                    </tr>
                                    <tr>
                                        <td align="center" bgcolor="#ffffff" height="40" style="width: 16%" valign="middle">请假类别</td>
                                        <td align="center" bgcolor="#ffffff" colspan="5" height="40" style="width: 16%" valign="middle">
                                            <p style="text-align: left">
                                                <select name="selectLeaveType">
                                                    <option selected="selected" value="1">事假</option>
                                                    <option value="2">病假</option>
                                                    <option value="3">婚假</option>
                                                    <option value="4">调休</option>
                                                    <option value="5">丧假</option>
                                                    <option value="6">其他</option>
                                                </select>
                                            </p>
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


            <!--操作区域开始-->
            <div id="form_control">
                <table class="small control_tbl" width="100%">
                    <tr>
                        <td align="center" valign="middle">
                            <input type="hidden" id="formId" name="formId" value="" runat="server" />
                            <input type="hidden" id="flowGuid" name="flowGuid" value="" runat="server" />
                            <input type="hidden" id="flowClass" name="flowClass" value="" runat="server" />

                            <input type="hidden" id="hiddenStepGuid" value="" runat="server" />
                            <input type="hidden" id="hiddenStepMember" value="" runat="server" />

                            <input type="button" value="转交下一步" class="btn" onclick="SaveFlowApply();">&nbsp;
                            
                        </td>
                    </tr>
                </table>
            </div>
            <!--操作区域结束-->


        </form>
    </div>
</body>
</html>
