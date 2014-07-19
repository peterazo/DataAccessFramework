using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAF
{
    public interface ICache : IDisposable
    {
        void Add(string key, object entity);
        void Add(string key, object entity, CachePolicy expirationPolicy);
        object Get(string key);
        bool TryGet(string key, out object entity);

        void Remove(string key);
        void RemoveAll();
    }
}
