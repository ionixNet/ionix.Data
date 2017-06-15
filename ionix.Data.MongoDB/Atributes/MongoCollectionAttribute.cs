namespace ionix.Data.MongoDB
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class MongoCollectionAttribute : Attribute
    {
        public MongoCollectionAttribute()
        {

        }
        public MongoCollectionAttribute(string name)
        {
            this.Name = name;
        }

        public string Database { get; set; }
        public string Name { get; set; }

        public bool AutoIndexId { get; set; } = true;
        public int Size { get; set; }
        public int Max { get; set; }
    }
}
