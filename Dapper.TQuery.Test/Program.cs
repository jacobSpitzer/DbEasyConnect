using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper.TQuery.Library;
using MyDapper.Tables;

namespace Dapper.TQuery.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //var list = new List<ForumUser>();
            //IQueryable<ForumUser> query = list.AsQueryable();
            //Console.WriteLine("");
            var con = new SqlConnection("Server=hgws27.win.hostgator.com; Database=quickpps_test; User ID=quickpps_test;Password=Hj4d6~4q; Trusted_Connection=False; MultipleActiveResultSets=True");
            var i = con.Open()//.CreateAllTables();
            //i = con.TQuery<ForumUser>().CreateTableIfNotExits().Execute();
            //var tables = con.GetAllTablesType();
            //var sql = con.CreateAllTables().Execute();
            //var users = new TQuery<ForumUser>(con) as TQueryable<ForumUser>;
            //var questions = new TQuery<Question>(con);
            //var answers = new TQuery<Answer>(con);
            //var sql = users.Join(questions, x => x.Id, x => x.UserId, (u, q) => new { u, q }).Where(x => x.q.Status == "test" && x.u.Role == 0)//.SqlString;
            //var sql = users.Join(questions, x => new { F = x.Id }, x => new { F = x.UserId }, (u, q) => new { u, Questionbody = q.Body }).Join(answers,x=>x.u.Id,x=>x.UserId,(q,a)=>a).Where(x=>x.Views >=1)
            //var sql = users.Join(questions, x => x.Id, x => x.UserId, (u, q) => new { u, q })//.Where(x=>x.q.Downvotes < 2)
            //var sql = users.Where(x => x.Role > 1).Take(5)
            //.SqlString;
            //var one = new List<string>(); var two = new List<string>();
            //Console.WriteLine(sql);
            Console.WriteLine("test");
            //            using (var connection = new SqlConnection("Data Source=hgws27.win.hostgator.com; Database=quickpps_tquey; User ID=quickpps_developer;Password=5k1jPd1T9AfWsKKs; MultipleActiveResultSets=True"))


            //Customer customer = new Customer { FirstName = "bla", Status = 0 };
            //var sql = users.Where(x => x.Username == "first" && x.Status >= 2 && x.Username.Contains("f"))
            //    //.Take(10)//.Skip(1)
            //    //.OrderBy(x=> x.FirstName).ThenBy(x=>x.Status)//.GroupBy(x=>x.Status)
            //    //.Update(x => new Customer { FirstName = x.MyProperty + "5", MyProp = null, MyProperty = "0", Status = 0 })
            //    .Join(person_tbl,x=>x.Id,y=>y.Id,(customer,person)=> new { customer, person })//.OrderBy(x=>x.customer.FirstName)
            //    //.Select(x=> new { A = x.FirstName,b = x.Id,c = x.MyProp })
            //    .SqlString;

            //Console.WriteLine(sql);
            //Console.WriteLine(customer_tbl.Select(x => x).SqlString);
            //Console.WriteLine(customer_tbl.Select(x => new { x.FirstName, x.MyProp }).SqlString);
            //Console.WriteLine(customer_tbl.Select(x => new { A = x.FirstName, G = x.MyProp}).SqlString);
            //Console.WriteLine(customer_tbl.Update(x => new Customer { FirstName = x.FirstName + x.MyProperty, MyProp = x.MyProp + "5", Status = x.Status + (x.Status / 2) }).SqlString);
            //Console.WriteLine(customer_tbl.Select(x => new Customer { FirstName = x.FirstName + x.MyProperty, MyProp = x.MyProp + "5", Status = x.Status + (x.Status / 2) }).SqlString);
        }

    }
}
