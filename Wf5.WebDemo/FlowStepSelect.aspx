<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FlowStepSelect.aspx.cs" Inherits="Wf5.WebDemo.FlowStepSelect" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <link href="Skin/css.css" rel="stylesheet" />
    <script src="js/jquery-1.8.0.min.js" type="text/javascript"></script>

    <script src="../../Js/common.js" type="text/javascript"></script>
    <script type="text/javascript">

        function SelectOK() {
            $("#hiddenOK").val("");
            var step = $(':radio[name="radioStep"]:checked').val();
            var user = $(':radio[name="radioUser"]:checked').val();
            if (step == undefined || step == null || step == "" || step == "0") {
                alert("未选择步骤！");
                return;
            }

            if (user == undefined || user == null || user == "" || user == "0") {
                alert("未选择办理人员！");
                return;
            }

            $("#hiddenStepGuid").val(step);
            $("#hiddenStepUser").val(user);

            $("#hiddenOK").val("OK");

            CloseWindowPage();
        }

        //关闭窗口
        function CloseWindowPage() {
            var index = parent.layer.getFrameIndex(window.name);
            parent.layer.close(index);
        }

    </script>

</head>
<body>
    <form id="form1" name="form1" runat="server">
        <div class="content">
            <div style="float: left; height: 300px;">
                <table width="550px" border="0px" cellpadding="0" cellspacing="0" class="tb">
                    <tr valign="middle" style="background: #EEEEEE">
                        <td align="left" width="50%" height="30px" align="left" class="fbold fsize14">转到步骤</td>
                        <td align="left" height="30px" align="left" class="fbold fsize14">办理人员</td>
                    </tr>
                    <tr valign="top">
                        <td align="left">
                            <span id="spanStep" runat="server">
                                <%=stepList %>
                            </span>
                        </td>
                        <td align="left">
                            <span id="spanMember" runat="server">
                                <%=userList %>
                            </span>
                        </td>
                    </tr>
                </table>
            </div>

            <div style="margin-top: 20px; float: right; text-align: right; line-height: 30px;">
                <input type="hidden" id="hiddenStepGuid" value="" runat="server" />
                <input type="hidden" id="hiddenStepUser" value="" runat="server" />
                <input type="hidden" id="hiddenOK" value="" runat="server" />

                <input type="button" value="确定" class="btn" onclick="SelectOK();" title="确定" />&nbsp;&nbsp;
                <input type="button" value="取消" class="btn" onclick="CloseWindowPage();" title="取消" />
            </div>
        </div>
    </form>
</body>
</html>
