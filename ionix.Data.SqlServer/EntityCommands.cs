namespace ionix.Data.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class EntityCommandUpdate : EntityCommandExecute, IEntityCommandUpdate
    {
        public EntityCommandUpdate(IDbAccess dataAccess)
            : base(dataAccess)
        { }

        public HashSet<string> UpdatedFields { get; set; }

        int IEntityCommandUpdate.Update<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            return this.Execute(entity, provider);
        }

        public override int Execute<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();
            return this.UpdateInternal(entity, metaData);
        }

        internal int UpdateInternal(object entity, IEntityMetaData metaData)
        {
            EntitySqlQueryBuilderUpdate builder = new EntitySqlQueryBuilderUpdate() { UpdatedFields = this.UpdatedFields };
            SqlQuery query = builder.CreateQuery(entity, metaData, 0);

            return base.DataAccess.ExecuteNonQuery(query);
        }
    }

    public class EntityCommandInsert : EntityCommandExecute, IEntityCommandInsert
    {
        public EntityCommandInsert(IDbAccess dataAccess)
            : base(dataAccess)
        { }

        public HashSet<string> InsertFields { get; set; }

        int IEntityCommandInsert.Insert<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            return this.Execute(entity, provider);
        }

        public override int Execute<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();
            return this.InsertInternal(entity, metaData);
        }

        internal int InsertInternal(object entity, IEntityMetaData metaData)
        {
            EntitySqlQueryBuilderInsert builder = new EntitySqlQueryBuilderInsert() { InsertFields = this.InsertFields };
            PropertyMetaData identity;
            SqlQuery query = builder.CreateQuery(entity, metaData, 0, out identity);

            int ret = base.DataAccess.ExecuteNonQuery(query);
            if (null != identity)
            {
                string parameterName = metaData.GetParameterName(identity, 0);
                object identityValue = query.Parameters.Find(parameterName).Value;
                identity.Property.SetValue(entity, identityValue, null);
            }
            return ret;
        }
    }

    public class EntityCommandUpsert : EntityCommandExecute, IEntityCommandUpsert
    {
        public EntityCommandUpsert(IDbAccess dataAccess)
            : base(dataAccess)
        { }

        public HashSet<string> UpdatedFields { get; set; }

        public HashSet<string> InsertFields { get; set; }

        int IEntityCommandUpsert.Upsert<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            return this.Execute(entity, provider);
        }

        public override int Execute<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();
            return this.UpsertInternal(entity, metaData);
        }
        internal int UpsertInternal(object entity, IEntityMetaData metaData)
        {
            EntitySqlQueryBuilderUpsert builder = new EntitySqlQueryBuilderUpsert() { UpdatedFields = this.UpdatedFields, InsertFields = this.InsertFields };
            PropertyMetaData identity;
            SqlQuery query = builder.CreateQuery(entity, metaData, 0, out identity);

            int ret = base.DataAccess.ExecuteNonQuery(query);
            if (null != identity)
            {
                string parameterName = metaData.GetParameterName(identity, 0);
                object identityValue = query.Parameters.Find(parameterName).Value;
                identity.Property.SetValue(entity, identityValue, null);
            }
            return ret;
        }
    }

    public class EntityCommandDelete : EntityCommandExecute, IEntityCommandDelete
    {
        public EntityCommandDelete(IDbAccess dataAccess)
            : base(dataAccess)
        { }

        int IEntityCommandDelete.Delete<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            return this.Execute(entity, provider);
        }

        public override int Execute<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            if (null == entity)
                throw new ArgumentNullException(nameof(entity));

            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();
            return this.DeleteInternal(entity, metaData);
        }

        internal int DeleteInternal(object entity, IEntityMetaData metaData)
        {
            SqlQuery query = new SqlQuery();
            StringBuilder text = query.Text;
            text.Append("DELETE FROM ");
            text.Append(metaData.TableName);

            query.Combine(SqlQueryHelper.CreateWhereSqlByKeys(metaData, 0, GlobalInternal.Prefix, entity));

            return base.DataAccess.ExecuteNonQuery(query);
        }
    }
}
