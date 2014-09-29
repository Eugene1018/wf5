using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Data;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Configuration;

namespace Wf5.WebDemo.Common
{
    public class Helper
    {

        /// <summary>
        /// 绑定DropDownList
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="dt"></param>
        /// <param name="textField"></param>
        /// <param name="valueField"></param>
        public static void BindDropDownList(DropDownList ddl, DataTable dt, string textField, string valueField)
        {
            BindDropDownList(ddl, dt, textField, valueField,false,"","");
        }

        /// <summary>
        /// 绑定DropDownList
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="dt"></param>
        /// <param name="textField"></param>
        /// <param name="valueField"></param>
        public static void BindDropDownList(DropDownList ddl, DataTable dt, string textField, string valueField, bool isInsertDefaultItem, string defaultItemText, string defaultItemValue)
        {
            ddl.DataSource = dt;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
            if (isInsertDefaultItem)
                ddl.Items.Insert(0, new ListItem(defaultItemText, defaultItemValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int GetLoginUserId()
        {
            if (HttpContext.Current.Session["UserId"] != null)
            {
                return Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
            }
            return 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetLoginUserName()
        {
            return HttpContext.Current.Session["UserName"] == null ? string.Empty : HttpContext.Current.Session["UserName"].ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int GetLoginRoleId()
        {
            return HttpContext.Current.Session["RoleId"] == null ? 0 : Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetLoginRoleName()
        {
            return HttpContext.Current.Session["RoleName"] == null ? string.Empty : HttpContext.Current.Session["RoleName"].ToString();
        }

        /// <summary>
        /// 对象反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T JsonToObject<T>(string json)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                throw new Exception("X2Jason.JsonToObj(): " + ex.Message);
            }
        }

        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjectToJson(object obj)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(obj);
        }

    }

}