namespace ionix.Data
{
    using System;
    using System.Collections.Generic;

    public interface ICommandAdapter
    {
        ICommandFactory Factory { get; }

        TEntity SelectById<TEntity>(params object[] idValues)
            where TEntity : new();
        TEntity SelectSingle<TEntity>(SqlQuery extendedQuery)
            where TEntity : new();
        IList<TEntity> Select<TEntity>(SqlQuery extendedQuery)
            where TEntity : new();

        TEntity QuerySingle<TEntity>(SqlQuery query)//Property adı kolondan farklı olan durumlar için IEntityMetaDataProvider provider eklendi.
            where TEntity : new();
        IList<TEntity> Query<TEntity>(SqlQuery query)
            where TEntity : new();

        int Update<TEntity>(TEntity entity, params string[] updatedFields);
        int Insert<TEntity>(TEntity entity, params string[] insertFields);
        int Upsert<TEntity>(TEntity entity, string[] updatedFields, string[] insertFields);
        int Delete<TEntity>(TEntity entity);

        int BatchUpdate<TEntity>(IEnumerable<TEntity> entityList, BatchCommandMode mode, params string[] updatedFields);
        int BatchInsert<TEntity>(IEnumerable<TEntity> entityList, BatchCommandMode mode, params string[] insertFields);
        int BatchUpsert<TEntity>(IEnumerable<TEntity> entityList, BatchCommandMode mode, string[] updatedFields, string[] insertFields);

        int Delsert<TEntity>(IEnumerable<TEntity> entityList, BatchCommandMode mode, Func<FluentWhere<TEntity>, FluentWhere<TEntity>> where
            , params string[] insertFields);
    }
}
