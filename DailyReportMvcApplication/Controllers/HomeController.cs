using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DailyReportMvcApplication.Models;

namespace DailyReportMvcApplication.Controllers
{
    [LoginCheckFilterAttribute]
    public class HomeController : Controller
    {
        private static log4net.ILog log4 = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();
        public ActionResult Index()
        {
            //log4.Debug("log4net debug");
            //nlog.Debug("Info logging");
            ViewBag.Message = "ASP.NET MVC アプリケーションを簡単に始めるには、このテンプレートを変更してください。";
            //SetUserInfo();

            return View();

        }

        private void SetUserInfo()
        {
            //統合認証でログインしたユーザIDを取得
            string identityName = this.ControllerContext.HttpContext.User.Identity.Name;
            string[] delimiter = { @"\" };
            string[] identityNameArray = identityName.Split(delimiter, StringSplitOptions.None);
            //DBから上記IDのユーザ名を取得
            string query =
              "SELECT VALUE a FROM DailyReportContext.Employees AS a "
              + "WHERE a.user_id = @user_id";
            List<ObjectParameter> parameters = new List<ObjectParameter>();
            parameters.Add(new ObjectParameter("user_id", int.Parse(identityNameArray[1])));

            DailyReportContext db = new DailyReportContext();
            var objectContext = new DailyReportContext().ObjectContext();
            //  CreateQueryメソッドでクエリを呼び出す
            ObjectQuery<Employee> entries = objectContext.CreateQuery<Employee>(query, parameters.ToArray());
            foreach (Employee entry in entries)
            {
                //ユーザ情報をセッションに詰める
                Session["UserInfo"] = entry;
                return;
            }

            //Employeeに該当user_idが登録されていない場合はユーザIDだけセットしてセッションに詰める
            Employee enp = new Employee
            {
                user_id = int.Parse(identityNameArray[1]),
                user_name = "????",
            };
            Session["UserInfo"] = enp;

        }

        public ActionResult About()
        {
            ViewBag.Message = "アプリケーション説明ページ。";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "連絡先ページ。";

            return View();
        }

    }
}
