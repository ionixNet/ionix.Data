namespace ionix.Data
{
    using System;
    using System.Collections.Generic;

    public interface ICommandAdapter
    {
        ICommandFactory Factory { get; }
        bool ConvertType { get; set; }

        TEntity SelectById<TEntity>(params object[] idValues);

        TEntity SelectSingle<TEntity>(SqlQuery extendedQuery);

        IList<TEntity> Select<TEntity>(SqlQuery extendedQuery);

        TEntity QuerySingle<TEntity>(SqlQuery query);

        Tuple<TEntity1, TEntity2> QuerySingle<TEntity1, TEntity2>(SqlQuery query, MapBy by);
        Tuple<TEntity1, TEntity2, TEntity3> QuerySingle<TEntity1, TEntity2, TEntity3>(SqlQuery query, MapBy by);
        Tuple<TEntity1, TEntity2, TEntity3, TEntity4> QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4>(SqlQuery query, MapBy by);
        Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5> QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(SqlQuery query, MapBy by);
        Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6> QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(SqlQuery query, MapBy by);
        Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7> QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(SqlQuery query, MapBy by);


        IList<TEntity> Query<TEntity>(SqlQuery query);

        IList<Tuple<TEntity1, TEntity2>> Query<TEntity1, TEntity2>(SqlQuery query, MapBy by);
        IList<Tuple<TEntity1, TEntity2, TEntity3>> Query<TEntity1, TEntity2, TEntity3>(SqlQuery query, MapBy by);
        IList<Tuple<TEntity1, TEntity2, TEntity3, TEntity4>> Query<TEntity1, TEntity2, TEntity3, TEntity4>(SqlQuery query, MapBy by);
        IList<Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>> Query<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(SqlQuery query, MapBy by);
        IList<Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>> Query<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(SqlQuery query, MapBy by);
        IList<Tuple<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>> Query<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(SqlQuery query, MapBy by);


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
