namespace ionixTests.MongoDB
{
    using global::MongoDB.Bson;
    using global::MongoDB.Bson.Serialization.Attributes;
    using ionix.Data.MongoDB;


    [MongoCollection(Database = "KASIRGA", Name = "Asset")]
    [MongoIndex("Asset", Unique = true)]
    //[BsonIgnoreExtraElements]
    public class MngAsset
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Asset { get; set; }

        public bool Active { get; set; } = true;

        public string Description { get; set; }
    }
}
