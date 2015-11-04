namespace ionix.Data
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class KeySchemaNotFoundException : Exception
    {
        public KeySchemaNotFoundException()
            : base("Key Schema Not Found")
        {

        }
    }


    public static class SqlQueryHelper
    {
        public static void EnsureEntityMetaData(this IEntityMetaData metaData)
        {
            if (null == metaData)
                throw new ArgumentNullException(nameof(metaData));
            if (metaData.Properties.IsEmptyList())
                throw new NullReferenceException("IEntityMetaData.Properties");
            if (String.IsNullOrEmpty(metaData.TableName))
                throw new NullReferenceException("IEntityMetaData.TableName");
        }

        public static IEntityMetaData EnsureCreateEntityMetaData<TEntity>(this IEntityMetaDataProvider provider)
        {
            if (null == provider)
                throw new ArgumentNullException(nameof(provider));

            IEntityMetaData ret = provider.CreateEntityMetaData(typeof(TEntity));
            ret.EnsureEntityMetaData();

            return ret;
        }

        //SelectById ile Kullanılıyor.
        public static IList<PropertyMetaData> OfKeys(this IEntityMetaData metaData, bool throwExcIfNoKeys)
        {
            List<PropertyMetaData> keys = metaData.Properties.Where(s => s.Schema.IsKey).ToList();
            if (!keys.IsEmptyList())
            {
                keys.Sort((x, y) => x.Schema.Order.CompareTo(y.Schema.Order));
                return keys;
            }
            else
            {
                if (throwExcIfNoKeys)
                    throw new KeySchemaNotFoundException();
                return null;
            }
        }

        public static SqlQuery CreateWhereSqlByKeys(IEntityMetaData metaData, char prefix, object entity)
        {
            if (null == entity)
                throw new ArgumentNullException(nameof(entity));

            IList<PropertyMetaData> keySchemas = metaData.OfKeys(true);

            object[] keyValues = new object[keySchemas.Count];
            for (int j = 0; j < keySchemas.Count; ++j)
            {
                keyValues[j] = keySchemas[j].Property.GetValue(entity, null);
            }

            FilterCriteriaList list = new FilterCriteriaList(prefix);
            for (int j = 0; j < keySchemas.Count; ++j)
            {
                PropertyMetaData keySchema = keySchemas[j];

                list.Add(keySchema, ConditionOperator.Equals, keyValues[j]);
            }
            return list.ToQuery();
        }

        //Upsert de Identity Parametre İçin Eklendi.
        public static SqlQueryParameter EnsureHasParameter(SqlQuery query, PropertyMetaData property, object entity)//inset de bu parametre normalde eklenmez ama upsert de update where de eklendiği için bu yapı kullanılıyor.
        {

            SqlQueryParameter identityParameter = query.Parameters.Find(property.ParameterName);
            if (null == identityParameter)
            {
                object parValue = property.Property.GetValue(entity, null);
                identityParameter = SqlQueryParameter.Create(property, parValue);

                query.Parameters.Add(identityParameter);
            }
            return identityParameter;
        }

        public static void IndexParameterNames(IEntityMetaData metaData, int parameterIndex)//ParameterIndex Batch ler için.
        {
            if (parameterIndex > -1)
            {
                IEnumerable<PropertyMetaData> properties = metaData.Properties;
                int fieldCount = ((ICollection<PropertyMetaData>)properties).Count;
                int factor = fieldCount * parameterIndex;

                foreach (PropertyMetaData prop in properties)
                {
                    prop.ParameterName = (factor++).ToString();
                }
            }
            else
            {
                int indexName = -1;
                foreach (PropertyMetaData prop in metaData.Properties)
                {
                    prop.ParameterName = (++indexName).ToString();
                }
            }
        }
        //public static void SetParameterName(PropertyMetaData property, int parameterIndex)
        //{
        //    property.ParameterName = property.Schema.ColumnName + parameterIndex;
        //}

        public static void SetColumnValue(DbValueSetter setter, SqlQuery query, PropertyMetaData metaData, object entity)//Parametewnin eklenip eklenmeyeceği bilinmdeğinden prefix ve entity verilmek zorunda.
        {
            setter.SetColumnValue(query, metaData, entity);
        }

        public static PropertyMetaData GetPrimaryKey(this IEntityMetaData metaData)//not unique keys.
        {
            IList<PropertyMetaData> keys = metaData.OfKeys(true);
            if (keys.Count == 1)
            {
                return keys[0];
            }

            return null;//means Multiple key,  unique keys not Primary Key.
        }
    }
}