using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Health_Clinic_Appointment_System.Areas.Identity.Data;

namespace Health_Clinic_Appointment_System.Areas.UsersServices
{
    public interface IUsersServices
    {
        public Task<List<HCASIdentityUser>> GetAllUsersAsync();
        public Task<HCASIdentityUser> GetUserByIdAsync(string userId);

    }
}
