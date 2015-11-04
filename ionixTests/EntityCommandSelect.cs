using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ionix.Data;
using ionixTests.Models;
using ionixTests.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ionixTests
{
    [TestClass]
    public class EntityCommandSelect
    {
        [TestMethod]
        public void QueryTest()
        {
            IEnumerable<Customers> customers = null;
            using (var client = DataFactory.CretDbClient())
            {
                customers = client.Cmd.Query(
                    Fluent.Select<Customers>()
                        .Column(c => c.ContactName)
                        .Column(c => c.Country, c => c.CompanyName)
                        .Where()
                        .Contains(c => c.Country, "Mex")
                        .AsSelect());
            }

            Assert.IsNotNull(customers);
        }

        [TestMethod]
        public void QuerySingleTest()
        {
            Customers customer = null;
            using (var client = DataFactory.CretDbClient())
            {
                customer = client.Cmd.QuerySingle<Customers>("select * from Customers where CustomerID=@0".ToQuery("ANATR"));
                customer = (Customers)client.Cmd.QuerySingle(typeof(Customers), "select * from Customers where CustomerID=@0".ToQuery("ANATR"));
            }

            Assert.IsNotNull(customer);
        }


        [TestMethod]
        public void SelectTest()
        {
            IEnumerable<Employees> employees = null;
            Task.Run(async () =>
            {
                using (var client = DataFactory.CretDbClient())
                {
                    employees = await client.Cmd.SelectAsync<Employees>();
                }

                Assert.IsNotNull(employees);
            }).Wait();

            Assert.IsNotNull(employees);
        }

        [TestMethod]
        public void SelectSingleTest()
        {
            Customers customer = null;
            using (var client = DataFactory.CretDbClient())
            {
                customer = client.Cmd.SelectSingle(Fluent.Where<Customers>().Equals(c => c.CustomerID, "ANATR"));
                customer = (Customers)client.Cmd.SelectSingle(typeof(Customers), " where CustomerID=@0".ToQuery("ANATR"));
            }

            Assert.IsNotNull(customer);
        }

        [TestMethod]
        public void InsertTest()
        {
            Categories c = new Categories()
            {
                CategoryName = Guid.NewGuid().ToString().Substring(0, 10),
                Description = Guid.NewGuid().ToString()
            };
            using (var client = DataFactory.CretDbClient())
            {
                client.Cmd.Insert(c);
            }

            Assert.AreNotEqual(c.CategoryID, 0);
        }

        [TestMethod]
        public void UpdateTest()
        {
            Categories c = new Categories()
            {
                CategoryID = 9,
                CategoryName = "CategoryName",
            };
            using (var client = DataFactory.CretDbClient())
            {
                client.Cmd.Update(c, p => p.CategoryName);
            }

            Assert.AreNotEqual(c.CategoryID, 0);
        }

        [TestMethod]
        public void UpsertTest()
        {
            Categories c = null;
            Task.Run(async() =>
            {
                c = new Categories()
                {
                    CategoryID = 999,
                    CategoryName = "CategoryName",
                };
                using (var client = DataFactory.CretDbClient())
                {
                    await client.Cmd.UpsertAsync(c, new []{ "CategoryName" }, null);
                }
            }).Wait();

            Assert.AreNotEqual(c.CategoryID, 0);
        }

        [TestMethod]
        public void BatchUpdateTest()
        {
            using (var client = DataFactory.CretDbClient())
            {
                var catagories = client.Cmd.Select<Categories>();

                client.Cmd.BatchUpdate(catagories);
            }
        }

        [TestMethod]
        public void BatchInsertTest()
        {
            var territories = new List<Territories>();
            for (int j = 0; j < 3; ++j)
                territories.Add(new Territories()
                {
                    TerritoryID = Guid.NewGuid().ToString().Substring(0, 20),
                    TerritoryDescription = Guid.NewGuid().ToString(),
                    RegionID = 1
                });

            using (var client = DataFactory.CretDbClient())
            {
                client.Cmd.BatchInsert(territories);
            }
        }

        [TestMethod]
        public void BatchUpsertTest()
        {
        //    var territories = new List<Territories>();
        //    for (int j = 0; j < 3; ++j)
        //        territories.Add(new Territories()
        //        {
        //            TerritoryID = Guid.NewGuid().ToString().Substring(0, 20),
        //            TerritoryDescription = Guid.NewGuid().ToString(),
        //            RegionID = 1
        //        });

            using (var client = DataFactory.CretDbClient())
            {
                var territories = client.Cmd.Select<Territories>();

                client.Cmd.BatchUpsert(territories, BatchCommandMode.Single);
            }
        }

        [TestMethod]
        public void DelsertTest()
        {
            var territories = new List<Territories>();
            for (int j = 0; j < 3; ++j)
                territories.Add(new Territories()
                {
                    TerritoryID = Guid.NewGuid().ToString().Substring(0, 20),
                    TerritoryDescription = Guid.NewGuid().ToString(),
                    RegionID = 1
                });
            using (var client = DataFactory.CretDbClient())
            {
                client.Cmd.Delsert(territories, (where) => where.Equals(t => t.TerritoryID, "12"));
            }
        }
    }
}
