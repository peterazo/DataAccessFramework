using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace DAF.Core
{
    /// <summary>
    /// Creates a proxy between caller and a real session object responsible for the query.
    /// </summary>
    public class Session : ISession
    {
        // Set of session drivers created in during this session.
        private readonly List<KeyValuePair<string, ISessionDriver>> Drivers;

        public ISessionFactory SessionFactory { get; private set; }

        public ISecurityToken SecurityToken { get; private set; }

        public bool IsCache
        {
            get { return this.SessionFactory.CachePool.IsCache; }
        }

        public ICache Cache
        {
            get { return SessionFactory.CachePool.GetCache(); }
        }

        public Session(ISessionFactory factory)
            : this(factory, new SecurityToken())
        {
        }

        public Session(ISessionFactory factory, ISecurityToken token)
        {
            this.SessionFactory = factory;
            this.SecurityToken = token;
            Drivers = new List<KeyValuePair<string,ISessionDriver>>();
        }

        public Q CreateQuery<Q>() where Q : IQuery
        {
            try
            {
                ServerConfiguration serverConfig = this.SessionFactory.Configuration.FindServerConfigurationByQuery(typeof(Q));
                ISessionDriver driver = ResolveDriverByServer(serverConfig);

                return driver.CreateQuery<Q>();
            }
            catch(Exception e)
            {
                throw new InvalidOperationException("Failed to create query '" + typeof(Q).FullName + "'", e);
            }
        }

        public void Dispose()
        {
            // Dispose all created drivers in during this session.
            Drivers.ForEach(x => x.Value.Dispose());
        }

        private ISessionDriver ResolveDriverByServer(ServerConfiguration serverConfig)
        {
            ISessionDriver driver = Drivers.FirstOrDefault(t => t.Key.Equals(serverConfig.Alias)).Value;
            if (driver == null)
            {
                ISecurityToken token = null;
                if (serverConfig.OverrideSecurityToken)
                {
                    token = serverConfig.CreateSecurityToken(SecurityToken);
                }
                else
                {
                    token = SecurityToken;
                }

                driver = serverConfig.CreateSessionDriver(this, token);
                Drivers.Add(new KeyValuePair<string, ISessionDriver>(serverConfig.Alias, driver));
            }
            return driver;
        }
    }
}
