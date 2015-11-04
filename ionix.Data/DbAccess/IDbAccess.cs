namespace ionix.Data
{
    using System;
    using System.Data;

    public interface IDbAccess : IDisposable
    {
        IDataReader CreateDataReader(SqlQuery query, CommandBehavior behavior);
        int ExecuteNonQuery(SqlQuery query);
        object ExecuteScalar(SqlQuery query);
    }

    public interface ITransactionalDbAccess : IDbAccess, IDbTransaction
    {

    }
}
