//namespace ionix.Data.MongoDB
//{
//    using Newtonsoft.Json;
//    using System;
//    using System.Collections;
//    using System.Collections.Generic;
//    using System.Text;
//    using global::MongoDB.Bson;
//    using global::MongoDB.Bson.IO;


//    public class ObjectIdListConverter : JsonConverter
//    {
//        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//        {
//            string strValue = value == null ? String.Empty : value.ToString();
//            IEnumerable list = value as IEnumerable;
//            if (list != null)
//            {
//                StringBuilder sb = new StringBuilder("[");
//                foreach (var str in list)
//                {
//                    sb.Append("'")
//                        .Append(str)
//                        .Append("',");
//                }
//                if (sb.Length > 1)
//                    sb.Remove(sb.Length - 1, 1);
//                sb.Append("]");

//                strValue = sb.ToString();
//            }
//            serializer.Serialize(writer, strValue);
//        }

//        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//        {
//            List<ObjectId> ret = new List<ObjectId>();
//            IEnumerable list = reader.Value as IEnumerable;
//            if (null != list)
//            {
//                foreach (var str in list)
//                {
//                    ObjectId id;
//                    if (ObjectId.TryParse(str.ToString(), out id))
//                    {
//                        ret.Add(id);
//                    }

//                }
//            }

//            return ret;
//        }

//        public override bool CanConvert(Type objectType)
//        {
//            return true;
//        }
//    }
//}
