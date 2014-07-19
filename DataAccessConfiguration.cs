using System;
using System.Collections.Generic;
using System.Data;
using DAF.Core;

namespace DAF
{
    /// <summary>
    /// Global configuration object
    /// </summary>
    public class DataAccessConfiguration
    {
        // List of registered server configurations.
        private readonly Dictionary<string, ServerConfiguration> Servers;

        // Reverse index between registered type of the query and server alias.
        private readonly Dictionary<string, string> QueryIndex;

        // Reverse index between registered type of the query and cache policy.
        private readonly Dictionary<string, CachePolicy> QueryCachePolicy;

        // Resolver of ICache and other global interfaces if needed.
        private readonly InterfaceResolver InterfaceResover;


        public DataAccessConfiguration()
        {
            Servers = new Dictionary<string, ServerConfiguration>();
            QueryIndex = new Dictionary<string, string>();
            QueryCachePolicy = new Dictionary<string,CachePolicy>();
            InterfaceResover = new InterfaceResolver();
        }

        public ServerConfiguration AddServer(string alias)
        {
            if (Servers.ContainsKey(alias))
            {
                throw new ArgumentException("Server with alias " + alias + " already registered");
            }

            ServerConfiguration serverConfig = new ServerConfiguration(this,alias);
            Servers.Add(alias, serverConfig);
            return serverConfig;
        }

        public ServerConfiguration GetServerConfiguration(string alias)
        {
            ServerConfiguration serverConfig;
            if (!Servers.TryGetValue(alias, out serverConfig))
            {
                throw new ArgumentException("Server configuration with alias " + alias + " is not registered.");
            }

            return serverConfig;
        }

        public DataAccessConfiguration SetCache(Type cacheType)
        {
            InterfaceResover.MapInterface(typeof(ICache), cacheType);
            return this;
        }

        public DataAccessConfiguration SetCache<T>() where T : ICache
        {
            InterfaceResover.MapInterface<ICache, T>();
            return this;
        }

        public ISessionFactory CreateSessionFactory()
        {
            return new Core.SessionFactory(this);
        }


        /// <summary>
        /// Maps specified query type and server alias that the last can be found by query type later.
        /// </summary>
        /// <param name="query">Type of the query need to be registered.</param>
        /// <param name="alias">Alias of the server.</param>
        /// /// <remarks>We do not expect query with the same type to be registered on this map more than once.</remarks>
        internal void MapQueryToServer(Type query, ServerConfiguration serverConfig)
        {
            // Verify that there is no query with the same type already registered.
            string serverAlias;
            if (QueryIndex.TryGetValue(query.FullName, out serverAlias))
            {
                throw new ArgumentException(
                    "Query with type " + query.FullName + " already registered on " + serverAlias + " server.");
            }

            // Map query type to server where it's been registered.
            QueryIndex.Add(query.FullName, serverConfig.Alias);
        }

        /// <summary>
        /// Associates specified query type with cache policy that later used to store result of the query.
        /// </summary>
        /// <param name="query">Type of the query need to be registered</param>
        /// <param name="cachePolicy">Cache policy associated with query.</param>
        /// <remarks>We do not expect query with the same type to be registered on this map more than once.</remarks>
        internal void MapQueryToServer(Type query, ServerConfiguration serverConfig, CachePolicy cachePolicy)
        {
            MapQueryToServer(query, serverConfig);
            // Map cache policy to the query type.
            QueryCachePolicy.Add(query.FullName, cachePolicy);
        }

        /// <summary>
        /// Resolve server configuration by query type.
        /// </summary>
        /// <param name="query">Type of the query</param>
        /// <returns>Resolved server configuration</returns>
        internal ServerConfiguration FindServerConfigurationByQuery(Type query)
        {
            string serverAlias;
            if (!QueryIndex.TryGetValue(query.FullName, out serverAlias))
            {
                throw new ArgumentException(
                    "Query with type " + query.FullName + " is not registered on this configuration.");
            }

            return GetServerConfiguration(serverAlias);
        }

        internal CachePolicy FindQueryCachePolicy(Type query)
        {
            CachePolicy cachePolicy;
            if (!QueryCachePolicy.TryGetValue(query.FullName, out cachePolicy))
            {
                return null;
            }
            return cachePolicy;
        }


        internal bool CanCreateCache()
        {
            return InterfaceResover.CanResolve<ICache>();
        }

        internal ICache CreateCache()
        {
            return InterfaceResover.Resolve<ICache>(this);
        }
    }
}
