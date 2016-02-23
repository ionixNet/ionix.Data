namespace ionix.Data
{
    using Utils;
    using Utils.Collections;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public sealed class PropertyMetaData : IEquatable<PropertyMetaData>, IPrototype<PropertyMetaData>
    {
        public PropertyMetaData(SchemaInfo schema, PropertyInfo property)
        {
            if (null == schema)
                throw new ArgumentNullException(nameof(schema));
            if (null == property)
                throw new ArgumentNullException(nameof(property));

            this.Schema = schema;
            this.Property = property;
        }

        public SchemaInfo Schema { get; }

        public PropertyInfo Property { get; }

        private string parameterName;
        public string ParameterName//Batch Command larda index ler paramatre isimlerine ekleniyor diye
        {
            get
            {
                if (null == this.parameterName)
                    return this.Schema.ColumnName;

                return this.parameterName;
            }
            set { this.parameterName = value; }
        }

        //Cache lenen nesnelerde schema info ve parametre isminin değişmesi sıkıntı çıkartıyor(ki command nesneleri bu edğişkikliği yapıyor.)
        public PropertyMetaData Copy()
        {
            PropertyMetaData copy = new PropertyMetaData(this.Schema.CopyProperties(), this.Property);//property zaten readonly bir object. tüm prop lar readonly.
            //ParameterName kopyalanmamalı.
            return copy;
        }

        public bool Equals(PropertyMetaData other)
        {
            if (null != other)
                return this.Schema.Equals(other.Schema);
            return false;
        }
        public override bool Equals(object obj)
        {
            return this.Equals(obj as PropertyMetaData);
        }
        public override int GetHashCode()
        {
            return this.Schema.GetHashCode();
        }

        public override string ToString()
        {
            return this.Schema.ColumnName;
        }
    }

    public class EntityMetaData : IEntityMetaData
    {
        private readonly ThrowingHashSet<PropertyMetaData> hash;

        public EntityMetaData(Type entityType, string tableName)
        {
            if (null == entityType)
                throw new ArgumentNullException(nameof(entityType));
            if (String.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            this.hash = new ThrowingHashSet<PropertyMetaData>();
            this.EntityType = entityType;
            this.TableName = tableName;
        }
        public EntityMetaData(Type entityType)
            : this(entityType, AttributeExtension.GetTableName(entityType))
        {

        }

        public void Add(SchemaInfo schema, PropertyInfo property)
        {
            this.hash.Add(new PropertyMetaData(schema, property));
        }
        public void Add(PropertyMetaData item)
        {
            if (null == item)
                throw new ArgumentNullException(nameof(item));

            this.hash.Add(item);
        }

        public string TableName { get; }

        public Type EntityType { get; }

        public IEnumerable<PropertyMetaData> Properties => this.hash;

        public int Count => this.hash.Count;

        public IEntityMetaData Copy()
        {
            EntityMetaData copy = new EntityMetaData(this.EntityType, this.TableName);//Type ReadOnly bir object dir. String de fixed char* kullanılmıyorsa immutable bir nesnedir.
            foreach (PropertyMetaData orginal in this.hash)
            {
                copy.hash.Add(orginal.Copy());
            }

            return copy;
        }

        public override string ToString()
        {
            return this.TableName;
        }

        private Dictionary<string, PropertyMetaData> dic;
        private Dictionary<string, PropertyMetaData> Dic
        {
            get
            {
                if (null == this.dic)
                {
                    this.dic = new Dictionary<string, PropertyMetaData>(this.hash.Count);
                    foreach (PropertyMetaData item in this.hash)
                        this.dic.Add(item.Schema.ColumnName, item);
                }

                return this.dic;
            }
        }

        public PropertyMetaData this[string columnName]
        {
            get
            {
                PropertyMetaData p;
                this.Dic.TryGetValue(columnName, out p);
                return p;
            }
        }
    }
}
