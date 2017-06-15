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
        public static MongoRepository<TEntity> Create()
        {
            return new MongoRepository<TEntity>();
        }

        public IMongoCollection<TEntity> Collection => Mongo.Cmd.Get<TEntity>();

        #region |   Select   |

        public long Count()
        {
            return Mongo.Cmd.Count<TEntity>();
        }

        public Task<long> CountAsync()
        {
            return Mongo.Cmd.CountAsync<TEntity>();
        }

        public IMongoQueryable<TEntity> AsQueryable()
        {
            return Mongo.Cmd.AsQueryable<TEntity>();
        }

        public TEntity GetById(object id)
        {
            return Mongo.Cmd.GetById<TEntity>(id);
        }

        public Task<TEntity> GetByIdAsync(object id)
        {
            return Mongo.Cmd.GetByIdAsync<TEntity>(id);
        }

        #endregion


        #region |   Insert   |

        public void InsertOne(TEntity entity, InsertOneOptions options = null)
        {
            Mongo.Cmd.InsertOne(entity, options);
        }

        public Task InsertOneAsync(TEntity entity, InsertOneOptions options = null)
        {
            return Mongo.Cmd.InsertOneAsync(entity, options);
        }

        public void InsertMany(IEnumerable<TEntity> entityList, InsertManyOptions options = null)
        {
            Mongo.Cmd.InsertMany(entityList, options);
        }

        public Task InsertManyAsync(IEnumerable<TEntity> entityList, InsertManyOptions options = null)
        {
            return Mongo.Cmd.InsertManyAsync(entityList, options);
        }

        #endregion


        #region |   Replace   |

        public ReplaceOneResult ReplaceOne(Expression<Func<TEntity, bool>> predicate, TEntity entity, UpdateOptions options = null)
        {
            return Mongo.Cmd.ReplaceOne(predicate, entity, options);
        }
        //id value must be set.
        public ReplaceOneResult ReplaceOne(TEntity entity, UpdateOptions options = null)
        {
            return Mongo.Cmd.ReplaceOne(entity, options);
        }

        //waring: _id values in MongoDB documents are immutable. If you specify an _id in the replacement document, it must match the _id of the existing document.
        public Task<ReplaceOneResult> ReplaceOneAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity, UpdateOptions options = null)
        {
            return Mongo.Cmd.ReplaceOneAsync(predicate, entity, options);
        }
        public Task<ReplaceOneResult> ReplaceOneAsync(TEntity entity, UpdateOptions options = null)
        {
            return Mongo.Cmd.ReplaceOneAsync(entity, options);
        }

        #endregion

        #region |   Update   |
        public UpdateResult UpdateOne(Expression<Func<TEntity, bool>> filter,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            return Mongo.Cmd.UpdateOne(filter, sets, options);
        }

        public UpdateResult UpdateOne(object id,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            return Mongo.Cmd.UpdateOne(id, sets, options);
        }

        public UpdateResult UpdateOne(TEntity entity
            , params Expression<Func<TEntity, object>>[] fields)
        {
            return Mongo.Cmd.UpdateOne(entity, fields);
        }

        public Task<UpdateResult> UpdateOneAsync(Expression<Func<TEntity, bool>> predicate,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            return Mongo.Cmd.UpdateOneAsync(predicate, sets, options);
        }

        public Task<UpdateResult> UpdateOneAsync(object id,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            return Mongo.Cmd.UpdateOneAsync(id, sets, options);
        }


        public UpdateResult UpdateMany(Expression<Func<TEntity, bool>> filter,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            return Mongo.Cmd.UpdateMany(filter, sets, options);
        }

        public Task<UpdateResult> UpdateManyAsync(Expression<Func<TEntity, bool>> filter,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            return Mongo.Cmd.UpdateManyAsync(filter, sets, options);
        }

        #endregion

        #region |   Select   |

        public DeleteResult DeleteOne(Expression<Func<TEntity, bool>> filter, DeleteOptions options = null)
        {
            return Mongo.Cmd.DeleteOne(filter, options);
        }

        public DeleteResult DeleteOne(object id, DeleteOptions options = null)
        {
            return Mongo.Cmd.DeleteOne<TEntity>(id, options);
        }

        public Task<DeleteResult> DeleteOneAsync(Expression<Func<TEntity, bool>> filter, DeleteOptions options = null)
        {
            return Mongo.Cmd.DeleteOneAsync(filter, options);
        }

        public Task<DeleteResult> DeleteOneAsync(object id, DeleteOptions options = null)
        {
            return Mongo.Cmd.DeleteOneAsync<TEntity>(id, options);
        }


        public DeleteResult DeleteMany(Expression<Func<TEntity, bool>> filter, DeleteOptions options = null)
        {
            return Mongo.Cmd.DeleteMany(filter, options);
        }
        public Task<DeleteResult> DeleteManyAsync(Expression<Func<TEntity, bool>> filter, DeleteOptions options = null)
        {
            return Mongo.Cmd.DeleteManyAsync(filter, options);
        }

        #endregion
    }
}
