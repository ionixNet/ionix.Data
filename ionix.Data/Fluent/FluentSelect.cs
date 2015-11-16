namespace ionix.Data
{
    using Utils.Extensions;
    using Utils.Reflection;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    public class FluentSelect<TEntity> : CrudFluentBase<TEntity>
    {
        private readonly SqlQuery select;
        private readonly List<String> columns;
        private readonly List<String> orderBys;

        private FluentWhere<TEntity> where;

        public FluentSelect(char prefix)
            : base(prefix)
        {
            this.select = new SqlQuery();
            this.columns = new List<String>();
            this.orderBys = new List<String>();

            this.where = new FluentWhere<TEntity>(prefix, this);

        }


        public FluentSelect<TEntity> From(string tableName)
        {
            this.TableName = tableName;
            return this;
        }


        public FluentSelect<TEntity> AllColumns()
        {
            this.columns.Add("*");
            return this;
        }

        public FluentSelect<TEntity> Column(params Expression<Func<TEntity, object>>[] exps)
        {
            if (!exps.IsEmptyList())
            {
                foreach (Expression<Func<TEntity, object>> exp in exps)
                {
                    PropertyInfo pi = ReflectionExtensions.GetPropertyInfo(exp.Body);
                    if (null != pi)
                    {
                        this.columns.Add(pi.Name);
                    }
                }
            }
            return this;
        }

        public FluentSelect<TEntity> Custom(SqlQuery customQuery)
        {
            if (this.columns.Count != 0)
                throw new InvalidOperationException("Columns not empty");

            this.select.Combine(customQuery);
            return this;
        }


        public FluentWhere<TEntity> Where()
        {
            return this.where;
        }

        public FluentSelect<TEntity> Where(FluentWhere<TEntity> where)
        {
            if (null != where)
                this.where = where;
            return this;
        }

        public FluentSelect<TEntity> OrderBy(Expression<Func<TEntity, object>> exp, SortDirection direction)
        {
            if (null != exp)
            {
                PropertyInfo pi = ReflectionExtensions.GetPropertyInfo(exp.Body);
                if (null != pi)
                {
                    this.orderBys.Add(' ' + pi.Name + ' ' + (direction == SortDirection.Asc ? "ASC" : "DESC"));
                }
            }
            return this;
        }
        public FluentSelect<TEntity> OrderBy(Expression<Func<TEntity, object>> exp)
        {
            return this.OrderBy(exp, SortDirection.Asc);
        }

        public override SqlQuery ToQuery()
        {
            SqlQuery ret = new SqlQuery();

            if (this.columns.Count != 0 || this.select.Text.Length != 0)
            {
                SqlQuery pureSelect = new SqlQuery();
                pureSelect.Text.Append("SELECT * FROM (");
                if (this.columns.Count != 0)
                {
                    StringBuilder text = pureSelect.Text;
                    text.Append("SELECT ");
                    foreach (string columnName in this.columns)
                    {
                        text.Append(columnName);
                        text.Append(", ");
                    }
                    text.Remove(text.Length - 2, 2);
                    text.Append(" FROM ");
                    text.Append(this.TableName);
                }
                else
                {
                    pureSelect.Combine(this.select);
                }

                pureSelect.Text.Append(") ");
                pureSelect.Text.Append(TableAlias);
                ret.Combine(pureSelect);
            }

            ret.Combine(this.where.ToQuery());

            if (this.orderBys.Count != 0)
            {
                ret.Text.AppendLine();
                ret.Text.Append("ORDER BY");
                foreach (string order in this.orderBys)
                {
                    ret.Text.Append(order);
                    ret.Text.Append(", ");
                }
                ret.Text.Remove(ret.Text.Length - 2, 2);
            }
            return ret;
        }
    }
}
