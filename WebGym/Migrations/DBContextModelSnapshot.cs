﻿// <auto-generated />
using System;
using Gym.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebGym.Migrations
{
    [DbContext(typeof(DBContext))]
    partial class DBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Gym.Model.Freeze", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Days")
                        .HasColumnType("integer");

                    b.Property<int>("MinDays")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("Freezes");
                });

            modelBuilder.Entity("Gym.Model.FreezeActive", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("DaysLeft")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("MembershipInstanceId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("MembershipInstanceId")
                        .IsUnique();

                    b.ToTable("ActiveFreezes");
                });

            modelBuilder.Entity("Gym.Model.FreezeInstance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("FreezeId")
                        .HasColumnType("integer");

                    b.Property<int>("MembershipInstanceId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FreezeId");

                    b.HasIndex("MembershipInstanceId");

                    b.HasIndex("UserId");

                    b.ToTable("FreezeInstances");
                });

            modelBuilder.Entity("Gym.Model.Membership", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("FreezeId")
                        .HasColumnType("integer");

                    b.Property<int?>("Months")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal?>("Price")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("FreezeId");

                    b.ToTable("Memberships");
                });

            modelBuilder.Entity("Gym.Model.MembershipInstance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("MembershipId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("PurchaseDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("MembershipId");

                    b.HasIndex("UserId");

                    b.ToTable("MembershipInstances");
                });

            modelBuilder.Entity("Gym.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("character varying(8)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsBanned")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasDiscriminator<string>("Discriminator").HasValue("User");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Gym.Model.Member", b =>
                {
                    b.HasBaseType("Gym.Model.User");

                    b.HasDiscriminator().HasValue("Member");
                });

            modelBuilder.Entity("Gym.Model.FreezeActive", b =>
                {
                    b.HasOne("Gym.Model.MembershipInstance", "MembershipInstance")
                        .WithOne("ActiveFreeze")
                        .HasForeignKey("Gym.Model.FreezeActive", "MembershipInstanceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MembershipInstance");
                });

            modelBuilder.Entity("Gym.Model.FreezeInstance", b =>
                {
                    b.HasOne("Gym.Model.Freeze", "Freeze")
                        .WithMany("FreezeInstances")
                        .HasForeignKey("FreezeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Gym.Model.MembershipInstance", "MembershipInstance")
                        .WithMany()
                        .HasForeignKey("MembershipInstanceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Gym.Model.User", "User")
                        .WithMany("UserFreezes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Freeze");

                    b.Navigation("MembershipInstance");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Gym.Model.Membership", b =>
                {
                    b.HasOne("Gym.Model.Freeze", "Freeze")
                        .WithMany("Memberships")
                        .HasForeignKey("FreezeId");

                    b.Navigation("Freeze");
                });

            modelBuilder.Entity("Gym.Model.MembershipInstance", b =>
                {
                    b.HasOne("Gym.Model.Membership", "Membership")
                        .WithMany("MembershipInstances")
                        .HasForeignKey("MembershipId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Gym.Model.User", "User")
                        .WithMany("UserMemberships")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Membership");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Gym.Model.Freeze", b =>
                {
                    b.Navigation("FreezeInstances");

                    b.Navigation("Memberships");
                });

            modelBuilder.Entity("Gym.Model.Membership", b =>
                {
                    b.Navigation("MembershipInstances");
                });

            modelBuilder.Entity("Gym.Model.MembershipInstance", b =>
                {
                    b.Navigation("ActiveFreeze");
                });

            modelBuilder.Entity("Gym.Model.User", b =>
                {
                    b.Navigation("UserFreezes");

                    b.Navigation("UserMemberships");
                });
#pragma warning restore 612, 618
        }
    }
}
