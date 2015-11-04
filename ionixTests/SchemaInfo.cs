using System;
using System.Diagnostics;
using ionix.Data;
using ionix.Utils;
using ionix.Utils.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ionixTests
{
    [TestClass]
    public class SchemaInfoTest
    {
        [TestMethod]
        public void XmlTest()
        {
            SchemaInfo schema = new SchemaInfo("Id", CachedTypes.Guid);
            schema.DatabaseGeneratedOption = StoreGeneratedPattern.None;
            schema.DefaultValue = Guid.Empty.ToString();
            schema.IsKey = true;
            schema.IsNullable = false;
            schema.MaxLength = 32;
            schema.Order = 42;
            schema.ReadOnly = false;
            schema.SqlValueType = SqlValueType.Parameterized;;

            string xml = schema.XmlSerialize();

            SchemaInfo copy = schema.Copy();

            Stopwatch bench = Stopwatch.StartNew();
            for (int j = 0; j < 100; ++j)
                copy = schema.Copy();
            bench.Stop();

            Debug.WriteLine("Schema Xml Copy: " +  bench.ElapsedMilliseconds);

            Assert.IsNotNull(copy);
        }
    }
}
