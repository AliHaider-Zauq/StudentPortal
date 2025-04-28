using System.Collections.Generic;

namespace StudentPortal.ViewModels
{
    public class ManageRoleUsersViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<UserRoleInfo> Users { get; set; } = new List<UserRoleInfo>();
    }

    public class UserRoleInfo
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
        public string ExistingRole { get; set; } // New field to store existing role
    }
}
