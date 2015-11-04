namespace ionix.Data
{
    using Utils.Extensions;
    using Utils.Reflection;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    partial class CommandAdapterExtensions
    {
        #region   |   Select   |

        public static TEntity SelectSingle<TEntity>(this ICommandAdapter adapter)
            where TEntity : new()
        {
            if (null != adapter)
            {
                return adapter.SelectSingle<TEntity>(null);
            }
            return default(TEntity);
        }

        public static IList<TEntity> Select<TEntity>(this ICommandAdapter adapter)
            where TEntity : new()
        {
            if (null != adapter)
            {
                return adapter.Select<TEntity>(null);
            }
            return new List<TEntity>();
        }

        #endregion


        #region   |   Entity   |

        internal static string[] ToStringArray<TEntity>(Expression<Func<TEntity, object>>[] updatedFields)
        {
            string[] arr = null;
            if (!updatedFields.IsEmptyList())
            {
                arr = new string[updatedFields.Length];
                for (int j = 0; j < updatedFields.Length; ++j)
                {
                    Expression<Func<TEntity, object>> exp = updatedFields[j];
                    PropertyInfo pi = ReflectionExtensions.GetPropertyInfo(exp.Body);
                    arr[j] = pi.Name;
                }
            }
            return arr;
        }

        public static int Update<TEntity>(this ICommandAdapter adapter, TEntity entity)
        {
            if (null != adapter)
                return adapter.Update(entity, null);
            return 0;
        }

        public static int Update<TEntity>(this ICommandAdapter adapter, TEntity entity,
            params Expression<Func<TEntity, object>>[] updatedFields)
        {
            if (null != adapter)
            {
                string[] arr = ToStringArray(updatedFields);
                return adapter.Update(entity, arr);
            }
            return 0;
        }


        public static int Insert<TEntity>(this ICommandAdapter adapter, TEntity entity)
        {
            if (null != adapter)
            {
                return adapter.Insert(entity, null);
            }
            return 0;
        }

        public static int Insert<TEntity>(this ICommandAdapter adapter, TEntity entity,
            params Expression<Func<TEntity, object>>[] insertFields)
        {
            if (null != adapter)
            {
                string[] arr = ToStringArray(insertFields);
                return adapter.Insert(entity, arr);
            }

            return 0;
        }


        public static int Upsert<TEntity>(this ICommandAdapter adapter, TEntity entity)
        {
            if (null != adapter)
            {
                return adapter.Upsert(entity, null, null);
            }
            return 0;
        }

        public static int Upsert<TEntity>(this ICommandAdapter adapter, TEntity entity, string[] updatedFields)
        {
            if (null != adapter)
            {
                return adapter.Upsert(entity, updatedFields, null);
            }
            return 0;
        }

        public static int Upsert<TEntity>(this ICommandAdapter adapter, TEntity entity,
            Expression<Func<TEntity, object>>[] updatedFields)
        {
            if (null != adapter)
            {
                string[] arr = ToStringArray(updatedFields);
                return adapter.Upsert(entity, arr, null);
            }
            return 0;
        }

        public static int Upsert<TEntity>(this ICommandAdapter adapter, TEntity entity,
            Expression<Func<TEntity, object>>[] updatedFields, Expression<Func<TEntity, object>>[] insertFields)
        {
            if (null != adapter)
            {
                string[] updateArr = ToStringArray(updatedFields);
                string[] insertArr = ToStringArray(insertFields);
                return adapter.Upsert(entity, updateArr, insertArr);
            }
            return 0;
        }

        #endregion


        #region   |   Batch   |

        public static int BatchUpdate<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList)
        {
            if (null != adapter)
            {
                return adapter.BatchUpdate(entityList, BatchCommandMode.Batch, null);
            }
            return 0;
        }

        public static int BatchUpdate<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            BatchCommandMode mode)
        {
            if (null != adapter)
            {
                return adapter.BatchUpdate(entityList, mode, null);
            }
            return 0;
        }

        public static int BatchUpdate<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            params string[] updatedFields)
        {
            if (null != adapter)
            {
                return adapter.BatchUpdate(entityList, BatchCommandMode.Batch, updatedFields);
            }
            return 0;
        }

        public static int BatchUpdate<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            params Expression<Func<TEntity, object>>[] updatedFields)
        {
            if (null != adapter)
            {
                string[] updateArr = ToStringArray(updatedFields);
                return adapter.BatchUpdate(entityList, BatchCommandMode.Batch, updateArr);
            }
            return 0;
        }

        public static int BatchUpdate<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            BatchCommandMode mode, params Expression<Func<TEntity, object>>[] updatedFields)
        {
            if (null != adapter)
            {
                string[] updateArr = ToStringArray(updatedFields);
                return adapter.BatchUpdate(entityList, mode, updateArr);
            }
            return 0;
        }



        public static int BatchInsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList)
        {
            if (null != adapter)
            {
                return adapter.BatchInsert(entityList, BatchCommandMode.Batch, null);
            }
            return 0;
        }

        public static int BatchInsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            BatchCommandMode mode)
        {
            if (null != adapter)
            {
                return adapter.BatchInsert(entityList, mode, null);
            }
            return 0;
        }

        public static int BatchInsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            params string[] insertFields)
        {
            if (null != adapter)
            {
                return adapter.BatchInsert(entityList, BatchCommandMode.Batch, insertFields);
            }
            return 0;
        }

        public static int BatchInsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            params Expression<Func<TEntity, object>>[] insertFields)
        {
            if (null != adapter)
            {
                string[] insertArr = ToStringArray(insertFields);
                return adapter.BatchInsert(entityList, BatchCommandMode.Batch, insertArr);
            }
            return 0;
        }

        public static int BatchInsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            BatchCommandMode mode, params Expression<Func<TEntity, object>>[] insertFields)
        {
            if (null != adapter)
            {
                string[] insertArr = ToStringArray(insertFields);
                return adapter.BatchInsert(entityList, mode, insertArr);
            }
            return 0;
        }




        public static int BatchUpsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList)
        {
            if (null != adapter)
            {
                return adapter.BatchUpsert(entityList, BatchCommandMode.Batch, null, null);
            }
            return 0;
        }

        public static int BatchUpsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            BatchCommandMode mode)
        {
            if (null != adapter)
            {
                return adapter.BatchUpsert(entityList, mode, null, null);
            }
            return 0;
        }

        public static int BatchUpsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            string[] updatedFields)
        {
            if (null != adapter)
            {
                return adapter.BatchUpsert(entityList, BatchCommandMode.Batch, updatedFields, null);
            }
            return 0;
        }

        public static int BatchUpsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            BatchCommandMode mode, string[] updatedFields)
        {
            if (null != adapter)
            {
                return adapter.BatchUpsert(entityList, mode, updatedFields, null);
            }
            return 0;
        }

        public static int BatchUpsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            BatchCommandMode mode, Expression<Func<TEntity, object>>[] updatedFields)
        {
            if (null != adapter)
            {
                string[] updateArr = ToStringArray(updatedFields);
                return adapter.BatchUpsert(entityList, mode, updateArr, null);
            }
            return 0;
        }

        public static int BatchUpsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            Expression<Func<TEntity, object>>[] updatedFields)
        {
            if (null != adapter)
            {
                string[] updateArr = ToStringArray(updatedFields);
                return adapter.BatchUpsert(entityList, BatchCommandMode.Batch, updateArr, null);
            }
            return 0;
        }

        public static int BatchUpsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            BatchCommandMode mode
            , Expression<Func<TEntity, object>>[] updatedFields, params Expression<Func<TEntity, object>>[] insertFields)
        {
            if (null != adapter)
            {
                string[] updateArr = ToStringArray(updatedFields);
                string[] insertArr = ToStringArray(insertFields);
                return adapter.BatchUpsert(entityList, mode, updateArr, insertArr);
            }
            return 0;
        }

        public static int BatchUpsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            string[] updatedFields, string[] insertFields)
        {
            if (null != adapter)
            {
                return adapter.BatchUpsert(entityList, BatchCommandMode.Batch, updatedFields, insertFields);
            }
            return 0;
        }

        public static int BatchUpsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            Expression<Func<TEntity, object>>[] updatedFields, params Expression<Func<TEntity, object>>[] insertFields)
        {
            if (null != adapter)
            {
                string[] updateArr = ToStringArray(updatedFields);
                string[] insertArr = ToStringArray(insertFields);
                return adapter.BatchUpsert(entityList, BatchCommandMode.Batch, updateArr, insertArr);
            }
            return 0;

        }





        public static int Delsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            Func<FluentWhere<TEntity>, FluentWhere<TEntity>> where)
        {
            if (null != adapter)
            {
                return adapter.Delsert(entityList, BatchCommandMode.Batch, where, null);
            }
            return 0;
        }

        public static int Delsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            BatchCommandMode mode,
            Func<FluentWhere<TEntity>, FluentWhere<TEntity>> where)
        {
            if (null != adapter)
            {
                return adapter.Delsert(entityList, mode, where, null);
            }
            return 0;
        }

        public static int Delsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            Func<FluentWhere<TEntity>, FluentWhere<TEntity>> where
            , params string[] insertFields)
        {
            if (null != adapter)
            {
                return adapter.Delsert(entityList, BatchCommandMode.Batch, where, insertFields);
            }
            return 0;

        }

        #endregion


        #region   |   Fluent   |

        public static IEnumerable<TEntity> Select<TEntity>(this ICommandAdapter adapter, FluentWhere<TEntity> where)
            where TEntity : class, new()
        {
            if (null != adapter && null != where)
            {
                SqlQuery query = where.ToQuery();
                return adapter.Select<TEntity>(query);
            }
            return new List<TEntity>();
        }

        public static TEntity SelectSingle<TEntity>(this ICommandAdapter adapter, FluentWhere<TEntity> where)
            where TEntity : class, new()
        {
            if (null != adapter && null != where)
            {
                SqlQuery query = where.ToQuery();
                return adapter.SelectSingle<TEntity>(query);
            }
            return default(TEntity);
        }


        public static TEntity QuerySingle<TEntity>(this ICommandAdapter adapter, FluentSelect<TEntity> select)
            where TEntity : class, new()
        {
            if (null != adapter && null != select)
            {
                SqlQuery query = select.ToQuery();
                return adapter.QuerySingle<TEntity>(query);
            }
            return default(TEntity);
        }

        public static TEntity QuerySingle<TEntity>(this ICommandAdapter adapter, FluentWhere<TEntity> where)
            where TEntity : class, new()
        {
            if (null != adapter && null != where)
            {
                FluentSelect<TEntity> select = new FluentSelect<TEntity>(where.ParameterPrefix);
                select.SelectAll().Where(where);
                return adapter.QuerySingle<TEntity>(select.ToQuery());
            }
            return default(TEntity);
        }

        public static IEnumerable<TEntity> Query<TEntity>(this ICommandAdapter adapter, FluentSelect<TEntity> select)
            where TEntity : class, new()
        {
            if (null != adapter && null != select)
            {
                SqlQuery query = select.ToQuery();
                return adapter.Query<TEntity>(query);
            }
            return new List<TEntity>();
        }

        public static IEnumerable<TEntity> Query<TEntity>(this ICommandAdapter adapter, FluentWhere<TEntity> where)
            where TEntity : class, new()
        {
            if (null != adapter && null != where)
            {
                FluentSelect<TEntity> select = new FluentSelect<TEntity>(where.ParameterPrefix);
                select.SelectAll().Where(where);
                return adapter.Query<TEntity>(select.ToQuery());
            }
            return new List<TEntity>();
        }

        #endregion
    }
}
