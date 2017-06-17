//namespace ionix.Data.MongoDB
//{
//    using System;

//    public class MongoDbContext
//    {
//        private static bool initialedGlobally;
//        public static void InitialGlobal(bool throwIfNotLatestVersion)
//        {
//            var connSrt = "mongodb://localhost:27017";
//            if (String.IsNullOrEmpty(connSrt))
//            {
//                throw new InvalidOperationException("MongoConnection was not specified in 'appsettings.json' file.");
//            }

//            MongoClientProxy.SetConnectionString(connSrt);
//            initialedGlobally = MongoHelper.InitializeMongo(Migration100.GetLibAssembly(), connSrt, Migration100.DatabaseName, throwIfNotLatestVersion);
//        }

//        public MongoDbContext()
//        {
//            if (!initialedGlobally)
//            {
//                throw new ArgumentException($"{nameof(MongoDbContext)} must be initialized globally.");
//            }
//        }
//    }
//}
