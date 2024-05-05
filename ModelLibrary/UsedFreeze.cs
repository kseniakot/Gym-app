using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Model
{
    public class UsedFreeze
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;
        public int FreezeId { get; set; }
        public Freeze Freeze { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public int Days { get; set; }
        public int MembershipInstanceId { get; set; }  
        public MembershipInstance MembershipInstance { get; set; } = null!;
    }
}


