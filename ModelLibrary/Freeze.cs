using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Model
{
    public class Freeze
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Days { get; set; }
        public decimal Price { get; set; }

        public List<FreezeInstance> FreezeInstances { get; set; }
    }
}
