using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Web;

namespace DailyReportMvcApplication.Models
{

    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer<DailyReportContext>(new DailyReportInitializer());

            // TODO: Make this program do something!
        }
    }

    public class DailyReportContext : DbContext
    {
        public DbSet<DailyReport> DailyReports { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Hoge> Hoges { get; set; }

        public ObjectContext ObjectContext()
        {
            return (this as IObjectContextAdapter).ObjectContext;
        }
    }

    public class DailyReportInitializer : DropCreateDatabaseIfModelChanges<DailyReportContext>
    {
        protected override void Seed(DailyReportContext context)
        {
            var customers = new List<Customer> {

        new Customer { id = 1,
                   customer_name = "トッパン・フォームズ（関西事業本部）",
                   customer_yomi = "とっぱんふぉーむず(かんさいじぎょうほんぶ)", 
                   owner = "830:藤木　昌史"},
                   

        new Customer {
                   customer_name = "ＮＥＣシステムテクノロジー株式会社（松山）",
                   customer_yomi = "えぬえいーしーしすてむてくのろじー（まつやま）",
                   owner = "877:有我　昌巳"},
        new Customer {
                   customer_name = "ＮＥＣシステムテクノロジー株式会社（高松）",
                   customer_yomi = "えぬえいーしーしすてむてくのろじー（たかまつ）",
                   owner = "58:大室　友伸"},
        new Customer {
                   customer_name = "キヤノンITソリューションズ(大阪)",
                   customer_yomi = "きやのんあいてぃそりゅーしょんず（おおさか）" ,
                   owner = "1168:石渡 敦"},
        new Customer {
                   customer_name = "シャープ株式会社",
                   customer_yomi = "しゃーぷ",
                   owner = "235:初村　啓二"},
      };
            customers.ForEach(b => context.Customers.Add(b));
            context.SaveChanges();


            var employees = new List<Employee>
      {
       //new Employee { user_id = 1168, user_name = "石渡 敦"},
       new Employee { user_id = 830, user_name="藤木　昌史"},
       new Employee { user_id = 719, user_name="坂田　一利"},
       new Employee { user_id = 877, user_name="有我　昌巳"},
       new Employee { user_id = 58, user_name="大室　友伸"},
       new Employee { user_id = 235, user_name="初村　啓二"},
       new Employee { user_id = 462, user_name="高木　勝"},
       new Employee { user_id = 553, user_name="前田　純子"},
       new Employee { user_id = 623, user_name="佐藤　正晃"},
       new Employee { user_id = 918, user_name="福本　寛"},
       new Employee { user_id = 1089, user_name="川本　良"},
       new Employee { user_id = 1477, user_name="平山　浩二"},
       new Employee { user_id = 1522, user_name="三宅　博公"},
       new Employee { user_id = 288, user_name="濱中　一公"},
       new Employee { user_id = 268, user_name="下石　和久"},
       new Employee { user_id = 727, user_name="松本　好隆"},
       new Employee { user_id = 1228, user_name="青山　武司"},
       new Employee { user_id = 1335, user_name="長谷川　智記"},
       new Employee { user_id = 1436, user_name="花井　秀行"},
       new Employee { user_id = 1476, user_name="河野　一弥"},
       new Employee { user_id = 450, user_name="安原　秀昭"},
       new Employee { user_id = 438, user_name="片山　警二"},
       new Employee { user_id = 347, user_name="小笠原　啓二"},
       new Employee { user_id = 111, user_name="久森 正治"},
       new Employee { user_id = 268, user_name="下石　和久"},
       new Employee { user_id = 848, user_name="平石　宣博"},
       new Employee { user_id = 1101, user_name="金高　由紀子"},
       new Employee { user_id = 1247, user_name="秋本　大介"},
       new Employee { user_id = 1380, user_name="木下　雄介"},
       new Employee { user_id = 1381, user_name="長澤　一毅"},
       new Employee { user_id = 1438, user_name="山口　健太"},
       new Employee { user_id = 1457, user_name="金持　奈那子"},
       new Employee { user_id = 1480, user_name="阿知波　亮"},
       new Employee { user_id = 1502, user_name="岡島　慎吾"},
       new Employee { user_id = 1505, user_name="橋本　美帆"},
      };

            employees.ForEach(b => context.Employees.Add(b));
            context.SaveChanges();
        }
    }
}