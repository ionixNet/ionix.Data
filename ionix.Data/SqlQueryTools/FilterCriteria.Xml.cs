namespace ionix.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    partial class FilterCriteria : IPrototypeXmlSerializable<FilterCriteria>
    {
        internal FilterCriteria() { }
        public static FilterCriteria FromXml(string xml)
        {
            return XmlExtension.Deserialize<FilterCriteria>(xml);
        }
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (!reader.IsEmptyElement)
                reader.ReadStartElement(FilterCriteria.FilterCriteriaType.Name);

            this.ReadXmlEmptyElement(reader);
        }
        private void ReadXmlEmptyElement(XmlReader reader)
        {
            reader.ReadStartElement("cn");
            this.columnName = reader.ReadContentAsString();
            reader.ReadEndElement();

            reader.ReadStartElement("pn");
            this.ParameterName = reader.ReadContentAsString();
            reader.ReadEndElement();

            reader.ReadStartElement("op");
            this.op = (ConditionOperator)reader.ReadContentAsInt();
            reader.ReadEndElement();

            reader.ReadStartElement("pf");
            this.prefix = reader.ReadContentAsString()[0];
            reader.ReadEndElement();


            reader.ReadStartElement("Values");
            string xml = reader.ReadContentAsString();

            XmlSerializer serializer = new XmlSerializer(typeof(List<object>));
            StringReader sr = new StringReader(xml);
            this.values = (List<object>)serializer.Deserialize(sr);

            reader.ReadEndElement();
        }

        internal static readonly Type FilterCriteriaType = typeof(FilterCriteria);
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("cn");
            writer.WriteString(this.columnName);
            writer.WriteEndElement();

            writer.WriteStartElement("pn");
            writer.WriteString(this.ParameterName);
            writer.WriteEndElement();

            writer.WriteStartElement("op");
            writer.WriteValue(((Int32)this.op));
            writer.WriteEndElement();

            writer.WriteStartElement("pf");
            writer.WriteString(this.prefix.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Values");

            XmlSerializer serializer = new XmlSerializer(typeof(List<object>));
            StringBuilder sb = new StringBuilder();
            XmlWriter wr = XmlWriter.Create(sb);
            serializer.Serialize(wr, this.values);
            writer.WriteString(sb.ToString());

            writer.WriteEndElement();
        }

        XmlSchema IXmlSerializable.GetSchema() { return null; }
        public string Serialize()
        {
            return XmlExtension.Serialize(this);
        }

        public FilterCriteria Copy()
        {
            return FromXml(this.Serialize());
        }
    }


    partial class FilterCriteriaList : IPrototypeXmlSerializable<FilterCriteriaList>
    {
        private static readonly Type FilterCriteriaListType = typeof(FilterCriteriaList);

        public static FilterCriteriaList FromXml(string xml)
        {
            return XmlExtension.Deserialize<FilterCriteriaList>(xml);
        }

        public string Serialize()
        {
            return XmlExtension.Serialize(this);
        }

        XmlSchema IXmlSerializable.GetSchema() { return null; }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (this.Count != 0)
                throw new InvalidOperationException("FilterCriteriaList must be empty.");

            bool isNotEmptyElement = !reader.IsEmptyElement;
            if (isNotEmptyElement)
                reader.ReadStartElement(FilterCriteriaList.FilterCriteriaListType.Name);

            reader.ReadStartElement("Prefix");
            this.prefix = reader.ReadContentAsString()[0];
            reader.ReadEndElement();

            reader.ReadStartElement("Filters");

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                FilterCriteria filter = new FilterCriteria();
                IXmlSerializable xs = filter;
                xs.ReadXml(reader);
                this.Add(filter);

                reader.Read();
            }

            if (this.Count > 0)
                reader.ReadEndElement();

            if (isNotEmptyElement)
                reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Prefix");
            writer.WriteString(this.prefix.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Filters");

            foreach (FilterCriteria par in this)
            {
                writer.WriteStartElement(FilterCriteria.FilterCriteriaType.Name);

                IXmlSerializable xs = par;
                xs.WriteXml(writer);

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public FilterCriteriaList Copy()
        {
            return FromXml(this.Serialize());
        }
    }
}
