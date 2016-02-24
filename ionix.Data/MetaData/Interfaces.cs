namespace ionix.Data
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Utils;

    public interface IEntityMetaData : IPrototype<IEntityMetaData>
    {
        IEnumerable<PropertyMetaData> Properties { get; }
        string TableName { get; }
        Type EntityType { get; }

        PropertyMetaData this[string columnName] { get; }

        string GetParameterName(PropertyMetaData pm, int index);
    }
    public interface IEntityMetaDataProvider
    {
        IEntityMetaData CreateEntityMetaData(Type entityType);
    }


    public interface IPrototypeXmlSerializable<T> : IXmlSerializable, IPrototype<T>
    {
        string Serialize();
    }
}
