namespace ionixTests.MongoDB
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Reflection;
    using global::MongoDB.Bson;
    using global::MongoDB.Bson.Serialization.Attributes;
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;
    using ionix.Data.MongoDB;
    using ionix.Data.MongoDB.Migration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MongoTests
    {
        internal const string MongoAddress = "mongodb://localhost:27017";//"mongodb://172.19.3.171:46000"

        static MongoTests()
        {
            MongoClientProxy.SetConnectionString(MongoAddress);
        }

        private static IEnumerable<TEntity> CreateMockData<TEntity>(int limit)
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

            List<TEntity> list = new List<TEntity>(limit);
            for (int j = 0; j < limit; ++j)
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

            return list;
        }
        private static Task InsertMany<TEntity>()
            where TEntity : new()
        {
            return new MongoRepository<TEntity>().InsertManyAsync(CreateMockData<TEntity>(100));
        }
        private static void InsertAssetTag()
        {
            Stopwatch bench = Stopwatch.StartNew();
            var assetList = Mongo.Cmd.AsQueryable<Person>().ToList();

           // Random rnd = new Random();

            foreach (Person person in assetList)
            {
                for (int j = 0; j < 3; ++j)
                {
                    PersonAddress en = new PersonAddress();
                    en.PersonId = person.Id;
                    en.Description = Guid.NewGuid().ToString("N");

                    if (Mongo.Cmd.AsQueryable<PersonAddress>().FirstOrDefault(p => p.PersonId == en.PersonId) == null)
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
            Console.WriteLine($"{typeof(PersonAddress).Name}, Elepsad: {bench.ElapsedTicks}");
        }

        //[TestMethod]
        public void Initialize()
        {
            //Migration
            var runner = new MigrationRunner(MongoAddress, "TestDb");

            runner.MigrationLocator.LookForMigrationsInAssembly(Assembly.GetExecutingAssembly());
            // runner.MigrationLocator.LookForMigrationsInAssemblyOfType<Migration1>();
           // runner.DatabaseStatus.ThrowIfNotLatestVersion();
            runner.UpdateToLatest();

            //

            var db = MongoAdmin.GetDatabase(MongoClientProxy.Instance, "TestDb");
             MongoAdmin.ExecuteScript(db, "db.LdapUser.remove({});");
            // MongoAdmin.ExecuteScript(db, "db.LdapUser.drop();");

            string json = File.ReadAllText("d:\\sil.txt");


            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LdapUser>>(json);
            Mongo.Cmd.InsertMany(list);


            MongoAdmin.ExecuteScript(db, "db.Asset.remove({});");
            MongoAdmin.ExecuteScript(db, "db.AssetTag.remove({});");

            InsertMany<Person>().Wait();
            InsertAssetTag();

            var result = Mongo.Cmd.Count<Person>();


            Assert.AreNotEqual(result, 0);
        }

        [TestMethod]
        public void CountTest()
        {
            var result = Mongo.Cmd.Count<Person>();

            Assert.AreNotEqual(result, 0);
        }

        [TestMethod]
        public async Task CountAsyncTest()
        {
            var result = await Mongo.Cmd.CountAsync<Person>();

            Assert.AreNotEqual(result, 0);
        }

        [TestMethod]
        public void AsQueryableTest()
        {
            var result = (from a in Mongo.Cmd.AsQueryable<Person>()
                join at in Mongo.Cmd.AsQueryable<PersonAddress>() on a.Id equals at.PersonId
                select new { Asset = a, AssetTag = at }).Take(10);

            var resultList = result.ToList();


            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetByIdTest()
        {
            var asset = Mongo.Cmd.AsQueryable<Person>().FirstOrDefault();
            if (null == asset)
                Assert.Fail();

            asset = Mongo.Cmd.GetById<Person>(asset.Id);

            Assert.IsNotNull(asset);
        }

        [TestMethod]
        public async Task GetByIdAsyncTest()
        {
            var asset = Mongo.Cmd.AsQueryable<Person>().FirstOrDefault();
            if (null == asset)
                Assert.Fail();

            asset = await Mongo.Cmd.GetByIdAsync<Person>(asset.Id);

            Assert.IsNotNull(asset);
        }

        private static Person CreateAsset()
        {
            Person asset = new Person();
            asset.Active = false;
            asset.Name = "Yeni_" + Guid.NewGuid().ToString("N");
            asset.Description = "Açıklama_" + Guid.NewGuid().ToString("N");

            return asset;
        }

        [TestMethod]
        public void InsertOneTest()
        {
            Person asset = CreateAsset();
            Mongo.Cmd.InsertOne(asset);

            asset = Mongo.Cmd.AsQueryable<Person>().FirstOrDefault(p => p.Name.Contains(asset.Name));

            Assert.IsNotNull(asset);
        }

        [TestMethod]
        public async Task InsertOneAsyncTest()
        {
            Person asset = CreateAsset();
            await Mongo.Cmd.InsertOneAsync(asset);

            asset = Mongo.Cmd.AsQueryable<Person>().FirstOrDefault(p => p.Name.Contains(asset.Name));

            Assert.IsNotNull(asset);
        }

        private const int ManyTestLength = 100;

        [TestMethod]
        public void InsertManyTest()
        {
            List<Person> list = new List<Person>(ManyTestLength);
            for (int j = 0; j < ManyTestLength; ++j)
            {
                list.Add(CreateAsset());
            }

            Mongo.Cmd.InsertMany(list);

            list = Mongo.Cmd.AsQueryable<Person>().Where(p => p.Name.Contains("Yeni")).ToList();

            Assert.AreNotEqual(list.Count, 0);
        }

        [TestMethod]
        public async Task InsertManyAsyncTest()
        {
            List<Person> list = new List<Person>(ManyTestLength);
            for (int j = 0; j < ManyTestLength; ++j)
            {
                list.Add(CreateAsset());
            }

            await Mongo.Cmd.InsertManyAsync(list);

            list = Mongo.Cmd.AsQueryable<Person>().Where(p => p.Name.Contains("Yeni")).ToList();

            Assert.AreNotEqual(list.Count, 0);
        }

        [TestMethod]
        public void ReplaceOneTest()
        {
            var asset = Mongo.Cmd.AsQueryable<Person>().FirstOrDefault();
            if (null == asset)
                Assert.Fail();

            string orginal = asset.Name;

            asset.Name = "Replaced_" + Guid.NewGuid().ToString("N");
            Mongo.Cmd.ReplaceOne(p => p.Id == asset.Id, asset);

            asset = Mongo.Cmd.GetById<Person>(asset.Id);

            bool result = orginal != asset.Name;

            if (result)
            {
                orginal = asset.Name;

                asset.Name = "Replaced_" + Guid.NewGuid().ToString("N");

                Mongo.Cmd.ReplaceOne(asset);

                asset = Mongo.Cmd.GetById<Person>(asset.Id);

                result = orginal != asset.Name;
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ReplaceOneAsyncTest()
        {
            var asset = Mongo.Cmd.AsQueryable<Person>().FirstOrDefault();
            if (null == asset)
                Assert.Fail();

            string orginal = asset.Name;

            asset.Name = "Replaced_" + Guid.NewGuid().ToString("N");
            await Mongo.Cmd.ReplaceOneAsync(p => p.Id == asset.Id, asset);

            asset = await Mongo.Cmd.GetByIdAsync<Person>(asset.Id);

            bool result = orginal != asset.Name;

            if (result)
            {
                orginal = asset.Name;

                asset.Name = "Replaced_" + Guid.NewGuid().ToString("N");

                await Mongo.Cmd.ReplaceOneAsync(asset);

                asset = await Mongo.Cmd.GetByIdAsync<Person>(asset.Id);

                result = orginal != asset.Name;
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UpdateOneTest()
        {
            var asset = Mongo.Cmd.AsQueryable<Person>().FirstOrDefault();
            if (null == asset)
                Assert.Fail();

            asset.Name = "UpdateOne_" + Guid.NewGuid().ToString("N");

            string assetStr = asset.Name;

            Mongo.Cmd.UpdateOne<Person>(p => p.Id == asset.Id,
                (builder) => builder.Set(p => p.Name, assetStr));

            bool result = Mongo.Cmd.GetById<Person>(asset.Id).Name == assetStr;

            if (result)
            {
                asset.Name = "UpdateOne_" + Guid.NewGuid().ToString("N");
                assetStr = asset.Name;

                Mongo.Cmd.UpdateOne<Person>(asset.Id,
                    (builder) => builder.Set(p => p.Name, assetStr));

                result = Mongo.Cmd.GetById<Person>(asset.Id).Name == assetStr;

                asset.Name = "Mehmet 2";
                asset.Description = "Gören 2";

                Mongo.Cmd.UpdateOne(asset, p => p.Name, p => p.Description);
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task UpdateOneAsyncTest()
        {
            var asset = Mongo.Cmd.AsQueryable<Person>().FirstOrDefault();
            if (null == asset)
                Assert.Fail();

            asset.Name = "UpdateOne_" + Guid.NewGuid().ToString("N");

            string assetStr = asset.Name;

            await Mongo.Cmd.UpdateOneAsync<Person>(p => p.Id == asset.Id,
                (builder) => builder.Set(p => p.Name, assetStr));

            bool result = (await Mongo.Cmd.GetByIdAsync<Person>(asset.Id)).Name == assetStr;

            if (result)
            {
                asset.Name = "UpdateOne_" + Guid.NewGuid().ToString("N");
                assetStr = asset.Name;

                await Mongo.Cmd.UpdateOneAsync<Person>(asset.Id,
                    (builder) => builder.Set(p => p.Name, assetStr));

                result = (await Mongo.Cmd.GetByIdAsync<Person>(asset.Id)).Name == assetStr;
            }

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void UpdateManyTest()
        {
            var result = Mongo.Cmd.UpdateMany<Person>(p => p.Active,
                (builder) => builder.Set(p => p.Active, false).Set(p => p.Description, "Mehmet"));

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DeleteOneTest()
        {
            var asset = Mongo.Cmd.AsQueryable<Person>().FirstOrDefault();
            if (null == asset)
                Assert.Fail();

            var count = Mongo.Cmd.Count<Person>();
            Mongo.Cmd.DeleteOne<Person>(p => p.Id == asset.Id);

            var result = count - Mongo.Cmd.Count<Person>() == 1;

            if (result)
            {
                count--;

                asset = Mongo.Cmd.AsQueryable<Person>().FirstOrDefault();
                if (null == asset)
                    Assert.Fail();

                Mongo.Cmd.DeleteOne<Person>(asset.Id);

                result = count - Mongo.Cmd.Count<Person>() == 1;
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task BulkReplaceAsyncTest()
        {
            var list = await Mongo.Cmd.AsQueryable<Person>().OrderBy(p => p.Id).Take(10).ToListAsync();


            list.ForEach(i => i.Name = "Changed By Ela " + Guid.NewGuid().ToString("N"));

            var result = await Mongo.Cmd.BulkReplaceAsync(list, null, false, p => p.Name);

            list.ForEach(i => i.Description = "Changed By Ela 2");

            result = await Mongo.Cmd.BulkReplaceAsync(list, null, false);

            list.ForEach(i =>
            {
                i.Id = ObjectId.GenerateNewId();
                i.Name = Guid.NewGuid().ToString("N");
            });

            result = await Mongo.Cmd.BulkReplaceAsync(list, null, true, p => p.Name);


            list.ForEach(i =>
            {
                i.Id = ObjectId.GenerateNewId();
                i.Name = Guid.NewGuid().ToString("N");
            });

            result = await Mongo.Cmd.BulkReplaceAsync(list, null, true);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task BulkUpdateAsync()
        {
            const int limit = 10;
            var list = await Mongo.Cmd.AsQueryable<Person>().OrderBy(p => p.Id).Take(limit).ToListAsync();

            Random rnd = new Random();
            list.ForEach(i => i.Description = rnd.Next(0, 100000).ToString());

            var result = await Mongo.Cmd.BulkUpdateAsync(list, null, p => p.Description);

            Assert.AreEqual((int)result.ModifiedCount, limit);
        }

        [TestMethod]
        public async Task BulkDeleteAsync()
        {
            const int limit = 3;
            var list = await Mongo.Cmd.AsQueryable<Person>().OrderByDescending(p => p.Id).Take(limit).ToListAsync();

            var result = await Mongo.Cmd.BulkDeleteAsync(list, null, p => p.Name);

            Assert.AreEqual((int)result.DeletedCount, limit);

            list = await Mongo.Cmd.AsQueryable<Person>().OrderByDescending(p => p.Id).Take(limit).ToListAsync();
            result = await Mongo.Cmd.BulkDeleteAsync(list, null);

            Assert.AreEqual((int)result.DeletedCount, limit);
        }

        [TestMethod]
        public async Task TextSearchTest()
        {
            var list = Mongo.Cmd.TextSearch<LdapUser>("ağrı şaban");

            list = await  Mongo.Cmd.TextSearchAsync<LdapUser>("bayburt");

            Assert.IsNotNull(list);
        }
    }
}
