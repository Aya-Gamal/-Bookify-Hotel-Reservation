using Bookify.Data.Models;
using System;
using System.Collections.Generic;

namespace Bookify.Web.Models
{
    public class ProfileViewModel
    {
        public Guid ProfileId { get; set; }

        public string UserId { get; set; }

        public string FullName { get; set; }

        public string Address { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Email { get; set; } // From AspNetUsers
        public string PhoneNumber { get; set; } // From IdentityUser.PhoneNumber

        public List<Reservation> Reservations { get; set; } = new();

        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; }
    }
}
