namespace ionixTests.MongoDB
{
    using global::MongoDB.Bson;
    using global::MongoDB.Bson.Serialization.Attributes;
    using ionix.Data.MongoDB;

    [MongoCollection(Database = "TestDb", Name = "PersonAddress")]
    [MongoIndex("PersonId", "AddressId")]
    [MongoIndex("AddressId")]
    [MongoIndex("AddressId")]
    public class PersonAddress
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public ObjectId PersonId { get; set; }

        public ObjectId AddressId { get; set; }

        public bool Active { get; set; } = true;
    }
}
