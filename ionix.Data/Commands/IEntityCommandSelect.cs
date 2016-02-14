namespace ionix.Data
{
    using Utils.Extensions;
    using Utils.Reflection;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    public interface IEntityCommandSelect//Select ler Entity üzerinden otomatik yazılan Select ifadeleri. Query ise custom için.
    {
        bool ConvertType { get; set; }

        TEntity SelectById<TEntity>(IEntityMetaDataProvider provider, params object[] idValues)
            where TEntity : new();

        TEntity SelectSingle<TEntity>(IEntityMetaDataProvider provider, SqlQuery extendedQuery)
            where TEntity : new();

        IList<TEntity> Select<TEntity>(IEntityMetaDataProvider provider, SqlQuery extendedQuery)
            where TEntity : new();

        TEntity QuerySingle<TEntity>(IEntityMetaDataProvider provider, SqlQuery query)//Property adı kolondan farklı olan durumlar için IEntityMetaDataProvider provider eklendi.
            where TEntity : new();

        IList<TEntity> Query<TEntity>(IEntityMetaDataProvider provider, SqlQuery query)
            where TEntity : new();
    }

    public class EntityCommandSelect : IEntityCommandSelect
    {
        public EntityCommandSelect(IDbAccess dataAccess, char parameterPrefix)
        {
            if (null == dataAccess)
                throw new ArgumentNullException(nameof(dataAccess));

            this.DataAccess = dataAccess;
            this.ParameterPrefix = parameterPrefix;
        }

        public IDbAccess DataAccess { get; }

        public char ParameterPrefix { get; }

        public bool ConvertType { get; set; }

        private enum MapType
        {
            Select,
            Query
        }

        private void Map<TEntity>(TEntity entity, IEntityMetaData metaData, IDataReader dr, MapType mapType)
        {
            switch (mapType)
            {
                case MapType.Select:
                    foreach (PropertyMetaData md in metaData.Properties)
                    {
                        string columnName = md.Schema.ColumnName;
                        PropertyInfo pi = md.Property;
                        if (pi.GetSetMethod() != null)
                        {
                            object dbValue = dr[columnName];
                            if (dbValue == DBNull.Value)
                            {
                                pi.SetValue(entity, null, null);
                            }
                            else
                            {
                                if (this.ConvertType)
                                    pi.SetValueSafely(entity, dbValue);
                                else
                                    pi.SetValue(entity, dbValue, null);
                            }
                        }
                    }
                    break;
                case MapType.Query:
                    int fieldCount = dr.FieldCount;
                    for (int j = 0; j < fieldCount; ++j)
                    {
                        string columnName = dr.GetName(j);
                        PropertyMetaData md = metaData[columnName];// metaData.Properties.FirstOrDefault(p => String.Equals(columnName, p.Schema.ColumnName));
                        if (null != md)
                        {
                            PropertyInfo pi = md.Property;
                            if (pi.GetSetMethod() != null)
                            {
                                object dbValue = dr[j];
                                if (dbValue == DBNull.Value)
                                {
                                    pi.SetValue(entity, null, null);
                                }
                                else
                                {
                                    if (this.ConvertType)
                                        pi.SetValueSafely(entity, dbValue);
                                    else
                                        pi.SetValue(entity, dbValue, null);
                                }
                            }
                        }
                    }
                    break;
                default:
                    throw new NotSupportedException(mapType.ToString());
            }
        }

        private TEntity ReadEntity<TEntity>(IEntityMetaData metaData, SqlQuery query, MapType mapType)
            where TEntity : new()
        {
            IDataReader dr = null;
            try
            {
                dr = this.DataAccess.CreateDataReader(query, CommandBehavior.SingleRow);

                if (dr.Read())
                {
                    TEntity entity = new TEntity();
                    this.Map<TEntity>(entity, metaData, dr, mapType);
                    return entity;
                }
            }
            finally
            {
                if (dr != null) dr.Dispose();
            }

            return default(TEntity);
        }

        private IList<TEntity> ReadEntityList<TEntity>(IEntityMetaData metaData, SqlQuery query, MapType mapType)
            where TEntity : new()
        {
            List<TEntity> ret = new List<TEntity>();

            IDataReader dr = null;
            try
            {
                dr = this.DataAccess.CreateDataReader(query, CommandBehavior.Default);
                //ret.Capacity = dr.FieldCount; ??? ne bu
                while (dr.Read())
                {
                    TEntity entity = new TEntity();
                    this.Map<TEntity>(entity, metaData, dr, mapType);
                    ret.Add(entity);
                }
            }
            finally
            {
                if (dr != null) dr.Dispose();
            }

            return ret;
        }

        public virtual TEntity SelectById<TEntity>(IEntityMetaDataProvider provider, params object[] idValues)
            where TEntity : new()
        {
            if (idValues.IsEmptyList())
                throw new ArgumentNullException(nameof(idValues));

            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            SqlQueryBuilderSelect builder = new SqlQueryBuilderSelect(metaData);
            SqlQuery query = builder.ToQuery();//Select sql yazıldı.

            FilterCriteriaList filters = new FilterCriteriaList(this.ParameterPrefix);

            IList<PropertyMetaData> keySchemas = metaData.OfKeys(true);//Order a göre geldiği için böyle.
            if (keySchemas.Count != idValues.Length)
                throw new InvalidOperationException("Keys and Valus count does not match");

            int index = -1;
            foreach (PropertyMetaData keyProperty in keySchemas)
            {
                filters.Add(keyProperty, ConditionOperator.Equals, idValues[++index]);
            }

            query.Combine(filters.ToQuery());//Where ifadesi oluşturuldu. Eğer ki

            return this.ReadEntity<TEntity>(metaData, query, MapType.Select);
        }
        public virtual TEntity SelectSingle<TEntity>(IEntityMetaDataProvider provider, SqlQuery extendedQuery)
            where TEntity : new()
        {
            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            SqlQueryBuilderSelect builder = new SqlQueryBuilderSelect(metaData);
            SqlQuery query = builder.ToQuery();
            if (null != extendedQuery)
                query.Combine(extendedQuery);

            return this.ReadEntity<TEntity>(metaData, query, MapType.Select);
        }


        public IList<TEntity> Select<TEntity>(IEntityMetaDataProvider provider, SqlQuery extendedQuery)
            where TEntity : new()
        {
            if (null == provider)
                throw new ArgumentNullException(nameof(provider));

            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            SqlQueryBuilderSelect builder = new SqlQueryBuilderSelect(metaData);
            SqlQuery query = builder.ToQuery();
            if (null != extendedQuery)
                query.Combine(extendedQuery);

            return this.ReadEntityList<TEntity>(metaData, query, MapType.Select);
        }





        public virtual TEntity QuerySingle<TEntity>(IEntityMetaDataProvider provider, SqlQuery query)
            where TEntity : new()
        {
            if (null == query)
                throw new ArgumentNullException(nameof(query));

            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            return this.ReadEntity<TEntity>(metaData, query, MapType.Query);
        }


        public virtual IList<TEntity> Query<TEntity>(IEntityMetaDataProvider provider, SqlQuery query)
            where TEntity : new()
        {
            if (null == query)
                throw new ArgumentNullException(nameof(query));

            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            return this.ReadEntityList<TEntity>(metaData, query, MapType.Query);
        }
    }
}
