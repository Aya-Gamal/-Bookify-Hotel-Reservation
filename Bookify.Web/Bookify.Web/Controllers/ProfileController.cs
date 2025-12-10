using Bookify.Data.Data;
using Bookify.Data.Models;
using Bookify.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Bookify.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppDbContext _db;

        public ProfileController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            AppDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        // GET: Profile (shows profile form + booking history)

        [HttpGet]
        public async Task<IActionResult> Index(string search, int page = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            // Load profile
            var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (profile == null)
            {
                profile = new UserProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id
                };

                _db.UserProfiles.Add(profile);
                await _db.SaveChangesAsync();
            }
            var reservationsQuery = _db.Reservations
     .Include(r => r.Items)
         .ThenInclude(i => i.Room)
             .ThenInclude(rt => rt.RoomType)
     .Where(r => r.UserId == User.Identity.Name)
     .AsQueryable();   // important

            if (!string.IsNullOrEmpty(search))
            {
                reservationsQuery = reservationsQuery.Where(r =>
                    r.Items.Any(i =>
                        EF.Functions.Like(i.Room.RoomNumber, $"%{search}%") ||
                        EF.Functions.Like(i.Room.RoomType.Name, $"%{search}%")
                    )
                );
            }

            // Default ordering AFTER filtering
            reservationsQuery = reservationsQuery.OrderByDescending(r => r.CreatedAt);





            var pageSize = 10;  // Number of reservations per page
            var totalReservations = await reservationsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalReservations / pageSize);

            // Fetch reservations with pagination
            var reservations = await reservationsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new ProfileViewModel
            {
                ProfileId = profile.Id,
                UserId = user.Id,
                FullName = profile.FullName,
                Address = profile.Address,
                BirthDate = profile.BirthDate,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Reservations = reservations,
                SearchTerm = search,
                PageIndex = page,
                TotalPages = totalPages
            };

            return View("~/Views/Profile/Index.cshtml", model);
        }

        // POST: Update profile (writes to UserProfile + AspNetUsers.PhoneNumber)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (profile == null) return NotFound();

            // Update profile fields
            bool changed = false;

            if (profile.FullName != model.FullName)
            {
                profile.FullName = model.FullName;
                changed = true;
            }

            if (profile.Address != model.Address)
            {
                profile.Address = model.Address;
                changed = true;
            }

            if (profile.BirthDate != model.BirthDate)
            {
                profile.BirthDate = model.BirthDate;
                changed = true;
            }

            if (changed)
            {
                _db.UserProfiles.Update(profile);
            }

            // Update phone on AspNetUsers (store phone on IdentityUser.PhoneNumber)
            if (user.PhoneNumber != model.PhoneNumber)
            {
                user.PhoneNumber = model.PhoneNumber;
                var usrRes = await _userManager.UpdateAsync(user);
                if (!usrRes.Succeeded)
                {
                    TempData["Error"] = string.Join("; ", usrRes.Errors.Select(e => e.Description));
                    return RedirectToAction(nameof(Index));
                }
            }

            if (changed)
            {
                await _db.SaveChangesAsync();
            }

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Change Password page
        public IActionResult ChangePassword()
        {
            return View("~/Views/Profile/ChangePassword.cshtml", new ChangePasswordViewModel());
        }

        // POST: Change password in AspNetUsers and redirect with success message
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Profile/ChangePassword.cshtml", vm);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var check = await _userManager.CheckPasswordAsync(user, vm.CurrentPassword);
            if (!check)
            {
                ModelState.AddModelError(nameof(vm.CurrentPassword), "Current password is incorrect.");
                return View("~/Views/Profile/ChangePassword.cshtml", vm);
            }

            if (vm.CurrentPassword == vm.NewPassword)
            {
                ModelState.AddModelError(nameof(vm.NewPassword), "New password must be different from current password.");
                return View("~/Views/Profile/ChangePassword.cshtml", vm);
            }

            // Validate with identity validators to produce friendly errors
            var pwdErrors = new List<string>();
            foreach (var validator in _userManager.PasswordValidators)
            {
                var res = await validator.ValidateAsync(_userManager, user, vm.NewPassword);
                if (!res.Succeeded) pwdErrors.AddRange(res.Errors.Select(e => e.Description));
            }
            if (pwdErrors.Any())
            {
                foreach (var e in pwdErrors.Distinct()) ModelState.AddModelError(nameof(vm.NewPassword), e);
                return View("~/Views/Profile/ChangePassword.cshtml", vm);
            }

            var result = await _userManager.ChangePasswordAsync(user, vm.CurrentPassword, vm.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
                return View("~/Views/Profile/ChangePassword.cshtml", vm);
            }

            await _signInManager.RefreshSignInAsync(user);
            // Set TempData success message so Index can display it
            TempData["Success"] = "Password changed successfully.";
            return RedirectToAction(nameof(Index));
        }
        
        // POST: Cancel booking
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var booking = await _db.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == user.Id);

            if (booking == null) return NotFound();

            if (booking.CheckInDate <= DateTime.UtcNow)
            {
                TempData["Error"] = "Cannot cancel past or ongoing bookings.";
                return RedirectToAction(nameof(Index));
            }

            _db.Bookings.Remove(booking);

            if (booking.Room != null)
            {
                booking.Room.IsAvailable = true;
                _db.Rooms.Update(booking.Room);
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "Booking cancelled.";
            return RedirectToAction(nameof(Index));
        }
        */
        // GET: Delete Account confirm
        public IActionResult DeleteAccount()
        {
            return View("~/Views/Profile/DeleteAccount.cshtml", new DeleteAccountViewModel());
        }
        /*

        // POST: Delete Account
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount(DeleteAccountViewModel vm)
        {
            if (!ModelState.IsValid) return View("~/Views/Profile/DeleteAccount.cshtml", vm);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            if (!await _userManager.CheckPasswordAsync(user, vm.Password))
            {
                ModelState.AddModelError("", "Password is incorrect.");
                return View("~/Views/Profile/DeleteAccount.cshtml", vm);
            }

            // Delete bookings
            var bookings = await _db.Bookings.Where(b => b.UserId == user.Id).ToListAsync();
            if (bookings.Any()) _db.Bookings.RemoveRange(bookings);

            // Delete profile
            var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (profile != null) _db.UserProfiles.Remove(profile);

            await _db.SaveChangesAsync();

            // Delete user
            var delRes = await _userManager.DeleteAsync(user);
            if (!delRes.Succeeded)
            {
                ModelState.AddModelError("", "Failed to delete account.");
                return View("~/Views/Profile/DeleteAccount.cshtml", vm);
            }

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        */

    }
}
