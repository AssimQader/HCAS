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
            var identityConnectionCommand = builder.Configuration.GetConnectionString("IdentityContextConnection") ?? throw new InvalidOperationException("Connection string 'IdentityContextConnection' not found.");
            var appConnectionCommand = builder.Configuration.GetConnectionString("ApplicationConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationConnection' not found.");

            //resolve database services: HCASDbContext & HCASIdentityDbContext
            builder.Services.AddDbContext<HCASDbContext>(options => options.UseSqlServer(appConnectionCommand));
            builder.Services.AddDbContext<HCASIdentityDbContext>(options => options.UseSqlServer(identityConnectionCommand));

            //include Roles with identity
            builder.Services.AddIdentity<HCASIdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<HCASIdentityDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();

            
            //HCAS Services
            builder.Services.AddScoped(typeof(IPatientServices), typeof(PatientServices));
            builder.Services.AddScoped(typeof(IDoctorServices), typeof(DoctorServices));
            builder.Services.AddScoped(typeof(IAppointmentServices), typeof(AppointmentServices));
            builder.Services.AddScoped(typeof(IPaymentServices), typeof(PaymentServices));
            builder.Services.AddScoped(typeof(IDoctorScheduleServices), typeof(DoctorScheduleServices));





            // Add services to the container //

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddControllers(); //api controllers
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddHttpClient();

            //configure what happend when user is not authenticated:
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.Cookie.Name = "HCAS";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(24);
                options.LoginPath = "/Identity/Account/Login";
                // ReturnUrlParameter requires 
                //using Microsoft.AspNetCore.Authentication.Cookies;
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });

            //configure password settings
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireUppercase = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 12;
                options.Password.RequiredUniqueChars = 1;
            });


            //add RolesSeeder & UsersSeeder classes as scooped (DI)
            builder.Services.AddScoped<RolesSeeder>();
            builder.Services.AddScoped<UsersSeeder>();
            builder.Services.AddHttpContextAccessor();





            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
