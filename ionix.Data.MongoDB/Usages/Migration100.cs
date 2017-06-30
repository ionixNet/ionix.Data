//namespace ionix.Data.MongoDB
//{
//    using System;
//    using System.Reflection;
//    using System.Text;

//    public class Migration100 : MigrationBase
//    {
//        public Migration100()
//            : base("1.0.0")
//        {
//        }


//        public override string GenerateMigrationScript()
//        {
//            var libAssembly = GetMigrationsAssembly();

//            StringBuilder sb = new StringBuilder();
//            foreach (var type in libAssembly.GetTypes())
//            {
//                var typeInfo = type.GetTypeInfo();
//                var collAttr = typeInfo.GetCustomAttribute<MongoCollectionAttribute>();
//                if (null != collAttr)
//                {
//                    if (String.IsNullOrEmpty(collAttr.Database) || collAttr.Database == DatabaseName)
//                    {
//                        string script = collAttr.Script(type);

//                        sb.Append(script).Append("; ");
//                    }

//                    //default index
//                    var indexAttrList = typeInfo.GetCustomAttributes<MongoIndexAttribute>();
//                    if (null != indexAttrList)
//                    {
//                        foreach (var script in indexAttrList.Scripts(type))
//                        {

//                            sb.Append(script).Append("; ");
//                        }
//                    }

//                    //text Index
//                    var textIndexAttrList = typeInfo.GetCustomAttributes<MongoTextIndexAttribute>();
//                    if (null != textIndexAttrList)
//                    {
//                        foreach (var script in textIndexAttrList.Scripts(type))
//                        {
//                            sb.Append(script).Append("; ");
//                        }
//                    }
//                }
//            }

//            return sb.ToString();
//        }
//    }
//}
