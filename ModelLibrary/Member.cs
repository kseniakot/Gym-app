using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace Gym.Model
{
    public class Member : User
    {
        //public List<MembershipInstance>? UserMemberships { get; set; }
        //public List<FreezeInstance>? UserFreezes { get; set;}

        public List<MembershipInstance>? UserMemberships { get; set; }
        [JsonIgnore]
        public List<WorkHour> Workouts { get; set; } = new List<WorkHour>();
        public Member()
        {
            UserMemberships = new List<MembershipInstance>();
        }

        public Member(User user)
        {
            this.Id = user.Id;
            this.Name = user.Name;
            this.PhoneNumber = user.PhoneNumber;
            this.Email = user.Email;
            this.Password = user.Password;
            this.IsBanned = user.IsBanned;
            this.ResetToken = user.ResetToken;
            this.ResetTokenCreationTime = user.ResetTokenCreationTime;
            this.Orders = user.Orders;
            UserMemberships = new List<MembershipInstance>();
            Workouts = new List<WorkHour>();
        }
    }

   
}
