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
            return _context.Users.ToList();
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
        public void DeleteUser(User user)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        public bool IsUserExist(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }
    }
}
