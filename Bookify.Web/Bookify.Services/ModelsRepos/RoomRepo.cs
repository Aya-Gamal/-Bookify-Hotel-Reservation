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
                .ToListAsync();  
            return ResponseHelper<IEnumerable<Room>>.Ok(rooms);


        }

        public async Task<ResponseHelper<IEnumerable<Room>>> GetAvailableRoomsByDate(DateTime checkin, DateTime checkout)
        {
            var rooms = await dbContext.Rooms
                .Include(r => r.Bookings)
                .Where(r => !r.Bookings.Any(b =>
                    b.CheckInDate < checkout && b.CheckOutDate > checkin)) 
                .ToListAsync();

            return ResponseHelper<IEnumerable<Room>>.Ok(rooms);
        }

        public async Task<ResponseHelper<Room>> GetRoomById(int id)
        {
            var roomres = await genericRepo.Find(x => x.Id == id);

            return ResponseHelper<Room>.Ok(roomres.Data);
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

    }
}
