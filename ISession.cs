using System;
using System.Data;

namespace DAF
{
    public interface ISession : IDisposable
    {
        ISessionFactory SessionFactory { get; }
        ISecurityToken SecurityToken { get; }
        bool IsCache { get; }
        ICache Cache { get; }
        Q CreateQuery<Q>() where Q : IQuery;
    }
}
