using Bookify.Data.Data;
using Bookify.Data.Models;
using Bookify.Services.Generic;
using Bookify.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Services.ModelsRepos
{
    public class PaymentRepo
    {
        private readonly AppDbContext _context;
        private readonly GenericRepository<Payment> _repo;

        public PaymentRepo(AppDbContext context)
        {
            _context = context;
            _repo = new GenericRepository<Payment>(_context);
        }

        public async Task<ResponseHelper<IEnumerable<Payment>>> GetAll()
        {
            return await _repo.FindAll();
        }

        public async Task<ResponseHelper<Payment>> Get(int id)
        {
            return await _repo.Find(p=> p.Id == id);
        }

        public async Task<ResponseHelper> Add(Payment payment)
        {
            return await _repo.Add(payment);
        }

        public async Task<ResponseHelper> Update (Payment payment)
        {
            return await _repo.Update(payment);
        }

        public async Task<ResponseHelper> Delete(Payment payment)
        {
            return await _repo.Delete(payment);
        }
    }
}
