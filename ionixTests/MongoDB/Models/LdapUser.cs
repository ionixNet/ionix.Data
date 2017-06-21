namespace ionixTests.MongoDB
{
    using global::MongoDB.Bson;
    using global::MongoDB.Bson.Serialization.Attributes;
    using ionix.Data.MongoDB;

    [MongoCollection(Database = "TestDb", Name = "LdapUser")]
    [MongoIndex("UserName", Unique = true)]
    [MongoTextIndex("DisplayName", "physicalDeliveryOfficeName")]
    public class LdapUser
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string EMail { get; set; }

        public string Description { get; set; }

        public bool IsMapped { get; set; }

        public string FirstName { get; set; }

        public string Lastname { get; set; }

        public string DepartmenName { get; set; }

        public string DepartmentCode { get; set; }

        public string Title { get; set; }

        public string SamaAcountName { get; set; }

        public string UserGroup { get; set; }

        public string physicalDeliveryOfficeName { get; set; }

        public object Enabled { get; set; }
    }
}
