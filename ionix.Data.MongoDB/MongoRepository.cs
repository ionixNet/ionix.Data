namespace ionix.Data.MongoDB
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;

    //a kind of proxy
    public class MongoRepository<TEntity>
    {
        private readonly Mongo mongo;
        public MongoRepository(IMongoClient client)
        {
            this.mongo = new Mongo(client);
        }

        public MongoRepository()
            : this(MongoClientProxy.Instance)
        {
            
        }

        public IMongoCollection<TEntity> Collection => this.mongo.Get<TEntity>();

        #region |   Select   |

        public long Count()
        {
            return this.mongo.Count<TEntity>();
        }

        public Task<long> CountAsync()
        {
            return this.mongo.CountAsync<TEntity>();
        }

        public IMongoQueryable<TEntity> AsQueryable()
        {
            return this.mongo.AsQueryable<TEntity>();
        }

        public TEntity GetById(object id)
        {
            return this.mongo.GetById<TEntity>(id);
        }

        public Task<TEntity> GetByIdAsync(object id)
        {
            return this.mongo.GetByIdAsync<TEntity>(id);
        }

        #endregion


        #region |   Insert   |

        public void InsertOne(TEntity entity, InsertOneOptions options = null)
        {
            this.mongo.InsertOne(entity, options);
        }

        public Task InsertOneAsync(TEntity entity, InsertOneOptions options = null)
        {
            return this.mongo.InsertOneAsync(entity, options);
        }

        public void InsertMany(IEnumerable<TEntity> entityList, InsertManyOptions options = null)
        {
            this.mongo.InsertMany(entityList, options);
        }

        public Task InsertManyAsync(IEnumerable<TEntity> entityList, InsertManyOptions options = null)
        {
            return this.mongo.InsertManyAsync(entityList, options);
        }

        #endregion


        #region |   Replace   |

        public ReplaceOneResult ReplaceOne(Expression<Func<TEntity, bool>> predicate, TEntity entity, UpdateOptions options = null)
        {
            return this.mongo.ReplaceOne(predicate, entity, options);
        }
        //id value must be set.
        public ReplaceOneResult ReplaceOne(TEntity entity, UpdateOptions options = null)
        {
            return this.mongo.ReplaceOne(entity, options);
        }

        //waring: _id values in MongoDB documents are immutable. If you specify an _id in the replacement document, it must match the _id of the existing document.
        public Task<ReplaceOneResult> ReplaceOneAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity, UpdateOptions options = null)
        {
            return this.mongo.ReplaceOneAsync(predicate, entity, options);
        }
        public Task<ReplaceOneResult> ReplaceOneAsync(TEntity entity, UpdateOptions options = null)
        {
            return this.mongo.ReplaceOneAsync(entity, options);
        }

        #endregion

        #region |   Update   |
        public UpdateResult UpdateOne(Expression<Func<TEntity, bool>> filter,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            return this.mongo.UpdateOne(filter, sets, options);
        }

        public UpdateResult UpdateOne(object id,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            return this.mongo.UpdateOne(id, sets, options);
        }

        public UpdateResult UpdateOne(TEntity entity
            , params Expression<Func<TEntity, object>>[] fields)
        {
            return this.mongo.UpdateOne(entity, fields);
        }

        public Task<UpdateResult> UpdateOneAsync(Expression<Func<TEntity, bool>> predicate,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            return this.mongo.UpdateOneAsync(predicate, sets, options);
        }

        public Task<UpdateResult> UpdateOneAsync(object id,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            return this.mongo.UpdateOneAsync(id, sets, options);
        }


        public UpdateResult UpdateMany(Expression<Func<TEntity, bool>> filter,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            return this.mongo.UpdateMany(filter, sets, options);
        }

        public Task<UpdateResult> UpdateManyAsync(Expression<Func<TEntity, bool>> filter,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            return this.mongo.UpdateManyAsync(filter, sets, options);
        }

        #endregion

        #region |   Select   |

        public DeleteResult DeleteOne(Expression<Func<TEntity, bool>> filter, DeleteOptions options = null)
        {
            return this.mongo.DeleteOne(filter, options);
        }

        public DeleteResult DeleteOne(object id, DeleteOptions options = null)
        {
            return this.mongo.DeleteOne<TEntity>(id, options);
        }

        public Task<DeleteResult> DeleteOneAsync(Expression<Func<TEntity, bool>> filter, DeleteOptions options = null)
        {
            return this.mongo.DeleteOneAsync(filter, options);
        }

        public Task<DeleteResult> DeleteOneAsync(object id, DeleteOptions options = null)
        {
            return this.mongo.DeleteOneAsync<TEntity>(id, options);
        }


        public DeleteResult DeleteMany(Expression<Func<TEntity, bool>> filter, DeleteOptions options = null)
        {
            return this.mongo.DeleteMany(filter, options);
        }
        public Task<DeleteResult> DeleteManyAsync(Expression<Func<TEntity, bool>> filter, DeleteOptions options = null)
        {
            return this.mongo.DeleteManyAsync(filter, options);
        }

        #endregion



        #region   |   BulkReplace  |

        public Task<BulkWriteResult<TEntity>> BulkReplaceAsync(IEnumerable<TEntity> list, BulkWriteOptions options, bool isUpsert, params Expression<Func<TEntity, object>>[] filterFields)
        {
            return this.mongo.BulkReplaceAsync(list, options, isUpsert, filterFields);
        }

        public Task<BulkWriteResult<TEntity>> BulkReplaceAsync(IEnumerable<TEntity> list,
            BulkWriteOptions options, bool isUpsert)
        {
            return this.mongo.BulkReplaceAsync(list, options, isUpsert);
        }

        #endregion


        #region   |   BulkUpdate  |

        //Filter id den alacak.
        public Task<BulkWriteResult<TEntity>> BulkUpdateAsync(IEnumerable<TEntity> list,
            BulkWriteOptions options, params Expression<Func<TEntity, object>>[] updateFields)
        {
            return this.mongo.BulkUpdateAsync(list, options, updateFields);
        }

        #endregion


        #region   |   BulkDelete   |

        public Task<BulkWriteResult<TEntity>> BulkDeleteAsync(IEnumerable<TEntity> list, BulkWriteOptions options, params Expression<Func<TEntity, object>>[] filterFields)
        {
            return this.mongo.BulkDeleteAsync(list, options, filterFields);
        }

        public Task<BulkWriteResult<TEntity>> BulkDeleteAsync(IEnumerable<TEntity> list,
            BulkWriteOptions options)
        {
            return this.mongo.BulkDeleteAsync(list, options);
        }

        #endregion



        #region   |   Search   |

        public IEnumerable<TEntity> TextSearch(string text)
        {
            return this.mongo.TextSearch<TEntity>(text);
        }

        public Task<IEnumerable<TEntity>> TextSearchAsync(string text)
        {
            return this.mongo.TextSearchAsync<TEntity>(text);
        }

        #endregion
    }
}
