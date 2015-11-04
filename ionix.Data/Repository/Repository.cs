namespace ionix.Data
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;


    public partial class Repository<TEntity> : IRepository<TEntity> //Repository and proxy pattern
        where TEntity : class, new()
    {
        private readonly EventHandlerList events;

        public Repository(ICommandAdapter cmd)
        {
            if (null == cmd)
                throw new ArgumentNullException(nameof(cmd));

            this.Cmd = cmd;
            this.events = new EventHandlerList();
        }

        public ICommandAdapter Cmd { get; }

        public IDbAccess DataAccess => this.Cmd.Factory.DataAccess;

        public void Dispose()
        {
            if (null != this.Cmd)
                this.Cmd.Factory.DataAccess.Dispose();
            if (null != this.events)
                this.events.Dispose();
        }

        #region   |   Select   |

        public virtual TEntity SelectById(params object[] idValues)
        {
            if (!idValues.IsEmptyList())
                return this.Cmd.SelectById<TEntity>(idValues);
            return null;
        }

        public virtual TEntity SelectSingle(SqlQuery extendedQuery)
        {
            return this.Cmd.SelectSingle<TEntity>(extendedQuery);
        }

        public virtual IList<TEntity> Select(SqlQuery extendedQuery)
        {
            return this.Cmd.Select<TEntity>(extendedQuery);
        }

        public virtual TEntity QuerySingle(SqlQuery query)
        {
            if (null != query && !query.IsEmpty())
            {
                return this.Cmd.QuerySingle<TEntity>(query);
            }
            return null;
        }

        public virtual IList<TEntity> Query(SqlQuery query)
        {
            if (null != query && !query.IsEmpty())
            {
                return this.Cmd.Query<TEntity>(query);
            }
            return new List<TEntity>();
        }

        #endregion


        #region   |   Entity   |

        public virtual int Update(TEntity entity, params string[] updatedFields)
        {
            using (CommandScope scope = new CommandScope(this, entity, ExecuteCommandType.Update))
            {
                return scope.Execute(() => this.Cmd.Update(entity, updatedFields));
            }
        }

        public virtual int Insert(TEntity entity, params string[] insertFields)
        {
            using (CommandScope scope = new CommandScope(this, entity, ExecuteCommandType.Insert))
            {
                return scope.Execute(() => this.Cmd.Insert(entity, insertFields));
            }
        }

        public virtual int Upsert(TEntity entity, string[] updatedFields, string[] insertFields)
        {
            using (CommandScope scope = new CommandScope(this, entity, ExecuteCommandType.Upsert))
            {
                return scope.Execute(() => this.Cmd.Upsert(entity, updatedFields, insertFields));
            }
        }

        public virtual int Delete(TEntity entity)
        {
            using (CommandScope scope = new CommandScope(this, entity, ExecuteCommandType.Delete))
            {
                return scope.Execute(() => this.Cmd.Delete(entity));
            }
        }

        #endregion


        #region   |   Batch

        public virtual int BatchUpdate(IEnumerable<TEntity> entityList, BatchCommandMode mode,
            params string[] updatedFields)
        {
            using (CommandScope scope = new CommandScope(this, entityList, ExecuteCommandType.Update))
            {
                return scope.Execute(() => this.Cmd.BatchUpdate(entityList, mode, updatedFields));
            }
        }


        public virtual int BatchInsert(IEnumerable<TEntity> entityList, BatchCommandMode mode,
            params string[] insertFields)
        {
            using (CommandScope scope = new CommandScope(this, entityList, ExecuteCommandType.Insert))
            {
                return scope.Execute(() => this.Cmd.BatchInsert(entityList, mode, insertFields));
            }
        }



        public virtual int BatchUpsert(IEnumerable<TEntity> entityList, BatchCommandMode mode, string[] updatedFields,
            string[] insertFields)
        {
            using (CommandScope scope = new CommandScope(this, entityList, ExecuteCommandType.Upsert))
            {
                return scope.Execute(() => this.Cmd.BatchUpsert(entityList, mode, updatedFields, insertFields));
            }
        }



        public virtual int Delsert(IEnumerable<TEntity> entityList, BatchCommandMode mode,
            Func<FluentWhere<TEntity>, FluentWhere<TEntity>> where, params string[] insertFields)
        {
            using (CommandScope scope = new CommandScope(this, entityList, ExecuteCommandType.Delsert))
            {
                return scope.Execute(() => this.Cmd.Delsert(entityList, mode, where, insertFields));
            }
        }

        #endregion
    }
}
