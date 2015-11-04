namespace ionix.Data
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.IO;
    using System.Reflection;
    using System.Xml.Serialization;

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
                XmlSerializer xs = new XmlSerializer(cachedType ?? typeof(T));
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

            return entityType.FullName;//schema, user name gibi yapıları namespace yap.
        }
        public static string GetTableName<TEntity>()
        {
            return GetTableName(typeof(TEntity));
        }
    }
}
