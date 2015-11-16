namespace ionix.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public static class RepositoryExtension
    {
        //Select
        public static IList<TEntity> Select<TEntity>(this IRepository<TEntity> repository)
            where TEntity : class, new()
        {
            if (null != repository)
                return repository.Select(null);
            return new List<TEntity>();
        }

        public static IList<TEntity> Select<TEntity>(this IRepository<TEntity> repository, FluentWhere<TEntity> where)
           where TEntity : class, new()
        {
            if (null != repository)
            {
                SqlQuery query = where != null ? where.ToQuery() : null;
                return repository.Select(query);
            }
            return new List<TEntity>();
        }

        public static TEntity SelectSingle<TEntity>(this IRepository<TEntity> repository, FluentWhere<TEntity> where)
             where TEntity : class, new()
        {
            if (null != repository)
            {
                SqlQuery query = where != null ? where.ToQuery() : null;
                return repository.SelectSingle(query);
            }
            return null;
        }

        public static TEntity SelectSingle<TEntity>(this IRepository<TEntity> repository)
             where TEntity : class, new()
        {
            if (null != repository)
                return repository.SelectSingle(null);
            return null;
        }

        public static IList<TEntity> Query<TEntity>(this IRepository<TEntity> repository, FluentWhere<TEntity> where)
            where TEntity : class, new()
        {
            if (null != repository && null != where)
            {
                FluentSelect<TEntity> select = new FluentSelect<TEntity>(where.ParameterPrefix);
                select.AllColumns().Where(where);
                return repository.Query(select.ToQuery());
            }
            return new List<TEntity>();
        }

        public static TEntity QuerySingle<TEntity>(this IRepository<TEntity> repository, FluentWhere<TEntity> where)
            where TEntity : class, new()
        {
            if (null != repository && null != where)
            {
                FluentSelect<TEntity> select = new FluentSelect<TEntity>(where.ParameterPrefix);
                select.AllColumns().Where(where);
                return repository.QuerySingle(select.ToQuery());
            }
            return null;
        }

        public static IList<TEntity> Query<TEntity>(this IRepository<TEntity> repository, FluentSelect<TEntity> select)
            where TEntity : class, new()
        {
            if (null != repository && null != select)
            {
                return repository.Query(select.ToQuery());
            }
            return new List<TEntity>();
        }

        public static TEntity QuerySingle<TEntity>(this IRepository<TEntity> repository, FluentSelect<TEntity> select)
             where TEntity : class, new()
        {
            if (null != repository && null != select)
            {
                return repository.QuerySingle(select.ToQuery());
            }
            return null;
        }
        //

        //Entity
        public static int Update<TEntity>(this IRepository<TEntity> repository, TEntity entity, params Expression<Func<TEntity, object>>[] updatedFields)
            where TEntity : class, new()
        {
            if (null != repository)
            {
                string[] arr = CommandAdapterExtensions.ToStringArray(updatedFields);// already checked null value.
                return repository.Update(entity, arr);
            }
            return 0;
        }
        public static int Update<TEntity>(this IRepository<TEntity> repository, TEntity entity)
            where TEntity : class, new()
        {
            return RepositoryExtension.Update(repository, entity, null);
        }




        public static int Insert<TEntity>(this IRepository<TEntity> repository, TEntity entity, params Expression<Func<TEntity, object>>[] insertFields)
            where TEntity : class, new()
        {
            if (null != repository)
            {
                string[] arr = CommandAdapterExtensions.ToStringArray(insertFields);
                return repository.Insert(entity, arr);
            }
            return 0;
        }
        public static int Insert<TEntity>(this IRepository<TEntity> repository, TEntity entity)
            where TEntity : class, new()
        {
            return RepositoryExtension.Insert(repository, entity, null);
        }



        public static int Upsert<TEntity>(this IRepository<TEntity> repository, TEntity entity, params string[] updatedFields)
            where TEntity : class, new()
        {
            if (null != repository)
            {
                return repository.Upsert(entity, updatedFields, null);
            }
            return 0;
        }
        public static int Upsert<TEntity>(this IRepository<TEntity> repository, TEntity entity)
            where TEntity : class, new()
        {
            return RepositoryExtension.Upsert(repository, entity, (string[])null);
        }

        public static int Upsert<TEntity>(this IRepository<TEntity> repository, TEntity entity, params Expression<Func<TEntity, object>>[] updatedFields)
            where TEntity : class, new()
        {
            return RepositoryExtension.Upsert(repository, entity, updatedFields, (Expression<Func<TEntity, object>>[])null);
        }

        public static int Upsert<TEntity>(this IRepository<TEntity> repository, TEntity entity, Expression<Func<TEntity, object>>[] updatedFields, Expression<Func<TEntity, object>>[] insertFields)
            where TEntity : class, new()
        {
            if (null != repository)
            {
                string[] updateArr = CommandAdapterExtensions.ToStringArray(updatedFields);
                string[] insertArr = CommandAdapterExtensions.ToStringArray(insertFields);

                return repository.Upsert(entity, updateArr, insertArr);
            }
            return 0;
        }

        //


        //Batch
        private static int BatchUpdate<TEntity>(IRepository<TEntity> repository, IEnumerable<TEntity> entityList, BatchCommandMode mode, string[] updatedFields)
            where TEntity : class, new()
        {
            if (null != repository)
            {
                return repository.BatchUpdate(entityList, mode, updatedFields);
            }
            return 0;
        }
        public static int BatchUpdate<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, BatchCommandMode mode)
            where TEntity : class, new()
        {
            return RepositoryExtension.BatchUpdate(repository, entityList, mode, (string[])null);
        }
        public static int BatchUpdate<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList)
            where TEntity : class, new()
        {
            return RepositoryExtension.BatchUpdate(repository, entityList, BatchCommandMode.Batch, (string[])null);
        }

        public static int BatchUpdate<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, BatchCommandMode mode, params Expression<Func<TEntity, object>>[] updatedFields)
            where TEntity : class, new()
        {
            string[] arr = CommandAdapterExtensions.ToStringArray(updatedFields);
            return RepositoryExtension.BatchUpdate(repository, entityList, mode, arr);
        }

        public static int BatchUpdate<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, params Expression<Func<TEntity, object>>[] updatedFields)
            where TEntity : class, new()
        {
            string[] arr = CommandAdapterExtensions.ToStringArray(updatedFields);
            return RepositoryExtension.BatchUpdate(repository, entityList, BatchCommandMode.Batch, arr);
        }


        private static int BatchInsert<TEntity>(IRepository<TEntity> repository, IEnumerable<TEntity> entityList, BatchCommandMode mode, string[] insertFields)
            where TEntity : class, new()
        {
            if (null != repository)
            {
                return repository.BatchInsert(entityList, mode, insertFields);
            }
            return 0;
        }
        public static int BatchInsert<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, BatchCommandMode mode)
            where TEntity : class, new()
        {
            return RepositoryExtension.BatchInsert(repository, entityList, mode, (string[])null);
        }
        public static int BatchInsert<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList)
            where TEntity : class, new()
        {
            return RepositoryExtension.BatchInsert(repository, entityList, BatchCommandMode.Batch, (string[])null);
        }

        public static int BatchInsert<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, BatchCommandMode mode, params Expression<Func<TEntity, object>>[] insertFields)
            where TEntity : class, new()
        {
            string[] arr = CommandAdapterExtensions.ToStringArray(insertFields);
            return RepositoryExtension.BatchInsert(repository, entityList, mode, arr);
        }

        public static int BatchInsert<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, params Expression<Func<TEntity, object>>[] insertFields)
            where TEntity : class, new()
        {
            string[] arr = CommandAdapterExtensions.ToStringArray(insertFields);
            return RepositoryExtension.BatchInsert(repository, entityList, BatchCommandMode.Batch, arr);
        }





        private static int BatchUpsert<TEntity>(IRepository<TEntity> repository, IEnumerable<TEntity> entityList, BatchCommandMode mode, string[] updatedFields, string[] insertFields)
            where TEntity : class, new()
        {
            if (null != repository)
            {
                return repository.BatchUpsert(entityList, mode, updatedFields, insertFields);
            }
            return 0;
        }
        public static int BatchUpsert<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, BatchCommandMode mode, string[] updatedFields)
            where TEntity : class, new()
        {
            return RepositoryExtension.BatchUpsert(repository, entityList, mode, updatedFields, (string[])null);
        }
        public static int BatchUpsert<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, BatchCommandMode mode)
            where TEntity : class, new()
        {
            return RepositoryExtension.BatchUpsert(repository, entityList, mode, (string[])null, (string[])null);
        }
        public static int BatchUpsert<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList)
            where TEntity : class, new()
        {
            return RepositoryExtension.BatchUpsert(repository, entityList, BatchCommandMode.Batch, (string[])null, (string[])null);
        }
        public static int BatchUpsert<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, BatchCommandMode mode, Expression<Func<TEntity, object>>[] updatedFields, Expression<Func<TEntity, object>>[] insertFields)
            where TEntity : class, new()
        {
            string[] updateArr = CommandAdapterExtensions.ToStringArray(updatedFields);
            string[] insertArr = CommandAdapterExtensions.ToStringArray(insertFields);
            return RepositoryExtension.BatchUpsert(repository, entityList, mode, updateArr, insertArr);
        }
        public static int BatchUpsert<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, BatchCommandMode mode, Expression<Func<TEntity, object>>[] updatedFields)
            where TEntity : class, new()
        {
            return RepositoryExtension.BatchUpsert(repository, entityList, mode, updatedFields, (Expression<Func<TEntity, object>>[])null);
        }



        private static int Delsert<TEntity>(IRepository<TEntity> repository, IEnumerable<TEntity> entityList, BatchCommandMode mode, Func<FluentWhere<TEntity>, FluentWhere<TEntity>> where, params string[] insertFields)
            where TEntity : class, new()
        {
            if (null != repository)
            {
                return repository.Delsert(entityList, mode, where, insertFields);
            }
            return 0;
        }

        public static int Delsert<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, BatchCommandMode mode, Func<FluentWhere<TEntity>, FluentWhere<TEntity>> where)
            where TEntity : class, new()
        {
            return RepositoryExtension.Delsert(repository, entityList, mode, where, (string[])null);
        }

        public static int Delsert<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, Func<FluentWhere<TEntity>, FluentWhere<TEntity>> where)
            where TEntity : class, new()
        {
            return RepositoryExtension.Delsert(repository, entityList, BatchCommandMode.Batch, where, (string[])null);
        }

        public static int Delsert<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, BatchCommandMode mode, Func<FluentWhere<TEntity>, FluentWhere<TEntity>> where, params Expression<Func<TEntity, object>>[] insertFields)
            where TEntity : class, new()
        {
            string[] insertArr = CommandAdapterExtensions.ToStringArray(insertFields);
            return RepositoryExtension.Delsert(repository, entityList, mode, where, insertArr);
        }
        //
    }
}
