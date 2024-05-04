using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Model
{
    public class FreezeInstance
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }
        public int FreezeId { get; set; }
        public Freeze Freeze { get; set; }
        public int MembershipInstanceId { get; set; }
        public MembershipInstance MembershipInstance { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
