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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // RoomType → Room (1-to-many)
            builder.Entity<Room>()
                .HasOne(r => r.RoomType)
                .WithMany(rt =>rt.Rooms)
                .HasForeignKey(r => r.RoomTypeId)
                 .OnDelete(DeleteBehavior.Cascade);

            // Room → Booking (1-to-many)
            builder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany(r=> r.Bookings)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // User (AspNetUsers) → Booking (1-to-many)
            builder.Entity<Booking>()
                   .HasOne<Microsoft.AspNetCore.Identity.IdentityUser>() 
                   .WithMany()
                   .HasForeignKey(b => b.UserId)
                   .OnDelete(DeleteBehavior.Restrict);


            // Decimal precision fixes
            builder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasPrecision(18, 2);   // total digits: 18, decimals: 2

            builder.Entity<RoomType>()
                .Property(rt => rt.PricePerNight)
                .HasPrecision(18, 2);



        }

    }

}