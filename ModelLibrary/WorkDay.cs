using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gym.Model
{
    public class WorkDay
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; } 
        public int TrenerId { get; set; }
        [JsonIgnore]
        public Trener Trener { get; set; } = null!;
        [JsonIgnore]
        public List<WorkHour> WorkHours { get; set; } = null!;
    }
}
