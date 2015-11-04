namespace ionix.Data
{
    using Attributes;
    using Utils;
    using Utils.Reflection;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;
    using System.Collections.Generic;


    public abstract class EntityMetaDataProviderBase : IEntityMetaDataProvider
    {
        private static readonly IDictionary<Type, IEntityMetaData> _cache = new Dictionary<Type, IEntityMetaData>();//new ConcurrentDictionary<Type, IEntityMetaData>();// new Dictionary<Type, IEntityMetaData>(); 

        protected virtual bool IsMapped(PropertyInfo pi)
        {
            if (pi.GetCustomAttribute<NotMappedAttribute>() != null)
                return false;

            if (ReflectionExtensions.IsEnumerable(pi))
                return false;

            return true;
        }
        protected abstract void SetExtendedSchema(SchemaInfo schema, PropertyInfo pi);
        protected virtual void SetExtendedMetaData(EntityMetaData metaData) { }

        private SchemaInfo FromPropertyInfo(PropertyInfo pi)
        {
            if (!this.IsMapped(pi))
                return null;

            Type propertyType = pi.PropertyType;

            bool nullableTypeDetected = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == CachedTypes.PureNullableType;

            SchemaInfo schema = new SchemaInfo(pi.Name, nullableTypeDetected ? propertyType.GetGenericArguments()[0] : propertyType);

            //NotMapped gibi bir standart
            KeyAttribute keyAtt = pi.GetCustomAttribute<KeyAttribute>();
            if (null != keyAtt)
                schema.IsKey = true;

            if (nullableTypeDetected)
                schema.IsNullable = true;
            else
            {
                if (propertyType.IsClass)
                    schema.IsNullable = true;
                else if (propertyType.IsValueType)
                    schema.IsNullable = false;
            }

            bool hasSetMethod = pi.SetMethod != null;
            if (!hasSetMethod)
                schema.ReadOnly = true;

            this.SetExtendedSchema(schema, pi);

            return schema;
        }

        private static readonly object syncRoot = new object();
        public IEntityMetaData CreateEntityMetaData(Type entityType)
        {
            lock (syncRoot)
            {
                IEntityMetaData metaData;
                if (!_cache.TryGetValue(entityType, out metaData))
                {
                    EntityMetaData temp = new EntityMetaData(entityType);
                    foreach (PropertyInfo pi in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        SchemaInfo schema = this.FromPropertyInfo(pi);
                        if (null == schema) //NotMapped.
                            continue;
                        schema.Lock();
                        temp.Add(schema, pi);
                    }

                    this.SetExtendedMetaData(temp);

                    _cache.Add(entityType, temp);
                    metaData = temp;
                }

                return metaData;
            }
        }
    }

    //DbSchemaAttribute ile kullanılacak custom yapılar için.
    public class DbSchemaMetaDataProvider : EntityMetaDataProviderBase
    {
        protected override bool IsMapped(PropertyInfo pi)
        {
            if (pi.GetCustomAttribute<NotMappedAttribute>() != null)
                return false;

            return true;
        }

        protected override void SetExtendedSchema(SchemaInfo schema, PropertyInfo pi)
        {
            DbSchemaAttribute att = pi.GetCustomAttribute<DbSchemaAttribute>();
            if (null != att)
            {
                if (!String.IsNullOrEmpty(att.ColumnName))
                    schema.ColumnName = att.ColumnName;
                schema.DatabaseGeneratedOption = att.DatabaseGeneratedOption;
                schema.DefaultValue = att.DefaultValue;
                schema.IsKey = att.IsKey;
                schema.MaxLength = att.MaxLength;
                schema.Order = att.Order;
                schema.IsNullable = att.IsNullable;
                schema.ReadOnly = att.ReadOnly;
                schema.SqlValueType = att.SqlValueType;
            }
        }
    }


    //Saf Select İfadeleri İçin
    public class MetaDataProvider : EntityMetaDataProviderBase
    {
        protected override bool IsMapped(PropertyInfo pi)
        {
            if (base.IsMapped(pi))
            {
                return ReflectionExtensions.IsPrimitiveType(pi);
            }

            return false;
        }
        protected override void SetExtendedSchema(SchemaInfo schema, PropertyInfo pi)
        {
            if (pi.GetCustomAttribute<KeyAttribute>() != null || String.Equals(pi.Name, "Id", StringComparison.OrdinalIgnoreCase))
            {
                schema.IsKey = true;
            }
        }
    }

    public delegate void SetExtendedSchemaMethod(PropertyInfo pi, SchemaInfo schema);
    public delegate void SetExtendedMetaDataMethod(EntityMetaData metaData);
    public sealed class StrategyMetaDataProvider<TEntity> : EntityMetaDataProviderBase
    {
        private readonly SetExtendedSchemaMethod set;
        private readonly Func<PropertyInfo, bool> isMappedMethod;
        private readonly SetExtendedMetaDataMethod setExtendedMetaDataMethod;
        public StrategyMetaDataProvider(SetExtendedSchemaMethod set, Func<PropertyInfo, bool> isMappedMethod, SetExtendedMetaDataMethod setExtendedMetaDataMethod)
        {
            if (null == set)
                throw new ArgumentNullException(nameof(set));

            this.set = set;
            this.isMappedMethod = isMappedMethod;
            this.setExtendedMetaDataMethod = setExtendedMetaDataMethod;
        }
        public StrategyMetaDataProvider(SetExtendedSchemaMethod set, Func<PropertyInfo, bool> isMappedMethod)
            : this(set, isMappedMethod, null)
        { }
        public StrategyMetaDataProvider(SetExtendedSchemaMethod set)
            : this(set, null, null)
        { }


        protected override void SetExtendedSchema(SchemaInfo schema, PropertyInfo pi)
        {
            this.set(pi, schema);
        }
        protected override bool IsMapped(PropertyInfo pi)
        {
            if (base.IsMapped(pi))
            {
                if (null != this.isMappedMethod)
                    return this.isMappedMethod(pi);
                return true;
            }
            return false;
        }
        protected override void SetExtendedMetaData(EntityMetaData metaData)
        {
            if (null != this.setExtendedMetaDataMethod)
                this.setExtendedMetaDataMethod(metaData);
        }
    }
}