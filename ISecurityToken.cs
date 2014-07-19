using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAF
{
    /// <summary>
    /// This interface provides security related information as User name and password on the time of session creation.
    /// </summary>
    /// <remarks>
    /// 1. Only one security token type register per session kind.
    /// 2. The token expires on session disposal.
    /// </remarks>
    public interface ISecurityToken
    {
        bool IsAuthenticated { get; }
        string User { get; }
        string Password { get; }
    }
}
