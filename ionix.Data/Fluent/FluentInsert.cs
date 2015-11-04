namespace ionix.Data
{
    using Utils.Reflection;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    public class FluentInsert<TEntity> : FluentBaseExecutable<TEntity>
    {
        private readonly Dictionary<string, object> adds;

        public FluentInsert(char parameterPrefix)
            : base(parameterPrefix)
        {
            this.adds = new Dictionary<string, object>();
        }


        public FluentInsert<TEntity> Table(string name)
        {
            this.TableName = name;
            return this;
        }

        private void AddValue(PropertyInfo pi, object value)
        {
            this.adds[pi.Name] = value;//null olas bile DbAccess null' ı DBNull.Value' ye çeviriyor.
        }
        public FluentInsert<TEntity> Include(TEntity entity)
        {
            if (null != entity)
            {
                foreach (PropertyInfo pi in typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    object value = pi.GetValue(entity);
                    this.AddValue(pi, value);
                }
            }
            return this;
        }
        public FluentInsert<TEntity> Add<TValue>(Expression<Func<TEntity, TValue>> exp, TValue value)
        {
            if (null != exp)
            {
                PropertyInfo pi = ReflectionExtensions.GetPropertyInfo(exp.Body);
                if (null != pi)
                {
                    this.AddValue(pi, value);
                }
            }
            return this;
        }

        public override SqlQuery ToQuery()
        {
            SqlQuery ret = new SqlQuery();

            if (this.adds.Count != 0)
            {
                StringBuilder text = ret.Text;
                text.Append("INSERT INTO ");
                text.Append(this.TableName);
                text.Append(" ( ");
                foreach (var kvp in this.adds)
                {
                    text.Append(kvp.Key);
                    text.Append(", ");
                }
                text.Remove(text.Length - 2, 2);
                text.Append(" ) VALUES ( ");
                foreach (var kvp in this.adds)
                {
                    text.Append(this.ParameterPrefix);
                    text.Append(kvp.Key);

                    ret.Parameters.Add(kvp.Key, kvp.Value);

                    text.Append(", ");
                }
                text.Remove(text.Length - 2, 2);
                text.Append(" )");
            }

            return ret;
        }
    }
}
