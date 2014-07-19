using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAF
{
    public interface ICachePool : IDisposable
    {
        bool IsCache { get; }
        ICache GetCache();
    }
}
