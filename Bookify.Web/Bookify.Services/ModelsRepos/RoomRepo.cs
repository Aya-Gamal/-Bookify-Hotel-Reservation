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
    }
}
