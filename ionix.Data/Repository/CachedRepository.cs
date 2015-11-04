namespace ionix.Data
{
    using Utils.Collections;
    using System.Linq;
    using System;
    using System.Linq.Expressions;
    using System.Collections.Generic;

    public class CachedRepository<TEntity> : Repository<TEntity>
        where TEntity : class, new()
    {
        private readonly bool throwExceptionOnNonCachedOperation;
        private readonly Expression<Func<TEntity, object>>[] keys;

        public CachedRepository(ICommandAdapter cmd, bool throwExceptionOnNonCachedOperation, params Expression<Func<TEntity, object>>[] keys)
            : base(cmd)
        {
            if (!keys.Any())
                throw new ArgumentNullException(nameof(keys));

            this.keys = keys;
            this.throwExceptionOnNonCachedOperation = throwExceptionOnNonCachedOperation;
        }


        private readonly object syncRoot = new object();
        private volatile IndexedEntityList<TEntity> list;

        private IndexedEntityList<TEntity> List
        {
            get
            {
                if (null == this.list)
                {
                    lock (this.syncRoot)
                    {
                        if (null == this.list)
                        {
                            var allRecords = this.Cmd.Select<TEntity>();
                            this.list = new IndexedEntityList<TEntity>(allRecords, this.keys);
                        }
                    }
                }
                return this.list;
            }
        }

        public void Refresh()
        {
            this.list = null;
        }

        public override TEntity SelectById(params object[] idValues)
        {
            return this.List.Find(idValues);
        }
        public override TEntity SelectSingle(SqlQuery extendedQuery)
        {
            if (this.throwExceptionOnNonCachedOperation)
                throw new NotSupportedException();

            return base.SelectSingle(extendedQuery);
        }
        public override IList<TEntity> Select(SqlQuery extendedQuery)
        {
            if (null != extendedQuery && this.throwExceptionOnNonCachedOperation)
                throw new NotSupportedException();

            return this.List.ToList();
        }
        public override TEntity QuerySingle(SqlQuery query)
        {
            if (this.throwExceptionOnNonCachedOperation)
                throw new NotSupportedException();

            return base.QuerySingle(query);
        }
        public override IList<TEntity> Query(SqlQuery query)
        {
            if (this.throwExceptionOnNonCachedOperation)
                throw new NotSupportedException();

            return base.Query(query);
        }

        public override int Update(TEntity entity, params string[] updatedFields)
        {
            int ret = base.Update(entity, updatedFields);
            if (ret > 0)
            {
                this.List.Replace(entity);
            }
            return ret;
        }

        public override int Insert(TEntity entity, params string[] insertFields)
        {
            int ret = base.Insert(entity, insertFields);
            if (ret > 0)
            {
                this.List.Add(entity);
            }
            return ret;
        }

        public override int Upsert(TEntity entity, string[] updatedFields, string[] insertFields)
        {
            int ret = base.Upsert(entity, updatedFields, insertFields);
            if (ret > 0)
            {
                this.List.Add(entity); //Add zaten Dictionary Indexer' ı Kullanıyor.
            }
            return ret;
        }

        public override int Delete(TEntity entity)
        {
            int ret = base.Delete(entity);
            if (ret > 0)
            {
                this.List.Remove(entity);
            }
            return ret;
        }

        public override int BatchUpdate(IEnumerable<TEntity> entityList, BatchCommandMode mode,
            params string[] updatedFields)
        {
            int ret = base.BatchUpdate(entityList, mode, updatedFields);
            if (ret > 0)
            {
                foreach (var entity in entityList)
                {
                    this.List.Replace(entity);
                }
            }
            return ret;
        }

        public override int BatchInsert(IEnumerable<TEntity> entityList, BatchCommandMode mode,
            params string[] insertFields)
        {
            int ret = base.BatchInsert(entityList, mode, insertFields);
            if (ret > 0)
            {
                foreach (var entity in entityList)
                {
                    this.List.Add(entity);
                }
            }
            return ret;
        }

        public override int BatchUpsert(IEnumerable<TEntity> entityList, BatchCommandMode mode, string[] updatedFields,
            string[] insertFieldss)
        {
            int ret = base.BatchUpsert(entityList, mode, updatedFields, insertFieldss);
            if (ret > 0)
            {
                foreach (var entity in entityList)
                {
                    this.List.Add(entity);
                }
            }
            return ret;
        }

        public override int Delsert(IEnumerable<TEntity> entityList, BatchCommandMode mode,
            Func<FluentWhere<TEntity>, FluentWhere<TEntity>> @where, params string[] insertFields)
        {
            int ret = base.Delsert(entityList, mode, @where, insertFields);
            if (ret > 0)
                this.Refresh();
            return ret;
        }
    }
}
