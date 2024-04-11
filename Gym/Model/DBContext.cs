using Microsoft.EntityFrameworkCore;
//using Org.Apache.Http.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace Gym.Model
{
    public class DBContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Membership> Memberships { get; set; } = null!;
       // public DbSet<Freeze> Freezes { get; set; } = null!;

        public DBContext()
        {
            
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost ;Username=ksenia ;Password=pass ;Database=gym");
        }
    }
}
