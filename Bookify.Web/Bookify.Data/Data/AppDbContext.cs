using Bookify.Data.Models;
﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Bookify.Data.Models;

namespace Bookify.Data.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationItem> ReservationItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // RoomType → Room (1-to-many)
            builder.Entity<Room>()
                .HasOne(r => r.RoomType)
                .WithMany(rt => rt.Rooms)
                .HasForeignKey(r => r.RoomTypeId)
                 .OnDelete(DeleteBehavior.Cascade);

            // Room → Booking (1-to-many)
            builder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // User (AspNetUsers) → Booking (1-to-many)
            builder.Entity<Booking>()
                   .HasOne(b => b.User)
                   .WithMany()
                   .HasForeignKey(b => b.UserId)
                   .OnDelete(DeleteBehavior.Restrict);


            // Booking → Payment (1-to-1)
            builder.Entity<Payment>()
           .HasOne(p => p.Booking)
           .WithOne(b => b.Payment)
           .HasForeignKey<Payment>(p => p.BookingId)
           .OnDelete(DeleteBehavior.Cascade);


            // Store enums as strings
            builder.Entity<Booking>()
            .Property(b => b.Status)
            .HasConversion<string>();

            builder.Entity<Payment>()
            .Property(p => p.Status)
            .HasConversion<string>();

            // Decimal precision fixes
            builder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasPrecision(18, 2);   // total digits: 18, decimals: 2

            builder.Entity<RoomType>()
                .Property(rt => rt.PricePerNight)
                .HasPrecision(18, 2);


            // Reservation → ReservationItem: One-to-Many
            builder.Entity<Reservation>()
                .HasMany(r => r.Items)
                .WithOne(i => i.Reservation)
                .HasForeignKey(i => i.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);



           builder.Entity<RoomType>().HasData(
          new RoomType { Id = 1, Name = "Single Room", Description = "A cozy room for one guest.", PricePerNight = 100 },
          new RoomType { Id = 2, Name = "Double Room", Description = "A comfortable room for two guests.", PricePerNight = 130 },
          new RoomType { Id = 3, Name = "Suite", Description = "A luxurious suite with living area.", PricePerNight = 2000 },
          new RoomType { Id = 4, Name = "Premium King Room", Description = "A spacious room featuring a king - size bed, elegant decor, and a stunning city view — perfect for couples or business travelers.", PricePerNight = 159 },
          new RoomType { Id = 5, Name = "Family Room", Description = "A large and comfortable room designed for families, featuring multiple beds and modern amenities to ensure a pleasant stay for everyone.", PricePerNight = 299 },
          new RoomType { Id = 6, Name = "Deluxe Room", Description = "A spacious and elegant room featuring premium furnishings, modern amenities, and a beautiful view — perfect for guests seeking extra comfort and style.", PricePerNight = 198 }


  );
            builder.Entity<Room>().HasData(
            new Room { Id = 1, RoomNumber = "101", RoomTypeId = 1, IsAvailable = true },
             new Room { Id = 2, RoomNumber = "102", RoomTypeId = 1, IsAvailable = false },
             new Room { Id = 3, RoomNumber = "201", RoomTypeId = 2, IsAvailable = true },
             new Room { Id = 4, RoomNumber = "202", RoomTypeId = 2, IsAvailable = false },
             new Room { Id = 5, RoomNumber = "301", RoomTypeId = 3, IsAvailable = true },
             new Room { Id = 6, RoomNumber = "302", RoomTypeId = 3, IsAvailable = false },
             new Room { Id = 7, RoomNumber = "401", RoomTypeId = 4, IsAvailable = true },
             new Room { Id = 8, RoomNumber = "402", RoomTypeId = 4, IsAvailable = false },
             new Room { Id = 9, RoomNumber = "501", RoomTypeId = 5, IsAvailable = true },
             new Room { Id = 10, RoomNumber = "502", RoomTypeId = 5, IsAvailable = false },
             new Room { Id = 11, RoomNumber = "601", RoomTypeId = 6, IsAvailable = true }

         );
        }


    }
}

