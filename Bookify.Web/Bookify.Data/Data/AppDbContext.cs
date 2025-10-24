using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
                   .HasOne<Microsoft.AspNetCore.Identity.IdentityUser>()
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

            builder.Entity<RoomType>().HasData(
      new RoomType { Id = 1, Name = "Single Room", Description = "A cozy room for one guest.", PricePerNight = 800 },
      new RoomType { Id = 2, Name = "Double Room", Description = "A comfortable room for two guests.", PricePerNight = 1200 },
      new RoomType { Id = 3, Name = "Suite", Description = "A luxurious suite with living area.", PricePerNight = 2000 }
  );
            builder.Entity<Room>().HasData(
     new Room { Id = 1, RoomNumber = "101", RoomTypeId = 1, IsAvailable = true },
     new Room { Id = 2, RoomNumber = "102", RoomTypeId = 1, IsAvailable = true },
     new Room { Id = 3, RoomNumber = "201", RoomTypeId = 2, IsAvailable = true },
     new Room { Id = 4, RoomNumber = "202", RoomTypeId = 2, IsAvailable = false }, 
     new Room { Id = 5, RoomNumber = "301", RoomTypeId = 3, IsAvailable = true },
     new Room { Id = 6, RoomNumber = "302", RoomTypeId = 3, IsAvailable = false }  
 );
        }


    }
}

