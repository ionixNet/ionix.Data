﻿namespace ionix.Data.SqlServer
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class EntitySqlQueryBuilderUpdate : IEntitySqlQueryBuilder
    {
        public EntitySqlQueryBuilderUpdate()
        {
            this.ParameterIndex = -1;
        }
        //Batch ler için Eklendi.
        public int ParameterIndex { get; set; }

        //EntityChangeSet  Kullanımı İçin Eklendi.
        public HashSet<string> UpdatedFields { get; set; }

        public virtual SqlQuery CreateQuery(object entity, IEntityMetaData metaData)
        {
            if (null == entity)
                throw new ArgumentNullException(nameof(entity));

            SqlQueryHelper.IndexParameterNames(metaData, this.ParameterIndex);

            bool updatedFieldsEnabled = !this.UpdatedFields.IsEmptyList();

            SqlQuery query = new SqlQuery();
            StringBuilder text = query.Text;
            text.Append("UPDATE ");
            text.Append(metaData.TableName);
            text.Append(" SET ");

            foreach (PropertyMetaData property in metaData.Properties)
            {
                SchemaInfo schema = property.Schema;

                if (schema.IsKey)
                    continue;

                if (schema.DatabaseGeneratedOption != StoreGeneratedPattern.None)
                    continue;

                if (schema.ReadOnly)
                    continue;

                if (updatedFieldsEnabled && !this.UpdatedFields.Contains(schema.ColumnName))
                    continue;

                text.Append(schema.ColumnName);
                text.Append('=');

                SqlQueryHelper.SetColumnValue(ValueSetter.Instance, query, property, entity);

                text.Append(',');
            }
            text.Remove(text.Length - 1, 1);

            query.Combine(SqlQueryHelper.CreateWhereSqlByKeys(metaData, '@', entity));

            return query;
        }
    }

    public class EntitySqlQueryBuilderInsert : IEntitySqlQueryBuilder
    {
        public EntitySqlQueryBuilderInsert()
        {
            this.ParameterIndex = -1;
        }
        public int ParameterIndex { get; set; }

        public HashSet<string> InsertFields { get; set; }

        public virtual SqlQuery CreateQuery(object entity, IEntityMetaData metaData, out PropertyMetaData identity)
        {
            if (null == entity)
                throw new ArgumentNullException(nameof(entity));

            identity = null;

            SqlQueryHelper.IndexParameterNames(metaData, this.ParameterIndex);

            bool insertFieldsEnabled = !this.InsertFields.IsEmptyList();

            SqlQuery query = new SqlQuery();
            StringBuilder text = query.Text;
            text.Append("INSERT INTO ");
            text.Append(metaData.TableName);
            text.Append(" (");

            List<PropertyMetaData> validInfos = new List<PropertyMetaData>();
            foreach (PropertyMetaData property in metaData.Properties)
            {
                SchemaInfo schema = property.Schema;

                switch (schema.DatabaseGeneratedOption)
                {
                    case StoreGeneratedPattern.Identity:
                        if (null != identity)
                            throw new MultipleIdentityColumnFoundException(entity);

                        identity = property;
                        break;

                    case StoreGeneratedPattern.Computed:
                        break;

                    default:
                        if (insertFieldsEnabled && !this.InsertFields.Contains(schema.ColumnName))
                            continue;

                        text.Append(schema.ColumnName);
                        text.Append(',');

                        validInfos.Add(property);
                        break;
                }
            }

            text.Remove(text.Length - 1, 1);
            text.Append(") VALUES (");

            foreach (PropertyMetaData property in validInfos)
            {
                SqlQueryHelper.SetColumnValue(ValueSetter.Instance, query, property, entity);

                text.Append(',');
            }
            text.Remove(text.Length - 1, 1);
            text.Append(')');
            if (null != identity)
            {
                text.AppendLine();

                text.Append("SELECT @");
                text.Append(identity.ParameterName);
                text.Append("=SCOPE_IDENTITY()");

                SqlQueryParameter identityParameter = SqlQueryHelper.EnsureHasParameter(query, identity, entity);
                identityParameter.Direction = System.Data.ParameterDirection.InputOutput;
            }

            return query;
        }

        SqlQuery IEntitySqlQueryBuilder.CreateQuery(object entity, IEntityMetaData metaData)
        {
            PropertyMetaData identity;
            return this.CreateQuery(entity, metaData, out identity);
        }
    }

    public class EntitySqlQueryBuilderUpsert : IEntitySqlQueryBuilder
    {
        public EntitySqlQueryBuilderUpsert()
        {
            this.ParameterIndex = -1;
        }
        public int ParameterIndex { get; set; }

        public HashSet<string> UpdatedFields { get; set; }

        public HashSet<string> InsertFields { get; set; }

        public virtual SqlQuery CreateQuery(object entity, IEntityMetaData metaData, out PropertyMetaData identity)
        {
            EntitySqlQueryBuilderUpdate builderUpdate = new EntitySqlQueryBuilderUpdate();
            builderUpdate.ParameterIndex = this.ParameterIndex;
            builderUpdate.UpdatedFields = this.UpdatedFields;

            SqlQuery query = builderUpdate.CreateQuery(entity, metaData);
            StringBuilder text = query.Text;

            text.AppendLine();
            text.Append("IF @@ROWCOUNT = 0");
            text.AppendLine();
            text.Append("BEGIN");
            text.AppendLine();

            EntitySqlQueryBuilderInsert builderInsert = new EntitySqlQueryBuilderInsert();
            builderInsert.ParameterIndex = this.ParameterIndex;
            builderInsert.InsertFields = this.InsertFields;
            query.Combine(builderInsert.CreateQuery(entity, metaData, out identity));

            text.AppendLine();
            text.Append("END");

            return query;
        }

        SqlQuery IEntitySqlQueryBuilder.CreateQuery(object entity, IEntityMetaData metaData)
        {
            PropertyMetaData identity;
            return this.CreateQuery(entity, metaData, out identity);
        }
    }
}
