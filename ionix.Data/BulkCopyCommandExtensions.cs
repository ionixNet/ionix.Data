namespace ionix.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Threading.Tasks;
    using ionix.Utils.Extensions;


    public static class BulkCopyCommandExtensions
    {
        public static Task ExecuteAsync<TEntity>(this IBulkCopyCommand cmd, IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            return Task.Run(() =>
            {
                if (null != cmd)
                    cmd.Execute(entityList, provider);
            });
        }

        public static Task ExecuteAsync(this IBulkCopyCommand cmd, DataTable dataTable)
        {
            return Task.Run(() =>
            {
                if (null != cmd)
                    cmd.Execute(dataTable);
            });
        }

        public static DataTable ToDataTable<TEntity>(this IEnumerable<TEntity> entityList)
        {
            Type entityType = typeof(TEntity);
            DataTable table = new DataTable(entityType.Name);
            if (!entityList.IsEmptyList())
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
                foreach (PropertyDescriptor prop in properties)
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                foreach (TEntity item in entityList)
                {
                    DataRow row = table.NewRow();
                    foreach (PropertyDescriptor prop in properties)
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    table.Rows.Add(row);
                }
                return table;
            }
            return table;
        }

        public static DataTable ToDataTable<TEntity>(this IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            DataTable ret = new DataTable();
            if (null != provider && !entityList.IsEmptyList())
            {
                IEntityMetaData metaData = provider.CreateEntityMetaData(typeof(TEntity));
                if (null != metaData)
                {
                    ret.TableName = metaData.TableName;
                    foreach (PropertyMetaData prop in metaData.Properties)
                    {
                        SchemaInfo schema = prop.Schema;
                        ret.Columns.Add(schema.ColumnName, schema.DataType);
                    }
                    foreach (TEntity item in entityList)
                    {
                        DataRow row = ret.NewRow();
                        foreach (PropertyMetaData prop in metaData.Properties)
                            row[prop.Schema.ColumnName] = prop.Property.GetValue(item) ?? DBNull.Value;
                        ret.Rows.Add(row);
                    }
                }
            }
            return ret;
        }
    }
}
