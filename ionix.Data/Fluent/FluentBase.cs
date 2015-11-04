namespace ionix.Data
{
    using System;

    public abstract class FluentBase : ISqlQueryProvider
    {
        protected const char TableAlias = 'T';

        public char ParameterPrefix { get; }

        protected FluentBase(char parameterPrefix)
        {
            this.ParameterPrefix = parameterPrefix;
        }

        public abstract SqlQuery ToQuery();

        public sealed override string ToString()
        {
            return this.ToQuery().Text.ToString();
        }
    }

    public abstract class CrudFluentBase<TEntity> : FluentBase
    {
        protected CrudFluentBase(char parameterPrefix)
            : base(parameterPrefix)
        {

        }

        private string tableName;
        protected string TableName
        {
            get
            {
                if (String.IsNullOrEmpty(this.tableName))
                    this.tableName = AttributeExtension.GetTableName<TEntity>();
                return this.tableName;
            }
            set { this.tableName = value; }
        }
    }

    public abstract class FluentBaseExecutable<TEntity> : CrudFluentBase<TEntity>
    {
        protected FluentBaseExecutable(char parameterPrefix)
            : base(parameterPrefix)
        {

        }
    }
}
