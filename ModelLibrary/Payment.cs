using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gym.Model
{
    public class Payment
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public bool Paid { get; set; } = false;
        [JsonIgnore]
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}
