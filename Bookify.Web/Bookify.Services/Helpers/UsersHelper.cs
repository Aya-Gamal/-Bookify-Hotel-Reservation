using Bookify.Data.Data;
using Bookify.Data.Models;
using Bookify.Services.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace Bookify.Services.Helpers
{
    public class UsersHelper
    {

        AppDbContext dbContext;
        DbSet<AspNetUsers> dbSet;
        GenericRepository<AspNetUsers> genericRepo;

        public UsersHelper(AppDbContext context)
        {
            dbContext = context;
            dbSet = dbContext.Set<AspNetUsers>();
            genericRepo = new(dbContext);
        }

        public async Task<ResponseHelper> CreateNewUser(string userName, string Password, string confirmPassword)
        {
            if (!IsEmail(userName))
            {
                return ResponseHelper.Fail("Invalid Username!");
            }

            var user = await genericRepo.Find(x => x.UserName == userName);

            if (user is not null)
            {
                return ResponseHelper.Fail("Username already exists");
            }

            var passwordHash = PasswordHelper.HashPassword(Password);
            var isVerified = PasswordHelper.VerifyPassword(passwordHash, confirmPassword);

            if (!isVerified)
            {
                return ResponseHelper.Fail("Password Missmatch!");
            }

            AspNetUsers newUser = new() { UserName = userName, PasswordHash = passwordHash };

            _ = await dbSet.AddAsync(newUser);

            _ = genericRepo.SaveChangesAsync();

            return ResponseHelper.OK();
        }


        public static bool IsEmail(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(username, pattern, RegexOptions.IgnoreCase);
        }
    }
}

