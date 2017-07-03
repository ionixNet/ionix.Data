namespace ionix.Data.MongoDB.Serializers
{
    using global::MongoDB.Bson;
    using global::MongoDB.Bson.Serialization;

    public static class BsonSerializerExtensions
    {
        public static string BsonSerialize(this object obj)
        {
            return obj?.ToBsonDocument().ToString();
        }

        public static T BsonDeserialize<T>(this string bson)
        {
            return BsonSerializer.Deserialize<T>(bson);
        }
    }
}
