using Bookify.Data.Data;
using Bookify.Data.Models;
using Bookify.Services.Generic;
using Bookify.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Services.ModelsRepos
{
    public class ReservationItemRepo
    {
        private readonly AppDbContext _context;
        private readonly GenericRepository<ReservationItem> _genericRepo;
        
        public ReservationItemRepo(AppDbContext context)
        {
            _context = context;
            _genericRepo = new GenericRepository<ReservationItem>(_context);
        }

        public async Task<ResponseHelper<IEnumerable<ReservationItem>>> GetAll()
        {
            return await _genericRepo.FindAll();
        }

        public async Task<ResponseHelper> Add(ReservationItem reservationItem)
        {
            return await _genericRepo.Add(reservationItem);
        }

        public async Task<ResponseHelper> Delete(ReservationItem reservationItem)
        {
            return await _genericRepo.Delete(reservationItem);
        }

        public async Task<ResponseHelper> Update(ReservationItem reservationItem)
        {
            return await _genericRepo.Update(reservationItem);
        }
    }
}
