using Health_Clinic_Appointment_System.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace Health_Clinic_Appointment_System.Areas.Seeders
{

    public class UsersSeeder
    {
        private readonly UserManager<HCASIdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersSeeder(UserManager<HCASIdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedUsersAsync()
        {
            //create Admin User
            HCASIdentityUser adminUser = new() { UserName = "superAdmin", Email = "asem.adel00@gmail.com", EmailConfirmed = true };
            await CreateUserAsync(adminUser, "AsemAdel@3299", "Admin");
        }

        private async Task CreateUserAsync(HCASIdentityUser user, string password, string role)
        {
            if (await _userManager.FindByEmailAsync(user.Email) == null)
            {
                //add Admin user
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    //assign "Admin" role to him/her
                    if (await _roleManager.RoleExistsAsync(role))
                    {
                        await _userManager.AddToRoleAsync(user, role);
                    }
                }
            }
        }
    }
}
