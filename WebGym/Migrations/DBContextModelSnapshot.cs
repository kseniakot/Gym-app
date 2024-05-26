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

                    b.Property<int?>("Days")
                        .HasColumnType("integer");

                    b.Property<int?>("MinDays")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal?>("Price")
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

                    b.ToTable("FreezeInstance");
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

            modelBuilder.Entity("Gym.Model.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Capture")
                        .HasColumnType("boolean");

                    b.Property<int?>("MembershipId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("MembershipId");

                    b.HasIndex("UserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Gym.Model.Payment", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("OrderId")
                        .HasColumnType("integer");

                    b.Property<bool>("Paid")
                        .HasColumnType("boolean");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("Payments");
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

                    b.Property<string>("ResetToken")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ResetTokenCreationTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasDiscriminator<string>("Discriminator").HasValue("User");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Gym.Model.WorkDay", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("TrenerId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TrenerId");

                    b.ToTable("WorkDays");
                });

            modelBuilder.Entity("Gym.Model.WorkHour", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Capacity")
                        .HasColumnType("integer");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("Start")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("WorkDayId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WorkDayId");

                    b.ToTable("WorkHours");
                });

            modelBuilder.Entity("MemberWorkHour", b =>
                {
                    b.Property<int>("WorkHourClientsId")
                        .HasColumnType("integer");

                    b.Property<int>("WorkoutsId")
                        .HasColumnType("integer");

                    b.HasKey("WorkHourClientsId", "WorkoutsId");

                    b.HasIndex("WorkoutsId");

                    b.ToTable("MemberWorkHour");
                });

            modelBuilder.Entity("Gym.Model.Member", b =>
                {
                    b.HasBaseType("Gym.Model.User");

                    b.HasDiscriminator().HasValue("Member");
                });

            modelBuilder.Entity("Gym.Model.Trener", b =>
                {
                    b.HasBaseType("Gym.Model.User");

                    b.HasDiscriminator().HasValue("Trener");
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
                        .WithMany()
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

                    b.HasOne("Gym.Model.Member", "User")
                        .WithMany("UserMemberships")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Membership");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Gym.Model.Order", b =>
                {
                    b.HasOne("Gym.Model.Membership", "Membership")
                        .WithMany("Orders")
                        .HasForeignKey("MembershipId");

                    b.HasOne("Gym.Model.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Gym.Model.Amount", "Amount", b1 =>
                        {
                            b1.Property<int>("OrderId")
                                .HasColumnType("integer");

                            b1.Property<string>("Currency")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("OrderId");

                            b1.ToTable("Orders");

                            b1.WithOwner()
                                .HasForeignKey("OrderId");
                        });

                    b.OwnsOne("Gym.Model.Redirection", "Confirmation", b1 =>
                        {
                            b1.Property<int>("OrderId")
                                .HasColumnType("integer");

                            b1.Property<string>("Return_url")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Type")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("OrderId");

                            b1.ToTable("Orders");

                            b1.WithOwner()
                                .HasForeignKey("OrderId");
                        });

                    b.Navigation("Amount")
                        .IsRequired();

                    b.Navigation("Confirmation")
                        .IsRequired();

                    b.Navigation("Membership");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Gym.Model.Payment", b =>
                {
                    b.HasOne("Gym.Model.Order", "Order")
                        .WithOne("Payment")
                        .HasForeignKey("Gym.Model.Payment", "OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Gym.Model.Confirmation", "Confirmation", b1 =>
                        {
                            b1.Property<string>("PaymentId")
                                .HasColumnType("text");

                            b1.Property<string>("Confirmation_url")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Type")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("PaymentId");

                            b1.ToTable("Payments");

                            b1.WithOwner()
                                .HasForeignKey("PaymentId");
                        });

                    b.OwnsOne("Gym.Model.Amount", "Amount", b1 =>
                        {
                            b1.Property<string>("PaymentId")
                                .HasColumnType("text");

                            b1.Property<string>("Currency")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("PaymentId");

                            b1.ToTable("Payments");

                            b1.WithOwner()
                                .HasForeignKey("PaymentId");
                        });

                    b.Navigation("Amount")
                        .IsRequired();

                    b.Navigation("Confirmation")
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Gym.Model.WorkDay", b =>
                {
                    b.HasOne("Gym.Model.Trener", "Trener")
                        .WithMany("WorkDays")
                        .HasForeignKey("TrenerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Trener");
                });

            modelBuilder.Entity("Gym.Model.WorkHour", b =>
                {
                    b.HasOne("Gym.Model.WorkDay", "WorkDay")
                        .WithMany("WorkHours")
                        .HasForeignKey("WorkDayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WorkDay");
                });

            modelBuilder.Entity("MemberWorkHour", b =>
                {
                    b.HasOne("Gym.Model.Member", null)
                        .WithMany()
                        .HasForeignKey("WorkHourClientsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Gym.Model.WorkHour", null)
                        .WithMany()
                        .HasForeignKey("WorkoutsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Gym.Model.Freeze", b =>
                {
                    b.Navigation("FreezeInstances");

                    b.Navigation("Memberships");
                });

            modelBuilder.Entity("Gym.Model.Membership", b =>
                {
                    b.Navigation("MembershipInstances");

                    b.Navigation("Orders");
                });

            modelBuilder.Entity("Gym.Model.MembershipInstance", b =>
                {
                    b.Navigation("ActiveFreeze");
                });

            modelBuilder.Entity("Gym.Model.Order", b =>
                {
                    b.Navigation("Payment");
                });

            modelBuilder.Entity("Gym.Model.User", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("Gym.Model.WorkDay", b =>
                {
                    b.Navigation("WorkHours");
                });

            modelBuilder.Entity("Gym.Model.Member", b =>
                {
                    b.Navigation("UserMemberships");
                });

            modelBuilder.Entity("Gym.Model.Trener", b =>
                {
                    b.Navigation("WorkDays");
                });
#pragma warning restore 612, 618
        }
    }
}
