using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Bookify.Data.Models 
{ 
    public class UserProfile 
    { 
        [Key] 
        public Guid Id { get; set; } 
        [Required] 
        public string UserId { get; set; } 
        [ForeignKey("UserId")] 
        public Microsoft.AspNetCore.Identity.IdentityUser User { get; set; } 
        [StringLength(100)] 
        public string? FullName { get; set; } 
        public string? Address { get; set; } 
        public DateTime? BirthDate { get; set; } 
    } 
}