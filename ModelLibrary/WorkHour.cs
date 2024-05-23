using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gym.Model
{
    public class WorkHour
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public int FirstMemberId { get; set; }
        [JsonIgnore]
       public List<Member> WorkHourClients { get; set; }
        [JsonIgnore]
        public int WorkDayId { get; set; }
        [JsonIgnore]
        public WorkDay WorkDay { get; set; } = null!;
        public bool IsAvailable { get; set; } = true;
    }
}
