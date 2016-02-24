namespace ionix.Data.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DbSchemaAttribute : Attribute
    {
        public string ColumnName { get; set; }//Proprty ismi kolon ismiyle farklılık gösteriyor mu diye.

        public bool IsKey { get; set; }
        public StoreGeneratedPattern DatabaseGeneratedOption { get; set; }

        public bool IsNullable { get; set; }
        public int MaxLength { get; set; }//UI Binding için.
        public string DefaultValue { get; set; }

        public bool ReadOnly { get; set; }

        public SqlValueType SqlValueType { get; set; }
    }
}
