using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gym.Model;

namespace Gym.Services
{
    public class DataBaseService
    {
        private readonly DBContext _context;

        public DataBaseService(DBContext context)
        {
            _context = context;
        }


        //Create
        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        // Read
        public List<User> GetAllUsers()
        {
           
                return _context.Users.Where(user => !(user.Name == "admin")).ToList();
            
        }

        //public User GetUserById(int id)
        //{
        //    return _context.Users.Find(id);
        //}

        // Update
        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        // Delete
        public async void DeleteUser(User user)
        {
           _context.Users.Remove(user);
           await _context.SaveChangesAsync();
        }

        public bool IsUserExist(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public async void BanUser(User user)
        {
            user.IsBanned = true;
           await _context.SaveChangesAsync();
        }

        public async void UnbanUser(User user)
        {
            user.IsBanned = false;
           await _context.SaveChangesAsync();
        }

        public List<User> GetBannedUsers()
        {
            return _context.Users.Where(user => user.IsBanned).ToList();
        }

        public List<User> GetUnbannedUsers()
        {
            return _context.Users.Where(user => !user.IsBanned && !(user.Name == "admin")).ToList();
        }

        public bool IsBannedByEmail(string email)
        {
            return _context.Users.Any(u => u.Email == email && u.IsBanned);
        }

        public List<Membership> GetAllMemberships()
        {
            return _context.Memberships.ToList();
        }

        public void AddMembership(Membership membership)
        {
            _context.Memberships.Add(membership);
            _context.SaveChanges();
        }

        public void DeleteMembership(Membership membership)
        {
            _context.Memberships.Remove(membership);
            _context.SaveChanges();
        }  
        
        public bool IsMembershipExist(Membership membership)
        {
            return _context.Memberships.Any(m => m.Name == membership.Name &&
                                            m.Price == membership.Price && m.Months == membership.Months);
        }

       

        public Membership GetMembershipById(int id)
        {
            return _context.Memberships.Find(id);
        }

        public void EditMembership(Membership membership)
        {
            _context.Memberships.Update(membership);
            _context.SaveChanges();
        }

        public bool IsPasswordCorrect(string email, string password)
        {
            return _context.Users.Any(u => u.Email == email && u.Password == password);
        }
    }
}
