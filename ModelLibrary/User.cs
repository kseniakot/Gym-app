namespace Gym.Model
{
    public class User
    {
        public int Id { get; set; }
      // public string Id { get; set; }

        public bool IsBanned { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    
    }
}
