namespace ionix.Data.SQLite
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;

    public class BatchCommandUpdate : BatchCommandExecute, IBatchCommandUpdate
    {
        public BatchCommandUpdate(IDbAccess dataAccess)
            : base(dataAccess)
        { }

        public HashSet<string> UpdatedFields { get; set; }

        int IBatchCommandUpdate.Update<TEntity>(IEnumerable<TEntity> entityList, BatchCommandMode mode, IEntityMetaDataProvider provider)
        {
            return this.Execute<TEntity>(entityList, mode, provider);
        }

        public override int Execute<TEntity>(IEnumerable<TEntity> entityList, BatchCommandMode mode, IEntityMetaDataProvider provider)
        {
            if (entityList.IsEmptyList())
                return 0;

            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            switch (mode)
            {
                case BatchCommandMode.Batch:

                    SqlQuery batchQuery = new SqlQuery();

                    batchQuery.Text.Append(GlobalInternal.BeginStatement);

                    int index = 0;
                    EntitySqlQueryBuilderUpdate updateBuilder = new EntitySqlQueryBuilderUpdate() { UseSemicolon = true };
                    updateBuilder.UpdatedFields = this.UpdatedFields;

                    foreach (TEntity entity in entityList)
                    {
                        batchQuery.Text.AppendLine();

                        batchQuery.Combine(updateBuilder.CreateQuery(entity, metaData, index++));
                    }

                    batchQuery.Text.AppendLine();
                    batchQuery.Text.Append(GlobalInternal.EndStatement);


                    return base.DataAccess.ExecuteNonQuery(batchQuery);

                case BatchCommandMode.Single:

                    int ret = 0;
                    EntityCommandUpdate updateCommand = new EntityCommandUpdate(base.DataAccess);
                    updateCommand.UpdatedFields = this.UpdatedFields;

                    foreach (TEntity entity in entityList)
                    {
                        ret += updateCommand.UpdateInternal(entity, metaData);
                    }
                    return ret;
                default:
                    throw new NotSupportedException(mode.ToString());
            }
        }
    }

    public class BatchCommandInsert : BatchCommandExecute, IBatchCommandInsert
    {
        public BatchCommandInsert(IDbAccess dataAccess)
            : base(dataAccess)
        { }

        public HashSet<string> InsertFields { get; set; }

        int IBatchCommandInsert.Insert<TEntity>(IEnumerable<TEntity> entityList, BatchCommandMode mode, IEntityMetaDataProvider provider)
        {
            return this.Execute(entityList, mode, provider);
        }

        public override int Execute<TEntity>(IEnumerable<TEntity> entityList, BatchCommandMode mode, IEntityMetaDataProvider provider)
        {
            if (entityList.IsEmptyList())
                return 0;

            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            switch (mode)
            {
                case BatchCommandMode.Batch:

                    SqlQuery batchQuery = new SqlQuery();

                    batchQuery.Text.Append(GlobalInternal.BeginStatement);


                    int index = 0;
                    EntitySqlQueryBuilderInsert insertBuilder = new EntitySqlQueryBuilderInsert() { UseSemicolon = true };
                    insertBuilder.InsertFields = this.InsertFields;

                    PropertyMetaData sequenceIdentity; //SQLite Output parametreyi desteklemediği için öylesine konuldu.
                    foreach (TEntity entity in entityList)
                    {
                        batchQuery.Text.AppendLine();

                        batchQuery.Combine(insertBuilder.CreateQuery(entity, metaData, index++, out sequenceIdentity));
                    }

                    batchQuery.Text.AppendLine();
                    batchQuery.Text.Append(GlobalInternal.EndStatement);

                    return base.DataAccess.ExecuteNonQuery(batchQuery);

                case BatchCommandMode.Single:

                    int sum = 0;
                    EntityCommandInsert insertCommand = new EntityCommandInsert(base.DataAccess);
                    insertCommand.InsertFields = this.InsertFields;

                    foreach (TEntity entity in entityList)
                    {
                        sum += insertCommand.InsertInternal(entity, metaData);
                    }
                    return sum;
                default:
                    throw new NotSupportedException(mode.ToString());
            }
        }
    }

    public class BatchCommandUpsert : BatchCommandExecute, IBatchCommandUpsert
    {
        public BatchCommandUpsert(IDbAccess dataAccess)
            : base(dataAccess)
        { }

        public HashSet<string> UpdatedFields { get; set; }
        public HashSet<string> InsertFields { get; set; }


        int IBatchCommandUpsert.Upsert<TEntity>(IEnumerable<TEntity> entityList, BatchCommandMode mode, IEntityMetaDataProvider provider)
        {
            return this.Execute(entityList, mode, provider);
        }

        public override int Execute<TEntity>(IEnumerable<TEntity> entityList, BatchCommandMode mode, IEntityMetaDataProvider provider)
        {
            if (entityList.IsEmptyList())
                return 0;

            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            switch (mode)//BatchUpsert sqlite da if @rowcount veya sql%rowcount gibi session bazlı kontrolü deskteklemediğinden batchMode Geçersiz.
            {
                case BatchCommandMode.Batch:
                case BatchCommandMode.Single:

                    int sum = 0;
                    EntityCommandUpsert upsertCommand = new EntityCommandUpsert(base.DataAccess);
                    upsertCommand.UpdatedFields = this.UpdatedFields;
                    upsertCommand.InsertFields = this.InsertFields;

                    foreach (TEntity entity in entityList)
                    {
                        sum += upsertCommand.UpsertInternal(entity, metaData);
                    }
                    return sum;

                default:
                    throw new NotSupportedException(mode.ToString());
            }
        }
    }



    public class BatchCommandDelsert : BatchCommandExecute, IBatchCommandDelsert
    {
        public BatchCommandDelsert(IDbAccess dataAccess)
            : base(dataAccess)
        { }

        public SqlQuery DeleteQuery { get; set; }

        public HashSet<string> InsertFields { get; set; }

        int IBatchCommandDelsert.Delsert<TEntity>(IEnumerable<TEntity> entityList, BatchCommandMode mode, IEntityMetaDataProvider provider)
        {
            return this.Execute(entityList, mode, provider);
        }

        public override int Execute<TEntity>(IEnumerable<TEntity> entityList, BatchCommandMode mode, IEntityMetaDataProvider provider)
        {
            if (null == this.DeleteQuery || this.DeleteQuery.Text.Length == 0)
                throw new InvalidOperationException("Delete query can not be null. Please use BatchCommandInsert for this purpuse ");

            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            switch (mode)
            {
                case BatchCommandMode.Batch:

                    SqlQuery batchQuery = new SqlQuery();

                    batchQuery.Text.Append(GlobalInternal.BeginStatement);

                    batchQuery.Text.AppendLine();
                    batchQuery.Combine(this.DeleteQuery);
                    batchQuery.Text.AppendLine(";");


                    int index = 0;
                    EntitySqlQueryBuilderInsert insertBuilder = new EntitySqlQueryBuilderInsert() { UseSemicolon = true };
                    insertBuilder.InsertFields = this.InsertFields;

                    PropertyMetaData sequenceIdentity; //SQLite Output parametreyi desteklemediği için öylesine konuldu.
                    foreach (TEntity entity in entityList)
                    {
                        batchQuery.Text.AppendLine();

                        batchQuery.Combine(insertBuilder.CreateQuery(entity, metaData, index++, out sequenceIdentity));
                    }

                    batchQuery.Text.AppendLine();
                    batchQuery.Text.Append(GlobalInternal.EndStatement);

                    return base.DataAccess.ExecuteNonQuery(batchQuery);

                case BatchCommandMode.Single:

                    int sum = base.DataAccess.ExecuteNonQuery(this.DeleteQuery);

                    EntityCommandInsert insertCommand = new EntityCommandInsert(base.DataAccess);
                    insertCommand.InsertFields = this.InsertFields;

                    foreach (TEntity entity in entityList)
                    {
                        sum += insertCommand.InsertInternal(entity, metaData);
                    }
                    return sum;
                default:
                    throw new NotSupportedException(mode.ToString());
            }
        }
    }
}
