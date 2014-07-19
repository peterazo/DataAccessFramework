using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAF.Core;

namespace DAF
{
    /// <summary>
    /// Server configuration class responsible for mapping between server alias, connection string and
    /// server cache. Also it saves IQuery mapping into parent Configuration object, that it late
    /// can be retrieve using server alias.
    /// </summary>
    public class ServerConfiguration
    {
        // Resolver of ISessionDriver and IQuery interfaces.
        private readonly InterfaceResolver interfaceResover;

        // Parent configuration object.
        public DataAccessConfiguration Configuration { get; private set; }

        public string Alias { get; private set; }

        public string ConnectionString { get; private set; }

        // Type of the security token used when creating connection from this session.
        // If this parameter is not set, then it will be security token that set for the session.
        public bool OverrideSecurityToken { get; private set; }

        public ServerConfiguration(DataAccessConfiguration configuration, string alias)
        {
            this.Configuration = configuration;
            this.Alias = alias;
            this.ConnectionString = string.Empty;
            this.OverrideSecurityToken = false;
            //
            this.interfaceResover = new InterfaceResolver();
        }

        public ServerConfiguration SetConnectionString(string connectionString)
        {
            this.ConnectionString = connectionString;
            return this;
        }

        public ServerConfiguration SetSessionDriver(Type sessionDriver)
        {
            this.interfaceResover.MapInterface(typeof(ISessionDriver), sessionDriver);
            return this;
        }

        public ServerConfiguration SetSessionDriver<T>() where T : ISessionDriver
        {
            this.interfaceResover.MapInterface<ISessionDriver, T>();
            return this;
        }

        public ServerConfiguration SetSecurityToken(Type tokenType)
        {
            this.interfaceResover.MapInterface(typeof(ISecurityToken), tokenType);
            this.OverrideSecurityToken = true;
            return this;
        }

        public ServerConfiguration SetSecurityToken<T>() where T : ISecurityToken
        {
            this.interfaceResover.MapInterface<ISecurityToken, T>();
            this.OverrideSecurityToken = true;
            return this;
        }

        public ServerConfiguration MapQuery(Type from, Type to)
        {
            // First try to registered this query on global configuration object.
            this.Configuration.MapQueryToServer(from, this);
            // Register on local resolver.
            this.interfaceResover.MapInterface(from, to);
            return this;
        }

        public ServerConfiguration MapQuery(Type from, Type to, CachePolicy cachePolicy)
        {
            // First try to registered this query on global configuration object.
            this.Configuration.MapQueryToServer(from, this, cachePolicy);
            // Register on local resolver.
            this.interfaceResover.MapInterface(from, to);
            return this;
        }

        public ServerConfiguration MapQuery<From, To>()
            where From : IQuery
            where To : From
        {
            // First try to registered this query on global configuration object.
            this.Configuration.MapQueryToServer(typeof(From), this);
            // Register on local resolver.
            this.interfaceResover.MapInterface<From, To>();
            return this;
        }

        public ServerConfiguration MapQuery<From, To>(CachePolicy cachePolicy)
            where From : IQuery
            where To : From
        {
            // First try to registered this query on global configuration object.
            this.Configuration.MapQueryToServer(typeof(From), this, cachePolicy);
            // Register on local resolver.
            this.interfaceResover.MapInterface<From, To>();
            return this;
        }

        public ISessionDriver CreateSessionDriver(ISession session, ISecurityToken token)
        {
            return this.interfaceResover.Resolve<ISessionDriver>(session, token, this);
        }

        public T CreateQuery<T>() where T : IQuery
        {
            return this.interfaceResover.Resolve<T>();
        }

        public T CreateQuery<T>(params object[] args) where T : IQuery
        {
            return this.interfaceResover.Resolve<T>(args);
        }

        public ISecurityToken CreateSecurityToken(ISecurityToken parent)
        {
            return this.interfaceResover.Resolve<ISecurityToken>(parent);
        }
    }
}
