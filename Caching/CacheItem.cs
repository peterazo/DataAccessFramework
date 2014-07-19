using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DAF.Caching
{
    public class CacheItem
    {
        private string key;
        private object value;
        private DateTime accessTime;
        private CachePolicy expirationPolicy;

        public string Key
        {
            get { return this.key; }
        }

        public DateTime AccessTime
        {
            get { return this.accessTime; }
        }

        public CachePolicy ExpirationPolicy
        {
            get { return this.expirationPolicy; }
        }

        public object Value
        {
            get
            {
                object current = this.value;
                accessTime = DateTime.UtcNow;
                return current;
            }
        }

        public CacheItem(string key, object value, CachePolicy expirationPolicy)
        {
            this.key = key;
            this.value = value;
            this.expirationPolicy = expirationPolicy;
        }
    }
}
