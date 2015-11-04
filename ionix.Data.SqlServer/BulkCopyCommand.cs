namespace ionix.Data.SqlServer
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    public class BulkCopyCommand : IBulkCopyCommand
    {
        private readonly SqlConnection conn;
        public BulkCopyCommand(SqlConnection conn)
        {
            if (null == conn)
                throw new ArgumentNullException(nameof(conn));

            if (conn.State != ConnectionState.Open)
                throw new ArgumentException("'connection' is not open.");

            this.conn = conn;
        }

        public void Execute(DataTable dataTable)
        {
            if (null != dataTable)
            {
                if (String.IsNullOrEmpty(dataTable.TableName))
                    throw new ArgumentException("DataTable.TableName Proeprty must be set");

                using (SqlBulkCopy s = new SqlBulkCopy(this.conn))
                {
                    s.BulkCopyTimeout = int.MaxValue;
                    s.DestinationTableName = dataTable.TableName;

                    foreach (var column in dataTable.Columns)
                        s.ColumnMappings.Add(column.ToString(), column.ToString());

                    s.WriteToServer(dataTable);
                }
            }
        }

        public void Execute<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            if (!entityList.IsEmptyList() && null != provider)
            {
                DataTable table = entityList.ToDataTable(provider);
                this.Execute(table);
            }
        }

        public void Dispose()
        {
            if (null != this.conn)
                this.conn.Dispose();
        }
    }
}
