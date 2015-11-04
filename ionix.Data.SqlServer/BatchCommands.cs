﻿namespace ionix.Data.SqlServer
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

                    batchQuery.Text.Append(GlobalInternal.SqlServerBeginStatement);

                    int index = 0;
                    EntitySqlQueryBuilderUpdate updateBuilder = new EntitySqlQueryBuilderUpdate();
                    updateBuilder.UpdatedFields = this.UpdatedFields;

                    foreach (TEntity entity in entityList)
                    {
                        batchQuery.Text.AppendLine();

                        updateBuilder.ParameterIndex = index++;
                        batchQuery.Combine(updateBuilder.CreateQuery(entity, metaData));
                    }

                    batchQuery.Text.AppendLine();
                    batchQuery.Text.Append(GlobalInternal.SqlServerEndStatement);


                    return base.DataAccess.ExecuteNonQuery(batchQuery);

                case BatchCommandMode.Single:

                    int ret = 0;
                    EntityCommandUpdate updateCommand = new EntityCommandUpdate(base.DataAccess);
                    updateCommand.UpdatedFields = this.UpdatedFields;
                    //using (TransactionScope tran = new TransactionScope())
                    // {
                    foreach (TEntity entity in entityList)
                    {
                        ret += updateCommand.UpdateInternal(entity, metaData);
                    }
                    // }
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

                    batchQuery.Text.Append(GlobalInternal.SqlServerBeginStatement);


                    int index = 0;
                    List<string> outParameterNames = new List<string>();
                    EntitySqlQueryBuilderInsert insertBuilder = new EntitySqlQueryBuilderInsert();
                    insertBuilder.InsertFields = this.InsertFields;

                    PropertyMetaData identity = null; //Tek Bir Tane Identity var sadece paremetre ismi değişiyor.
                    foreach (TEntity entity in entityList)
                    {
                        batchQuery.Text.AppendLine();

                        insertBuilder.ParameterIndex = index++;

                        batchQuery.Combine(insertBuilder.CreateQuery(entity, metaData, out identity));
                        if (null != identity)
                            outParameterNames.Add(identity.ParameterName);
                    }

                    batchQuery.Text.AppendLine();
                    batchQuery.Text.Append(GlobalInternal.SqlServerEndStatement);

                    int ret = base.DataAccess.ExecuteNonQuery(batchQuery);

                    if (null != identity)//outParameterNames.Count must be equal to entityList.Count.
                    {
                        index = -1;
                        foreach (TEntity entity in entityList)
                        {
                            string outParameterName = outParameterNames[++index];
                            object identityValue = batchQuery.Parameters.Find(outParameterName).Value;
                            identity.Property.SetValue(entity, identityValue, null);
                        }
                    }

                    return ret;

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

            switch (mode)
            {
                case BatchCommandMode.Batch:

                    SqlQuery batchQuery = new SqlQuery();

                    batchQuery.Text.Append(GlobalInternal.SqlServerBeginStatement);


                    int index = 0;
                    List<string> outParameterNames = new List<string>();
                    EntitySqlQueryBuilderUpsert upsertBuilder = new EntitySqlQueryBuilderUpsert();
                    upsertBuilder.UpdatedFields = this.UpdatedFields;
                    upsertBuilder.InsertFields = this.InsertFields;

                    PropertyMetaData identity = null; //Tek Bir Tane Identity var sadece paremetre ismi değişiyor.
                    foreach (TEntity entity in entityList)
                    {
                        batchQuery.Text.AppendLine();

                        upsertBuilder.ParameterIndex = index++;

                        batchQuery.Combine(upsertBuilder.CreateQuery(entity, metaData, out identity));
                        if (null != identity)
                            outParameterNames.Add(identity.ParameterName);
                    }

                    batchQuery.Text.AppendLine();
                    batchQuery.Text.Append(GlobalInternal.SqlServerEndStatement);

                    int ret = base.DataAccess.ExecuteNonQuery(batchQuery);

                    if (null != identity)//outParameterNames.Count must be equal to entityList.Count.
                    {
                        index = -1;
                        foreach (TEntity entity in entityList)
                        {
                            string outParameterName = outParameterNames[++index];
                            object identityValue = batchQuery.Parameters.Find(outParameterName).Value;
                            identity.Property.SetValue(entity, identityValue, null);
                        }
                    }

                    return ret;

                case BatchCommandMode.Single:

                    int sum = 0;
                    EntityCommandUpsert upsertCommand = new EntityCommandUpsert(base.DataAccess);
                    upsertCommand.UpdatedFields = this.UpdatedFields;
                    upsertCommand.InsertFields = this.InsertFields;
                    //using (TransactionScope tran = new TransactionScope())
                    //{
                    foreach (TEntity entity in entityList)
                    {
                        sum += upsertCommand.UpsertInternal(entity, metaData);
                    }
                    // }
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

                    batchQuery.Text.Append(GlobalInternal.SqlServerBeginStatement);

                    batchQuery.Text.AppendLine();
                    batchQuery.Combine(this.DeleteQuery);
                    batchQuery.Text.AppendLine();


                    int index = 0;
                    List<string> outParameterNames = new List<string>();
                    EntitySqlQueryBuilderInsert insertBuilder = new EntitySqlQueryBuilderInsert();
                    insertBuilder.InsertFields = this.InsertFields;

                    PropertyMetaData identity = null; //Tek Bir Tane Identity var sadece paremetre ismi değişiyor.
                    foreach (TEntity entity in entityList)
                    {
                        batchQuery.Text.AppendLine();

                        insertBuilder.ParameterIndex = index++;

                        batchQuery.Combine(insertBuilder.CreateQuery(entity, metaData, out identity));
                        if (null != identity)
                            outParameterNames.Add(identity.ParameterName);
                    }

                    batchQuery.Text.AppendLine();
                    batchQuery.Text.Append(GlobalInternal.SqlServerEndStatement);

                    int ret = base.DataAccess.ExecuteNonQuery(batchQuery);

                    if (null != identity)//outParameterNames.Count must be equal to entityList.Count.
                    {
                        index = -1;
                        foreach (TEntity entity in entityList)
                        {
                            string outParameterName = outParameterNames[++index];
                            object identityValue = batchQuery.Parameters.Find(outParameterName).Value;
                            identity.Property.SetValue(entity, identityValue, null);
                        }
                    }

                    return ret;

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
