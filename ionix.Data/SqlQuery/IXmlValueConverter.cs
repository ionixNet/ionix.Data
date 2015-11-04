namespace ionix.Data
{
    using System;
    using Utils;

    public interface IXmlValueConverter
    {
        object FromXml(string value, Type conversionType);
        string ToXmlString(object value);
    }

    public class XmlValueConverter : IXmlValueConverter
    {
        public object FromXml(string value, Type conversionType)
        {
            if (conversionType == CachedTypes.Guid)
                return Guid.Parse(value);
            if (conversionType == CachedTypes.Nullable_Guid)
            {
                if (String.IsNullOrEmpty(value))
                    return Guid.Parse(value);
                return null;
            }

            return Convert.ChangeType(value, conversionType);
        }

        public string ToXmlString(object value)
        {
            return value?.ToString() ?? String.Empty;
        }
    }
}
