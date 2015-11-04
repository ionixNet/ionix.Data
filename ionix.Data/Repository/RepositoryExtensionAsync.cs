namespace ionix.Data
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;


    public static class RepositoryExtensionAsync
    {
        #region   |   Select   |

        public static Task<TEntity> SelectByIdAsync<TEntity>(this IRepository<TEntity> rep, params object[] keyValues)
            where TEntity : class, new()
        {
            return Task.Run(() =>
            {
                if (null != rep && !keyValues.IsEmptyList())
                    return rep.SelectById(keyValues);

                return null;
            });
        }

        public static Task<IList<TEntity>> SelectAsync<TEntity>(this IRepository<TEntity> rep, SqlQuery extendedQuery)
            where TEntity : class, new()
        {
            return Task.Run(() =>
            {
                if (null != rep)
                    return rep.Select(extendedQuery);

                return null;
            });
        }

        public static Task<IList<TEntity>> SelectAsync<TEntity>(this IRepository<TEntity> rep, FluentWhere<TEntity> where)
             where TEntity : class, new()
        {
            SqlQuery query = null != where ? where.ToQuery() : null;
            return SelectAsync(rep, query);
        }

        public static Task<IList<TEntity>> SelectAsync<TEntity>(this IRepository<TEntity> rep)
            where TEntity : class, new()
        {
            return SelectAsync(rep, (SqlQuery)null);
        }

        public static Task<TEntity> SelectSingleAsync<TEntity>(this IRepository<TEntity> rep, SqlQuery extendedQuery)
            where TEntity : class, new()
        {
            return Task.Run<TEntity>(() =>
            {
                if (null != rep)
                    return rep.SelectSingle(extendedQuery);

                return null;
            });
        }

        public static Task<TEntity> SelectSingleAsync<TEntity>(this IRepository<TEntity> rep, FluentWhere<TEntity> where)
            where TEntity : class, new()
        {
            SqlQuery query = null != where ? where.ToQuery() : null;
            return SelectSingleAsync(rep, query);
        }

        public static Task<TEntity> SelectSingleAsync<TEntity>(this IRepository<TEntity> rep)
             where TEntity : class, new()
        {
            return SelectSingleAsync(rep, (SqlQuery)null);
        }

        public static Task<IList<TEntity>> QueryAsync<TEntity>(this IRepository<TEntity> rep, SqlQuery query)
            where TEntity : class, new()
        {
            return Task.Run(() =>
            {
                if (null != rep)
                    return rep.Query(query);

                return null;
            });
        }

        public static Task<TEntity> QuerySingleAsync<TEntity>(this IRepository<TEntity> rep, SqlQuery query)
            where TEntity : class, new()
        {
            return Task.Run(() =>
            {
                if (null != rep)
                    return rep.QuerySingle(query);

                return null;
            });
        }

        public static Task<IList<TEntity>> QueryAsync<TEntity>(this IRepository<TEntity> rep, FluentWhere<TEntity> where)
            where TEntity : class, new()
        {
            return Task.Run(() =>
            {
                if (null != rep)
                    return rep.Query(where);

                return null;
            });
        }

        public static Task<TEntity> QuerySingleAsync<TEntity>(this IRepository<TEntity> rep, FluentWhere<TEntity> where)
            where TEntity : class, new()
        {
            return Task.Run(() =>
            {
                if (null != rep)
                    return rep.QuerySingle(where);

                return null;
            });
        }

        public static Task<IList<TEntity>> QueryAsync<TEntity>(this IRepository<TEntity> rep, FluentSelect<TEntity> select)
             where TEntity : class, new()
        {
            return Task.Run(() =>
            {
                if (null != rep)
                    return rep.Query(select);

                return null;
            });
        }

        public static Task<TEntity> QuerySingleAsync<TEntity>(this IRepository<TEntity> rep, FluentSelect<TEntity> select)
            where TEntity : class, new()
        {
            return Task.Run(() =>
            {
                if (null != rep)
                    return rep.QuerySingle(select);

                return null;
            });
        }

        #endregion


        #region   |   Entity   |

        public static Task<int> UpdateAsync<TEntity>(this IRepository<TEntity> rep, TEntity entity,
            params string[] updatedFields)
            where TEntity : class, new()
        {
            return Task.Run(() =>
            {
                if (null != rep)
                    return rep.Update(entity, updatedFields);
                return 0;
            });
        }

        public static Task<int> UpdateAsync<TEntity>(this IRepository<TEntity> rep, TEntity entity,
            params Expression<Func<TEntity, object>>[] updatedFields)
            where TEntity : class, new()
        {
            string[] arr = CommandAdapterExtensions.ToStringArray(updatedFields);
            return UpdateAsync(rep, entity, arr);
        }

        public static Task<int> UpdateAsync<TEntity>(this IRepository<TEntity> rep, TEntity entity)
            where TEntity : class, new()
        {
            return UpdateAsync<TEntity>(rep, entity, (string[])null);
        }



        public static Task<int> InsertAsync<TEntity>(this IRepository<TEntity> rep, TEntity entity,
            params string[] insertFields)
            where TEntity : class, new()
        {
            return Task.Run(() =>
            {
                if (null != rep)
                    return rep.Insert(entity, insertFields);
                return 0;
            });
        }

        public static Task<int> InsertAsync<TEntity>(this IRepository<TEntity> rep, TEntity entity,
            params Expression<Func<TEntity, object>>[] insertFields)
            where TEntity : class, new()
        {
            string[] arr = CommandAdapterExtensions.ToStringArray(insertFields);
            return InsertAsync(rep, entity, arr);
        }

        public static Task<int> InsertAsync<TEntity>(this IRepository<TEntity> rep, TEntity entity)
            where TEntity : class, new()
        {
            return InsertAsync(rep, entity, (string[])null);
        }



        public static Task<int> UpsertAsync<TEntity>(this IRepository<TEntity> rep, TEntity entity,
            string[] updatedFields, string[] insertFields)
            where TEntity : class, new()
        {
            return Task.Run(() =>
            {
                if (null != rep)
                    return rep.Upsert(entity, updatedFields, insertFields);
                return 0;
            });
        }

        public static Task<int> UpsertAsync<TEntity>(this IRepository<TEntity> rep, TEntity entity,
            string[] updatedFields)
            where TEntity : class, new()
        {
            return UpsertAsync(rep, entity, updatedFields, null);

        }

        public static Task<int> UpsertAsync<TEntity>(this IRepository<TEntity> rep, TEntity entity)
            where TEntity : class, new()
        {
            return UpsertAsync(rep, entity, (string[])null, (string[])null);

        }

        public static Task<int> UpsertAsync<TEntity>(this IRepository<TEntity> rep, TEntity entity,
            Expression<Func<TEntity, object>>[] updatedFields)
            where TEntity : class, new()
        {
            string[] arr = CommandAdapterExtensions.ToStringArray(updatedFields);
            return UpsertAsync(rep, entity, arr, null);
        }

        public static Task<int> UpsertAsync<TEntity>(this IRepository<TEntity> rep, TEntity entity,
            Expression<Func<TEntity, object>>[] updatedFields, Expression<Func<TEntity, object>>[] insertFields)
            where TEntity : class, new()
        {
            string[] updateArr = CommandAdapterExtensions.ToStringArray(updatedFields);
            string[] insertArr = CommandAdapterExtensions.ToStringArray(insertFields);

            return UpsertAsync(rep, entity, updateArr, insertArr);
        }


        public static Task<int> DeleteAsync<TEntity>(this IRepository<TEntity> rep, TEntity entity)
            where TEntity : class, new()
        {
            return Task.Run(() =>
            {
                if (null != rep)
                    return rep.Delete(entity);
                return 0;
            });
        }

        #endregion


        #region   |   Batch   |

        //Default
        public static Task<int> BatchUpdateAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            BatchCommandMode mode, params string[] updatedFields)
            where TEntity : class, new()
        {
            return Task.Run(() =>
            {
                if (null != rep)
                    return rep.BatchUpdate(entityList, mode, updatedFields);
                return 0;
            });
        }

        public static Task<int> BatchUpdateAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList)
            where TEntity : class, new()
        {
            return BatchUpdateAsync(rep, entityList, BatchCommandMode.Batch, (string[])null);
        }

        public static Task<int> BatchUpdateAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            BatchCommandMode mode)
            where TEntity : class, new()
        {
            return BatchUpdateAsync(rep, entityList, mode, (string[])null);
        }

        public static Task<int> BatchUpdateAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            params string[] updatedFields)
            where TEntity : class, new()
        {
            return BatchUpdateAsync(rep, entityList, BatchCommandMode.Batch, updatedFields);
        }

        public static Task<int> BatchUpdateAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            params Expression<Func<TEntity, object>>[] updatedFields)
            where TEntity : class, new()
        {
            return BatchUpdateAsync(rep, entityList, BatchCommandMode.Batch, updatedFields);
        }

        public static Task<int> BatchUpdateAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            BatchCommandMode mode, params Expression<Func<TEntity, object>>[] updatedFields)
            where TEntity : class, new()
        {
            string[] arr = CommandAdapterExtensions.ToStringArray(updatedFields);
            return BatchUpdateAsync(rep, entityList, mode, arr);
        }



        //Default
        public static Task<int> BatchInsertAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            BatchCommandMode mode, params string[] insertFields)
            where TEntity : class, new()
        {
            return Task.Run(() =>
            {
                if (null != rep)
                    return rep.BatchInsert(entityList, mode, insertFields);
                return 0;
            });
        }

        public static Task<int> BatchInsertAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList)
            where TEntity : class, new()
        {
            return BatchInsertAsync(rep, entityList, BatchCommandMode.Batch, (string[])null);
        }

        public static Task<int> BatchInsertAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            BatchCommandMode mode)
            where TEntity : class, new()
        {
            return BatchInsertAsync(rep, entityList, mode, (string[])null);
        }

        public static Task<int> BatchInsertAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            params string[] insertFields)
            where TEntity : class, new()
        {
            return BatchInsertAsync(rep, entityList, BatchCommandMode.Batch, insertFields);
        }

        public static Task<int> BatchInsertAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            params Expression<Func<TEntity, object>>[] insertFields)
            where TEntity : class, new()
        {
            return BatchInsertAsync(rep, entityList, BatchCommandMode.Batch, insertFields);
        }

        public static Task<int> BatchInsertAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            BatchCommandMode mode, params Expression<Func<TEntity, object>>[] insertFields)
            where TEntity : class, new()
        {
            string[] arr = CommandAdapterExtensions.ToStringArray(insertFields);
            return BatchInsertAsync(rep, entityList, mode, arr);
        }




        //Default
        public static Task<int> BatchUpsertAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            BatchCommandMode mode, string[] updatedFields, string[] insertFields)
            where TEntity : class, new()
        {
            return Task.Run(() =>
            {
                if (null != rep)
                    return rep.BatchUpsert(entityList, mode, updatedFields, insertFields);
                return 0;
            });
        }

        public static Task<int> BatchUpsertAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList)
            where TEntity : class, new()
        {
            return BatchUpsertAsync(rep, entityList, BatchCommandMode.Batch, (string[])null, (string[])null);
        }

        public static Task<int> BatchUpsertAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            BatchCommandMode mode)
            where TEntity : class, new()
        {
            return BatchUpsertAsync(rep, entityList, mode, (string[])null, (string[])null);
        }

        public static Task<int> BatchUpsertAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            string[] updatedFields)
            where TEntity : class, new()
        {
            return BatchUpsertAsync(rep, entityList, BatchCommandMode.Batch, updatedFields, null);
        }

        public static Task<int> BatchUpsertAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            BatchCommandMode mode, string[] updatedFields)
            where TEntity : class, new()
        {
            return BatchUpsertAsync(rep, entityList, mode, updatedFields, null);
        }

        public static Task<int> BatchUpsertAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            BatchCommandMode mode, Expression<Func<TEntity, object>>[] updatedFields)
            where TEntity : class, new()
        {
            string[] arr = CommandAdapterExtensions.ToStringArray(updatedFields);
            return BatchUpsertAsync(rep, entityList, mode, arr, null);
        }

        public static Task<int> BatchUpsertAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            Expression<Func<TEntity, object>>[] updatedFields)
            where TEntity : class, new()
        {
            return BatchUpsertAsync(rep, entityList, BatchCommandMode.Batch, updatedFields);
        }

        public static Task<int> BatchUpsertAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            BatchCommandMode mode
            , Expression<Func<TEntity, object>>[] updatedFields, Expression<Func<TEntity, object>>[] insertFields)
            where TEntity : class, new()
        {
            string[] updateArr = CommandAdapterExtensions.ToStringArray(updatedFields);
            string[] insertArr = CommandAdapterExtensions.ToStringArray(insertFields);

            return BatchUpsertAsync(rep, entityList, mode, updateArr, insertArr);
        }

        public static Task<int> BatchUpsertAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            string[] updatedFields, string[] insertFields)
            where TEntity : class, new()
        {
            return BatchUpsertAsync(rep, entityList, BatchCommandMode.Batch, updatedFields, insertFields);
        }

        public static Task<int> BatchUpsertAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList
            , Expression<Func<TEntity, object>>[] updatedFields, Expression<Func<TEntity, object>>[] insertFields)
            where TEntity : class, new()
        {
            string[] updateArr = CommandAdapterExtensions.ToStringArray(updatedFields);
            string[] insertArr = CommandAdapterExtensions.ToStringArray(insertFields);

            return BatchUpsertAsync(rep, entityList, BatchCommandMode.Batch, updateArr, insertArr);
        }




        //default
        public static Task<int> DelsertAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            BatchCommandMode mode, Func<FluentWhere<TEntity>, FluentWhere<TEntity>> where, params string[] insertFields)
            where TEntity : class, new()
        {
            return Task.Run<int>(() =>
            {
                if (null != rep)
                    return rep.Delsert(entityList, mode, where, insertFields);
                return 0;
            });
        }

        public static Task<int> DelsertAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            Func<FluentWhere<TEntity>, FluentWhere<TEntity>> where)
            where TEntity : class, new()
        {
            return DelsertAsync(rep, entityList, BatchCommandMode.Batch, where, null);
        }

        public static Task<int> DelsertAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            BatchCommandMode mode, Func<FluentWhere<TEntity>, FluentWhere<TEntity>> where)
            where TEntity : class, new()
        {
            return DelsertAsync(rep, entityList, mode, where, null);
        }

        public static Task<int> DelsertAsync<TEntity>(this IRepository<TEntity> rep, IEnumerable<TEntity> entityList,
            Func<FluentWhere<TEntity>, FluentWhere<TEntity>> where, params string[] insertFields)
            where TEntity : class, new()
        {
            return DelsertAsync(rep, entityList, BatchCommandMode.Batch, where, insertFields);
        }

        #endregion
    }
}
