using Microsoft.AspNetCore.Identity;

namespace Health_Clinic_Appointment_System.Areas.Seeders
{
    public class RolesSeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesSeeder(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task SeedRolesAsync()
        {
            //define the roles to be added
            string[] roles = { "Admin", "Doctor", "Patient" };

            foreach (var role in roles)
            {
                //check if the role already exists
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    //create the role if it does not exist
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
