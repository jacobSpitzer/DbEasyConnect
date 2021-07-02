using Dapper;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using TQueryable;
using TQueryable.Library;

namespace TQueryable
{
    class Program
    {
        public class Customer
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public int Status { get; set; }
            public string MyProperty { get; set; }
            public string MyProp { get; set; }
        }

        public class Person
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public int Status { get; set; }
        }

        static void Main(string[] args)
        {
            TQueryable<Person> customer_tbl = new TQuery<Person>();
            var h = customer_tbl.Create().ExecuteCommand();
            Console.WriteLine(h);
            //TQueryable<Person> person_tbl = new TQuery<Person>();
            //            using (var connection = new SqlConnection("Data Source=hgws27.win.hostgator.com; Database=quickpps_tquey; User ID=quickpps_developer;Password=5k1jPd1T9AfWsKKs; MultipleActiveResultSets=True"))


            //Customer customer = new Customer { FirstName = "bla", Status = 0 };
            //var sql = customer_tbl//.Where(x => x.FirstName == "first" && x.Status >= 2 && x.FirstName.Contains("f"))
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




