using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using DailyReportMvcApplication.Models;

namespace DailyReportMvcApplication.Controllers
{
    public class LoginCheckFilterAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Employee enp = (Employee)filterContext.HttpContext.Session["UserInfo"];
            if("????".Equals(enp.user_name)){
                filterContext.HttpContext.Response.Redirect("/LoginError.html");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}