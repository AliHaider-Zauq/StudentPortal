using System.ComponentModel.DataAnnotations;

namespace StudentPortal.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
        public List<string> UserClaims { get; set; } = new List<string>();
    }
}
