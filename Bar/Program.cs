using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using LinqToDB;
using System.Data.Entity.Core.Objects;
using LinqToDB.Linq;
using Bar;
using System.Linq.Expressions;
using Bar.DapperSql;
using static LinqExpToSql.Program;

namespace LinqExpToSql
{
    class Program
    {
        public class Customer
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public int Status { get; set; }
        }

        static void Main(string[] args)
        {
            ISqlable<Customer> customer_tbl = new Sqlable<Customer>();
            var sql = customer_tbl.Where(x => x.FirstName == "first" && x.Status >= 2 && x.FirstName.Contains("f"))
                .Take(10).Skip(1)
                //.OrderBy(x=> x.FirstName).ThenBy(x=>x.Status).GroupBy(x=>x.Status)
                .Update(x => new Customer {FirstName = "Hi"})
                .SqlString;

            Console.WriteLine(sql);
        }
   
    }

}




