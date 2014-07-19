using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DAF.Query
{
    public abstract class BaseQuery : IQuery
    {
        protected BaseQuery()
            : this(null)
        {
        }

        protected BaseQuery(ISessionDriver driver)
        {
            this.Driver = driver;
        }

        public ISessionDriver Driver { get; private set; }

        public IQuery SetParameter<V>(string name, V value)
        {
            ParameterMapper.GetMapper(GetType()).SetParameter(this, name, value);
            return this;
        }

        public List<Tuple<string, object>> GetParameters()
        {
            return ParameterMapper.GetMapper(GetType()).GetParameters(this);
        }

        abstract public int Execute();
    }

    public abstract class BaseQuery<T> : BaseQuery, IQuery<T> where T : class, new()
    {
        public static readonly int FetchSizeMax = -1;

        protected BaseQuery()
            : this(null)
        {
        }

        protected BaseQuery(ISessionDriver driver)
            : base(driver)
        {
            Result = new List<T>();
            Executed = false;
            FirstFetched = 0;
            FetchSize = FetchSizeMax;
        }

        public List<T> Result { get; protected set; }
        public bool Executed { get; protected set; }

        public int FirstFetched { get; set; }
        public int FetchSize { get; set; }

        public IQuery<T> SetFirstFetched(int index)
        {
            this.FirstFetched = index;
            return this;
        }

        public IQuery<T> SetFetchSize(int size)
        {
            this.FetchSize = size;
            return this;
        }

        public new IQuery<T> SetParameter<V>(string name, V val)
        {
            base.SetParameter(name, val);
            return this;
        }

        public IList<T> List()
        {
            if (!this.Executed)
                Execute();

            return this.Result;
        }

        public T First()
        {
            if (!this.Executed)
                Execute();

            return this.Result.First();
        }

        public T FirstOrDefault()
        {
            if (!this.Executed)
                Execute();

            return this.Result.FirstOrDefault();
        }

        public void Clear()
        {
            this.Executed = false;
            this.Result.Clear();
        }

        public override int Execute()
        {
            ParameterMapper pmapper = ParameterMapper.GetMapper(GetType());
            CachePolicy policy = this.GetCachePolicy();

            Clear();

            // Verify that we can use cache.
            if (policy != null)
            {
                // Create cache key and check if result already in the cache.
                string allParameters = string.Join(";", pmapper.GetParameters(this));
                string cacheKey = this.GetType().FullName + ":" + allParameters;
                object cachedObject;

                if (!Driver.Session.Cache.TryGet(cacheKey, out cachedObject))
                {
                    PopulateResult(pmapper);
                    Driver.Session.Cache.Add(cacheKey, this.Result, policy);
                }
                else
                {
                    this.Result = cachedObject as List<T>;
                }
            }
            else
            {
                PopulateResult(pmapper);
            }

            this.Executed = true;
            return this.Result.Count;
        }

        // Method returns  cache policy associated with this query or null.
        private CachePolicy GetCachePolicy()
        {
            if (this.Driver != null)
            {
                if (this.Driver.Session.IsCache)
                {
                    return this.Driver.GetCachePolicy(this.GetType());
                }
            }

            return null;
        }

        protected abstract void PopulateResult(ParameterMapper pmapper);
    }
}
