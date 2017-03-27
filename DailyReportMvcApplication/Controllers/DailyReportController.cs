using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using DailyReportMvcApplication.Models;

namespace DailyReportMvcApplication.Controllers
{
     //Actionフィルタの定義
     [LoginCheckFilterAttribute]
     [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
      public class SubmitCommandAttribute : ActionMethodSelectorAttribute
      {
        private string _submitName;
        private string _submitValue;
        private static readonly AcceptVerbsAttribute _innerAttribute = 
                                     new AcceptVerbsAttribute(HttpVerbs.Post);
    
        public SubmitCommandAttribute(string name) : this(name, string.Empty) { }
            public SubmitCommandAttribute(string name, string value)
        {
              _submitName = name;
              _submitValue = value;
        }

        public override bool IsValidForRequest(ControllerContext controllerContext, 
                                               MethodInfo methodInfo)
        {
          if (!_innerAttribute.IsValidForRequest(controllerContext, methodInfo))
            return false;

          // Form Value
          var submitted = controllerContext.RequestContext
                                           .HttpContext
                                           .Request.Form[_submitName];
          return string.IsNullOrEmpty(_submitValue)
                   ? !string.IsNullOrEmpty(submitted)
                   : string.Equals(submitted, _submitValue, 
                                   StringComparison.InvariantCultureIgnoreCase);
        }
      }


    public class DailyReportController : Controller
    {
        private DailyReportContext db = new DailyReportContext();
        private string customerIdError = "顧客コードは必須入力です。顧客名を選択すると自動入力されますが、該当顧客名が補完されない場合は顧客マスタを追加してください。";
        
        //
        // GET: /DailyReport/

        public ActionResult Index(int page = 1)
        {
            ViewBag.page = page;
            if (Session["DailyReportCondition"] != null)
            {
                System.Diagnostics.Debug.WriteLine("セッションあり");
                DailyReport condition = (DailyReport)(Session["DailyReportCondition"]);
                ObjectQuery<DailyReport> entries = searchData(condition);
                ViewBag.SearchCondition = condition;
                return View("index", entries);

            }
            else
            {
                //デフォルトのソート条件はidの降順
                return View(db.DailyReports.OrderByDescending(w => w.id).ToList());
            }
        }

        //
        // GET: /DailyReport/Details/5
        public ActionResult Details(int id = 0, int page =1)
        {
            ViewBag.page = page;
            DailyReport dailyreport = db.DailyReports.Find(id);
            if (dailyreport == null)
            {
                return HttpNotFound();
            }
            return View(dailyreport);
        }

        //
        // GET: /DailyReport/Create
        public ActionResult Create(int page = 1)
        {
            ViewBag.page = page;
            setEmployeeList();
            //コピー対応
            if (TempData["copydata"] != null)
            {
                DailyReport srcDailyreport = (DailyReport)TempData["copydata"];
                Employee UserInfo = (Employee)Session["UserInfo"];
                srcDailyreport.user_id = UserInfo.user_id + ":" + UserInfo.user_name;
                srcDailyreport.contact_date = null;
                srcDailyreport.contact_time_start = null;
                srcDailyreport.contact_time_end = null;
                return View(srcDailyreport);
            }
   
            return View();
        }

        //
        // POST: /DailyReport/Search
        [HttpPost]
        [ActionName("Index")]
        [ValidateAntiForgeryToken]
        [SubmitCommand("Search")]
        public ActionResult Search(DailyReport dailyreport)
        {
            ObjectQuery<DailyReport> entries = searchData(dailyreport);
            Session.Add("DailyReportCondition", dailyreport);
            return View("index", entries);

        }


        // POST: /DailyReport/Download
        [HttpPost]
        [ActionName("Index")]
        [ValidateAntiForgeryToken]
        [SubmitCommand("Download")]
        public FileStreamResult Download(DailyReport dailyreport)
        {
            ObjectQuery<DailyReport> entries = searchData(dailyreport);
            Session.Add("DailyReportCondition", dailyreport);

            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(List<DailyReport>));

            List<DailyReport> data = new List<DailyReport>();
            foreach (DailyReport rec in entries)
            {
                data.Add(rec);
            }
            serializer.Serialize(stream, data);
            stream.Position = 0;
            //We return the XML from the memory as a .xls file
            ViewBag.SearchCondition = dailyreport;
            return File(stream, "application/vnd.ms-excel", "活動報告.xls");
        }

        //
        // POST: /DailyReport/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DailyReport dailyreport, int page = 1)
        {
            setEmployeeList();
            if (ModelState.IsValid)
            {
                //customer_id必須チェック
                if (dailyreport.customer_id == 0 || !ExistenceCustomer(dailyreport))
                {
                    //エラーメッセージをセット
                    ModelState.AddModelError("customer_id", customerIdError);
                    //エラー発生時のフォームの内容を再現
                    return View(InitRequest(dailyreport));
                }
                //エラーが無かったら登録して一覧に戻る
                db.DailyReports.Add(dailyreport);
                db.SaveChanges();
                return RedirectToAction("Index", new { page = page });
            }

            return View(dailyreport);
        }

        //
        // GET: /DailyReport/Edit/5

        public ActionResult Edit(int id = 0, int page = 1)
        {
            ViewBag.page = page;
            DailyReport dailyreport = db.DailyReports.Find(id);
            if (dailyreport == null)
            {
                return HttpNotFound();
            }

            setEmployeeList();
            
            return View(dailyreport);
        }

        //
        // POST: /DailyReport/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DailyReport dailyreport, int page = 1)
        {
            setEmployeeList();
            if (ModelState.IsValid)
            {
                if (dailyreport.customer_id == 0 || !ExistenceCustomer(dailyreport))
                {
                    //エラーメッセージをセット
                    ModelState.AddModelError("customer_id", customerIdError);
                    DailyReport returnRequest = InitRequest(dailyreport);
                    returnRequest.id = dailyreport.id;
                    return View(returnRequest);
                }

                db.Entry(dailyreport).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { page = page });
            }
            return View(dailyreport);
        }

        //
        // GET: /DailyReport/Delete/5

        public ActionResult Delete(int id = 0, int page = 1)
        {
            ViewBag.page = page;
            DailyReport dailyreport = db.DailyReports.Find(id);
            if (dailyreport == null)
            {
                return HttpNotFound();
            }
            return View(dailyreport);
        }

        //
        // GET: /DailyReport/Copy/5
        
        public ActionResult Copy(int id = 0)
        {
            DailyReport dailyreport = db.DailyReports.Find(id);
            if (dailyreport == null)
            {
                return HttpNotFound();
            }
            //コピーの場合、詳細表示データを一時データ領域に保存し、登録アクションにリダイレクト
            TempData["copydata"] = dailyreport;
            return RedirectToAction("Create");
        }

        //
        // POST: /DailyReport/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int page = 1)
        {
            DailyReport dailyreport = db.DailyReports.Find(id);
            db.DailyReports.Remove(dailyreport);
            db.SaveChanges();
            return RedirectToAction("Index", new { page = page });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        //検索条件のQuery組立
        private ObjectQuery<DailyReport> searchData(DailyReport dailyreport)
        {
            ViewBag.SearchCondition = dailyreport;

            List<ObjectParameter> parameters = new List<ObjectParameter>();
            //  Entity SQL文字列の設定
            string query =
              "SELECT VALUE a FROM DailyReportContext.DailyReports AS a "
              + "WHERE 1 = 1";

            //Where句の組立
            if (dailyreport.subject != null)
            {
                //バインド変数が使いたいのだが対処法わからず
                //query = query + " AND a.subject Like @subject";
                //parameters.Add(new ObjectParameter("subject", dailyreport.subject));
                query = query + " AND a.subject Like '%" + dailyreport.subject + "%'";
            }
            if (dailyreport.owner != null)
            {
                //query = query + " AND a.user_id = @user_id";
                //parameters.Add(new ObjectParameter("user_id", 1168));
                query = query + " AND a.owner Like '%" + dailyreport.owner + "%'";
            }
            if (dailyreport.customer != null)
            {
                //query = query + " AND a.customer Like @customer";
                //parameters.Add(new ObjectParameter("customer", dailyreport.customer));
                query = query + " AND a.customer Like '%" + dailyreport.customer + "%'";

            }
            if (dailyreport.customer_attendee != null)
            {
                //query = query + " AND a.customer_attendee Like '%@customer_attendee%'";
                //parameters.Add(new ObjectParameter("customer_attendee", dailyreport.customer_attendee));
                query = query + " AND a.customer_attendee Like '%" + dailyreport.customer_attendee + "%'";
            }
            if (dailyreport.attendee != null)
            {
                //query = query + " AND a.attendee Like @attendee";
                //parameters.Add(new ObjectParameter("attendee", "'%" + dailyreport.attendee + "%'"));
                query = query + " AND a.attendee Like '%" + dailyreport.attendee + "%'";
            }
            if (dailyreport.contact_date != null)
            {
                query = query + " AND a.contact_date = @contact_date";
                parameters.Add(new ObjectParameter("contact_date", dailyreport.contact_date));
            }
            if (dailyreport.description != null)
            {
                //TO DO http://support.microsoft.com/kb/2252955/ja フルテキスト検索化
                //query = query  + " AND a.description Like '%@description%'";
                //parameters.Add(new ObjectParameter("description", dailyreport.description));
                query = query + " AND a.description Like '%" + dailyreport.description + "%'";
            }
            if (dailyreport.speciation != null)
            {
                query = query + " AND a.speciation = @speciation";
                parameters.Add(new ObjectParameter("speciation", dailyreport.speciation));
            }
            query = query + " ORDER BY a.id DESC";

            DailyReportContext db = new DailyReportContext();
            var objectContext = new DailyReportContext().ObjectContext();
            //  CreateQueryメソッドでクエリを呼び出す
            ObjectQuery<DailyReport> entries = objectContext.CreateQuery<DailyReport>(query, parameters.ToArray());

            System.Diagnostics.Debug.WriteLine(entries.ToTraceString());
            return entries;
        }

        //顧客存在チェック
        private bool ExistenceCustomer(DailyReport dailyreport)
        {
            Customer customers = db.Customers.Find(dailyreport.customer_id);
            if(!customers.customer_name.Equals(dailyreport.customer)){
                return false;
            }

            return true;
        }

        //(DailyReportのディープコピー
        private DailyReport InitRequest(DailyReport dailyreport)
        {
            DailyReport returnDailyreport = new DailyReport
            {
                subject = dailyreport.subject,
                user_id = dailyreport.user_id,
                customer_id = 0,
                customer = dailyreport.customer,
                owner = dailyreport.owner,
                customer_attendee = dailyreport.attendee,
                attendee = dailyreport.attendee,
                contact_date = dailyreport.contact_date,
                contact_time_start = dailyreport.contact_time_start,
                contact_time_end = dailyreport.contact_time_end,
                description = dailyreport.description,
                speciation = dailyreport.speciation
            };

            return returnDailyreport;

        }

        //担当者選択リストボックスの内容を取得
        private void setEmployeeList()
        {
            List<Employee> employees = db.Employees.OrderBy(w => w.user_id).ToList();
            List<SelectListItem> hoge = new List<SelectListItem>();
            foreach (Employee enp in employees)
            {
                hoge.Add(new SelectListItem() { Value = enp.user_id + ":" + enp.user_name, Text = enp.user_name });
            }
            ViewBag.employees = hoge.ToArray<SelectListItem>();
        }

    }
}