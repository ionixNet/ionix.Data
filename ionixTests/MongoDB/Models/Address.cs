namespace ionixTests.MongoDB
{
    using System;
    using global::MongoDB.Bson;
    using global::MongoDB.Bson.Serialization.Attributes;
    using ionix.Data.MongoDB;

    [MongoCollection(Database = "TestDb", Name = "Address")]
    [MongoIndex("Name")]
    public class Address
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public string Street { get; set; }

        public string HouseNumber { get; set; }
    }
}
