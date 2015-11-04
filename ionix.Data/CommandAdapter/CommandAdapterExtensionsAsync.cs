namespace ionix.Data
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public static class CommandAdapterExtensionsAsync
    {

        #region   |   Select   |

        public static Task<TEntity> SelectByIdAsync<TEntity>(this ICommandAdapter adapter, params object[] keyValues)
            where TEntity : class, new()
        {
            return Task.Run<TEntity>(() =>
            {
                if (null != adapter && !keyValues.IsEmptyList())
                    return adapter.SelectById<TEntity>(keyValues);
                else
                    return null;
            });
        }

        public static Task<IList<TEntity>> SelectAsync<TEntity>(this ICommandAdapter adapter, SqlQuery extendedQuery)
            where TEntity : class, new()
        {
            return Task.Run<IList<TEntity>>(() =>
            {
                if (null != adapter)
                    return adapter.Select<TEntity>(extendedQuery);
                else
                    return null;
            });
        }
        public static Task<IList<TEntity>> SelectAsync<TEntity>(this ICommandAdapter adapter)
            where TEntity : class, new()
        {
            return SelectAsync<TEntity>(adapter, (SqlQuery)null);
        }

        public static Task<TEntity> SelectSingleAsync<TEntity>(this ICommandAdapter adapter, SqlQuery extendedQuery)
            where TEntity : class, new()
        {
            return Task.Run<TEntity>(() =>
            {
                if (null != adapter)
                    return adapter.SelectSingle<TEntity>(extendedQuery);
                else
                    return null;
            });
        }
        public static Task<TEntity> SelectSingleAsync<TEntity>(this ICommandAdapter adapter)
            where TEntity : class, new()
        {
            return SelectSingleAsync<TEntity>(adapter, (SqlQuery)null);
        }

        public static Task<IList<TEntity>> QueryAsync<TEntity>(this ICommandAdapter adapter, SqlQuery query)
            where TEntity : class, new()
        {
            return Task.Run<IList<TEntity>>(() =>
            {
                if (null != adapter)
                    return adapter.Query<TEntity>(query);
                else
                    return null;
            });
        }
        public static Task<TEntity> QuerySingleAsync<TEntity>(this ICommandAdapter adapter, SqlQuery query)
            where TEntity : class, new()
        {
            return Task.Run<TEntity>(() =>
            {
                if (null != adapter)
                    return adapter.QuerySingle<TEntity>(query);
                else
                    return null;
            });
        }

        #endregion


        #region   |   Entity   |

        public static Task<int> UpdateAsync<TEntity>(this ICommandAdapter adapter, TEntity entity, params string[] updatedFields)
        {
            return Task.Run<int>(() =>
            {
                if (null != adapter)
                    return adapter.Update(entity, updatedFields);
                return 0;
            });
        }
        public static Task<int> UpdateAsync<TEntity>(this ICommandAdapter adapter, TEntity entity, params Expression<Func<TEntity, object>>[] updatedFields)
        {
            string[] arr = CommandAdapterExtensions.ToStringArray(updatedFields);
            return UpdateAsync(adapter, entity, arr);
        }
        public static Task<int> UpdateAsync<TEntity>(this ICommandAdapter adapter, TEntity entity)
        {
            return UpdateAsync<TEntity>(adapter, entity, (string[])null);
        }


        public static Task<int> InsertAsync<TEntity>(this ICommandAdapter adapter, TEntity entity, params string[] insertFields)
        {
            return Task.Run<int>(() =>
            {
                if (null != adapter)
                    return adapter.Insert(entity, insertFields);
                return 0;
            });
        }
        public static Task<int> InsertAsync<TEntity>(this ICommandAdapter adapter, TEntity entity, params Expression<Func<TEntity, object>>[] insertFields)
        {
            string[] arr = CommandAdapterExtensions.ToStringArray(insertFields);
            return InsertAsync(adapter, entity, arr);
        }
        public static Task<int> InsertAsync<TEntity>(this ICommandAdapter adapter, TEntity entity)
        {
            return InsertAsync(adapter, entity, (string[])null);
        }



        public static Task<int> UpsertAsync<TEntity>(this ICommandAdapter adapter, TEntity entity, string[] updatedFields, string[] insertFields)
        {
            return Task.Run<int>(() =>
            {
                if (null != adapter)
                    return adapter.Upsert(entity, updatedFields, insertFields);
                return 0;
            });
        }
        public static Task<int> UpsertAsync<TEntity>(this ICommandAdapter adapter, TEntity entity, string[] updatedFields)
        {
            return UpsertAsync(adapter, entity, updatedFields, null);

        }
        public static Task<int> UpsertAsync<TEntity>(this ICommandAdapter adapter, TEntity entity)
        {
            return UpsertAsync(adapter, entity, (string[])null, (string[])null);

        }
        public static Task<int> UpsertAsync<TEntity>(this ICommandAdapter adapter, TEntity entity, Expression<Func<TEntity, object>>[] updatedFields)
        {
            string[] arr = CommandAdapterExtensions.ToStringArray(updatedFields);
            return UpsertAsync(adapter, entity, arr, null);
        }
        public static Task<int> UpsertAsync<TEntity>(this ICommandAdapter adapter, TEntity entity, Expression<Func<TEntity, object>>[] updatedFields, Expression<Func<TEntity, object>>[] insertFields)
        {
            string[] updateArr = CommandAdapterExtensions.ToStringArray(updatedFields);
            string[] insertArr = CommandAdapterExtensions.ToStringArray(insertFields);

            return UpsertAsync(adapter, entity, updateArr, insertArr);
        }


        public static Task<int> DeleteAsync<TEntity>(this ICommandAdapter adapter, TEntity entity)
        {
            return Task.Run<int>(() =>
            {
                if (null != adapter)
                    return adapter.Delete(entity);
                return 0;
            });
        }
        #endregion


        #region   |   Batch   |

        //Default
        public static Task<int> BatchUpdateAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, BatchCommandMode mode, params string[] updatedFields)
        {
            return Task.Run<int>(() =>
            {
                if (null != adapter)
                    return adapter.BatchUpdate(entityList, mode, updatedFields);
                return 0;
            });
        }
        public static Task<int> BatchUpdateAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList)
        {
            return BatchUpdateAsync(adapter, entityList, BatchCommandMode.Batch, (string[])null);
        }
        public static Task<int> BatchUpdateAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, BatchCommandMode mode)
        {
            return BatchUpdateAsync(adapter, entityList, mode, (string[])null);
        }
        public static Task<int> BatchUpdateAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, params string[] updatedFields)
        {
            return BatchUpdateAsync(adapter, entityList, BatchCommandMode.Batch, updatedFields);
        }
        public static Task<int> BatchUpdateAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, params Expression<Func<TEntity, object>>[] updatedFields)
        {
            return BatchUpdateAsync(adapter, entityList, BatchCommandMode.Batch, updatedFields);
        }
        public static Task<int> BatchUpdateAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, BatchCommandMode mode, params Expression<Func<TEntity, object>>[] updatedFields)
        {
            string[] arr = CommandAdapterExtensions.ToStringArray(updatedFields);
            return BatchUpdateAsync(adapter, entityList, mode, arr);
        }



        //Default
        public static Task<int> BatchInsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, BatchCommandMode mode, params string[] insertFields)
        {
            return Task.Run<int>(() =>
            {
                if (null != adapter)
                    return adapter.BatchInsert(entityList, mode, insertFields);
                return 0;
            });
        }
        public static Task<int> BatchInsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList)
        {
            return BatchInsertAsync(adapter, entityList, BatchCommandMode.Batch, (string[])null);
        }
        public static Task<int> BatchInsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, BatchCommandMode mode)
        {
            return BatchInsertAsync(adapter, entityList, mode, (string[])null);
        }
        public static Task<int> BatchInsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, params string[] insertFields)
        {
            return BatchInsertAsync(adapter, entityList, BatchCommandMode.Batch, insertFields);
        }
        public static Task<int> BatchInsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, params Expression<Func<TEntity, object>>[] insertFields)
        {
            return BatchInsertAsync(adapter, entityList, BatchCommandMode.Batch, insertFields);
        }
        public static Task<int> BatchInsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, BatchCommandMode mode, params Expression<Func<TEntity, object>>[] insertFields)
        {
            string[] arr = CommandAdapterExtensions.ToStringArray(insertFields);
            return BatchInsertAsync(adapter, entityList, mode, arr);
        }




        //Default
        public static Task<int> BatchUpsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, BatchCommandMode mode, string[] updatedFields, string[] insertFields)
        {
            return Task.Run<int>(() =>
            {
                if (null != adapter)
                    return adapter.BatchUpsert(entityList, mode, updatedFields, insertFields);
                return 0;
            });
        }

        public static Task<int> BatchUpsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList)
        {
            return BatchUpsertAsync(adapter, entityList, BatchCommandMode.Batch, (string[])null, (string[])null);
        }
        public static Task<int> BatchUpsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, BatchCommandMode mode)
        {
            return BatchUpsertAsync(adapter, entityList, mode, (string[])null, (string[])null);
        }
        public static Task<int> BatchUpsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, string[] updatedFields)
        {
            return BatchUpsertAsync(adapter, entityList, BatchCommandMode.Batch, updatedFields, null);
        }
        public static Task<int> BatchUpsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, BatchCommandMode mode, string[] updatedFields)
        {
            return BatchUpsertAsync(adapter, entityList, mode, updatedFields, null);
        }
        public static Task<int> BatchUpsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, BatchCommandMode mode, Expression<Func<TEntity, object>>[] updatedFields)
        {
            string[] arr = CommandAdapterExtensions.ToStringArray(updatedFields);
            return BatchUpsertAsync(adapter, entityList, mode, arr, null);
        }
        public static Task<int> BatchUpsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, Expression<Func<TEntity, object>>[] updatedFields)
        {
            return BatchUpsertAsync(adapter, entityList, BatchCommandMode.Batch, updatedFields);
        }
        public static Task<int> BatchUpsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, BatchCommandMode mode
            , Expression<Func<TEntity, object>>[] updatedFields, Expression<Func<TEntity, object>>[] insertFields)
        {
            string[] updateArr = CommandAdapterExtensions.ToStringArray(updatedFields);
            string[] insertArr = CommandAdapterExtensions.ToStringArray(insertFields);

            return BatchUpsertAsync(adapter, entityList, mode, updateArr, insertArr);
        }
        public static Task<int> BatchUpsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, string[] updatedFields, string[] insertFields)
        {
            return BatchUpsertAsync(adapter, entityList, BatchCommandMode.Batch, updatedFields, insertFields);
        }
        public static Task<int> BatchUpsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList
            , Expression<Func<TEntity, object>>[] updatedFields, Expression<Func<TEntity, object>>[] insertFields)
        {
            string[] updateArr = CommandAdapterExtensions.ToStringArray(updatedFields);
            string[] insertArr = CommandAdapterExtensions.ToStringArray(insertFields);

            return BatchUpsertAsync(adapter, entityList, BatchCommandMode.Batch, updateArr, insertArr);
        }




        //default
        public static Task<int> DelsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, BatchCommandMode mode, Func<FluentWhere<TEntity>, FluentWhere<TEntity>> where
            , params string[] insertFields)
        {
            return Task.Run<int>(() =>
            {
                if (null != adapter)
                    return adapter.Delsert(entityList, mode, where, insertFields);
                return 0;
            });
        }

        public static Task<int> DelsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, Func<FluentWhere<TEntity>, FluentWhere<TEntity>> where)
        {
            return DelsertAsync(adapter, entityList, BatchCommandMode.Batch, where, null);
        }
        public static Task<int> DelsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, BatchCommandMode mode, Func<FluentWhere<TEntity>, FluentWhere<TEntity>> where)
        {
            return DelsertAsync(adapter, entityList, mode, where, null);
        }
        public static Task<int> DelsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList, Func<FluentWhere<TEntity>, FluentWhere<TEntity>> where, params string[] insertFields)
        {
            return DelsertAsync(adapter, entityList, BatchCommandMode.Batch, where, insertFields);
        }

        #endregion


        #region   |   Fluent   |


        public static Task<IEnumerable<TEntity>> SelectAsync<TEntity>(this ICommandAdapter adapter, FluentWhere<TEntity> where)
            where TEntity : class, new()
        {
            return Task.Run(() => adapter?.Select(where));
        }

        public static Task<TEntity> SelectSingleAsync<TEntity>(this ICommandAdapter adapter, FluentWhere<TEntity> where)
            where TEntity : class, new()
        {
            return Task.Run(() => adapter?.SelectSingle(where));
        }

        public static Task<TEntity> QuerySingleAsync<TEntity>(this ICommandAdapter adapter, FluentSelect<TEntity> select)
           where TEntity : class, new()
        {
            return Task.Run<TEntity>(() =>
            {
                if (null != adapter)
                    return adapter.QuerySingle<TEntity>(select);
                else
                    return null;
            });
        }

        public static Task<TEntity> QuerySingleAsync<TEntity>(this ICommandAdapter adapter, FluentWhere<TEntity> where)
            where TEntity : class, new()
        {
            return Task.Run<TEntity>(() =>
            {
                if (null != adapter)
                    return adapter.QuerySingle<TEntity>(where);
                else
                    return null;
            });
        }

        public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this ICommandAdapter adapter, FluentSelect<TEntity> select)
            where TEntity : class, new()
        {
            return Task.Run<IEnumerable<TEntity>>(() =>
            {
                if (null != adapter)
                    return adapter.Query<TEntity>(select);
                else
                    return null;
            });
        }

        public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this ICommandAdapter adapter, FluentWhere<TEntity> where)
            where TEntity : class, new()
        {
            return Task.Run<IEnumerable<TEntity>>(() =>
            {
                if (null != adapter)
                    return adapter.Query<TEntity>(where);
                else
                    return null;
            });
        }

        #endregion
    }
}

