namespace ionixTests.MongoDB
{
    using System.Collections.Generic;
    using System.IO;
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;
    using ionix.Data.MongoDB;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MongoDbPerformanceTests
    {
        static MongoDbPerformanceTests()
        {
            MongoClientProxy.SetConnectionString(MongoTests.MongoAddress);
        }

        [TestMethod]
        public void InitMongoDbPerformanceTests()
        {
            long cout = Mongo.Cmd.Count<MngAsset>();
        }

        [TestMethod]
        public void IndexContainsSearchTest()
        {
            var result = Mongo.Cmd.AsQueryable<MngAsset>().Where(p => p.Asset.Contains("bbbb")).ToList();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TextIndexTest()
        {
            var db = MongoAdmin.GetDatabase(MongoClientProxy.Instance, "KASIRGA");
            //  MongoAdmin.ExecuteScript(db, "db.TextIndexModel.remove({});");
            // MongoAdmin.ExecuteScript(db, "db.TextIndexModel.drop();");

            string json = File.ReadAllText("d:\\sil.txt");


            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LdapUser>>(json);
            Mongo.Cmd.InsertMany(list);


            Assert.Fail();

        }
    }
}
