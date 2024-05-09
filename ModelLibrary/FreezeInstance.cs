using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Model
{
    public class FreezeInstance
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int FreezeId { get; set; }
        public Freeze Freeze { get; set; } = null!;
        public int MembershipInstanceId { get; set; }
        public MembershipInstance MembershipInstance { get; set; } = null!;


    }
}
