using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAF
{
    /// <summary>
    /// Provides restrictions and expiration policies
    /// for the objects in the cache
    /// NOTE: This class can be replaced in the future
    /// to the one provided by the Microsoft Enterprise Library
    /// </summary>
    public class CachePolicy
    {
        public static readonly CachePolicy NoPolicy;

        static CachePolicy()
        {
            NoPolicy = new CachePolicy();
        }

        public DateTime AbsoluteExpiration { get; set; }

        public bool IsAbsoluteExpiration
        {
            get { return (this.AbsoluteExpiration != DateTime.MaxValue); }
        }

        public CachePolicy()
        {
            this.AbsoluteExpiration = DateTime.MaxValue;
        }
    }
}
