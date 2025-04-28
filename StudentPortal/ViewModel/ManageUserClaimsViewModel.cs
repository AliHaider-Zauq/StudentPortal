namespace StudentPortal.ViewModels
{
    public class ManageUserClaimsViewModel
    {
        public string UserId { get; set; } // ✅ Now using ViewModel instead of ViewBag
        public string Email { get; set; }  // ✅ User Email
        public List<UserClaim> Claims { get; set; } = new List<UserClaim>();
    }

    public class UserClaim
    {
        public string ClaimType { get; set; }
        public bool IsSelected { get; set; }
    }
}
