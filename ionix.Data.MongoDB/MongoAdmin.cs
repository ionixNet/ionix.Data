namespace ionix.Data.MongoDB
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using global::MongoDB.Bson;
    using global::MongoDB.Driver;
    using Utils.Reflection;

    public static class MongoAdmin
    {
        private static string ConvertToEvalScript(string script)
        {
            return "{ eval: \"" + script + "\"}";
        }

        public static void ExecuteScript<TEntity>(IMongoDatabase db, string script)
        {
            var command = new JsonCommand<TEntity>(ConvertToEvalScript(script));

            db.RunCommand(command);
        }
        public static void ExecuteScript(IMongoDatabase db, string script)
        {
            ExecuteScript<BsonDocument>(db, script);
        }

        public static IEnumerable<Database> GetDatabaseList()
        {
            List<Database> ret = new List<Database>();
            using (var cursor = MongoClientProxy.Instance.ListDatabases())
            {
                foreach (var document in cursor.ToEnumerable())
                {
                    ret.Add(document.ToString().BsonDeserialize<Database>());
                }
            }
            return ret;
        }

        public static void DropDatabase(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            MongoClientProxy.Instance.DropDatabase(name);
        }

        public static async void DropDatabaseAsync(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            await MongoClientProxy.Instance.DropDatabaseAsync(name);
        }

        public static IMongoDatabase GetDatabase(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            return MongoClientProxy.Instance.GetDatabase(name);
        }

        private static MongoCollectionAttribute GetNames(Type type)
        {
            MongoCollectionAttribute ret = type.GetTypeInfo().GetCustomAttribute<MongoCollectionAttribute>();
            if (null == ret)
            {
                string[] splits = type.FullName.Split('.');
                if (splits.Length != 2)
                    throw new InvalidOperationException(
                        $"{type.FullName} is not compatible with CreateCollection desing rules. Use MongoCollectionAttribute to set database and collection name correctly or set namespace as Database and class name as collection.");

                ret.Database = splits[0];
                ret.Name = splits[1];
            }

            return ret;
        }

        public static void CreateCollection<TEntity>()
        {
            var info = GetNames(typeof(TEntity));

            var db = GetDatabase(info.Database);

            db.CreateCollection(info.Name);
        }

        public static async void CreateCollectionAsync<TEntity>()
        {
            var info = GetNames(typeof(TEntity));

            var db = GetDatabase(info.Database);

            await db.CreateCollectionAsync(info.Name);
        }

        public static IMongoCollection<TEntity> GetCollection<TEntity>()
        {
            var info = GetNames(typeof(TEntity));
            var db = GetDatabase(info.Database);
            var table = db.GetCollection<TEntity>(info.Name);
            return table;
        }

        private static string CreateIndexTemplate<TEntity>(CreateIndexOptions<TEntity> options, Expression<Func<TEntity, object>>[] exps
            , Func<FieldDefinition<TEntity>, IndexKeysDefinition<TEntity>> func)
        {
            if (null == exps || 0 == exps.Length)
                throw new ArgumentNullException(nameof(exps));

            var props = new PropertyInfo[exps.Length];
            for (int j = 0; j < exps.Length; ++j)
                props[j] = ReflectionExtensions.GetPropertyInfo(exps[j]);

            var table = GetCollection<TEntity>();

            if (props.Length == 1)
            {
                var prop = props[0];
                FieldDefinition<TEntity> field = prop.Name;
                var keys = func(field);// Builders<TEntity>.IndexKeys.Ascending(prop.Name);

                return table.Indexes.CreateOne(keys, options);
            }
            else
            {
                List<IndexKeysDefinition<TEntity>> indexes = new List<IndexKeysDefinition<TEntity>>();
                foreach (var prop in props)
                {
                    FieldDefinition<TEntity> field = prop.Name;
                    IndexKeysDefinition<TEntity> index = func(field);// Builders<TEntity>.IndexKeys.Ascending(field);
                    indexes.Add(index);
                }

                return table.Indexes.CreateOne(Builders<TEntity>.IndexKeys.Combine(indexes.ToArray()), options);
            }
        }

        //İleride Text Index i de ekle.
        public static string CreateIndex<TEntity>(CreateIndexOptions<TEntity> options, params Expression<Func<TEntity, object>>[] exps)
        {
            return CreateIndexTemplate(options, exps, (field) => Builders<TEntity>.IndexKeys.Ascending(field));
        }

        public static string CreateIndex<TEntity>(params Expression<Func<TEntity, object>>[] exps)
        {
            return CreateIndex(null, exps);
        }

        public static string CreateTextIndex<TEntity>(CreateIndexOptions<TEntity> options, params Expression<Func<TEntity, object>>[] exps)
        {
            return CreateIndexTemplate(options, exps, (field) => Builders<TEntity>.IndexKeys.Text(field));
        }

        public static string CreateTextIndex<TEntity>(params Expression<Func<TEntity, object>>[] exps)
        {
            return CreateTextIndex(null, exps);
        }
    }
}
