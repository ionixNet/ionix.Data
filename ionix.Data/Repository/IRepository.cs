namespace ionix.Data
{
    using System;
    using System.Collections.Generic;

    public interface IRepository<TEntity> : IDisposable
           where TEntity : class, new()
    {
        //Props
        ICommandAdapter Cmd { get; }
        IDbAccess DataAccess { get; }
        //

        //Evets
        event EventHandler<PreExecuteCommandEventArgs<TEntity>> PreExecuteCommand;
        event EventHandler<ExecuteCommandCompleteEventArgs<TEntity>> ExecuteCommandCompleted;
        //

        //Select
        TEntity SelectById(params object[] idValues);
        TEntity SelectSingle(SqlQuery extendedQuery);
        IList<TEntity> Select(SqlQuery extendedQuery);

        TEntity QuerySingle(SqlQuery query);
        IList<TEntity> Query(SqlQuery query);
        //

        //Entity
        int Update(TEntity entity, params string[] updatedFields);
        int Insert(TEntity entity, params string[] insertFields);
        int Upsert(TEntity entity, string[] updatedFields, string[] insertFields);
        int Delete(TEntity entity);
        //

        //Batch
        int BatchUpdate(IEnumerable<TEntity> entityList, BatchCommandMode mode, params string[] updatedFields);
        int BatchInsert(IEnumerable<TEntity> entityList, BatchCommandMode mode, params string[] insertFields);
        int BatchUpsert(IEnumerable<TEntity> entityList, BatchCommandMode mode, string[] updatedFields, string[] insertFields);
        int Delsert(IEnumerable<TEntity> entityList, BatchCommandMode mode, Func<FluentWhere<TEntity>, FluentWhere<TEntity>> where, params string[] insertFields);
        //
    }
}
