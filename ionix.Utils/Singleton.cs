namespace ionix.Utils
{
    using System;
    using System.Collections.Generic;
    using Collections;

    //Türemiş Tipler için Singleton kontrolü. Micro Servisler için kullanılıyor.
    public abstract class Singleton
    {
        private static readonly object locker = new object();
        private static readonly ThrowingHashSet<Type> registeredTypes = new ThrowingHashSet<Type>();
        protected Singleton()
        {
            lock (locker)
            {
                //if (registeredTypes.Contains(this.GetType()))
                //{
                //    throw new InvalidOperationException("Only one instance can ever be registered.");
                //}

                registeredTypes.Add(this.GetType());
            }
        }
    }
}
