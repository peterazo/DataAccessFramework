using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace DAF.Core
{
    public class EntityMapper
    {
        private static Dictionary<string, EntityMapper> mappers;

        static EntityMapper()
        {
            mappers = new Dictionary<string, EntityMapper>();
        }

        public static EntityMapper GetMapper(Type type)
        {
            EntityMapper mapper;
            string key = type.FullName;
            if (!mappers.TryGetValue(key, out mapper))
            {
                mapper = new EntityMapper(type);
                mappers.Add(key, mapper);
            }
            return mapper;
        }

        public Dictionary<string, PropertyInfo> Properties { get; private set; }

        public EntityMapper(Type type)
        {
            CreateMapping(type);
        }

        private void CreateMapping(Type type)
        {
            Properties = type.GetProperties().ToDictionary(x => x.Name);
        }
    }
}
