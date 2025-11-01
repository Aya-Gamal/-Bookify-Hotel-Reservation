using Bookify.Data.Data;
using Bookify.Data.Models;
using Bookify.Services.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Services.ModelsRepos
{
    public class RoomRepo
    {
        AppDbContext dbContext;
        DbSet<Room> dbSet;
        GenericRepository<Room> genericRepo;

        public RoomRepo(AppDbContext context)
        {
            dbContext = context;
            dbSet = dbContext.Set<Room>();
            genericRepo = new(dbContext);
        }

        public async Task<Response<IEnumerable<Room>>> GetRoomsWithoutReservations()
        {
            return await genericRepo.FindAll(x => x.IsAvailable);
        }

        public async Task<Response<IEnumerable<Room>>> GetRoomsWithoutReservations(RoomType roomType)
        {
            return await genericRepo.FindAll(x => x.IsAvailable && x.RoomTypeId == roomType.Id);
        }

        public async Task<Response<IEnumerable<Room>>> GetAvailableRoomsByDate(DateTime checkIn, DateTime checkOut)
        {
            var bookedRoomIds = await dbContext.Bookings
                .Where(b => b.Status != BookingStatus.Cancelled &&
                            checkIn < b.CheckOutDate &&
                            checkOut > b.CheckInDate)
                .Select(b => b.RoomId)
                .ToListAsync();

            var availableRooms = await dbContext.Rooms
                .Include(r => r.RoomType)
                .Where(r => !bookedRoomIds.Contains(r.Id) && r.IsAvailable)
                .GroupBy(r => r.Id)   
                .Select(g => g.First())
                .ToListAsync();

            return Response<IEnumerable<Room>>.Ok(availableRooms);
        }

        public async Task<Response<IEnumerable<Room>>> GetAllRooms()
        {
            var rooms = await dbContext.Rooms
                .Include(r => r.RoomType)
                .ToListAsync();

            return Response<IEnumerable<Room>>.Ok(rooms);
        }


    }
}
