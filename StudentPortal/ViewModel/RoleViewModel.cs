using System.ComponentModel.DataAnnotations;

namespace StudentPortal.ViewModels
{
    public class RoleViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Role name must be between 3 and 50 characters.")]
        public string RoleName { get; set; }
    }
}
