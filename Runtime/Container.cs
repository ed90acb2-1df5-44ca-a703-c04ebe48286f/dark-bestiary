using System;
using System.Collections.Generic;
using Zenject;

namespace DarkBestiary
{
    public class Container
    {
        public static Container Instance;

        private readonly DiContainer m_Container;

        public Container(DiContainer container)
        {
            m_Container = container;
        }

        public bool HasBinding<T>()
        {
            return m_Container.HasBinding<T>();
        }

        public T Resolve<T>()
        {
            return m_Container.Resolve<T>();
        }

        public T Instantiate<T>()
        {
            return m_Container.Instantiate<T>();
        }

        public object Instantiate(Type type)
        {
            return m_Container.Instantiate(type);
        }

        public object Instantiate(Type type, IEnumerable<object> args)
        {
            return m_Container.Instantiate(type, args);
        }

        public T Instantiate<T>(IEnumerable<object> args)
        {
            return m_Container.Instantiate<T>(args);
        }
    }
}