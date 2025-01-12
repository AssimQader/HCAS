using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace HCAS.DL
{
    public class HCASDbContext : DbContext
    {
        public HCASDbContext(DbContextOptions<HCASDbContext> options) : base(options) { }

   
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<DoctorSchedule> DoctorsSchedules { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Payment>()
                 .HasOne(p => p.Patient)
                 .WithMany(p => p.Payments)
                 .HasForeignKey(p => p.PatientID)
                 .OnDelete(DeleteBehavior.NoAction);  //no action as i need the payment data of patient even if he is deleted from database


            //add non-clustere indexes//
            builder.Entity<Appointment>()
                .HasIndex(a => a.StartDateTime)
                .HasDatabaseName("IX_Appointment_StartDateTime");

            builder.Entity<Appointment>()
                .HasIndex(a => a.EndDateTime)
                .HasDatabaseName("IX_Appointment_EndDateTime");

            builder.Entity<Doctor>()
                .HasIndex(a => a.FullName)
                .HasDatabaseName("IX_Doctor_FullName");

            builder.Entity<Patient>()
               .HasIndex(a => a.FullName)
               .HasDatabaseName("IX_Patient_FullName");


            //add "Unique" Constraint to Prevent Overlapping Appointments for the Same Doctor
            builder.Entity<Appointment>()
                .HasIndex(a => new { a.DoctorID, a.StartDateTime, a.EndDateTime }) 
                .HasDatabaseName("IX_Appointment_Doctor_Overlap")
                .IsUnique();


            //add "Unique" Constraint to Prevent repeating schedules for the same Doctor
            builder.Entity<DoctorSchedule>()
                .HasIndex(a => new { a.DoctorID, a.DayOfWeek, a.StartTime })
                .HasDatabaseName("IX_DoctorSchedule_Overlap")
                .IsUnique();



            builder.Entity<Doctor>()
                .HasIndex(d => d.PhoneNumber)
                .HasDatabaseName("IX_Doctor_PhoneNum")
                .IsUnique();


            builder.Entity<Patient>()
                .HasIndex(p => p.PhoneNumber)
                .HasDatabaseName("IX_Patient_PhoneNum")
                .IsUnique();

        }
    }
}
