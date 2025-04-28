using System.Collections.Generic;

namespace StudentPortal.Models
{
    public class UserClaims
    {
        public static List<string> AllClaims = new List<string>
        {
            ClaimTypesConstants.CreateRoles,
            ClaimTypesConstants.DeleteRoles,
            ClaimTypesConstants.EditRoles,
            ClaimTypesConstants.CreateStudent,
             ClaimTypesConstants.DeleteStudent

        };
    }
}
