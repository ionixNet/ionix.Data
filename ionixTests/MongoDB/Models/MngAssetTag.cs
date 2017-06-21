namespace ionixTests.MongoDB
{
    using global::MongoDB.Bson;
    using global::MongoDB.Bson.Serialization.Attributes;
    using ionix.Data.MongoDB;

    [MongoCollection(Database = "KASIRGA", Name = "AssetTag")]
    [MongoIndex("AssetId", "IsDirect", "TagId", "PrimaryTagId", Unique = true)]
    [MongoIndex("TagId")]
    public class MngAssetTag
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public ObjectId AssetId { get; set; }

        public ObjectId TagId { get; set; }

        public bool IsDirect { get; set; }

        public ObjectId PrimaryTagId { get; set; }
    }
}
