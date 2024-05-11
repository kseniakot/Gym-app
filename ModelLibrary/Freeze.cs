using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gym.Model
{
    public class Freeze
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Days { get; set; }
        public int? MinDays { get; set; }
        public decimal? Price { get; set; }
        [JsonIgnore]
        public List<Membership>? Memberships { get; set; }

        public List<FreezeInstance> FreezeInstances { get; set; }
    }
}
