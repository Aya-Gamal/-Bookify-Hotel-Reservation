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
        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            const int PageSize = 10;
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Load or create profile
            var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (profile == null)
            {
                profile = new UserProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    FullName = null,
                    Address = null,
                    BirthDate = null
                };
                _db.UserProfiles.Add(profile);
                await _db.SaveChangesAsync();
            }

            // Bookings query
            var baseQuery = _db.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomType)
                .Where(b => b.UserId == user.Id);

            if (!string.IsNullOrWhiteSpace(search))
            {
                baseQuery = baseQuery.Where(b =>
                    b.Room.RoomNumber.Contains(search) ||
                    b.Room.RoomType.Name.Contains(search));
            }

            var total = await baseQuery.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
            page = Math.Clamp(page, 1, totalPages);

            var bookings = await baseQuery
                .OrderByDescending(b => b.Id)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(b => new BookingHistoryItem
                {
                    BookingId = b.Id,
                    RoomNumber = b.Room.RoomNumber,
                    RoomType = b.Room.RoomType.Name,
                    CheckIn = b.CheckInDate,
                    CheckOut = b.CheckOutDate,
                    TotalPrice = b.TotalPrice,
                    CanCancel = b.CheckInDate > DateTime.UtcNow
                })
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
                BookingHistory = bookings,
                PageIndex = page,
                TotalPages = totalPages,
                SearchTerm = search
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

        // GET: Delete Account confirm
        public IActionResult DeleteAccount()
        {
            return View("~/Views/Profile/DeleteAccount.cshtml", new DeleteAccountViewModel());
        }

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
    }
}
