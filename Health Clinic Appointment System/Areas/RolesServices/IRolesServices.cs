using Microsoft.AspNetCore.Identity;

namespace Health_Clinic_Appointment_System.Areas.RolesServices
{
    public interface IRolesServices
    {
        public Task<List<IdentityRole>> GetAllRolesAsync();
        public Task<IList<string>> GetRoleByUserIdAsync(string userId);
    }
}
