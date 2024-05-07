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
        public DbSet<Member> Members { get; set; } = null!;
        public DbSet<Membership> Memberships { get; set; } = null!;
        public DbSet<MembershipInstance> MembershipInstances { get; set; } = null!;
        public DbSet<Freeze> Freezes { get; set; } = null!;
        public DbSet<FreezeInstance> FreezeInstances { get; set; } = null!;
        public DbSet<FreezeActive> ActiveFreezes { get; set; } = null!;

        public DBContext(DbContextOptions<DBContext> options)
        : base(options)
        {
           // Database.EnsureCreated();   // создаем базу данных при первом обращении
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<MembershipInstance>()
        //         .HasOne(m => m.Member)
        //         .WithMany(m => m.UserMemberships)
        //         .HasForeignKey(m => m.MemberId);

        //    modelBuilder.Entity<MembershipInstance>()
        //        .HasOne(m => m.Membership)
        //        .WithMany(m => m.MembershipInstances)
        //        .HasForeignKey(m => m.MembershipId);

        //    modelBuilder.Entity<Freeze>()
        //        .


        //}

    }
}
