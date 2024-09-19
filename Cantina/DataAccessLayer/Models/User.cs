using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    public class User : IdentityUser
    {
        [Required]
        [PersonalData]
        public string? FirstName { get; set; }
        [Required]
        [PersonalData]
        public string? LastName { get; set; }
        [PersonalData]
        public string? Location { get; set; }
    }
}
