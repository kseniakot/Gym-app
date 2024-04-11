using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Model
{
    public class Membership
    {
        public int Id { get; set; }
        public string Name { get; set; }    
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
       
        public int? Months { get; set; }
        public decimal? Price { get; set; }
        public bool IsActive { get; set; }

    }
}
