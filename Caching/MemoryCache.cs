using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAF.Caching
{
    public class MemoryCache : ICache
    {
        private readonly DataAccessConfiguration config;
        private Dictionary<string, CacheItem> cache;

        public MemoryCache()
        {
            this.cache = new Dictionary<string, CacheItem>();
        }
        
        public MemoryCache(DataAccessConfiguration config)
            :this()
        {
            this.config = config;
        }

        public void Add(string key, object entity)
        {
            cache[key] = new CacheItem(key, entity, CachePolicy.NoPolicy);
        }

        public void Add(string key, object entity, CachePolicy expirationPolicy)
        {
            if (expirationPolicy == null)
                throw new ArgumentNullException("expirationPolicy");

            cache[key] = new CacheItem(key, entity, expirationPolicy);
        }

        public object Get(string key)
        {
            CacheItem cacheItem;
            if (cache.TryGetValue(key, out cacheItem))
            {
                if (cacheItem.ExpirationPolicy.IsAbsoluteExpiration)
                {
                    if (cacheItem.ExpirationPolicy.AbsoluteExpiration < DateTime.Now)
                    {
                        cache.Remove(key);
                        return null;
                    }
                }

                return cacheItem.Value;
            }

            return null;
        }

        public bool TryGet(string key, out object entity)
        {
            object cachedEntity = this.Get(key);
            if (cachedEntity != null)
            {
                entity = cachedEntity;
                return true;
            }

            entity = null;
            return false;
        }

        public void Remove(string key)
        {
            this.cache.Remove(key);
        }

        public void RemoveAll()
        {
            this.cache.Clear();
        }


        public void Dispose()
        {
            RemoveAll();
        }
    }
}
