//namespace ionix.Data.MongoDB
//{
//    using System;
//    using System.Reflection;

//    public class Migration100 : Migration.Migration
//    {
//        public const string DatabaseName = "KASIRGA";

//        public Migration100()
//            : base("1.0.0")
//        {
//        }

//        public override void Update()
//        {
//            var libAssembly = GetLibAssembly();

//            foreach (var type in libAssembly.GetTypes())
//            {
//                var typeInfo = type.GetTypeInfo();
//                var collAttr = typeInfo.GetCustomAttribute<MongoCollectionAttribute>();
//                if (null != collAttr)
//                {
//                    if (String.IsNullOrEmpty(collAttr.Database) || collAttr.Database == DatabaseName)
//                    {
//                        string script = collAttr.Script(type);
//                        MongoAdmin.ExecuteScript(this.Database, script);
//                    }

//                    //default index
//                    var indexAttrList = typeInfo.GetCustomAttributes<MongoIndexAttribute>();
//                    if (null != indexAttrList)
//                    {
//                        foreach (var script in indexAttrList.Scripts(type))
//                        {
//                            MongoAdmin.ExecuteScript(this.Database, script);
//                        }
//                    }

//                    //text Index
//                    var textIndexAttrList = typeInfo.GetCustomAttributes<MongoTextIndexAttribute>();
//                    if (null != textIndexAttrList)
//                    {
//                        foreach (var script in textIndexAttrList.Scripts(type))
//                        {
//                            MongoAdmin.ExecuteScript(this.Database, script);
//                        }
//                    }
//                }
//            }
//        }

//        public static Assembly GetLibAssembly()
//        {
//            var name = new AssemblyName("<assembly name here!");
//            return Assembly.Load(name);
//        }
//    }
//}
