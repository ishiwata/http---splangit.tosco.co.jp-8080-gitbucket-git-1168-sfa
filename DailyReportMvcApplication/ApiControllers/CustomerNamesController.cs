using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DailyReportMvcApplication.Models;

namespace DailyReportMvcApplication.ApiControllers
{
    public class CustomerNamesController : ApiController
    {
        // GET api/customernames
        public string Get()
        {
            return null;
        }

        // GET api/customernames/5
        public string Get(string q)
        {
          System.Diagnostics.Debug.WriteLine("クエリ = " + q);
          DailyReportContext db = new DailyReportContext();
          var query = from cus in db.Customers
                      where cus.customer_yomi.StartsWith(q)
                        orderby cus.customer_name
                        select cus;

            System.Diagnostics.Debug.WriteLine("All blogs in the database:");
            string returnStr = "";
            foreach (var item in query)
            {
                System.Diagnostics.Debug.WriteLine(item.customer_name);
                returnStr = returnStr + item.customer_name + "|" + item.id + "|" + item.owner + "\n";
            }

            return returnStr;
        }

        // POST api/customernames
        public void Post([FromBody]string value)
        {
        }

        // PUT api/customernames/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/customernames/5
        public void Delete(int id)
        {
        }
    }
}
