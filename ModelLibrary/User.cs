namespace Gym.Model
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
       // public List<FreezeInstance>? UserFreezes { get; set; }

        // FOR PASSWORD RESET
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenCreationTime { get; set; }

        // FOR PAYMENT
       // public string? PaymentId { get; set; }
       // public Payment? Payment { get; set; } = null;
       public List<Order> Orders { get; set; }


        public User()
        {
            Orders = new List<Order>();
        }

        public User(User user)
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
        }

    }
}
