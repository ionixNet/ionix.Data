namespace ionix.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;


    public interface IEntitySqlQueryBuilder
    {
        SqlQuery CreateQuery(object entity, IEntityMetaData metaData, int index);//Index ler batch ler için eklendi.
    }

    public interface IBulkCopyCommand : IDisposable
    {
        void Execute<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider);
        void Execute(DataTable dataTable);
    }
}
