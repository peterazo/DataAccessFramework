using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAF.Core
{
    public abstract class SessionDriver : ISessionDriver
    {
        internal SessionDriver(ISession session, ISecurityToken token, ServerConfiguration configuration)
        {
            this.Session = session;
            this.SecurityToken = token;
            this.Configuration = configuration;
            this.ConnectionString = CreateConnectionStringFromSecurityToken(this.Configuration.ConnectionString, this.SecurityToken);
        }

        public ISession Session { get; private set; }

        public ISecurityToken SecurityToken { get; private set; }

        public ServerConfiguration Configuration { get; private set; }

        public string ConnectionString { get; private set; }

        public CachePolicy GetCachePolicy(Type query)
        {
            return this.Configuration.Configuration.FindQueryCachePolicy(query);
        }

        abstract public Q CreateQuery<Q>() where Q : IQuery;

        abstract public void Dispose();

        static private string CreateConnectionStringFromSecurityToken(string connectionString, ISecurityToken token)
        {
            return connectionString
                .Replace("$(User)", token.User)
                .Replace("$(Password)", token.Password);
        }
    }
}
