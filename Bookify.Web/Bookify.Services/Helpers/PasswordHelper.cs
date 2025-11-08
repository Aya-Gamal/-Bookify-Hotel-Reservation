using Microsoft.AspNetCore.Identity;

namespace Bookify.Services.Helpers
{
    public class PasswordHelper
    {
        // Hash a password using ASP.NET Identity
        public static string HashPassword(string plainPassword)
        {
            var hasher = new PasswordHasher<IdentityUser>();
            string hashedPassword = hasher.HashPassword(null, plainPassword);
            return hashedPassword;
        }

        // Verify a plain password against a stored hash
        public static bool VerifyPassword(string hashedPassword, string plainPassword)
        {
            var hasher = new PasswordHasher<IdentityUser>();
            var result = hasher.VerifyHashedPassword(null, hashedPassword, plainPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}