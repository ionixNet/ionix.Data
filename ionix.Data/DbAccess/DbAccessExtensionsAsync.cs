namespace ionix.Data
{
    using System.Collections.Generic;
    using System.Data;
    using System.Dynamic;
    using System.Threading.Tasks;


    public static class DbAccessExtensionsAsync
    {
        public static Task<IDataReader> CreateDataReaderAsync(this IDbAccess dataAccess, SqlQuery query, CommandBehavior behavior)
        {
            return Task.Run<IDataReader>(() =>
            {
                if (null != dataAccess)
                    return dataAccess.CreateDataReader(query, behavior);
                return null;
            });
        }
        public static Task<IDataReader> CreateDataReaderAsync(this IDbAccess dataAccess, SqlQuery query)
        {
            return CreateDataReaderAsync(dataAccess, query, CommandBehavior.Default);
        }

        public static Task<IDataReader> CreateDataReaderAsync(this IDbAccess dataAccess, string sql, params object[] pars)
        {
            return Task.Run<IDataReader>(() =>
            {
                if (null != dataAccess)
                    return dataAccess.CreateDataReader(sql, pars);
                return null;
            });
        }
        public static Task<IDataReader> CreateDataReaderAsync(this IDbAccess dataAccess, string sql)
        {
            return CreateDataReaderAsync(dataAccess, sql, null);
        }



        public static Task<int> ExecuteNonQueryAsync(this IDbAccess dataAccess, SqlQuery query)
        {
            return Task.Run<int>(() =>
            {
                if (null != dataAccess)
                    return dataAccess.ExecuteNonQuery(query);
                return 0;
            });
        }
        public static Task<int> ExecuteNonQueryAsync(this IDbAccess dataAccess, string sql, params object[] pars)
        {
            return Task.Run<int>(() =>
            {
                if (null != dataAccess)
                    return dataAccess.ExecuteNonQuery(sql, pars);
                return 0;
            });
        }
        public static Task<int> ExecuteNonQueryAsync(this IDbAccess dataAccess, string sql)
        {
            return ExecuteNonQueryAsync(dataAccess, sql, null);
        }



        public static Task<object> ExecuteScalarAsync(this IDbAccess dataAccess, SqlQuery query)
        {
            return Task.Run<object>(() =>
            {
                if (null != dataAccess)
                    return dataAccess.ExecuteScalar(query);
                return null;
            });
        }
        public static Task<object> ExecuteScalarAsync(this IDbAccess dataAccess, string sql, params object[] pars)
        {
            return Task.Run<object>(() =>
            {
                if (null != dataAccess)
                    return dataAccess.ExecuteScalar(sql, pars);
                return 0;
            });
        }
        public static Task<object> ExecuteScalarAsync(this IDbAccess dataAccess, string sql)
        {
            return ExecuteScalarAsync(dataAccess, sql, null);
        }



        public static Task<IEnumerable<object>> ExecuteScalarListAsync(this IDbAccess dataAccess, SqlQuery query)
        {
            return Task.Run<IEnumerable<object>>(() =>
            {
                if (null != dataAccess)
                    return dataAccess.ExecuteScalarList(query);
                return null;
            });
        }
        public static Task<IEnumerable<object>> ExecuteScalarListAsync(this IDbAccess dataAccess, string sql, params object[] pars)
        {
            return Task.Run<IEnumerable<object>>(() =>
            {
                if (null != dataAccess)
                    return dataAccess.ExecuteScalarList(sql, pars);
                return null;
            });
        }
        public static Task<IEnumerable<object>> ExecuteScalarListAsync(this IDbAccess dataAccess, string sql)
        {
            return ExecuteScalarListAsync(dataAccess, sql, null);
        }



        public static Task<T> ExecuteScalarAsync<T>(this IDbAccess dataAccess, SqlQuery query)
        {
            return Task.Run<T>(() =>
            {
                if (null != dataAccess)
                    return dataAccess.ExecuteScalar<T>(query);
                return default(T);
            });
        }
        public static Task<T> ExecuteScalarAsync<T>(this IDbAccess dataAccess, string sql, params object[] pars)
        {
            return Task.Run<T>(() =>
            {
                if (null != dataAccess)
                    return dataAccess.ExecuteScalar<T>(sql, pars);
                return default(T);
            });
        }
        public static Task<T> ExecuteScalarAsync<T>(this IDbAccess dataAccess, string sql)
        {
            return ExecuteScalarAsync<T>(dataAccess, sql, null);
        }

        public static Task<IEnumerable<T>> ExecuteScalarListAsync<T>(this IDbAccess dataAccess, SqlQuery query)
        {
            return Task.Run<IEnumerable<T>>(() =>
            {
                if (null != dataAccess)
                    return dataAccess.ExecuteScalarList<T>(query);
                return new List<T>();
            });
        }
        public static Task<IEnumerable<T>> ExecuteScalarListAsync<T>(this IDbAccess dataAccess, string sql, params object[] pars)
        {
            return Task.Run<IEnumerable<T>>(() =>
            {
                if (null != dataAccess)
                    return dataAccess.ExecuteScalarList<T>(sql, pars);
                return new List<T>();
            });
        }
        public static Task<IEnumerable<T>> ExecuteScalarListAsync<T>(this IDbAccess dataAccess, string sql)
        {
            return ExecuteScalarListAsync<T>(dataAccess, sql, null);
        }



        public static Task<IList<ExpandoObject>> QueryAsync(this IDbAccess dataAccess, SqlQuery query)
        {
            return Task.Run<IList<ExpandoObject>>(() =>
            {
                if (null != dataAccess)
                    return dataAccess.Query(query);
                return new List<ExpandoObject>();
            });
        }
        public static Task<IList<ExpandoObject>> QueryAsync(this IDbAccess dataAccess, string sql, params object[] pars)
        {
            return Task.Run<IList<ExpandoObject>>(() =>
            {
                if (null != dataAccess)
                    return dataAccess.Query(sql, pars);
                return new List<ExpandoObject>();
            });
        }
        public static Task<IList<ExpandoObject>> QueryAsync(this IDbAccess dataAccess, string sql)
        {
            return QueryAsync(dataAccess, sql, null);
        }



        public static Task<ExpandoObject> QuerySingleAsync(this IDbAccess dataAccess, SqlQuery query)
        {
            return Task.Run<ExpandoObject>(() =>
            {
                if (null != dataAccess)
                    return dataAccess.QuerySingle(query);
                return null;
            });
        }
        public static Task<ExpandoObject> QuerySingleAsync(this IDbAccess dataAccess, string sql, params object[] pars)
        {
            return Task.Run<ExpandoObject>(() =>
            {
                if (null != dataAccess)
                    return dataAccess.QuerySingle(sql, pars);
                return null;
            });
        }
        public static Task<ExpandoObject> QuerySingleAsync(this IDbAccess dataAccess, string sql)
        {
            return QuerySingleAsync(dataAccess, sql, null);
        }




        public static Task<DataTable> QueryDataTableAsync(this IDbAccess dataAccess, SqlQuery query)
        {
            return Task.Run<DataTable>(() =>
            {
                if (null != dataAccess)
                    return dataAccess.QueryDataTable(query);
                return null;
            });
        }
        public static Task<DataTable> QueryDataTableAsync(this IDbAccess dataAccess, string sql, params object[] pars)
        {
            return Task.Run<DataTable>(() =>
            {
                if (null != dataAccess)
                    return dataAccess.QueryDataTable(sql, pars);
                return null;
            });
        }
        public static Task<DataTable> QueryDataTableAsync(this IDbAccess dataAccess, string sql)
        {
            return QueryDataTableAsync(dataAccess, sql, null);
        }



        public static Task<int> ExecuteAsync<TEntity>(this FluentBaseExecutable<TEntity> fluent, IDbAccess dataAccess)
        {
            return Task.Run<int>(() =>
            {
                if (null != dataAccess)
                    return fluent.Execute(dataAccess);
                return 0;
            });
        }

    }
}
