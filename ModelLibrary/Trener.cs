using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Gym.Model
{
    public class Trener : User
    {
        public List<WorkDay> WorkDays { get; set; } = new List<WorkDay>();
    }
}
