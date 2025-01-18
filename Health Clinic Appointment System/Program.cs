using HCAS.DL;
using HCAS.Services.AppointmentServices;
using HCAS.Services.DoctorScheduleServices;
using HCAS.Services.DoctorServices;
using HCAS.Services.PatientServices;
using HCAS.Services.PaymentServices;
using Health_Clinic_Appointment_System.Areas.Identity.Data;
using Health_Clinic_Appointment_System.Areas.Seeders;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Health_Clinic_Appointment_System
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Connection strings
            var identityConnection = builder.Configuration.GetConnectionString("IdentityContextConnection")
                ?? throw new InvalidOperationException("Connection string 'IdentityContextConnection' not found.");
            var appConnection = builder.Configuration.GetConnectionString("ApplicationConnection")
                ?? throw new InvalidOperationException("Connection string 'ApplicationConnection' not found.");

            // Register database services
            builder.Services.AddDbContext<HCASDbContext>(options => options.UseSqlServer(appConnection));
            builder.Services.AddDbContext<HCASIdentityDbContext>(options => options.UseSqlServer(identityConnection));

            // Identity configuration (with roles)
            builder.Services.AddIdentity<HCASIdentityUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 12;
                options.Password.RequiredUniqueChars = 1;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<HCASIdentityDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();

            // Cookie authentication settings
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.Cookie.Name = "HCAS.Auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.ExpireTimeSpan = TimeSpan.FromHours(24);
                options.SlidingExpiration = true;
            });

            // Register HCAS services
            builder.Services.AddScoped<IPatientServices, PatientServices>();
            builder.Services.AddScoped<IDoctorServices, DoctorServices>();
            builder.Services.AddScoped<IAppointmentServices, AppointmentServices>();
            builder.Services.AddScoped<IPaymentServices, PaymentServices>();
            builder.Services.AddScoped<IDoctorScheduleServices, DoctorScheduleServices>();
            builder.Services.AddScoped<RolesSeeder>();
            builder.Services.AddScoped<UsersSeeder>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();



            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //database creation and seeding//
            using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                var DataContext = serviceScope.ServiceProvider.GetRequiredService<HCASDbContext>();
                DataContext.Database.EnsureCreated();

                var Identitycontext = serviceScope.ServiceProvider.GetRequiredService<HCASIdentityDbContext>();
                Identitycontext.Database.EnsureCreated();


                //Roles Seeding Logic
                var roleSeeder = serviceScope.ServiceProvider.GetRequiredService<RolesSeeder>();
                await roleSeeder.SeedRolesAsync();

                //Users Seeding Logic
                var usersSeeder = serviceScope.ServiceProvider.GetRequiredService<UsersSeeder>();
                await usersSeeder.SeedUsersAsync();
            }



            #region Database creation and seeding : bETTER WAY
            // Database creation and seeding : bETTER WAY //
            //using (var scope = app.Services.CreateScope())
            //{
            //    var dbContext = scope.ServiceProvider.GetRequiredService<HCASDbContext>();
            //    await dbContext.Database.MigrateAsync(); // Use migrations instead of EnsureCreated

            //    var identityContext = scope.ServiceProvider.GetRequiredService<HCASIdentityDbContext>();
            //    await identityContext.Database.MigrateAsync();

            //    // Seed roles and users
            //    var roleSeeder = scope.ServiceProvider.GetRequiredService<RolesSeeder>();
            //    await roleSeeder.SeedRolesAsync();

            //    var userSeeder = scope.ServiceProvider.GetRequiredService<UsersSeeder>();
            //    await userSeeder.SeedUsersAsync();
            //} 
            #endregion


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapRazorPages();

            //// Set default route to the Identity Login page
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapRazorPages();
            //    endpoints.MapFallbackToAreaPage("/Account/Login", "Identity");
            //});

            // Map the default route for Identity
            app.MapControllerRoute(
                name: "home",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            app.Run();
        }
    }
}
