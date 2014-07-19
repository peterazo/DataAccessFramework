using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

using DAF.Core;
using DAF.Query;

namespace DAF.SQL
{
    public abstract class SqlQuery : BaseQuery
    {
        protected SqlQuery(SqlSessionDriver driver, string query, CommandType commandType)
            : base (driver)
        {
            this.Command = driver.CurrentConnection.CreateCommand();
            this.Command.CommandText = query;
            this.Command.CommandType = commandType;
        }

        public IDbCommand Command { get; private set; }

        public int Timeout
        {
            get { return this.Command.CommandTimeout; }
            set { this.Command.CommandTimeout = value; }
        }

        public override int Execute()
        {
            ParameterMapper mapper = ParameterMapper.GetMapper(GetType());

            // Mapping query parameters to SQL command.
            mapper.MapTo(this, this.Command);
            return this.Command.ExecuteNonQuery();
        }
    }

    public abstract class SqlQuery<T> : BaseQuery<T>, IQuery<T> where T : class, new()
    {
        protected SqlQuery(SqlSessionDriver driver, string query, CommandType commandType)
            : base (driver)
        {
            this.Command = driver.CurrentConnection.CreateCommand();
            this.Command.CommandText = query;
            this.Command.CommandType = commandType;
        }

        public IDbCommand Command { get; private set; }

        protected override void PopulateResult(ParameterMapper pmapper)
        {
            // Mapping query parameters to SQL command.
            pmapper.MapTo(this, this.Command);

            using (IDataReader reader = this.Command.ExecuteReader())
            {
                int row = 0;
                EntityMapper emapper = EntityMapper.GetMapper(typeof(T));

                while (reader.Read() && ((-1 == this.FetchSize) || (row < this.FetchSize)))
                {
                    if (row < this.FirstFetched)
                        continue;

                    T t = new T();
                    // Mapping result of the SQL back to the entity.
                    emapper.MapTo(reader, t);
                    this.Result.Add(t);

                    ++row;
                }
            }
        }
    }
}
