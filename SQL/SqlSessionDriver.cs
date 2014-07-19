using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DAF.Core;

namespace DAF.SQL
{
    public class SqlSessionDriver : SessionDriver
    {
        private IDbConnection connection;

        public SqlSessionDriver(ISession session, ISecurityToken token, ServerConfiguration config)
            : base(session, token, config)
        {
        }

        public IDbConnection CurrentConnection
        {
            get
            {
                if (this.connection == null)
                {
                    this.connection = new SqlConnection(this.ConnectionString);
                    this.connection.Open();
                }

                return this.connection;
            }
        }

        public override Q CreateQuery<Q>()
        {
            return this.Configuration.CreateQuery<Q>(this);
        }

        public override void Dispose()
        {
            if (this.connection != null)
            {
                this.connection.Dispose();
                this.connection = null;
            }
        }
    }
}
