namespace ionixTests.MongoDB
{
    using global::MongoDB.Bson;
    using global::MongoDB.Bson.Serialization.Attributes;
    using ionix.Data.MongoDB;

    [MongoCollection(Database = "TestDb", Name = "PersonAddress")]
    [MongoIndex("PersonId", Unique = true)]
    public class PersonAddress
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public ObjectId PersonId { get; set; }

        public string Description { get; set; }
    }
}
