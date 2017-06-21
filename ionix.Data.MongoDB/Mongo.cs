namespace ionix.Data.MongoDB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using System.Reflection;
    using global::MongoDB.Bson;
    using global::MongoDB.Bson.Serialization.Attributes;
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;
    using Utils.Extensions;
    using Utils.Reflection;

    public sealed class Mongo
    {
        public static readonly Mongo Cmd = new Mongo();

        private readonly IMongoClient client;
        public Mongo(IMongoClient client)
        {
            if (null == client)
                throw new ArgumentNullException(nameof(client));

            this.client = client;
        }

        public Mongo()
            : this(MongoClientProxy.Instance)
        {
            
        }


        public IMongoCollection<TEntity> Get<TEntity>()
        {
            return MongoAdmin.GetCollection<TEntity>(this.client);
        }

        #region |   Select   |

        public long Count<TEntity>()
        {
            var table = this.Get<TEntity>();
            return table.Count(new BsonDocument());
        }

        public Task<long> CountAsync<TEntity>()
        {
            var table = this.Get<TEntity>();
            return table.CountAsync(new BsonDocument());
        }

        public IMongoQueryable<TEntity> AsQueryable<TEntity>()
        {
            var table = this.Get<TEntity>();
            return table.AsQueryable();
        }

        private static FilterDefinition<TEntity> GetIdFilterDefination<TEntity>(object id)
        {
            return Builders<TEntity>.Filter.Eq("_id", id);
        }
        private static PropertyInfo GetIdProperty<TEntity>(bool throwIfNotFound)
        {
            foreach (var pi in typeof(TEntity).GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (pi.GetCustomAttribute<BsonIdAttribute>() != null)
                    return pi;
            }

            if (throwIfNotFound)
                throw new NotSupportedException($"{typeof(TEntity).Name} does not have an BsonId Property.");

            return null;
        }

        public TEntity GetById<TEntity>(object id)
        {
            return this.Get<TEntity>().Find(GetIdFilterDefination<TEntity>(id)).FirstOrDefault();
        }

        public async Task<TEntity> GetByIdAsync<TEntity>(object id)
        {
            var result = await this.Get<TEntity>().FindAsync(GetIdFilterDefination<TEntity>(id));
            return result.FirstOrDefault();
        }

        #endregion

        #region |   Insert     |

        public void InsertOne<TEntity>(TEntity entity, InsertOneOptions options = null)
        {
            if (null != entity)
                this.Get<TEntity>().InsertOne(entity, options);
        }

        public Task InsertOneAsync<TEntity>(TEntity entity, InsertOneOptions options = null)
        {
            if (null != entity)
                return this.Get<TEntity>().InsertOneAsync(entity, options);

            return Task.Delay(0);
        }

        public void InsertMany<TEntity>(IEnumerable<TEntity> entityList, InsertManyOptions options = null)
        {
            if (!entityList.IsEmptyList())
                this.Get<TEntity>().InsertMany(entityList, options);
        }

        public Task InsertManyAsync<TEntity>(IEnumerable<TEntity> entityList, InsertManyOptions options = null)
        {
            if (!entityList.IsEmptyList())
                return this.Get<TEntity>().InsertManyAsync(entityList, options);

            return Task.Delay(0);
        }

        #endregion

        #region |   Replace     |


        private ReplaceOneResult ReplaceOneInternal<TEntity>(FilterDefinition<TEntity> filterDefination
            , TEntity entity, UpdateOptions options)
        {
            //FilterDefinition<TEntity> filter = "";
            if (null != filterDefination && null != entity)
                return this.Get<TEntity>().ReplaceOne(filterDefination, entity, options);

            return null;
        }

        public ReplaceOneResult ReplaceOne<TEntity>(Expression<Func<TEntity, bool>> predicate
            , TEntity entity, UpdateOptions options = null)
        {
            return this.ReplaceOneInternal(predicate, entity, options);
        }
        //id değeri var ise bunu kullan.
        public ReplaceOneResult ReplaceOne<TEntity>(TEntity entity, UpdateOptions options = null)
        {
            var idPi = GetIdProperty<TEntity>(true);
            return this.ReplaceOneInternal(GetIdFilterDefination<TEntity>(idPi.GetValue(entity)), entity, options);
        }


        private Task<ReplaceOneResult> ReplaceOneInternalAsync<TEntity>(FilterDefinition<TEntity> filterDefination
            , TEntity entity, UpdateOptions options)
        {
            //FilterDefinition<TEntity> filter = "";
            if (null != filterDefination && null != entity)
                return this.Get<TEntity>().ReplaceOneAsync(filterDefination, entity, options);

            return null;
        }

        public Task<ReplaceOneResult> ReplaceOneAsync<TEntity>(Expression<Func<TEntity, bool>> predicate
            , TEntity entity, UpdateOptions options = null)
        {
            return this.ReplaceOneInternalAsync(predicate, entity, options);
        }
        //id değeri var ise bunu kullan.
        public Task<ReplaceOneResult> ReplaceOneAsync<TEntity>(TEntity entity, UpdateOptions options = null)
        {
            var idPi = GetIdProperty<TEntity>(true);
            return this.ReplaceOneInternalAsync(GetIdFilterDefination<TEntity>(idPi.GetValue(entity)), entity, options);
        }

        #endregion

        #region |   Update     |
        //Örneğin şöyle genişlet; Id prop u reflection ile bul ve predicate' e gerek kalmasın.
        public UpdateResult UpdateOne<TEntity>(Expression<Func<TEntity, bool>> filter,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            var set = Builders<TEntity>.Update;
            var def = sets(set);

            return this.Get<TEntity>().UpdateOne(filter, def, options);
        }

        public UpdateResult UpdateOne<TEntity>(object id,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            var filter = GetIdFilterDefination<TEntity>(id);
            var set = Builders<TEntity>.Update;
            var def = sets(set);

            return this.Get<TEntity>().UpdateOne(filter, def, options);
        }

        public UpdateResult UpdateOne<TEntity>(TEntity entity
            , params Expression<Func<TEntity, object>>[] fields)
        {
            if (null != entity && null != fields && 0 != fields.Length)
            {
                var idPi = GetIdProperty<TEntity>(true);
                object idValue = idPi.GetValue(entity);

                Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets = (builder) =>
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>(fields.Length);
                    foreach (var exp in fields)
                    {
                        var pi = ReflectionExtensions.GetPropertyInfo(exp);
                        if (null != pi)
                        {
                            dic[pi.Name] = pi.GetValue(entity);
                        }
                    }

                    var bd = new BsonDocument("$set", new BsonDocument(dic));
                    return bd;
                };

                return this.UpdateOne(idValue, sets, null);
            }

            return null;
        }



        public Task<UpdateResult> UpdateOneAsync<TEntity>(Expression<Func<TEntity, bool>> filter,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            var set = Builders<TEntity>.Update;
            var def = sets(set);

            return this.Get<TEntity>().UpdateOneAsync(filter, def, options);
        }

        public Task<UpdateResult> UpdateOneAsync<TEntity>(object id,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            var filter = GetIdFilterDefination<TEntity>(id);
            var set = Builders<TEntity>.Update;
            var def = sets(set);

            return this.Get<TEntity>().UpdateOneAsync(filter, def, options);
        }


        public UpdateResult UpdateMany<TEntity>(Expression<Func<TEntity, bool>> filter,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            var set = Builders<TEntity>.Update;
            var def = sets(set);

            return this.Get<TEntity>().UpdateMany(filter, def, options);
        }

        public Task<UpdateResult> UpdateManyAsync<TEntity>(Expression<Func<TEntity, bool>> filter,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            var set = Builders<TEntity>.Update;
            var def = sets(set);

            return this.Get<TEntity>().UpdateManyAsync(filter, def, options);
        }
        #endregion

        #region |   Delete     |

        public DeleteResult DeleteOne<TEntity>(Expression<Func<TEntity, bool>> filter, DeleteOptions options = null)
        {
            var coll = this.Get<TEntity>();
            return coll.DeleteOne<TEntity>(filter, options);
        }

        public DeleteResult DeleteOne<TEntity>(object id, DeleteOptions options = null)
        {
            var coll = this.Get<TEntity>();
            return coll.DeleteOne(GetIdFilterDefination<TEntity>(id), options);
        }

        public Task<DeleteResult> DeleteOneAsync<TEntity>(Expression<Func<TEntity, bool>> filter, DeleteOptions options = null)
        {
            var coll = this.Get<TEntity>();
            return coll.DeleteOneAsync<TEntity>(filter, options);
        }
        public Task<DeleteResult> DeleteOneAsync<TEntity>(object id, DeleteOptions options = null)
        {
            var coll = this.Get<TEntity>();
            return coll.DeleteOneAsync(GetIdFilterDefination<TEntity>(id), options);
        }


        //Örneğin şöyle genişlet; Id prop u reflection ile bul ve predicate' e gerek kalmasın.
        public DeleteResult DeleteMany<TEntity>(Expression<Func<TEntity, bool>> filter, DeleteOptions options = null)
        {
            var coll = this.Get<TEntity>();
            return coll.DeleteMany<TEntity>(filter, options);
        }

        public Task<DeleteResult> DeleteManyAsync<TEntity>(Expression<Func<TEntity, bool>> filter, DeleteOptions options = null)
        {
            var coll = this.Get<TEntity>();
            return coll.DeleteManyAsync<TEntity>(filter, options);
        }
        #endregion


        #region   |   BulkReplace  |


        public Task<BulkWriteResult<TEntity>> BulkReplaceAsync<TEntity>(IEnumerable<TEntity> list, BulkWriteOptions options, bool isUpsert, params Expression<Func<TEntity, object>>[] filterFields)
        {
            if (filterFields.IsEmptyList())
                throw new ArgumentException(nameof(filterFields) + " can not be null or empty.");

            var properties = filterFields.Select(ReflectionExtensions.GetPropertyInfo).ToList();
            List<WriteModel<TEntity>> requests = new List<WriteModel<TEntity>>();
            foreach (var entity in list)
            {
                var dic = new Dictionary<string, object>();
                foreach (PropertyInfo pi in properties)
                {
                    dic[pi.Name] = pi.GetValue(entity);
                }
                BsonDocument bd = new BsonDocument(dic);

                FilterDefinition<TEntity> fd = bd;

                requests.Add(new ReplaceOneModel<TEntity>(fd, entity) { IsUpsert = isUpsert });
            }
            return this.Get<TEntity>().BulkWriteAsync(requests, options);
        }

        public Task<BulkWriteResult<TEntity>> BulkReplaceAsync<TEntity>(IEnumerable<TEntity> list,
            BulkWriteOptions options, bool isUpsert)
        {
            var pi = typeof(TEntity).GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(p => p.GetCustomAttribute<BsonIdAttribute>() != null);
            if (null == pi)
                throw new InvalidOperationException($"{typeof(TEntity).Name} has no BsonId field");

            List<WriteModel<TEntity>> requests = new List<WriteModel<TEntity>>();
            foreach (var entity in list)
            {
                var dic = new Dictionary<string, object>();
                dic["_id"] = pi.GetValue(entity);

                BsonDocument bd = new BsonDocument(dic);

                FilterDefinition<TEntity> fd = bd;
                ;
                requests.Add(new ReplaceOneModel<TEntity>(fd, entity) { IsUpsert = isUpsert });
            }
            return this.Get<TEntity>().BulkWriteAsync(requests, options);
        }

        #endregion


        #region   |   BulkUpdate  |

        //Filter id den alacak.
        public Task<BulkWriteResult<TEntity>> BulkUpdateAsync<TEntity>(IEnumerable<TEntity> list,
            BulkWriteOptions options, params Expression<Func<TEntity, object>>[] updateFields)
        {
            var idPi = typeof(TEntity).GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(p => p.GetCustomAttribute<BsonIdAttribute>() != null);
            if (null == idPi)
                throw new InvalidOperationException($"{typeof(TEntity).Name} has no BsonId field");

            var properties = updateFields.Select(ReflectionExtensions.GetPropertyInfo).ToList();

            List<WriteModel<TEntity>> requests = new List<WriteModel<TEntity>>();
            foreach (var entity in list)
            {
                var dic = new Dictionary<string, object>();
                dic["_id"] = idPi.GetValue(entity);

                BsonDocument filterBd = new BsonDocument(dic);

                FilterDefinition<TEntity> fd = filterBd;

                dic.Clear();
                foreach (var pi in properties)
                {
                    dic[pi.Name] = pi.GetValue(entity);
                }

                var update = new BsonDocument("$set", new BsonDocument(dic));

                requests.Add(new UpdateOneModel<TEntity>(fd, update));
            }
            return this.Get<TEntity>().BulkWriteAsync(requests, options);
        }

        #endregion


        #region   |   BulkDelete   |

        public Task<BulkWriteResult<TEntity>> BulkDeleteAsync<TEntity>(IEnumerable<TEntity> list, BulkWriteOptions options, params Expression<Func<TEntity, object>>[] filterFields)
        {
            if (filterFields.IsEmptyList())
                throw new ArgumentException(nameof(filterFields) + " can not be null or empty.");

            var properties = filterFields.Select(ReflectionExtensions.GetPropertyInfo).ToList();
            List<WriteModel<TEntity>> requests = new List<WriteModel<TEntity>>();
            foreach (var entity in list)
            {
                var dic = new Dictionary<string, object>();
                foreach (PropertyInfo pi in properties)
                {
                    dic[pi.Name] = pi.GetValue(entity);
                }
                BsonDocument bd = new BsonDocument(dic);

                FilterDefinition<TEntity> fd = bd;

                requests.Add(new DeleteOneModel<TEntity>(fd));
            }
            return this.Get<TEntity>().BulkWriteAsync(requests, options);
        }

        public Task<BulkWriteResult<TEntity>> BulkDeleteAsync<TEntity>(IEnumerable<TEntity> list,
            BulkWriteOptions options)
        {
            var idPi = typeof(TEntity).GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(p => p.GetCustomAttribute<BsonIdAttribute>() != null);

            List<WriteModel<TEntity>> requests = new List<WriteModel<TEntity>>();
            foreach (var entity in list)
            {
                var dic = new Dictionary<string, object>();
                dic["_id"] = idPi.GetValue(entity);
                BsonDocument bd = new BsonDocument(dic);

                FilterDefinition<TEntity> fd = bd;

                requests.Add(new DeleteOneModel<TEntity>(fd));
            }
            return this.Get<TEntity>().BulkWriteAsync(requests, options);
        }

        #endregion


        #region   |   Search   |

        public IEnumerable<TEntity> TextSearch<TEntity>(string text)
        {
            return this.Get<TEntity>().Find(Builders<TEntity>.Filter.Text(text)).ToList();
        }

        public async Task<IEnumerable<TEntity>> TextSearchAsync<TEntity>(string text)
        {
            var result = await this.Get<TEntity>().FindAsync(Builders<TEntity>.Filter.Text(text));
            return await result.ToListAsync();
        }

        #endregion
    }
}
