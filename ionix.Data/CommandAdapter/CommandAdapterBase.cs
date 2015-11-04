namespace ionix.Data
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;


    public abstract class CommandAdapterBase : ICommandAdapter
    {
        private readonly ICommandFactory factory;

        protected CommandAdapterBase(ICommandFactory factory)
        {
            if (null == factory)
                throw new ArgumentNullException(nameof(factory));

            this.factory = factory;
        }

        public ICommandFactory Factory => this.factory;

        protected abstract IEntityMetaDataProvider CreateProvider();

        private IEntityMetaDataProvider entityMetaDataProvider;
        public IEntityMetaDataProvider EntityMetaDataProvider
        {
            get
            {
                if (null == this.entityMetaDataProvider)
                    this.entityMetaDataProvider = this.CreateProvider();
                return this.entityMetaDataProvider;
            }
        }

        #region   |      Select     |

        public virtual TEntity SelectById<TEntity>(params object[] idValues)
            where TEntity : new()
        {
            IEntityCommandSelect cmd = this.factory.CreateSelectCommand();
            cmd.ConvertType = true;
            return cmd.SelectById<TEntity>(this.EntityMetaDataProvider, idValues);
        }

        public virtual TEntity SelectSingle<TEntity>(SqlQuery extendedQuery)
            where TEntity : new()
        {
            IEntityCommandSelect cmd = this.factory.CreateSelectCommand();
            cmd.ConvertType = true;
            return cmd.SelectSingle<TEntity>(this.EntityMetaDataProvider, extendedQuery);
        }



        public virtual IList<TEntity> Select<TEntity>(SqlQuery extendedQuery)
            where TEntity : new()
        {
            IEntityCommandSelect cmd = this.factory.CreateSelectCommand();
            cmd.ConvertType = true;

            return cmd.Select<TEntity>(this.EntityMetaDataProvider, extendedQuery);
        }


        public virtual TEntity QuerySingle<TEntity>(SqlQuery query)
            where TEntity : new()
        {
            IEntityCommandSelect cmd = this.factory.CreateSelectCommand();
            cmd.ConvertType = true;
            return cmd.QuerySingle<TEntity>(this.EntityMetaDataProvider, query);
        }
        public virtual IList<TEntity> Query<TEntity>(SqlQuery query)
            where TEntity : new()
        {
            IEntityCommandSelect cmd = this.factory.CreateSelectCommand();
            cmd.ConvertType = true;

            return cmd.Query<TEntity>(this.EntityMetaDataProvider, query);
        }

        #endregion

        #region   |      Entity     |

        public virtual int Update<TEntity>(TEntity entity, params string[] updatedFields)
        {
            IEntityCommandUpdate cmd = (IEntityCommandUpdate)this.factory.CreateEntityCommand(EntityCommandType.Update);
            if (!updatedFields.IsEmptyList())
                cmd.UpdatedFields = new HashSet<string>(updatedFields);

            return cmd.Update(entity, this.EntityMetaDataProvider);
        }

        public virtual int Insert<TEntity>(TEntity entity, params string[] insertFields)
        {
            IEntityCommandInsert cmd = (IEntityCommandInsert)this.factory.CreateEntityCommand(EntityCommandType.Insert);
            if (!insertFields.IsEmptyList())
                cmd.InsertFields = new HashSet<string>(insertFields);

            return cmd.Insert(entity, this.EntityMetaDataProvider);
        }

        public virtual int Upsert<TEntity>(TEntity entity, string[] updatedFields, string[] insertFields)
        {
            IEntityCommandUpsert cmd = (IEntityCommandUpsert)this.factory.CreateEntityCommand(EntityCommandType.Upsert);
            if (!updatedFields.IsEmptyList())
                cmd.UpdatedFields = new HashSet<string>(updatedFields);
            if (!insertFields.IsEmptyList())
                cmd.InsertFields = new HashSet<string>(insertFields);

            return cmd.Upsert(entity, this.EntityMetaDataProvider);
        }

        public virtual int Delete<TEntity>(TEntity entity)
        {
            IEntityCommandDelete cmd = (IEntityCommandDelete)this.factory.CreateEntityCommand(EntityCommandType.Delete);
            return cmd.Delete(entity, this.EntityMetaDataProvider);
        }

        #endregion


        #region   |     Batch     |

        public virtual int BatchUpdate<TEntity>(IEnumerable<TEntity> entityList, BatchCommandMode mode, params string[] updatedFields)
        {
            IBatchCommandUpdate cmd = (IBatchCommandUpdate)this.factory.CreateBatchCommand(BatchCommandType.Update);
            if (!updatedFields.IsEmptyList())
                cmd.UpdatedFields = new HashSet<string>(updatedFields);

            return cmd.Update(entityList, mode, this.EntityMetaDataProvider);
        }

        public virtual int BatchInsert<TEntity>(IEnumerable<TEntity> entityList, BatchCommandMode mode, params string[] insertFields)
        {
            IBatchCommandInsert cmd = (IBatchCommandInsert)this.factory.CreateBatchCommand(BatchCommandType.Insert);
            if (!insertFields.IsEmptyList())
                cmd.InsertFields = new HashSet<string>(insertFields);

            return cmd.Insert(entityList, mode, this.EntityMetaDataProvider);
        }

        public virtual int BatchUpsert<TEntity>(IEnumerable<TEntity> entityList, BatchCommandMode mode, string[] updatedFields, string[] insertFields)
        {
            IBatchCommandUpsert cmd = (IBatchCommandUpsert)this.factory.CreateBatchCommand(BatchCommandType.Upsert);
            if (!updatedFields.IsEmptyList())
                cmd.UpdatedFields = new HashSet<string>(updatedFields);
            if (!insertFields.IsEmptyList())
                cmd.InsertFields = new HashSet<string>(insertFields);

            return cmd.Upsert(entityList, mode, this.EntityMetaDataProvider);
        }


        public virtual int Delsert<TEntity>(IEnumerable<TEntity> entityList, BatchCommandMode mode, Func<FluentWhere<TEntity>, FluentWhere<TEntity>> where
            , params string[] insertFields)
        {
            if (null != where)
            {
                IBatchCommandDelsert cmd = (IBatchCommandDelsert)this.factory.CreateBatchCommand(BatchCommandType.Delsert);
                FluentWhere<TEntity> fluentWhere = where(new FluentWhere<TEntity>(this.Factory.ParameterPrefix));
                if (null != fluentWhere)
                {
                    SqlQuery fluentWhereQuery = fluentWhere.ToQuery();
                    if (fluentWhereQuery.Text.Length != 0)
                    {
                        IEntityMetaDataProvider provider = this.EntityMetaDataProvider;
                        IEntityMetaData metaData = provider.CreateEntityMetaData(typeof(TEntity));
                        SqlQuery deleteQuery = new SqlQuery();
                        deleteQuery.Text.Append("DELETE FROM ");
                        deleteQuery.Text.Append(metaData.TableName);
                        deleteQuery.Combine(fluentWhereQuery);

                        cmd.DeleteQuery = deleteQuery;

                        if (!insertFields.IsEmptyList())
                            cmd.InsertFields = new HashSet<string>(insertFields);

                        return cmd.Delsert(entityList, mode, provider);
                    }
                }
            }

            return 0;
        }

        #endregion
    }
}
