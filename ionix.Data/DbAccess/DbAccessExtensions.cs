namespace ionix.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Dynamic;

    public static class DbAccessExtensions
    {
        private static void EnsureDbAccess(IDbAccess dataAccess)
        {
            if (null == dataAccess)
                throw new ArgumentNullException(nameof(dataAccess));
        }


        #region   |   Execute   |

        public static int ExecuteNonQuery(this IDbAccess dataAccess, string sql, params object[] pars)
        {
            EnsureDbAccess(dataAccess);

            return dataAccess.ExecuteNonQuery(sql.ToQuery(pars));
        }
        public static int ExecuteNonQuery(this IDbAccess dataAccess, string sql)
        {
            return ExecuteNonQuery(dataAccess, sql, null);
        }



        public static object ExecuteScalar(this IDbAccess dataAccess, string sql, params object[] pars)
        {
            EnsureDbAccess(dataAccess);

            return dataAccess.ExecuteScalar(sql.ToQuery(pars));
        }
        public static object ExecuteScalar(this IDbAccess dataAccess, string sql)
        {
            return ExecuteScalar(dataAccess, sql, null);
        }
        //Restful Servisler İçin Eklendi
        public static IEnumerable<object> ExecuteScalarList(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            List<object> ret = new List<object>();
            using (IDataReader dr = dataAccess.CreateDataReader(query, CommandBehavior.Default))
            {
                while (dr.Read())
                {
                    object value = dr[0];

                    ret.Add(value);
                }
            }
            return ret;
        }
        public static IEnumerable<object> ExecuteScalarList(this IDbAccess dataAccess, string sql, params object[] pars)
        {
            return ExecuteScalarList(dataAccess, sql.ToQuery(pars));
        }
        public static IEnumerable<object> ExecuteScalarList(this IDbAccess dataAccess, string sql)
        {
            return ExecuteScalarList(dataAccess, sql, null);
        }


        public static IDataReader CreateDataReader(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            return dataAccess.CreateDataReader(query, CommandBehavior.Default);
        }
        public static IDataReader CreateDataReader(this IDbAccess dataAccess, string sql, params object[] pars)
        {
            return CreateDataReader(dataAccess, sql.ToQuery(pars));
        }
        public static IDataReader CreateDataReader(this IDbAccess dataAccess, string sql)
        {
            return CreateDataReader(dataAccess, sql, null);
        }


        //Generics
        public static T ExecuteScalar<T>(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            return (T)Convert.ChangeType(dataAccess.ExecuteScalar(query), typeof(T));
        }
        public static T ExecuteScalar<T>(this IDbAccess dataAccess, string sql, params object[] pars)
        {
            return ExecuteScalar<T>(dataAccess, sql.ToQuery(pars));
        }
        public static T ExecuteScalar<T>(this IDbAccess dataAccess, string sql)
        {
            return ExecuteScalar<T>(dataAccess, sql, null);
        }



        public static T ExecuteScalarSafely<T>(this IDbAccess dataAccess, SqlQuery query)
        {
            if (null != dataAccess)
            {
                try
                {
                    return (T)Convert.ChangeType(dataAccess.ExecuteScalar(query), typeof(T));
                }
                catch { }
            }

            return default(T);
        }
        public static T ExecuteScalarSafely<T>(this IDbAccess dataAccess, string sql, params object[] pars)
        {
            return ExecuteScalarSafely<T>(dataAccess, sql.ToQuery(pars));
        }
        public static T ExecuteScalarSafely<T>(this IDbAccess dataAccess, string sql)
        {
            return ExecuteScalarSafely<T>(dataAccess, sql, null);
        }


        public static IEnumerable<T> ExecuteScalarList<T>(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            List<T> ret = new List<T>();
            using (IDataReader dr = dataAccess.CreateDataReader(query, CommandBehavior.Default))
            {
                while (dr.Read())
                {
                    object value = dr[0];

                    ret.Add((T)Convert.ChangeType(value, typeof(T)));
                }
            }
            return ret;
        }
        public static IEnumerable<T> ExecuteScalarList<T>(this IDbAccess dataAccess, string sql, params object[] pars)
        {
            return ExecuteScalarList<T>(dataAccess, sql.ToQuery(pars));
        }
        public static IEnumerable<T> ExecuteScalarList<T>(this IDbAccess dataAccess, string sql)
        {
            return ExecuteScalarList<T>(dataAccess, sql, null);
        }


        #endregion

        #region   |   ExpandoObject   |

        public static IList<ExpandoObject> Query(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            IList<ExpandoObject> ret = new List<ExpandoObject>();
            IDataReader dr = null;
            try
            {
                dr = dataAccess.CreateDataReader(query, CommandBehavior.Default);

                int fieldCount = dr.FieldCount;
                while (dr.Read())
                {
                    ExpandoObject expando = new ExpandoObject();
                    IDictionary<string, object> dic = expando;
                    for (int j = 0; j < fieldCount; ++j)
                    {
                        object dbValue = dr.IsDBNull(j) ? null : dr[j];

                        dic.Add(dr.GetName(j), dbValue);
                    }

                    ret.Add(expando);
                }
            }
            finally
            {
                if (dr != null) dr.Dispose();
            }

            return ret;
        }
        public static IList<ExpandoObject> Query(this IDbAccess dataAccess, string sql, params object[] pars)
        {
            return Query(dataAccess, sql.ToQuery(pars));
        }
        public static IList<ExpandoObject> Query(this IDbAccess dataAccess, string sql)
        {
            return Query(dataAccess, sql, null);
        }

        public static ExpandoObject QuerySingle(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            IDataReader dr = null;
            try
            {
                dr = dataAccess.CreateDataReader(query, CommandBehavior.SingleRow);

                int fieldCount = dr.FieldCount;
                if (dr.Read())
                {
                    ExpandoObject expando = new ExpandoObject();
                    IDictionary<string, object> dic = expando;
                    for (int j = 0; j < fieldCount; ++j)
                    {
                        object dbValue = dr.IsDBNull(j) ? null : dr[j];

                        dic.Add(dr.GetName(j), dbValue);
                    }

                    return expando;
                }
            }
            finally
            {
                if (dr != null) dr.Dispose();
            }

            return null;
        }
        public static ExpandoObject QuerySingle(this IDbAccess dataAccess, string sql, params object[] pars)
        {
            return QuerySingle(dataAccess, sql.ToQuery(pars));
        }
        public static ExpandoObject QuerySingle(this IDbAccess dataAccess, string sql)
        {
            return QuerySingle(dataAccess, sql, null);
        }

        public static dynamic QuerySingle(this IDbAccess dataAccess, IEntityMetaDataProvider provider, SqlQuery query, MapBy by, char prefix, params Type[] types)
        {
            var result = new EntityCommandSelect(dataAccess, prefix).QueryTemplateSingle(provider, query, by, types);
            if (null != result)
            {
                IDictionary<string, object> dic = new ExpandoObject();
                for (int j = 0; j < types.Length; ++j)
                {
                    dic[types[j].Name] = result[j];
                }
                return dic;
            }

            return null;
        }

        public static List<dynamic> Query(this IDbAccess dataAccess, IEntityMetaDataProvider provider, SqlQuery query, MapBy by, char prefix, params Type[] types)
        {
            List<dynamic> ret = new List<dynamic>();
            var result = new EntityCommandSelect(dataAccess, prefix).QueryTemplate(provider, query, by, types);
            if (result.Count != 0)
            {
                foreach (object[] arr in result)//örneğin 3 lü dönüyor
                {
                    IDictionary<string, object> dic = new ExpandoObject();
                    for (int j = 0; j < types.Length; ++j)
                    {
                        dic[types[j].Name] = arr[j];
                    }
                    ret.Add(dic);
                }
            }

            return ret;
        }

        #endregion

        #region   |    DataTable     |

        public static DataTable QueryDataTable(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            DataTable ret = new DataTable();
            IDataReader dr = null;
            try
            {
                dr = dataAccess.CreateDataReader(query, CommandBehavior.Default);
                ret.Load(dr);
            }
            finally
            {
                if (dr != null) dr.Dispose();
            }

            return ret;
        }
        public static DataTable QueryDataTable(this IDbAccess dataAccess, string sql, params object[] pars)
        {
            return QueryDataTable(dataAccess, sql.ToQuery(pars));
        }
        public static DataTable QueryDataTable(this IDbAccess dataAccess, string sql)
        {
            return QueryDataTable(dataAccess, sql, null);
        }

        #endregion


        public static int Execute<TEntity>(this FluentBaseExecutable<TEntity> fluent, IDbAccess dataAccess)
        {
            EnsureDbAccess(dataAccess);

            if (null != fluent)
            {
                SqlQuery query = fluent.ToQuery();
                return dataAccess.ExecuteNonQuery(query);
            }

            return 0;
        }
    }
}
