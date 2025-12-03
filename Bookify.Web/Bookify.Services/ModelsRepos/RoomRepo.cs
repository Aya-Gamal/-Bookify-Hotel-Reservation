using Azure;
using Bookify.Data.Data;
using Bookify.Data.Models;
using Bookify.Services.Generic;
using Bookify.Services.Helpers;
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

        public async Task<ResponseHelper<IEnumerable<Room>>> GetRoomsWithoutReservations()
        {
            return await genericRepo.FindAll(x => x.IsAvailable);
        }

        public async Task<ResponseHelper<IEnumerable<Room>>> GetRoomsWithoutReservations(RoomType roomType)
        {
            return await genericRepo.FindAll(x => x.IsAvailable && x.RoomTypeId == roomType.Id);
        }

        public async Task<ResponseHelper<IEnumerable<Room>>> GetRoomsWithReservations()
        {
            var rooms = await dbContext.Rooms
                .Include(r => r.RoomType)
                .ToListAsync();

            return ResponseHelper<IEnumerable<Room>>.Ok(rooms);
        }

        public async Task<ResponseHelper<IEnumerable<Room>>> GetAllRooms()
        {


                var rooms = await dbContext.Rooms
                .Include(x => x.RoomType)
                .Include(r => r.ReservationItems)
                .ToListAsync();  
            return ResponseHelper<IEnumerable<Room>>.Ok(rooms);


        }

        public async Task<ResponseHelper<Room>> GetRoomById(int id)
        {
            var room = await dbContext.Rooms
       .Include(r => r.RoomType)
       .Include(r => r.ReservationItems)    
       .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
                return ResponseHelper<Room>.Fail("Room not found.");

            return ResponseHelper<Room>.Ok(room);
        }


        public async Task<ResponseHelper> Add(Room room)
        {
            return await genericRepo.Add(room);
        }
        public async Task<ResponseHelper> Delete(Room room)
        {
            return await genericRepo.Delete(room);
        }
        public async Task<ResponseHelper> Update(Room room)
        {
            return await genericRepo.Update(room);
        }
        //========


        // Get rooms according availbility dates
        public async Task<IEnumerable<Room>> GetRoomsWithBookingStatus(DateTime checkIn, DateTime checkOut)
        {
            var rooms = await dbContext.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.ReservationItems)
                .ToListAsync();

            foreach (var room in rooms)
            {
                
                var hasConflict = room.ReservationItems.Any(res =>
                    checkIn < res.CheckOut &&
                    checkOut > res.CheckIn
                );

                room.IsAvailable = !hasConflict;
                room.ReservationItems = room.ReservationItems
                    .Where(res => checkIn < res.CheckOut && checkOut > res.CheckIn)
                    .ToList();
            }

            return rooms;
        }





    }
}
