namespace ionix.Data
{
    using System;
    using System.Data;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    partial class SqlQuery : IPrototypeXmlSerializable<SqlQuery>
    {
        public static SqlQuery FromXml(string xml)
        {
            return XmlExtension.Deserialize<SqlQuery>(xml);
        }
        internal static SqlQuery FromXmlReader(XmlReader reader)
        {
            SqlQuery ret = new SqlQuery();
            ret.ReadXmlEmptyElement(reader);
            return ret;
        }
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (!reader.IsEmptyElement)
                reader.ReadStartElement(SqlQuery.SqlQueryType.Name);

            this.ReadXmlEmptyElement(reader);
        }
        private void ReadXmlEmptyElement(XmlReader reader)
        {
            reader.ReadStartElement("text");
            this.text.Append(reader.ReadContentAsString());
            reader.ReadEndElement();

            reader.ReadStartElement("ct");
            this.cmdType = (CommandType)Int32.Parse(reader.ReadContentAsString());
            reader.ReadEndElement();

            reader.ReadStartElement("Parameters");
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                SqlQueryParameter par = new SqlQueryParameter();
                IXmlSerializable xs = par;
                xs.ReadXml(reader);

                this.parameters.Add(par);

                reader.Read();
            }
            reader.ReadEndElement();
        }

        private static readonly Type SqlQueryType = typeof(SqlQuery);
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("text");
            writer.WriteString(this.text.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("ct");
            writer.WriteString(((Int32)this.cmdType).ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Parameters");
            foreach (SqlQueryParameter par in this.parameters)
            {
                IXmlSerializable xs = par;
                xs.WriteXml(writer);
            }

            writer.WriteEndElement();
        }
        XmlSchema IXmlSerializable.GetSchema() { return null; }
        public string Serialize()
        {
            return XmlExtension.Serialize(this);
        }

        public SqlQuery Copy()
        {
            return FromXml(this.Serialize());
        }
    }
}
