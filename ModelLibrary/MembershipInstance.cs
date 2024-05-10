using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Gym.Model
{
    public class MembershipInstance
    {
        public int Id { get; set; }
        public int MembershipId { get; set; }
        public Membership Membership { get; set; } = null!;
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; } = null!;

        public DateTime? PurchaseDate { get; set; } = DateTime.Today.ToUniversalTime();
        public DateTime? StartDate { get; set; }
       public DateTime? EndDate { get; set; }

        public Status Status { get; set; } = Status.Inactive;

        public FreezeActive? ActiveFreeze { get; set; }
        
    }
}
