using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace Gym.Model
{
    public class Trener : User
    {
        [JsonIgnore]
        public List<WorkDay> WorkDays { get; set; } = new List<WorkDay>();
    }
}
