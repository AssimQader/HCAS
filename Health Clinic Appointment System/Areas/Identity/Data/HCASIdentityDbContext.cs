using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Health_Clinic_Appointment_System.Areas.Identity.Data;

public class HCASIdentityDbContext : IdentityDbContext<HCASIdentityUser>
{
    public HCASIdentityDbContext(DbContextOptions<HCASIdentityDbContext> options) : base(options)
    {

    }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
