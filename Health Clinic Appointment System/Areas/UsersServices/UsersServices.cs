using Health_Clinic_Appointment_System.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Health_Clinic_Appointment_System.Areas.UsersServices
{
    public class UsersServices : IUsersServices
    {
        private readonly UserManager<HCASIdentityUser> _userManager;


        public UsersServices(UserManager<HCASIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<HCASIdentityUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }


        public async Task<HCASIdentityUser> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                return user;
            }

            return null;
        }
    }
}
