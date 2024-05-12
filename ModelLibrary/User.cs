﻿namespace Gym.Model
{
    public class User
    {
        public int Id { get; set; }
      // public string Id { get; set; }

        public bool IsBanned { get; set; } = false;
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public List<MembershipInstance>? UserMemberships { get; set; }
        public List<FreezeInstance>? UserFreezes { get; set; }

        // FOR PASSWORD RESET
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenCreationTime { get; set; }
    }
}
