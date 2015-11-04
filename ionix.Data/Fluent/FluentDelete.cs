namespace ionix.Data
{
    using System.Text;

    public class FluentDelete<TEntity> : FluentBaseExecutable<TEntity>
    {
        private readonly FluentWhere<TEntity> where;

        public FluentDelete(char parameterPrefix)
            : base(parameterPrefix)
        {
            this.where = new FluentWhere<TEntity>(parameterPrefix, this);
        }

        public FluentDelete<TEntity> Table(string name)
        {
            this.TableName = name;
            return this;
        }

        public FluentWhere<TEntity> Where()
        {
            return this.where;
        }

        public override SqlQuery ToQuery()
        {
            SqlQuery ret = new SqlQuery();

            SqlQuery whereQuery = this.where.ToQuery();
            if (whereQuery.Text.Length != 0)
            {
                StringBuilder text = ret.Text;
                text.Append("DELETE FROM ");
                text.Append(this.TableName);
                ret.Combine(whereQuery);
            }

            return ret;
        }
    }
}
