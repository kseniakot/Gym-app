using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Model
{
    public class MembershipInstance
    {
        public int Id { get; set; }
        public int MembershipId { get; set; }
        public Membership Membership { get; set; } = null!;
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
