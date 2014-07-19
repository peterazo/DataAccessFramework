using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAF
{
    [AttributeUsage(AttributeTargets.Property,
                    Inherited = true,
                    AllowMultiple = false)]
    public class QueryParameterAttribute : Attribute
    {
        public string Name { get; set; }

        public object Default { get; set; }

        public bool Optional { get; set; }
    }
}
