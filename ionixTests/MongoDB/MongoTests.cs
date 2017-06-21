namespace ionixTests.MongoDB
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Reflection;
    using global::MongoDB.Bson;
    using global::MongoDB.Bson.Serialization.Attributes;
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;
    using ionix.Data.MongoDB;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MongoTests
    {
        internal const string MongoAddress = "mongodb://localhost:27017";//"mongodb://172.19.3.171:46000"

        static MongoTests()
        {
            MongoClientProxy.SetConnectionString(MongoAddress);
        }

        static int Limit = 1000;// 100000;
        private static Task InsertMany<TEntity>()
            where TEntity : new()
        {
            var properties = typeof(TEntity).GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => pi.GetCustomAttribute<BsonIgnoreAttribute>() == null && pi.GetSetMethod() != null);

            Random rnd = new Random();
            IDictionary<Type, Action<TEntity, PropertyInfo>> dic = new ConcurrentDictionary<Type, Action<TEntity, PropertyInfo>>();
            dic.Add(typeof(ObjectId), (en, pi) => pi.SetValue(en, ObjectId.GenerateNewId()));
            dic.Add(typeof(ObjectId?), (en, pi) => pi.SetValue(en, null));
            dic.Add(typeof(string), (en, pi) => pi.SetValue(en, Guid.NewGuid().ToString()));
            dic.Add(typeof(int), (en, pi) => pi.SetValue(en, rnd.Next(10, int.MaxValue)));
            dic.Add(typeof(bool), (en, pi) => pi.SetValue(en, rnd.Next(2, 100) % 2 == 0));
            dic.Add(typeof(DateTimeOffset), (en, pi) => pi.SetValue(en, DateTimeOffset.Now));
            dic.Add(typeof(TimeSpan), (en, pi) => pi.SetValue(en, TimeSpan.MinValue));
            dic.Add(typeof(UInt16), (en, pi) => pi.SetValue(en, (UInt16)rnd.Next(10, UInt16.MaxValue)));
            dic.Add(typeof(byte), (en, pi) => pi.SetValue(en, (byte)rnd.Next(10, byte.MaxValue)));

            List<TEntity> list = new List<TEntity>(Limit);
            for (int j = 0; j < Limit; ++j)
            {
                TEntity en = new TEntity();
                foreach (var pi in properties)
                {
                    if (!pi.PropertyType.GetTypeInfo().IsEnum)
                    {
                        var f = dic[pi.PropertyType];
                        f(en, pi);
                    }
                }
                list.Add(en);
            }

            return MongoRepository<TEntity>.Create().InsertManyAsync(list);
        }
        private static void InsertAssetTag()
        {
            int limit = Limit / 100;
            Stopwatch bench = Stopwatch.StartNew();
            var assetList = Mongo.Cmd.AsQueryable<MngAsset>().Take(limit).ToList();

           // Random rnd = new Random();

            foreach (MngAsset asset in assetList)
            {
                for (int j = 0; j < 3; ++j)
                {
                    MngAssetTag en = new MngAssetTag();
                    en.AssetId = asset.Id;
                    en.TagId = ObjectId.GenerateNewId();

                    if (Mongo.Cmd.AsQueryable<MngAssetTag>().FirstOrDefault(p => p.AssetId == en.AssetId
                                                                                 && p.TagId == en.TagId) == null)
                    {
                        try
                        {
                            Mongo.Cmd.InsertOne(en);
                        }
                        catch { }
                    }
                }
            }

            bench.Stop();
            Console.WriteLine($"{typeof(MngAssetTag).Name}, Limit: {limit * 3}, Elepsad: {bench.ElapsedTicks}");
        }

        [TestMethod]
        public void InitializeMock()
        {
         //   var db = MongoAdmin.GetDatabase(MongoClientProxy.Instance, "KASIRGA");
          //  MongoAdmin.ExecuteScript(db, "db.Asset.remove({});");
          //  MongoAdmin.ExecuteScript(db, "db.AssetTag.remove({});");

            InsertMany<MngAsset>().Wait();
            InsertAssetTag();

            var result = Mongo.Cmd.Count<MngAsset>();


            Assert.AreNotEqual(result, 0);
        }

        [TestMethod]
        public void CountTest()
        {
            var result = Mongo.Cmd.Count<MngAsset>();

            Assert.AreNotEqual(result, 0);
        }

        [TestMethod]
        public async Task CountAsyncTest()
        {
            var result = await Mongo.Cmd.CountAsync<MngAsset>();

            Assert.AreNotEqual(result, 0);
        }

        [TestMethod]
        public void AsQueryableTest()
        {
            var result = (from a in Mongo.Cmd.AsQueryable<MngAsset>()
                join at in Mongo.Cmd.AsQueryable<MngAssetTag>() on a.Id equals at.AssetId
                select new { Asset = a, AssetTag = at }).Take(10);

            var resultList = result.ToList();


            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetByIdTest()
        {
            var asset = Mongo.Cmd.AsQueryable<MngAsset>().FirstOrDefault();
            if (null == asset)
                Assert.Fail();

            asset = Mongo.Cmd.GetById<MngAsset>(asset.Id);

            Assert.IsNotNull(asset);
        }

        [TestMethod]
        public async Task GetByIdAsyncTest()
        {
            var asset = Mongo.Cmd.AsQueryable<MngAsset>().FirstOrDefault();
            if (null == asset)
                Assert.Fail();

            asset = await Mongo.Cmd.GetByIdAsync<MngAsset>(asset.Id);

            Assert.IsNotNull(asset);
        }

        private static MngAsset CreateAsset()
        {
            MngAsset asset = new MngAsset();
            asset.Active = false;
            asset.Asset = "Yeni_" + Guid.NewGuid().ToString("N");
            asset.Description = "Açıklama_" + Guid.NewGuid().ToString("N");

            return asset;
        }

        [TestMethod]
        public void InsertOneTest()
        {
            MngAsset asset = CreateAsset();
            Mongo.Cmd.InsertOne(asset);

            asset = Mongo.Cmd.AsQueryable<MngAsset>().FirstOrDefault(p => p.Asset.Contains(asset.Asset));

            Assert.IsNotNull(asset);
        }

        [TestMethod]
        public async Task InsertOneAsyncTest()
        {
            MngAsset asset = CreateAsset();
            await Mongo.Cmd.InsertOneAsync(asset);

            asset = Mongo.Cmd.AsQueryable<MngAsset>().FirstOrDefault(p => p.Asset.Contains(asset.Asset));

            Assert.IsNotNull(asset);
        }

        private const int ManyTestLength = 100;

        [TestMethod]
        public void InsertManyTest()
        {
            List<MngAsset> list = new List<MngAsset>(ManyTestLength);
            for (int j = 0; j < ManyTestLength; ++j)
            {
                list.Add(CreateAsset());
            }

            Mongo.Cmd.InsertMany(list);

            list = Mongo.Cmd.AsQueryable<MngAsset>().Where(p => p.Asset.Contains("Yeni")).ToList();

            Assert.AreNotEqual(list.Count, 0);
        }

        [TestMethod]
        public async Task InsertManyAsyncTest()
        {
            List<MngAsset> list = new List<MngAsset>(ManyTestLength);
            for (int j = 0; j < ManyTestLength; ++j)
            {
                list.Add(CreateAsset());
            }

            await Mongo.Cmd.InsertManyAsync(list);

            list = Mongo.Cmd.AsQueryable<MngAsset>().Where(p => p.Asset.Contains("Yeni")).ToList();

            Assert.AreNotEqual(list.Count, 0);
        }

        [TestMethod]
        public void ReplaceOneTest()
        {
            var asset = Mongo.Cmd.AsQueryable<MngAsset>().FirstOrDefault();
            if (null == asset)
                Assert.Fail();

            string orginal = asset.Asset;

            asset.Asset = "Replaced_" + Guid.NewGuid().ToString("N");
            Mongo.Cmd.ReplaceOne(p => p.Id == asset.Id, asset);

            asset = Mongo.Cmd.GetById<MngAsset>(asset.Id);

            bool result = orginal != asset.Asset;

            if (result)
            {
                orginal = asset.Asset;

                asset.Asset = "Replaced_" + Guid.NewGuid().ToString("N");

                Mongo.Cmd.ReplaceOne(asset);

                asset = Mongo.Cmd.GetById<MngAsset>(asset.Id);

                result = orginal != asset.Asset;
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ReplaceOneAsyncTest()
        {
            var asset = Mongo.Cmd.AsQueryable<MngAsset>().FirstOrDefault();
            if (null == asset)
                Assert.Fail();

            string orginal = asset.Asset;

            asset.Asset = "Replaced_" + Guid.NewGuid().ToString("N");
            await Mongo.Cmd.ReplaceOneAsync(p => p.Id == asset.Id, asset);

            asset = await Mongo.Cmd.GetByIdAsync<MngAsset>(asset.Id);

            bool result = orginal != asset.Asset;

            if (result)
            {
                orginal = asset.Asset;

                asset.Asset = "Replaced_" + Guid.NewGuid().ToString("N");

                await Mongo.Cmd.ReplaceOneAsync(asset);

                asset = await Mongo.Cmd.GetByIdAsync<MngAsset>(asset.Id);

                result = orginal != asset.Asset;
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UpdateOneTest()
        {
            var asset = Mongo.Cmd.AsQueryable<MngAsset>().FirstOrDefault();
            if (null == asset)
                Assert.Fail();

            asset.Asset = "UpdateOne_" + Guid.NewGuid().ToString("N");

            string assetStr = asset.Asset;

            Mongo.Cmd.UpdateOne<MngAsset>(p => p.Id == asset.Id,
                (builder) => builder.Set(p => p.Asset, assetStr));

            bool result = Mongo.Cmd.GetById<MngAsset>(asset.Id).Asset == assetStr;

            if (result)
            {
                asset.Asset = "UpdateOne_" + Guid.NewGuid().ToString("N");
                assetStr = asset.Asset;

                Mongo.Cmd.UpdateOne<MngAsset>(asset.Id,
                    (builder) => builder.Set(p => p.Asset, assetStr));

                result = Mongo.Cmd.GetById<MngAsset>(asset.Id).Asset == assetStr;

                asset.Asset = "Mehmet 2";
                asset.Description = "Gören 2";

                Mongo.Cmd.UpdateOne(asset, p => p.Asset, p => p.Description);
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task UpdateOneAsyncTest()
        {
            var asset = Mongo.Cmd.AsQueryable<MngAsset>().FirstOrDefault();
            if (null == asset)
                Assert.Fail();

            asset.Asset = "UpdateOne_" + Guid.NewGuid().ToString("N");

            string assetStr = asset.Asset;

            await Mongo.Cmd.UpdateOneAsync<MngAsset>(p => p.Id == asset.Id,
                (builder) => builder.Set(p => p.Asset, assetStr));

            bool result = (await Mongo.Cmd.GetByIdAsync<MngAsset>(asset.Id)).Asset == assetStr;

            if (result)
            {
                asset.Asset = "UpdateOne_" + Guid.NewGuid().ToString("N");
                assetStr = asset.Asset;

                await Mongo.Cmd.UpdateOneAsync<MngAsset>(asset.Id,
                    (builder) => builder.Set(p => p.Asset, assetStr));

                result = (await Mongo.Cmd.GetByIdAsync<MngAsset>(asset.Id)).Asset == assetStr;
            }

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void UpdateManyTest()
        {
            var result = Mongo.Cmd.UpdateMany<MngAsset>(p => p.Active,
                (builder) => builder.Set(p => p.Active, false).Set(p => p.Description, "Mehmet"));

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DeleteOneTest()
        {
            var asset = Mongo.Cmd.AsQueryable<MngAsset>().FirstOrDefault();
            if (null == asset)
                Assert.Fail();

            var count = Mongo.Cmd.Count<MngAsset>();
            Mongo.Cmd.DeleteOne<MngAsset>(p => p.Id == asset.Id);

            var result = count - Mongo.Cmd.Count<MngAsset>() == 1;

            if (result)
            {
                count--;

                asset = Mongo.Cmd.AsQueryable<MngAsset>().FirstOrDefault();
                if (null == asset)
                    Assert.Fail();

                Mongo.Cmd.DeleteOne<MngAsset>(asset.Id);

                result = count - Mongo.Cmd.Count<MngAsset>() == 1;
            }

            Assert.IsTrue(result);
        }
    }
}
