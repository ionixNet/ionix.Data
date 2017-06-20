using System;
using System.Diagnostics;
using System.Linq;
using ionix.Data;
using ionix.Utils;
using ionix.Utils.Serialization;
using ionixTests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ionixTests
{
    [TestClass]
    public class EntityMetaDataProviderTests
    {
        [TestMethod]
        public void CopyTest()
        {
            IEntityMetaDataProvider provider = new DbSchemaMetaDataProvider();

            EntityMetaData metaData = (EntityMetaData)provider.CreateEntityMetaData(typeof (Invoices));

            var copy = metaData.Copy();

            int len = metaData.Properties.Count();

            Stopwatch bench = Stopwatch.StartNew();
            for (int j = 0; j < 1000000; ++j)
                metaData.Copy();
            bench.Stop();

            Debug.WriteLine("Schema Xml Copy: " +  bench.ElapsedMilliseconds);

            Assert.IsNotNull(metaData);
        }

        [TestMethod]
        public void IsModelValid()
        {
            Employees e = new Employees();
            e.LastName = "LastName";
            e.FirstName = "FirstName";
            e.BirthDate = DateTime.Now;
            e.HireDate = DateTime.Now;
            e.Title = Guid.NewGuid().ToString() + Guid.NewGuid().ToString() + Guid.NewGuid().ToString() +
                      Guid.NewGuid().ToString() + Guid.NewGuid().ToString();

            bool result = EntityMetadaExtensions.IsModelValid(e);

            Assert.IsTrue(!result);
        }
    }
}
