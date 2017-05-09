namespace ionix.Data.MongoDB
{
    using global::MongoDB.Bson.Serialization.Attributes;

    public sealed class Database
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("sizeOnDisk")]
        public double SizeOnDisk { get; set; }

        [BsonElement("empty")]
        public bool Empty { get; set; }
    }
}
