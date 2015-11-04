namespace ionix.Data
{
    using Utils.Extensions;
    using Utils.Reflection;
    using System;
    using System.Data;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    partial class SqlQueryParameter : IPrototypeXmlSerializable<SqlQueryParameter>
    {
        private static readonly Type SqlQueryParameterType = typeof(SqlQueryParameter);

        public static SqlQueryParameter FromXml(string xml)
        {
            return XmlExtension.Deserialize<SqlQueryParameter>(xml);
        }
        internal static SqlQueryParameter FromXmlReader(XmlReader reader)
        {
            SqlQueryParameter ret = new SqlQueryParameter();
            ret.ReadXmlEmptyElement(reader);
            return ret;
        }
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (!reader.IsEmptyElement)
                reader.ReadStartElement(SqlQueryParameter.SqlQueryParameterType.Name);

            this.ReadXmlEmptyElement(reader);
        }
        private void ReadXmlEmptyElement(XmlReader reader)
        {
            this.parameterName = reader[0];//0

            string typeFullName = reader[1];//1
            if (!String.IsNullOrEmpty(typeFullName))
            {
                Type parameterValueType = ReflectionExtensions.GetType(typeFullName);
                var converter = SqlQueryParameterList.XmlValueConverter;

                string valueString = reader[2];//2
                this.value = converter.FromXml(valueString, parameterValueType);
            }
            this.direction = (ParameterDirection)Int32.Parse(reader[3]);//3
            this.isNullable = reader[4] == "1";//4

            string dbType = reader[5];//5
            if (!String.IsNullOrEmpty(dbType))
                this.dbType = (DbType)Int32.Parse(dbType);
        }
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(SqlQueryParameter.SqlQueryParameterType.Name);

            writer.WriteAttributeString("pn", this.ParameterName);//0

            if (this.value.IsNull())//ToString().Lenght== 0 yani DBNUll.Value içinde geçerli.
            {
                writer.WriteAttributeString("pvt", String.Empty);//1
                writer.WriteAttributeString("vl", String.Empty);//2
            }
            else
            {
                writer.WriteAttributeString("pvt", value.GetType().FullName);//1

                var converter = SqlQueryParameterList.XmlValueConverter;
                writer.WriteAttributeString("vl", converter.ToXmlString(this.value));//2
            }

            writer.WriteAttributeString("dr", ((Int32)this.direction).ToString());//3
            writer.WriteAttributeString("in", this.isNullable ? "1" : "0");//4
            writer.WriteAttributeString("dbt", this.dbType.HasValue ? ((Int32)this.dbType.Value).ToString() : String.Empty);//5

            writer.WriteEndElement();
        }

        XmlSchema IXmlSerializable.GetSchema() { return null; }
        public string Serialize()
        {
            return XmlExtension.Serialize(this);
        }
        //IPrototype
        public SqlQueryParameter Copy()
        {
            return SqlQueryParameter.FromXml(this.Serialize());
        }
    }
}
