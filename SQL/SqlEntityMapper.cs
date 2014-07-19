using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Reflection;

using DAF.Core;

namespace DAF.SQL
{
    // SQL based extension of the entity mapper.
    static class SqlEntityMapper
    {
        static public void MapTo<T>(this EntityMapper mapper, IDataReader reader, T entity) where T : class
        {
            if (mapper.Properties == null)
                throw new InvalidOperationException("Property map not defined");

            // populate fields
            for (int ord = 0; ord < reader.FieldCount; ++ord)
            {
                string fieldName = reader.GetName(ord);
                PropertyInfo property;
                if (!mapper.Properties.TryGetValue(fieldName, out property))
                    continue;

                if (property.CanWrite)
                    mapper.Properties[fieldName].SetValue(entity, reader.GetValue(ord), null);
            }
        }

    }
}
