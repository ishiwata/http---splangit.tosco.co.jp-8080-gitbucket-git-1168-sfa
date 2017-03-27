using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DailyReportMvcApplication.Models
{
    public class DailyReport
    {
        //[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int id { get; set; }
        [Required(ErrorMessage = "表題は必須入力です。")]
        public string subject { get; set; }	//表題　必須
        public string user_id { get; set; } //作成者（自動入力）
        //[Required(ErrorMessage = "顧客コードは必須入力です。顧客名を選択すると自動入力されますが、該当顧客名が補完されない場合は顧客マスタを追加してください。")]
        public int customer_id { get; set; }	//顧客コード
        public string customer { get; set; }	//顧客名
        public string owner { get; set; }	//営業主担当
        public string customer_attendee { get; set; }	//相手参加者
        public string attendee { get; set; }	//自社参加者（取り込み）　必須
        public string contact_date { get; set; }	//日付（デフォルト当日日付）必須
        public string contact_time_start { get; set; }	//時刻開始
        public string contact_time_end { get; set; }	//時刻終了
        public string description { get; set; }	//内容
        public string speciation { get; set; }	//種別（往訪・来訪・メール・電話）
        public string ToString()
        {
            return id + ",\"" + subject + "\",\"" + user_id + "\"," + customer_id + ",\"" + customer + "\",\"" + owner + "\",\"" + customer_attendee + "'," + attendee + "\",\"" + contact_date + "\",\"" + contact_time_start + "\",\"" + contact_time_end + "\",\"" + description + "\",\"" + speciation + "\""; 
        }

    }

    public class Customer
    {
        [Key]
        public int id { get; set; }
        [Required(ErrorMessage = "顧客名は必須入力です。")]
        public string customer_name { get; set; }	//顧客名 必須
        [Required(ErrorMessage = "顧客よみは必須入力です。")]
        public string customer_yomi { get; set; } //顧客よみ
        public string department { get; set; }	//部課
        public string flag { get; set; }	//フラグ
        public string sales_department { get; set; }	//営業部署
        [Required(ErrorMessage = "営業主担当は必須入力です。")]
        public string owner { get; set; }	//営業主担当
        public string zip_code { get; set; }	//郵便番号
        public string address { get; set; }	//住所
        public string tel { get; set; }	//会社Tel
        public string fax { get; set; }	//会社Fax
        public string url { get; set; }	//URL
        public string category { get; set; }	//カテゴリ
        public string memo { get; set; }	//会社メモ
        public string cybozu_seq { get; set; }	//サイボウズSEQ
        public string ma_eyes_seq { get; set; }	//MA-EYES_SEQ
        public int order { get; set; }	//表示優先度
    }

    public class Employee
    {
        [Key]
        public int id { get; set; }
        public int user_id { get; set; } //社員番号
        public string user_name { get; set; }	//ユーザ
        public string department { get; set; }	//営業部署
    }


    public class Hoge
    {
        [Key]
        public int id { get; set; }
        
        public string hoge { get; set; }	//ユーザ
    }


}