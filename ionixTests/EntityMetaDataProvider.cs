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
    public class EntityMetaDataProviderTest
    {
        [TestMethod]
        public void CopyTest()
        {
            IEntityMetaDataProvider provider = new DbSchemaMetaDataProvider();

            var metaData = provider.CreateEntityMetaData(typeof (Invoices));

            int len = metaData.Properties.Count();

            Stopwatch bench = Stopwatch.StartNew();
            for (int j = 0; j < 1; ++j)
                metaData.Copy();
            bench.Stop();

            Debug.WriteLine("Schema Xml Copy: " +  bench.ElapsedMilliseconds);

            Assert.IsNotNull(metaData);
        }
    }
}
