using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Model
{
    public class FreezeActive
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysLeft { get; set; }

        public int MembershipInstanceId { get; set; }
        public MembershipInstance MembershipInstance { get; set; } = null!;
    }
}
