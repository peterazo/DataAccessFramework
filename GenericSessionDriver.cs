using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAF.Core;

namespace DAF
{
    public class GenericSessionDriver : SessionDriver
    {
        public GenericSessionDriver(ISession session, ISecurityToken token, ServerConfiguration configuration)
            : base(session, token, configuration)
        {
        }

        public override Q CreateQuery<Q>()
        {
            return this.Configuration.CreateQuery<Q>();
        }

        public override void Dispose()
        {
            // do nothing
        }
    }
}
