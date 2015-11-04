namespace ionix.Data
{
    using System;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    partial class SchemaInfo : IPrototypeXmlSerializable<SchemaInfo>
    {
        private SchemaInfo() { }

        public static SchemaInfo FromXml(string xml)
        {
            return XmlExtension.Deserialize<SchemaInfo>(xml, SchemaInfo.SchemaInfoType);
        }
        internal static SchemaInfo FromXmlReader(XmlReader reader)
        {
            SchemaInfo ret = new SchemaInfo();
            ret.ReadXmlEmptyElement(reader);
            return ret;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (!reader.IsEmptyElement)
                reader.ReadStartElement(SchemaInfoType.Name);

            this.ReadXmlEmptyElement(reader);
        }
        private void ReadXmlEmptyElement(XmlReader reader)
        {
            this.ColumnName = reader[0];//0
            string fullTypeName = reader[1];//1
            if (fullTypeName.Length != 0)
                this.DataType = ionix.Utils.Reflection.ReflectionExtensions.GetType(fullTypeName);
            this.IsNullable = reader[2] == "1";//2

            this.IsKey = reader[3] == "1";//3
            this.ReadOnly = reader[4] == "1";//4

            this.DatabaseGeneratedOption = (StoreGeneratedPattern)Int32.Parse(reader[5]);//5
            this.DefaultValue = reader[6];//6

            this.MaxLength = Int32.Parse(reader[7]);//7
            this.Order = Int32.Parse(reader[8]);//8

            this.SqlValueType = (SqlValueType)Int32.Parse(reader[9]);
        }
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(SchemaInfoType.Name);

            writer.WriteAttributeString("cn", this.ColumnName);//0
            writer.WriteAttributeString("dt", this.DataType == null ? String.Empty : this.DataType.FullName);//1
            writer.WriteAttributeString("an", this.IsNullable ? "1" : "0");//2

            writer.WriteAttributeString("ik", this.IsKey ? "1" : "0");//3
            writer.WriteAttributeString("ro", this.ReadOnly ? "1" : "0");//4

            writer.WriteAttributeString("dgo", ((Int32)this.DatabaseGeneratedOption).ToString());//5
            writer.WriteAttributeString("df", this.DefaultValue);//6

            writer.WriteAttributeString("ml", this.MaxLength.ToString());//7
            writer.WriteAttributeString("or", this.Order.ToString());//8

            //SchemaInfo.Table xml serileştirimeyecek tabiki de.

            writer.WriteAttributeString("svt", ((Int32)this.SqlValueType).ToString());//9

            writer.WriteEndElement();
        }


        internal static readonly Type SchemaInfoType = typeof(SchemaInfo);
        XmlSchema IXmlSerializable.GetSchema() { return null; }
        public string Serialize()
        {
            return XmlExtension.Serialize(this, SchemaInfoType);
        }
    }
}
