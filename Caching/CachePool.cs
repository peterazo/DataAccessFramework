using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAF.Caching
{
    public class CachePool : ICachePool
    {
        private bool? isCache;
        private ICache cache;

        public DataAccessConfiguration Configuration { get; private set; }

        public bool IsCache
        {
            get
            {
                if (!isCache.HasValue)
                {
                    isCache = Configuration.CanCreateCache();
                }
                return isCache.Value;
            }
        }

        internal CachePool(DataAccessConfiguration configuration)
        {
            Configuration = configuration;
        }

        public ICache GetCache()
        {
            if (cache == null)
            {
                cache = Configuration.CreateCache();
            }
            return cache;
        }

        public void Dispose()
        {
            if (cache != null)
            {
                cache.Dispose();
                cache = null;
            }
        }
    }
}
