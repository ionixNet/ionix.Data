namespace ionix.Data
{
    using System;

    public static class FluentExtensions
    {
        public static FluentSelect<TEntity> AsSelect<TEntity>(this FluentWhere<TEntity> where)
        {
            if (null != where)
            {
                return where.Parent as FluentSelect<TEntity>;
            }
            return null;
        }
        public static FluentUpdate<TEntity> AsUpdate<TEntity>(this FluentWhere<TEntity> where)
        {
            if (null != where)
            {
                return where.Parent as FluentUpdate<TEntity>;
            }
            return null;
        }
        public static FluentDelete<TEntity> AsDelete<TEntity>(this FluentWhere<TEntity> where)
        {
            if (null != where)
            {
                return where.Parent as FluentDelete<TEntity>;
            }
            return null;
        }
    }
}
