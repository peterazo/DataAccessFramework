using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Reflection;

using DAF.Query;

namespace DAF.SQL
{
    static class SqlParameterMapper
    {
        static public void MapTo<T>(this ParameterMapper mapper, T entity, IDbCommand command) where T : class
        {
            IDataParameterCollection parameters = command.Parameters;
            foreach (Tuple<string, object> entityParam in mapper.GetParameters(entity))
            {
                IDbDataParameter sqlParam = command.CreateParameter();

                string name = entityParam.Item1;
                if (command.CommandType == CommandType.StoredProcedure && name[0] != '@')
                {
                    sqlParam.ParameterName = '@' + name;
                }
                else
                {
                    sqlParam.ParameterName = name;
                }

                object value = entityParam.Item2;

                if (value != null)
                {
                    sqlParam.Value = value;
                }
                else
                {
                    sqlParam.Value = DBNull.Value;
                }

                command.Parameters.Add(sqlParam);
            }
        }
    }
}
