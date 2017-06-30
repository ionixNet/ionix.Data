namespace ionix.Data.MongoDB
{
    using global::MongoDB.Bson;

    public static class HelperExtensions
    {

        public static ObjectId ToObjectId(this string id)
        {
            ObjectId temp;
            if (ObjectId.TryParse(id, out temp))
            {
                return temp;
            }
            return ObjectId.Empty;
        }

        public static ObjectId? ToObjectIdNullable(this string id)
        {
            ObjectId temp;
            if (ObjectId.TryParse(id, out temp))
            {
                return temp;
            }
            return null;
        }

        public static ObjectId ToNonNullable(this ObjectId? id)
        {
            return id ?? ObjectId.Empty;
        }

        public static bool IsValidObjectId(this string id)
        {
            ObjectId temp;
            if (ObjectId.TryParse(id, out temp))
            {
                return temp != ObjectId.Empty;
            }
            return false;
        }

        public static bool IsEmpty(this ObjectId id)
        {
            return id == ObjectId.Empty;
        }

        public static bool IsEmpty(this ObjectId? id)
        {
            return id == ObjectId.Empty;
        }
    }
}
