﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using ionix.Data;
using ionix.Utils.Extensions;
using ionixTests.Models;
using ionixTests.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ionixTests
{
    using System.Collections;

    [TestClass]
    public class EntityCommandTests
    {
        [TestMethod]
        public void SelectByIdTest()
        {
            Customers customer = null;
            using (var client = DataFactory.CretDbClient())
            {
                customer = client.Cmd.SelectById<Customers>("ALFKI");
                customer = (Customers)client.Cmd.SelectById(typeof(Customers), "ALFKI");
            }

            Assert.IsNotNull(customer);
        }

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
                customer = (Customers)client.Cmd.QuerySingle(typeof(Customers), "select * from Customers where CustomerID=@0".ToQuery("WOLZA"));
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

                client.Cmd.BatchUpdate(catagories, BatchCommandMode.Batch);
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
                client.Cmd.BatchInsert(territories, BatchCommandMode.Batch);
            }
        }


        [TestMethod]
        public void BatchInsertTest2()
        {
            var categories = new List<Categories>();
            for (int j = 0; j < 3; ++j)
                categories.Add(new Categories()
                {
                    CategoryName = Guid.NewGuid().ToString().Substring(0, 12),
                    Description = "İşte Bu Parametresiz"
                });

            using (var client = DataFactory.CretDbClient())
            {
                client.Cmd.BatchInsert(categories);
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
                var categories = client.Cmd.Query<Categories>("select top 3 * from [NORTHWND].[dbo].[Categories]".ToQuery());
               
                categories.ForEach((item) => item.CategoryID = -1);

                client.Cmd.BatchUpsert(categories, BatchCommandMode.Batch);
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

        [TestMethod]
        public void MultipleQuerySingleTests()
        {
            using (var c = DataFactory.CretDbClient())
            {
                var cmd = c.Cmd;// new EntityCommandSelect(c.DataAccess, '@');

                SqlQuery q = @"select top 1 o.*, c.* from Orders o
                inner join Customers c on o.CustomerID = c.CustomerID".ToQuery();

                var order = cmd.QuerySingle<Orders, Customers>(q, MapBy.Name);

                for (int j = 0; j < 1000; ++j)
                {
                    order = cmd.QuerySingle<Orders, Customers>(q, MapBy.Name);
                    order = cmd.QuerySingle<Orders, Customers>(q, MapBy.Sequence);
                }

                order = cmd.QuerySingle<Orders, Customers>(q, MapBy.Sequence);

                q = @"select top 1 o.ShipName, o.ShipCountry, c.ContactName, c.Country from Orders o
                inner join Customers c on o.CustomerID = c.CustomerID".ToQuery();

                order = cmd.QuerySingle<Orders, Customers>(q, MapBy.Name);
               // order = cmd.QuerySingle<Orders, Customers>(provider, q, MapBy.Sequence);


                q = @"select top 1 o.*, c.*, e.* from Orders o
                inner join Customers c on o.CustomerID = c.CustomerID
                inner join Employees e on o.EmployeeID = e.EmployeeID".ToQuery();
                var order3 = cmd.QuerySingle<Orders, Customers, Employees>(q, MapBy.Name);

                var provider = new DbSchemaMetaDataProvider();
                dynamic dyn = c.DataAccess.QuerySingle(provider, q, MapBy.Name, '@', typeof(Orders), typeof(Customers), typeof(Employees));
                dyn = c.DataAccess.QuerySingle(provider, q, MapBy.Sequence, '@', typeof(Orders), typeof(Customers), typeof(Employees));


                q = @"select top 1 o.ShipName, o.ShipCountry, c.ContactName, c.Country, e.FirstName, e.LastName from Orders o
                    inner join Customers c on o.CustomerID = c.CustomerID
                    inner join Employees e on o.EmployeeID = e.EmployeeID".ToQuery();

                order3 = cmd.QuerySingle<Orders, Customers, Employees>(q, MapBy.Name);

                dyn = c.DataAccess.QuerySingle(provider, q, MapBy.Name, '@', typeof(Orders), typeof(Customers), typeof(Employees));

                Assert.IsTrue(true);

            }
        }

        [TestMethod]
        public void MultipleQueryTests()
        {
            using (var c = DataFactory.CretDbClient())
            {
                var cmd = c.Cmd;// new EntityCommandSelect(c.DataAccess, '@');

                var provider = new DbSchemaMetaDataProvider();

                SqlQuery q = @"select o.*, c.* from Orders o
                inner join Customers c on o.CustomerID = c.CustomerID".ToQuery();

                var order = cmd.Query<Orders, Customers>(q, MapBy.Name);
                order = cmd.Query<Orders, Customers>(q, MapBy.Sequence);

                q = @"select o.ShipName, o.ShipCountry, c.ContactName, c.Country from Orders o
                inner join Customers c on o.CustomerID = c.CustomerID".ToQuery();

                order = cmd.Query<Orders, Customers>(q, MapBy.Name);
                // order = cmd.QuerySingle<Orders, Customers>(provider, q, MapBy.Sequence);

                q = @"select o.*, c.*, e.* from Orders o
                inner join Customers c on o.CustomerID = c.CustomerID
                inner join Employees e on o.EmployeeID = e.EmployeeID".ToQuery();
                var order3 = cmd.Query<Orders, Customers, Employees>(q, MapBy.Name);
                order3 = cmd.Query<Orders, Customers, Employees>(q, MapBy.Sequence);
                dynamic dyn = c.DataAccess.Query(provider, q, MapBy.Name, '@', typeof(Orders), typeof(Customers), typeof(Employees));
                dyn = c.DataAccess.Query(provider, q, MapBy.Sequence, '@',typeof(Orders), typeof(Customers), typeof(Employees));


                for (int j = 0; j < 10; ++j)
                {
                    order3 = cmd.Query<Orders, Customers, Employees>(q, MapBy.Name);
                    order3 = cmd.Query<Orders, Customers, Employees>(q, MapBy.Sequence);
                }


                Assert.IsNotNull(order);

            }
        }


        [TestMethod]
        public void QuerySingleEntityPrimitiveTest()
        {
            using (var c = DataFactory.CretDbClient())
            {
                object result = c.Cmd.QuerySingle<Customers>("select top 1 * from Customers".ToQuery());

                result = c.Cmd.QuerySingle<int>("select top 1 CategoryID from Categories".ToQuery());

                result = c.Cmd.QuerySingle<string>("select top 1 CategoryName from Categories".ToQuery());

                Assert.IsNotNull(result);
            }
        }


        [TestMethod]
        public void QueryEntityPrimitiveTest()
        {
            using (var c = DataFactory.CretDbClient())
            {
                IEnumerable result = c.Cmd.Query<Customers>("select * from Customers".ToQuery());

                result = c.Cmd.Query<int>("select CategoryID from Categories".ToQuery());

                result = c.Cmd.Query<string>("select CategoryName from Categories".ToQuery());

                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void DynamicQuerySingleTest()
        {
            using (var c = DataFactory.CretDbClient())
            {
                var result = c.Cmd.QuerySingle<dynamic>("select top 1 * from Customers".ToQuery());
                var result2 = c.Cmd.QuerySingle<ExpandoObject>("select top 1 * from Customers".ToQuery());

                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void DynamicQueryTest()
        {
            using (var c = DataFactory.CretDbClient())
            {
                var result = c.Cmd.Query<dynamic>("select * from Customers".ToQuery());
                var result2 = c.Cmd.Query<ExpandoObject>("select * from Customers".ToQuery());

                Assert.IsNotNull(result);
            }
        }
    }
}
