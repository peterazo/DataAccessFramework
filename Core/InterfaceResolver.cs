using System;
using System.Collections.Generic;

namespace DAF.Core
{
    public class InterfaceResolver
    {
        private readonly Dictionary<string, Type> interfaceToImplementationMap;

        public InterfaceResolver()
        {
            interfaceToImplementationMap = new Dictionary<string, Type>();
        }

        public void MapInterface(Type from, Type to)
        {
            interfaceToImplementationMap.Add(from.FullName, to);
        }

        public void MapInterface<From, To>()
        {
            MapInterface(typeof(From), typeof(To));
        }

        public bool CanResolve<T>()
        {
            return this.interfaceToImplementationMap.ContainsKey(typeof(T).FullName);
        }

        public Type Get<T>()
        {
            Type impl;
            if (!this.interfaceToImplementationMap.TryGetValue(typeof(T).FullName, out impl))
                throw new ArgumentException("Can't resolve type : " + typeof(T).FullName);
            return impl;
        }

        public T Resolve<T>()
        {
            return (T)Activator.CreateInstance(Get<T>());
        }

        public T Resolve<T>(params object[] args)
        {
            return (T)Activator.CreateInstance(Get<T>(), args);
        }

    }
}
