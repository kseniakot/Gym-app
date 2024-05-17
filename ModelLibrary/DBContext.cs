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
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Payment>()
            //    .HasOne(p => p.Confirmation)
            //    .WithOne(c => c.Payment)
            //    .HasForeignKey<Confirmation>(c => c.PaymentId);

            //modelBuilder.Entity<Payment>()
            //       .HasOne(p => p.Amount)
            //       .WithOne(a => a.Payment)
            //       .HasForeignKey<Amount>(a => a.PaymentId);

            modelBuilder.Entity<Payment>().OwnsOne(p => p.Amount);
            modelBuilder.Entity<Payment>().OwnsOne(p => p.Confirmation);
            modelBuilder.Entity<Order>().OwnsOne(p => p.Confirmation);
            modelBuilder.Entity<Order>().OwnsOne(p => p.Amount);
        }


    }
}
