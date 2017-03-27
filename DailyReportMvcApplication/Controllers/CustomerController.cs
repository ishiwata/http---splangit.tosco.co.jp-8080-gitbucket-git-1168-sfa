using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using DailyReportMvcApplication.Models;

namespace DailyReportMvcApplication.Controllers
{

    [LoginCheckFilterAttribute]
    public class CustomerController : Controller
    {
        private DailyReportContext db = new DailyReportContext();

        //
        // GET: /Customer/
        public ActionResult Index(int page = 1)
        {
            ViewBag.page = page;
            if (Session["CustomerCondition"] != null)
            {
                System.Diagnostics.Debug.WriteLine("セッションあり");
                Customer seacCond = (Customer)(Session["CustomerCondition"]);
                ObjectQuery<Customer> entries = searchData(seacCond);
                ViewBag.SearchCondition = seacCond;
                return View("index", entries);

            } else {
                return View(db.Customers.OrderBy(w => w.customer_yomi).ToList());
            }
        }

        //
        // GET: /Customer/Details/5
        public ActionResult Details(int id = 0, int page = 1)
        {
            ViewBag.page = page;
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        //
        // GET: /Customer/Create
        public ActionResult Create(int page =1)
        {
            ViewBag.page = page;
            setEmployeeList();
            return View();
        }

        //
        // GET: /Customer/Create{dailyflg}
        public ActionResult DailyCallCreate()
        {
            setEmployeeList(); 
            ViewBag.DailyCall = "true"; 
            return View("Create");
        }

        [HttpPost]
        [ActionName("Index")]
        [ValidateAntiForgeryToken]
        [SubmitCommand("Search")]
        public ActionResult Search(Customer customer)
        {
            ObjectQuery<Customer> entries = searchData(customer);
            Session.Add("CustomerCondition", customer);
            return View("index", entries);

        }

        // POST: /Customer/Download
        [HttpPost]
        [ActionName("Index")]
        [ValidateAntiForgeryToken]
        [SubmitCommand("Download")]
        public FileStreamResult Download(Customer customer)
        {
            //検索条件組立
            ObjectQuery<Customer> entries = searchData(customer);
            Session.Add("CustomerCondition", customer);
            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(List<Customer>));

            List<Customer> data = new List<Customer>();
            foreach (Customer rec in entries)
            {
                data.Add(rec);
            }
            serializer.Serialize(stream, data);
            stream.Position = 0;
            return File(stream, "application/vnd.ms-excel", "顧客一覧.xls");
        }

        //
        // POST: /Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer customer, int page = 1)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index", new { page = page });
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DailyCallCreate(Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                return View("Dummy");
            }

            return View(customer);
        }

        //
        // GET: /Customer/Edit/5

        public ActionResult Edit(int id = 0, int page = 1)
        {
            ViewBag.page = page;
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            setEmployeeList();

            return View(customer);
        }

        //
        // POST: /Customer/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer customer,int page = 1)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        //
        // GET: /Customer/Delete/5

        public ActionResult Delete(int id = 0, int page = 1)
        {
            ViewBag.page = page;
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        //
        // POST: /Customer/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int page = 1)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index", new { page = page });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

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

        private ObjectQuery<Customer> searchData(Customer customer)
        {
            ViewBag.SearchCondition = customer;

            List<ObjectParameter> parameters = new List<ObjectParameter>();
            //  Entity SQL文字列の設定
            string query =
              "SELECT VALUE a FROM DailyReportContext.Customers AS a "
              + "WHERE 1 = 1";

            //Where句の組立
            if (customer.customer_name != null)
            {
                //バインド変数が使いたいのだが対処法わからず
                query = query + " AND a.customer_name Like '%" + customer.customer_name + "%'";
            }
            if (customer.customer_yomi != null)
            {
                query = query + " AND a.customer_yomi Like '%" + customer.customer_yomi + "%'";
            }
            if (customer.sales_department != null)
            {
                query = query + " AND a.sales_department Like '%" + customer.sales_department + "%'";

            }
            if (customer.owner != null)
            {
                query = query + " AND a.owner Like '%" + customer.owner + "%'";
            }

            if (customer.category != null)
            {
                query = query + " AND a.category = '" + customer.category + "'";
            }

            query = query + " ORDER BY a.customer_yomi ASC";

            DailyReportContext db = new DailyReportContext();
            var objectContext = new DailyReportContext().ObjectContext();
            //  CreateQueryメソッドでクエリを呼び出す
            ObjectQuery<Customer> entries = objectContext.CreateQuery<Customer>(query, parameters.ToArray());

            System.Diagnostics.Debug.WriteLine(entries.ToTraceString());
            return entries;
        }
    }
}