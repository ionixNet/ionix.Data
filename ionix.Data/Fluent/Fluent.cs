namespace ionix.Data
{
    using System;

    public static class Fluent
    {
        private static char _DefaultPrefix = '@';// @ or :

        public static char DefaultPrefix
        {
            get
            {
                if (_DefaultPrefix == ' ')
                    throw new InvalidOperationException("Fluent.DefaultPrefix is not set.");

                return _DefaultPrefix; ;
            }
            set { _DefaultPrefix = value; }
        }

        public static FluentSelect<TEntity> Select<TEntity>()
        {
            return new FluentSelect<TEntity>(DefaultPrefix);
        }
        public static FluentUpdate<TEntity> Update<TEntity>()
        {
            return new FluentUpdate<TEntity>(DefaultPrefix);
        }
        public static FluentInsert<TEntity> Insert<TEntity>()
        {
            return new FluentInsert<TEntity>(DefaultPrefix);
        }
        public static FluentDelete<TEntity> Delete<TEntity>()
        {
            return new FluentDelete<TEntity>(DefaultPrefix);
        }


        public static FluentWhere<TEntity> Where<TEntity>()
        {
            return new FluentWhere<TEntity>(DefaultPrefix);
        }
    }
}
