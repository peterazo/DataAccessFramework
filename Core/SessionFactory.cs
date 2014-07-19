using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAF.Core
{
    /// <summary>
    /// The global session factory. It holds global configuration for all sessions and create global session object.
    /// </summary>
    public class SessionFactory : ISessionFactory
    {
        public DataAccessConfiguration Configuration { get; private set; }

        public ICachePool CachePool { get; private set; }

        public SessionFactory(DataAccessConfiguration configuration)
        {
            Configuration = configuration;
            CachePool = new Caching.CachePool(configuration);
        }

        public ISession CreateSession()
        {
            return new Core.Session(this);
        }

        public ISession CreateSession(ISecurityToken token)
        {
            return new Core.Session(this, token);
        }
    }
}
