namespace ionix.Data
{
    using Utils.Extensions;
    using Utils.Reflection;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    public class FluentWhere<TEntity> : FluentBase
    {
        private readonly SqlQuery where;

        private readonly Dictionary<string, int> parameterNameDic;

        internal FluentBase Parent { get; }

        internal FluentWhere(char parameterPrefix, FluentBase parent)
            : base(parameterPrefix)
        {
            this.where = new SqlQuery();

            this.parameterNameDic = new Dictionary<string, int>();

            this.Parent = parent;
        }
        public FluentWhere(char parameterPrefix)
            : this(parameterPrefix, null)
        { }

        private string GetUniqueParameterName(PropertyInfo pi)
        {
            int num;
            if (this.parameterNameDic.TryGetValue(pi.Name, out num))
            {
                ++num;
                this.parameterNameDic[pi.Name] = num;
            }
            else
            {
                this.parameterNameDic.Add(pi.Name, num);//num == 0;
            }
            return pi.Name + num;
        }

        public FluentWhere<TEntity> And()
        {
            this.where.Text.Append(" AND");
            return this;
        }
        public FluentWhere<TEntity> Or()
        {
            this.where.Text.Append(" OR");
            return this;
        }
        public FluentWhere<TEntity> Not()
        {
            this.where.Text.Append(" NOT");
            return this;
        }



        private void Filter<TValue>(ConditionOperator op, Expression<Func<TEntity, TValue>> exp, TValue value)
        {
            if (null != exp)
            {
                PropertyInfo pi = ReflectionExtensions.GetPropertyInfo(exp.Body);
                if (null != pi)
                {
                    this.where.Text.Append(" (");
                    if (null != value)
                    {
                        FilterCriteria criteria = new FilterCriteria(pi.Name, op, this.ParameterPrefix, value);
                        criteria.ParameterName = this.GetUniqueParameterName(pi);

                        this.where.Combine(criteria.ToQuery());
                    }
                    else
                    {
                        this.where.Text.Append(pi.Name);
                        this.where.Text.Append(" IS NULL");
                    }
                    this.where.Text.Append(')');
                }
            }
        }

        public FluentWhere<TEntity> Equals<TValue>(Expression<Func<TEntity, TValue>> exp, TValue value)
        {
            this.Filter<TValue>(ConditionOperator.Equals, exp, value);
            return this;
        }
        public FluentWhere<TEntity> NotEquals<TValue>(Expression<Func<TEntity, TValue>> exp, TValue value)
        {
            this.Filter<TValue>(ConditionOperator.NotEquals, exp, value);
            return this;
        }
        public FluentWhere<TEntity> GreaterThan<TValue>(Expression<Func<TEntity, TValue>> exp, TValue value)
        {
            this.Filter<TValue>(ConditionOperator.GreaterThan, exp, value);
            return this;
        }
        public FluentWhere<TEntity> LessThan<TValue>(Expression<Func<TEntity, TValue>> exp, TValue value)
        {
            this.Filter<TValue>(ConditionOperator.LessThan, exp, value);
            return this;
        }
        public FluentWhere<TEntity> GreaterThanOrEqualsTo<TValue>(Expression<Func<TEntity, TValue>> exp, TValue value)
        {
            this.Filter<TValue>(ConditionOperator.GreaterThanOrEqualsTo, exp, value);
            return this;
        }
        public FluentWhere<TEntity> LessThanOrEqualsTo<TValue>(Expression<Func<TEntity, TValue>> exp, TValue value)
        {
            this.Filter<TValue>(ConditionOperator.LessThanOrEqualsTo, exp, value);
            return this;
        }

        public FluentWhere<TEntity> Contains(Expression<Func<TEntity, string>> exp, string value)
        {
            this.Filter<string>(ConditionOperator.Contains, exp, value);
            return this;
        }
        public FluentWhere<TEntity> StartsWith(Expression<Func<TEntity, string>> exp, string value)
        {
            this.Filter<string>(ConditionOperator.StartsWith, exp, value);
            return this;
        }
        public FluentWhere<TEntity> EndsWith(Expression<Func<TEntity, string>> exp, string value)
        {
            this.Filter<string>(ConditionOperator.EndsWith, exp, value);
            return this;
        }


        public FluentWhere<TEntity> Between<TValue>(Expression<Func<TEntity, TValue>> exp, TValue value1, TValue value2)
        {
            if (null != exp)
            {
                PropertyInfo pi = ReflectionExtensions.GetPropertyInfo(exp.Body);
                if (null != pi && (null != value1 && null != value2))
                {
                    this.where.Text.Append(" (");
                    FilterCriteria criteria = new FilterCriteria(pi.Name, ConditionOperator.Between, this.ParameterPrefix, value1, value2);
                    criteria.ParameterName = this.GetUniqueParameterName(pi);

                    this.where.Combine(criteria.ToQuery());
                    this.where.Text.Append(')');
                }
            }
            return this;
        }

        public FluentWhere<TEntity> In<TValue>(Expression<Func<TEntity, TValue>> exp, params TValue[] values)
        {
            if (null != exp)
            {
                PropertyInfo pi = ReflectionExtensions.GetPropertyInfo(exp.Body);
                if (null != pi && !values.IsEmptyList())
                {
                    this.where.Text.Append(" (");
                    object[] arr = new object[values.Length];
                    Array.Copy(values, arr, values.Length);
                    FilterCriteria criteria = new FilterCriteria(pi.Name, ConditionOperator.In, this.ParameterPrefix, arr);
                    criteria.ParameterName = this.GetUniqueParameterName(pi);

                    this.where.Combine(criteria.ToQuery());
                    this.where.Text.Append(')');
                }
            }
            return this;
        }

        private void IsNull(string bitwise, Expression<Func<TEntity, object>> exp)
        {
            if (null != exp)
            {
                PropertyInfo pi = ReflectionExtensions.GetPropertyInfo(exp.Body);
                if (null != pi)
                {
                    this.where.Text.Append(" (");
                    this.where.Text.Append(pi.Name);
                    this.where.Text.Append(' ');
                    this.where.Text.Append(bitwise);
                    this.where.Text.Append(" NULL");
                    this.where.Text.Append(')');
                }
            }
        }
        public FluentWhere<TEntity> IsNull(Expression<Func<TEntity, object>> exp)
        {
            this.IsNull("IS", exp);
            return this;
        }
        public FluentWhere<TEntity> IsNotNull(Expression<Func<TEntity, object>> exp)
        {
            this.IsNull("IS NOT", exp);
            return this;
        }


        public override SqlQuery ToQuery()
        {
            SqlQuery ret = new SqlQuery();

            if (this.where.Text.Length != 0)
            {
                ret.Text.AppendLine();
                ret.Text.Append("WHERE");
                ret.Combine(this.where);
            }

            return ret;
        }
    }
}
