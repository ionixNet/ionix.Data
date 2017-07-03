namespace ionix.Data.MongoDB.Serializers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using global::MongoDB.Bson;
    using global::MongoDB.Bson.Serialization.Attributes;
    using Utils.Reflection;

    public static class DictionarySerializer
    {
        public static string GetFieldName(PropertyInfo pi)
        {
            BsonIdAttribute bia = pi.GetCustomAttribute<BsonIdAttribute>();
            if (null != bia)
                return "_id";
            BsonElementAttribute bea = pi.GetCustomAttribute<BsonElementAttribute>();
            if (null != bea)
                return bea.ElementName;

            return pi.Name;
        }

        //_id olmuyor.
        public static IDictionary<string, PropertyInfo> GetValidProperties(Type type)
        {
            Dictionary<string, PropertyInfo> ret = new Dictionary<string, PropertyInfo>();
            foreach (var pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (pi.GetCustomAttribute<BsonIgnoreAttribute>() != null)
                    continue;

                Type propertyType = pi.PropertyType;
                bool isObjectIdType = propertyType == typeof(ObjectId) || propertyType == typeof(ObjectId?);
                if (!isObjectIdType)
                {
                    if (!ReflectionExtensions.IsPrimitiveType(pi.PropertyType))
                        continue;

                    if (ReflectionExtensions.IsEnumerable(pi.PropertyType))
                        continue;
                }

                ret[GetFieldName(pi)] = pi;
            }

            return ret;
        }

        public static IDictionary<string, object> ToDictionary(this object model)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            if (null != model)
            {
                foreach (var kvp in GetValidProperties(model.GetType()))
                {
                    ret[kvp.Key] = kvp.Value.GetValue(model);
                }
            }
            return ret;
        }

        public static object To(this IDictionary<string, object> dic, Type target)
        {
            if (null != dic && null != target)
            {
                var model = Activator.CreateInstance(target);

                foreach (var kvp in GetValidProperties(target))
                {
                    object value;
                    if (dic.TryGetValue(kvp.Key, out value))
                    {
                        var pi = kvp.Value;
                        pi.SetValueSafely(model, value);
                    }

                }

                return model;
            }

            return null;
        }

        public static T To<T>(this IDictionary<string, object> dic)
        {
            return (T)To(dic, typeof(T));
        }

        public static T To<T>(this object dyn)
        {
            return (T)To(dyn as IDictionary<string, object>, typeof(T));
        }
    }
}
