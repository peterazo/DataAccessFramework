using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DAF.Query
{
    public class ParameterMapper
    {
        static private Dictionary<string, ParameterMapper> Mappers;

        static ParameterMapper()
        {
            Mappers = new Dictionary<string,ParameterMapper>();
        }

        static public ParameterMapper GetMapper(Type type)
        {
            ParameterMapper mapper;
            if (!Mappers.TryGetValue(type.FullName, out mapper))
            {
                mapper = new ParameterMapper(type);
                Mappers.Add(type.FullName, mapper);
            }
            return mapper;
        }

        public readonly Dictionary<string, Tuple<PropertyInfo, QueryParameterAttribute>> MappedProperties;

        private ParameterMapper(Type type)
        {
            MappedProperties = new Dictionary<string, Tuple<PropertyInfo, QueryParameterAttribute>>();
            CreateMapping(MappedProperties, type);
        }

        public void SetParameter(object query, string name, object value)
        {
            Tuple<PropertyInfo, QueryParameterAttribute> mapper;
            if (!MappedProperties.TryGetValue(name, out mapper))
            {
                throw new InvalidOperationException("Invalid query parameter : '" + name + "' (Please check spelling or specify QueryParameter attribute)");
            }

            mapper.Item1.SetValue(query, value, null);
        }

        public List<Tuple<string, object>> GetParameters(object query)
        {
            List<Tuple<string, object>> parameters = new List<Tuple<string,object>>();

            foreach (KeyValuePair<string, Tuple<PropertyInfo, QueryParameterAttribute>> mapping in MappedProperties)
            {
                PropertyInfo property = mapping.Value.Item1;
                QueryParameterAttribute attribute = mapping.Value.Item2;

                // Populate query parameter name.
                string name;
                if (string.IsNullOrEmpty(attribute.Name))
                {
                    name = mapping.Key;
                }
                else
                {
                    name = attribute.Name;
                }

                // Populate query parameter value.
                object value = property.GetValue(query, null);
                if (value == null)
                {
                    // Look for defaults
                    if (attribute.Default != null)
                    {
                        value = attribute.Default;
                    }
                    else if (!attribute.Optional)
                    {
                        throw new InvalidOperationException("Mandatory parameter '" + name + "' not set for query: " + query.GetType().FullName);
                    }
                }

                parameters.Add(Tuple.Create(name, value));
            }

            return parameters;
        }

        private void CreateMapping(Dictionary<string, Tuple<PropertyInfo, QueryParameterAttribute>> mappedProperties, Type type)
        {
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                QueryParameterAttribute queryParamAttribute =
                    System.Attribute.GetCustomAttribute(propertyInfo, typeof(QueryParameterAttribute), true) as QueryParameterAttribute;

                if (queryParamAttribute == null)
                    continue;

                // Check that we don't used default attributes on non nullable properties.
                if (queryParamAttribute.Default != null && propertyInfo.PropertyType.IsPrimitive)
                {
                    throw new InvalidOperationException(
                        "Can't use Default option on primitive parameter '" +
                        propertyInfo.Name +
                        "'. Use nullable types."
                    );
                }

                mappedProperties.Add(propertyInfo.Name, Tuple.Create(propertyInfo, queryParamAttribute));
            }
        }
    }
}
