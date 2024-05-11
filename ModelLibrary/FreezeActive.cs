using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gym.Model
{
    public class FreezeActive
    {
        public int Id { get; set; }
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
        public int DaysLeft { get; set; }   
        [JsonIgnore]
        public int MembershipInstanceId { get; set; }
        [JsonIgnore]
        public MembershipInstance MembershipInstance { get; set; } = null!;
    }
}
