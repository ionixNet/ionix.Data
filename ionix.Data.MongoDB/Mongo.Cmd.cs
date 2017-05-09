namespace ionix.Data.MongoDB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using global::MongoDB.Bson;
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;
    using Utils.Extensions;
    using Utils.Reflection;

    public sealed class Mongo
    {
        public static readonly Mongo Cmd = new Mongo();

        private Mongo() { }

        public long Count<TEntity>()
        {
            var table = Admin.GetCollection<TEntity>();
            return table.Count(new BsonDocument());
        }

        public Task<long> CountAsync<TEntity>()
        {
            var table = Admin.GetCollection<TEntity>();
            return table.CountAsync(new BsonDocument());
        }

        //for all query operations
        public IMongoQueryable<TEntity> AsQueryable<TEntity>()
        {
            var table = Admin.GetCollection<TEntity>();
            return table.AsQueryable();
        }
        //

        #region Insert

        public void Insert<TEntity>(TEntity entity)
        {
            if (null == entity)
                throw new ArgumentNullException(nameof(entity));

            var table = Admin.GetCollection<TEntity>();
            table.InsertOne(entity);
        }

        public Task InsertAsync<TEntity>(TEntity entity)
        {
            if (null == entity)
                throw new ArgumentNullException(nameof(entity));

            var table = Admin.GetCollection<TEntity>();
            return table.InsertOneAsync(entity);
        }

        public void BatchInsert<TEntity>(IEnumerable<TEntity> entityList)
        {
            if (entityList.IsEmptyList())
                throw new ArgumentNullException(nameof(entityList));

            var table = Admin.GetCollection<TEntity>();
            table.InsertMany(entityList);
        }

        public Task BatchInsertAsync<TEntity>(IEnumerable<TEntity> entityList)
        {
            if (entityList.IsEmptyList())
                throw new ArgumentNullException(nameof(entityList));

            var table = Admin.GetCollection<TEntity>();
            return table.InsertManyAsync(entityList);
        }

        #endregion


        #region Update

        private static FilterDefinition<TEntity> FilterDefinitionForUpdate<TEntity>(TEntity entity, Expression<Func<TEntity, object>> updateField,
            out UpdateDefinition<TEntity> update, out IMongoCollection<TEntity> table)
        {
            if (null == entity)
                throw new ArgumentNullException(nameof(entity));
            if (null == updateField)
                throw new ArgumentNullException(nameof(updateField));

            var id = typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(p => p.PropertyType == typeof(ObjectId));
            if (null == id)
                throw new ArgumentNullException($"No ObjectId property found on '{typeof(TEntity).FullName}'");

            var filter = Builders<TEntity>.Filter.Eq(id.Name, BsonValue.Create(id.GetValue(entity))); //new BsonDocument("_id", BsonValue.Create(id.GetValue(entity)));

            var pi = ReflectionExtensions.GetPropertyInfo(updateField);
            update = Builders<TEntity>.Update.Set(pi.Name, BsonValue.Create(pi.GetValue(entity)));

            table = Admin.GetCollection<TEntity>();

            return filter;
        }
        //TEntity should contains a ObjectId property
        public UpdateResult Update<TEntity>(TEntity entity, Expression<Func<TEntity, object>> updateField)
        {
            UpdateDefinition<TEntity> update;
            IMongoCollection<TEntity> table;
            var filter = FilterDefinitionForUpdate(entity, updateField, out update, out table);
            return table.UpdateOne(filter, update);
        }
        public Task<UpdateResult> UpdateAsync<TEntity>(TEntity entity, Expression<Func<TEntity, object>> updateField)
        {
            UpdateDefinition<TEntity> update;
            IMongoCollection<TEntity> table;
            var filter = FilterDefinitionForUpdate(entity, updateField, out update, out table);

            return table.UpdateOneAsync(filter, update);
        }



        private static FilterDefinition<TEntity> FilterDefinitionForReplace<TEntity>(TEntity entity, out IMongoCollection<TEntity> table)
        {
            if (null == entity)
                throw new ArgumentNullException(nameof(entity));

            var id = typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(p => p.PropertyType == typeof(ObjectId));
            if (null == id)
                throw new ArgumentNullException($"No ObjectId property found on '{typeof(TEntity).FullName}'");

            var filter =
                Builders<TEntity>.Filter.Eq(id.Name,
                    BsonValue.Create(id.GetValue(entity))); //new BsonDocument("_id", BsonValue.Create(id.GetValue(entity)));

            table = Admin.GetCollection<TEntity>();
            return filter;
        }
        public ReplaceOneResult Update<TEntity>(TEntity entity)
        {
            IMongoCollection<TEntity> table;
            var filter = FilterDefinitionForReplace(entity, out table);
            return table.ReplaceOne(filter, entity);
        }
        public Task<ReplaceOneResult> UpdateAsync<TEntity>(TEntity entity)
        {
            IMongoCollection<TEntity> table;
            var filter = FilterDefinitionForReplace(entity, out table);
            return table.ReplaceOneAsync(filter, entity);
        }



        private static IMongoCollection<TEntity> GetBatchUpdateNeedsForUpdate<TEntity>(IEnumerable<TEntity> entityList, Expression<Func<TEntity, object>> updateField,
            out List<UpdateOneModel<TEntity>> list)
        {
            if (entityList.IsEmptyList())
                throw new ArgumentNullException(nameof(entityList));
            if (null == updateField)
                throw new ArgumentNullException(nameof(updateField));

            IMongoCollection<TEntity> table = null;
            list = new List<UpdateOneModel<TEntity>>();
            foreach (var entity in entityList)
            {
                UpdateDefinition<TEntity> update;
                var filter = FilterDefinitionForUpdate(entity, updateField, out update, out table);

                var item = new UpdateOneModel<TEntity>(filter, update);
                list.Add(item);
            }
            return table;
        }

        private static readonly BulkWriteOptions DefaultBulkWriteOptions = new BulkWriteOptions() { IsOrdered = true };
        public BulkWriteResult<TEntity> BatchUpdate<TEntity>(IEnumerable<TEntity> entityList, Expression<Func<TEntity, object>> updateField)
        {
            List<UpdateOneModel<TEntity>> list;
            var table = GetBatchUpdateNeedsForUpdate(entityList, updateField, out list);

            return table?.BulkWrite(list, DefaultBulkWriteOptions);
        }
        public Task<BulkWriteResult<TEntity>> BatchUpdateAsync<TEntity>(IEnumerable<TEntity> entityList, Expression<Func<TEntity, object>> updateField)
        {
            List<UpdateOneModel<TEntity>> list;
            var table = GetBatchUpdateNeedsForUpdate(entityList, updateField, out list);

            return table?.BulkWriteAsync(list, DefaultBulkWriteOptions);
        }


        private static IMongoCollection<TEntity> GetBatchUpdateNeedsForReplace<TEntity>(IEnumerable<TEntity> entityList, out List<ReplaceOneModel<TEntity>> list)
        {
            if (entityList.IsEmptyList())
                throw new ArgumentNullException(nameof(entityList));

            IMongoCollection<TEntity> table = null;
            list = new List<ReplaceOneModel<TEntity>>();
            foreach (var entity in entityList)
            {
                var filter = FilterDefinitionForReplace(entity, out table);

                var item = new ReplaceOneModel<TEntity>(filter, entity);
                list.Add(item);
            }
            return table;
        }
        public BulkWriteResult<TEntity> BatchUpdate<TEntity>(IEnumerable<TEntity> entityList)
        {
            List<ReplaceOneModel<TEntity>> list;
            var table = GetBatchUpdateNeedsForReplace(entityList, out list);

            return table?.BulkWrite(list, DefaultBulkWriteOptions);
        }

        public Task<BulkWriteResult<TEntity>> BatchUpdateAsync<TEntity>(IEnumerable<TEntity> entityList)
        {
            List<ReplaceOneModel<TEntity>> list;
            var table = GetBatchUpdateNeedsForReplace(entityList, out list);

            return table?.BulkWriteAsync(list, DefaultBulkWriteOptions);
        }

        #endregion


        #region Upsert

        private static readonly UpdateOptions UpsertUpdateOptions = new UpdateOptions() {IsUpsert = true};
        public UpdateResult Upsert<TEntity>(TEntity entity, Expression<Func<TEntity, object>> updateField)
        {
            UpdateDefinition<TEntity> update;
            IMongoCollection<TEntity> table;
            var filter = FilterDefinitionForUpdate(entity, updateField, out update, out table);
            return table.UpdateOne(filter, update, UpsertUpdateOptions);
        }
        public Task<UpdateResult> UpsertAsync<TEntity>(TEntity entity, Expression<Func<TEntity, object>> updateField)
        {
            UpdateDefinition<TEntity> update;
            IMongoCollection<TEntity> table;
            var filter = FilterDefinitionForUpdate(entity, updateField, out update, out table);
            return table.UpdateOneAsync(filter, update, UpsertUpdateOptions);
        }
        public ReplaceOneResult Upsert<TEntity>(TEntity entity)
        {
            IMongoCollection<TEntity> table;
            var filter = FilterDefinitionForReplace(entity, out table);
            return table.ReplaceOne(filter, entity, UpsertUpdateOptions);
        }
        public Task<ReplaceOneResult> UpsertAsync<TEntity>(TEntity entity)
        {
            IMongoCollection<TEntity> table;
            var filter = FilterDefinitionForReplace(entity, out table);
            return table.ReplaceOneAsync(filter, entity, UpsertUpdateOptions);
        }

        #endregion

        #region Delete

        public DeleteResult Delete<TEntity>(TEntity entity)
        {
            IMongoCollection<TEntity> table;
            var filter = FilterDefinitionForReplace(entity, out table);
            return table.DeleteOne(filter);
        }
        public Task<DeleteResult> DeleteAsync<TEntity>(TEntity entity)
        {
            IMongoCollection<TEntity> table;
            var filter = FilterDefinitionForReplace(entity, out table);
            return table.DeleteOneAsync(filter);
        }


        #endregion
    }
}
