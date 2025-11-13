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
    public class BookingRepo
    {


        AppDbContext dbContext;
        DbSet<Booking> dbSet;
        GenericRepository<Booking> genericRepo;

        public BookingRepo(AppDbContext context)
        {
            dbContext = context;
            dbSet = dbContext.Set<Booking>();
            genericRepo = new(dbContext);
        }

        public async Task<ResponseHelper<IEnumerable<Booking>>> GetAll()
        {
            return await genericRepo.FindAll();
        }

        public async Task<ResponseHelper> Add(Booking booking)
        {
            return await genericRepo.Add(booking);
        }
        public async Task<ResponseHelper> Delete(Booking booking)
        {
            return await genericRepo.Delete(booking);
        }
        public async Task<ResponseHelper> Update(Booking booking)
        {
            return await genericRepo.Update(booking);
        }


    }
}
