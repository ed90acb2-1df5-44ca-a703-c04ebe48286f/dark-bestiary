using System;
using System.Collections.Generic;
using Zenject;

namespace DarkBestiary
{
    public class Container
    {
        public static Container Instance;

        private readonly DiContainer container;

        public Container(DiContainer container)
        {
            this.container = container;
        }

        public bool HasBinding<T>()
        {
            return this.container.HasBinding<T>();
        }

        public T Resolve<T>()
        {
            return this.container.Resolve<T>();
        }

        public T Instantiate<T>()
        {
            return this.container.Instantiate<T>();
        }

        public object Instantiate(Type type)
        {
            return this.container.Instantiate(type);
        }

        public object Instantiate(Type type, IEnumerable<object> args)
        {
            return this.container.Instantiate(type, args);
        }

        public T Instantiate<T>(IEnumerable<object> args)
        {
            return this.container.Instantiate<T>(args);
        }
    }
}