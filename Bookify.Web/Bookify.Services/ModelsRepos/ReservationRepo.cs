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
    public class ReservationRepo
    {


        AppDbContext dbContext;
        GenericRepository<Reservation> genericRepo;

        public ReservationRepo(AppDbContext context)
        {
            dbContext = context;
            var dbSet = dbContext.Set<Reservation>();
            genericRepo = new(dbContext);
        }

        public async Task<ResponseHelper<IEnumerable<Reservation>>> GetAll()
        {
            return await genericRepo.FindAll();
        }

        public async Task<ResponseHelper<Reservation>> Get(int id)
        {
            return await genericRepo.Find(r => r.Id == id);
        }

        public async Task<ResponseHelper> Add(Reservation booking)
        {
            return await genericRepo.Add(booking);
        }
        public async Task<ResponseHelper> Delete(Reservation booking)
        {
            return await genericRepo.Delete(booking);
        }
        public async Task<ResponseHelper> Update(Reservation booking)
        {
            return await genericRepo.Update(booking);
        }


    }
}
