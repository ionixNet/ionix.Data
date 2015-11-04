namespace ionix.Data
{
    using System;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    partial class SqlQueryParameterList : IPrototypeXmlSerializable<SqlQueryParameterList>
    {
        private static IXmlValueConverter xmlValueConverter;
        public static IXmlValueConverter XmlValueConverter
        {
            get
            {
                if (null == xmlValueConverter)
                    xmlValueConverter = new XmlValueConverter();
                return xmlValueConverter;
            }
            set { xmlValueConverter = value; }
        }

        private static readonly Type SqlQueryParameterListType = typeof(SqlQueryParameterList);

        public static SqlQueryParameterList FromXml(string xml)
        {
            return XmlExtension.Deserialize<SqlQueryParameterList>(xml);
        }

        public string Serialize()
        {
            return XmlExtension.Serialize(this);
        }

        XmlSchema IXmlSerializable.GetSchema() { return null; }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (this.hash.Count != 0)
                throw new InvalidOperationException("SqlQueryParameterList must be empty.");

            bool isNotEmptyElement = !reader.IsEmptyElement;
            if (isNotEmptyElement)
                reader.ReadStartElement(SqlQueryParameterList.SqlQueryParameterListType.Name);

            reader.ReadStartElement("Parameters");

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                SqlQueryParameter parameter = SqlQueryParameter.FromXmlReader(reader);
                this.Add(parameter);

                reader.Read();
            }

            if (this.hash.Count > 0)
                reader.ReadEndElement();

            if (isNotEmptyElement)
                reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Parameters");

            foreach (SqlQueryParameter par in this)
            {
                IXmlSerializable xs = par;
                xs.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        //IPrototaype
        public SqlQueryParameterList Copy()
        {
            return SqlQueryParameterList.FromXml(this.Serialize());
        }
    }
}
