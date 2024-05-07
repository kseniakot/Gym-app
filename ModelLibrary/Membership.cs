using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gym.Model
{
    public class Membership
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int? Months { get; set; }
        public decimal? Price { get; set; }
        public int? FreezeId { get; set; }
       
        public Freeze? Freeze { get; set; } 
        
        public List<MembershipInstance> MembershipInstances { get; set; } = new List<MembershipInstance>();
    }
}
