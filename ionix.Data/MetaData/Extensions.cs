namespace ionix.Data
{
    using Utils;
    using Utils.Reflection;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.IO;
    using System.Reflection;
    using System.Xml.Serialization;
    using Utils.Extensions;

    public static class XmlExtension
    {
        internal static string Serialize(IXmlSerializable xs, Type cachedType)
        {
            StringWriter sw = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(cachedType ?? xs.GetType());
            serializer.Serialize(sw, xs);
            return sw.ToString();
        }

        internal static T Deserialize<T>(string xml, Type cachedType)
            where T : class, IXmlSerializable
        {
            if (!String.IsNullOrEmpty(xml))
            {
                StringReader sr = new StringReader(xml);
                XmlSerializer xs = new XmlSerializer(cachedType ?? typeof (T));
                return xs.Deserialize(sr) as T;
            }
            return null;
        }

        public static string Serialize(IXmlSerializable xs)
        {
            if (null == xs)
                throw new ArgumentNullException(nameof(xs));

            return Serialize(xs, null);
        }

        public static T Deserialize<T>(string xml)
            where T : class, IXmlSerializable
        {
            return Deserialize<T>(xml, null);
        }
    }

    public static class AttributeExtension
    {
        public static string GetTableName(Type entityType)
        {
            if (null == entityType)
                throw new ArgumentNullException(nameof(entityType));

            TableAttribute ta = entityType.GetCustomAttribute<TableAttribute>();
            if (null != ta)
                return String.IsNullOrEmpty(ta.Schema) ? ta.Name : ta.Schema + '.' + ta.Name;

            return entityType.FullName; //schema, user name gibi yapıları namespace yap.
        }

        public static string GetTableName<TEntity>()
        {
            return GetTableName(typeof (TEntity));
        }
    }

    public static class EntityMetadaExtensions
    {
        //SchemaInfo' ya göre validation. Zaten Typesafect bir ortamdayız. Sadece isnullable ve maxlength validation' ı.
        //Kullanıcı isteğe göre ekleyebilir.
        public static bool IsModelValid<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            if (null != entity && null != provider)
            {
                foreach (var metaData in provider.CreateEntityMetaData(typeof (TEntity)).Properties)
                {
                    PropertyInfo pi = metaData.Property;
                    if (!metaData.Schema.IsNullable)
                    {
                        if (pi.PropertyType.IsClass || pi.PropertyType.IsNullableType())
                            //ValueType zaten default value zore lar
                        {
                            object value = pi.GetValue(entity);
                            if (null == value)
                            {
                                return false;
                            }
                        }
                    }
                    int maxLength = metaData.Schema.MaxLength;
                    if (maxLength > 0)
                    {
                        if (pi.PropertyType == CachedTypes.String)
                        {
                            object value = pi.GetValue(entity);
                            if (value?.ToString().Length > maxLength)
                                return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public static bool IsModelValid<TEntity>(TEntity entity)
        {
            return IsModelValid(entity, DbSchemaMetaDataProvider.Instance);
        }

        public static bool IsModelListValid<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            bool ret = !entityList.IsEmptyList();
            if (ret)
            {
                foreach (var entity in entityList)
                {
                    ret = IsModelValid(entity, provider);
                    if (!ret) return false;
                }
            }
            return ret;
        }

        public static bool IsModelListValid<TEntity>(IEnumerable<TEntity> entityList)
        {
            return IsModelListValid(entityList, DbSchemaMetaDataProvider.Instance);
        }
    }
}
