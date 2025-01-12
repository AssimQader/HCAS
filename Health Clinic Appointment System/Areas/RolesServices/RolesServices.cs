using Health_Clinic_Appointment_System.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Health_Clinic_Appointment_System.Areas.RolesServices
{
    public class RolesServices : IRolesServices
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<HCASIdentityUser> _userManager;

        public RolesServices(RoleManager<IdentityRole> roleManager, UserManager<HCASIdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<IList<string>> GetRoleByUserIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            //get all roles assigned to the user
            return await _userManager.GetRolesAsync(user);
        }
    }

}
