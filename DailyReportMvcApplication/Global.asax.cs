using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.WebPages;
using DailyReportMvcApplication.Models;

namespace DailyReportMvcApplication
{
    // メモ: IIS6 または IIS7 のクラシック モードの詳細については、
    // http://go.microsoft.com/?LinkId=9394801 を参照してください

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Database.SetInitializer<DailyReportContext>(new DailyReportInitializer());

        }


        protected void Session_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            //統合認証でログインしたユーザIDを取得
            string identityName = HttpContext.Current.User.Identity.Name;
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

        private object View()
        {
            throw new NotImplementedException();
        }


    }
}