using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty; // ✅ Provide default value

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty; // ✅ Provide default value

        public DateTime DateOfBirth { get; set; } 

        public string FullName => $"{FirstName} {LastName}"; // Computed Property
    }
}
