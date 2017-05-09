namespace ionix.Data.MongoDB
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using global::MongoDB.Driver;
    using Utils;
    using Utils.Extensions;
    using Utils.Reflection;

    public static class Admin
    {
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

        private static string[] GetNames(Type type)
        {
            string[] splits = type.FullName.Split('.');
            if (splits.Length != 2)
                throw new InvalidOperationException($"{type.FullName} is not compatible with CreateCollection desing rules.");

            return splits;
        }

        public static void CreateCollection<TEntity>()
        {
            var splits = GetNames(typeof(TEntity));

            var db = GetDatabase(splits[0]);

            db.CreateCollection(splits[1]);
        }

        public static async void CreateCollectionAsync<TEntity>()
        {
            var splits = GetNames(typeof(TEntity));

            var db = GetDatabase(splits[0]);

            await db.CreateCollectionAsync(splits[1]);
        }

        public static IMongoCollection<TEntity> GetCollection<TEntity>()
        {
            var splits = GetNames(typeof(TEntity));
            var db = GetDatabase(splits[0]);
            var table = db.GetCollection<TEntity>(splits[1]);
            return table;
        }

        public static IEnumerable<string> CreateIndex<TEntity>(params Expression<Func<TEntity, object>>[] exps)
        {
            if (null == exps || 0 == exps.Length)
                throw new ArgumentNullException(nameof(exps));

            var props = new PropertyInfo[exps.Length];
            for(int j = 0; j < exps.Length; ++j)
                props[j] = ReflectionExtensions.GetPropertyInfo(exps[j]);

            var table = GetCollection<TEntity>();

            if (props.Length == 1)
            {
                var prop = props[0];
                var keys = prop.PropertyType == CachedTypes.String ? Builders<TEntity>.IndexKeys.Text(prop.Name) :  Builders<TEntity>.IndexKeys.Ascending(prop.Name);
                return table.Indexes.CreateOne(keys).ToSingleItemList();
            }
            else
            {
                List<CreateIndexModel<TEntity>> indexes = new List<CreateIndexModel<TEntity>>(props.Length);
                foreach (var prop in props)
                {
                    var keys = prop.PropertyType == CachedTypes.String ? Builders<TEntity>.IndexKeys.Text(prop.Name) : Builders<TEntity>.IndexKeys.Ascending(prop.Name);
                    CreateIndexModel<TEntity> cim = new CreateIndexModel<TEntity>(keys);
                    indexes.Add(cim);
                }

                return table.Indexes.CreateMany(indexes);
            }
        }
    }
}
