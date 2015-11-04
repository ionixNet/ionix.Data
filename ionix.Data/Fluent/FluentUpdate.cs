namespace ionix.Data
{
    using Utils.Reflection;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    public class FluentUpdate<TEntity> : FluentBaseExecutable<TEntity>
    {
        private readonly Dictionary<string, object> sets;
        private readonly FluentWhere<TEntity> where;

        public FluentUpdate(char parameterPrefix)
            : base(parameterPrefix)
        {
            this.sets = new Dictionary<string, object>();
            this.where = new FluentWhere<TEntity>(parameterPrefix, this);
        }

        public FluentUpdate<TEntity> Table(string name)
        {
            this.TableName = name;
            return this;
        }

        public FluentWhere<TEntity> Where()
        {
            return this.where;
        }


        private void SetValue(PropertyInfo pi, object value)
        {
            this.sets[pi.Name] = value;//null olas bile DbAccess null' ı DBNull.Value' ye çeviriyor.
        }
        public FluentUpdate<TEntity> Include(TEntity entity)
        {
            if (null != entity)
            {
                foreach (PropertyInfo pi in typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    object value = pi.GetValue(entity);
                    this.SetValue(pi, value);
                }
            }
            return this;
        }
        public FluentUpdate<TEntity> Set<TValue>(Expression<Func<TEntity, TValue>> exp, TValue value)
        {
            if (null != exp)
            {
                PropertyInfo pi = ReflectionExtensions.GetPropertyInfo(exp.Body);
                if (null != pi)
                {
                    this.SetValue(pi, value);
                }
            }
            return this;
        }

        public override SqlQuery ToQuery()
        {
            SqlQuery ret = new SqlQuery();

            if (this.sets.Count != 0)
            {
                StringBuilder text = ret.Text;
                text.Append("UPDATE ");
                text.Append(this.TableName);
                text.Append(" SET ");
                foreach (var kvp in this.sets)
                {
                    text.Append(kvp.Key);
                    text.Append(" = ");
                    text.Append(this.ParameterPrefix);
                    text.Append(kvp.Key);

                    ret.Parameters.Add(kvp.Key, kvp.Value);

                    text.Append(", ");
                }
                text.Remove(text.Length - 2, 2);

                ret.Combine(this.where.ToQuery());
            }

            return ret;
        }
    }
}
