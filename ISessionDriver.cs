using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DAF
{
    public interface ISessionDriver : IDisposable
    {
        ISession Session { get; }
        ISecurityToken SecurityToken { get; }
        CachePolicy GetCachePolicy(Type query);
        Q CreateQuery<Q>() where Q : IQuery;
    }
}
