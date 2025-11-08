using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookify.Data.Data;
using Bookify.Data.Models;
using Bookify.Services.Generic;
using Bookify.Services.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Services.ModelsRepos
{
    public class RoomTypeRepo
    {


        AppDbContext dbContext;
        DbSet<RoomType> dbSet;
        GenericRepository<RoomType> genericRepo;

        public RoomTypeRepo(AppDbContext context)
        {
            dbContext = context;
            dbSet = dbContext.Set<RoomType>();
            genericRepo = new(dbContext);
        }

        public async Task<ResponseHelper<IEnumerable<RoomType>>> GetAll()
        {
            return await genericRepo.FindAll();
        }

        public async Task<ResponseHelper> Add(RoomType roomType)
        {
            return await genericRepo.Add(roomType);
        }
        public async Task<ResponseHelper> Delete(RoomType roomType)
        {
            return await genericRepo.Delete(roomType);
        }
        public async Task<ResponseHelper> Update(RoomType roomType)
        {
            return await genericRepo.Update(roomType);
        }
    }
}
