using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Exceptions
{
    public class SessionExpiredException : Exception
    {
        public SessionExpiredException()
        : base("Your session has expired. Please sign in again.")
        { }
    }
}
