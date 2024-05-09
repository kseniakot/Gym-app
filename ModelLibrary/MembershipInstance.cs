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
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime? PurchaseDate { get; set; } = DateTime.Now.ToUniversalTime();
        public DateTime? StartDate { get; set; }

        public FreezeActive? ActiveFreeze { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
