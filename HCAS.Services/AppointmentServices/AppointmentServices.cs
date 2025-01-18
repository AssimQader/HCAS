using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCAS.DL;
using HCAS.DTO;
using HCAS.Services.Mapperly;
using Microsoft.EntityFrameworkCore;

namespace HCAS.Services.AppointmentServices
{
    public class AppointmentServices : IAppointmentServices
    {
        private readonly HCASDbContext _dbContext;

        public AppointmentServices(HCASDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<AppointmentDto>> GetAll()
        {
            try
            {
                List<Appointment> appointments = await _dbContext.Appointments.ToListAsync();
                if (appointments == null || appointments.Count == 0)
                    return [];

                return Mapper.Map(appointments);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error fetching all appointments.", ex);
            }
        }

        public async Task<AppointmentDto> GetById(int id)
        {
            try
            {
                Appointment appointment = await _dbContext.Appointments.FindAsync(id);
                return appointment == null 
                    ? throw new KeyNotFoundException($"Appointment with ID {id} not found.") 
                    : Mapper.Map(appointment);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error fetching appointment by ID {id}.", ex);
            }
        }

        public async Task<bool> AddAppointment(AppointmentDto appointmentDto)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(appointmentDto);

                Appointment appointment = Mapper.Map(appointmentDto);
                await _dbContext.Appointments.AddAsync(appointment);
                int affectedRows = await _dbContext.SaveChangesAsync();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error adding appointment.", ex);
            }
        }

        public async Task<bool> UpdateAppointment(AppointmentDto appointmentDto)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(appointmentDto);

                Appointment appointment = await _dbContext.Appointments.FindAsync(appointmentDto.ID)
                    ?? throw new KeyNotFoundException($"Appointment with ID {appointmentDto.ID} not found.");

                appointment.PatientID = appointmentDto.PatientID;
                appointment.DoctorID = appointmentDto.DoctorID;
                appointment.StartDateTime = appointmentDto.StartDateTime;
                appointment.EndDateTime = appointmentDto.EndDateTime;
                appointment.Status = appointmentDto.Status;
                appointment.PaymentStatus = appointmentDto.PaymentStatus;

                _dbContext.Appointments.Update(appointment);
                int affectedRows = await _dbContext.SaveChangesAsync();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating appointment with ID {appointmentDto.ID}.", ex);
            }
        }

        public async Task<bool> DeleteAppointment(int id)
        {
            try
            {
                Appointment appointment = await _dbContext.Appointments.FindAsync(id)
                    ?? throw new KeyNotFoundException($"Appointment with ID {id} not found.");

                _dbContext.Appointments.Remove(appointment);
                int affectedRows = await _dbContext.SaveChangesAsync();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting appointment with ID {id}.", ex);
            }
        }



        //Fetches the patient associated with the given appointment
        public async Task<PatientDto> GetPatientByAppointmentId(int appointmentId)
        {
            try
            {
                Appointment appointment = await _dbContext.Appointments
                    .Include(a => a.Patient)
                    .FirstOrDefaultAsync(a => a.ID == appointmentId)
                    ?? throw new KeyNotFoundException($"Appointment with ID {appointmentId} not found.");

                return Mapper.Map(appointment.Patient);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error fetching patient for appointment with ID {appointmentId}.", ex);
            }
        }


        //Fetches the doctor associated with the given appointment
        public async Task<DoctorDto> GetDoctorByAppointmentId(int appointmentId)
        {
            try
            {
                Appointment appointment = await _dbContext.Appointments
                    .Include(a => a.Doctor)
                    .FirstOrDefaultAsync(a => a.ID == appointmentId)
                    ?? throw new KeyNotFoundException($"Appointment with ID {appointmentId} not found.");

                return Mapper.Map(appointment.Doctor);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error fetching doctor for appointment with ID {appointmentId}.", ex);
            }
        }

        public async Task<bool> IsAppointmentSlotAvailable(int doctorId, DateTime startDateTime, DateTime endDateTime)
        {
            var conflictingAppointments = await _dbContext.Appointments
                .Where(a => a.DoctorID == doctorId &&
                            a.StartDateTime < endDateTime && //where start date of an existing appointment is before another ending date fo the new one
                            a.EndDateTime > startDateTime) //where end date of an existing appointment is after another starting date fo the new one
                .ToListAsync();

            return conflictingAppointments.Count == 0;
        }


        public async Task<List<AppointmentDto>> GetAppointmentDoctorPatientDetails()
        {
            try
            {
                var appointments = await _dbContext.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .Select(a => new AppointmentDto
                    {
                        ID = a.ID,
                        StartDateTime = a.StartDateTime,
                        EndDateTime = a.EndDateTime,
                        Status = a.Status,
                        PaymentStatus = a.PaymentStatus,
                        Patient = new PatientDto
                        {
                            FullName = a.Patient.FullName
                        },
                        Doctor = new DoctorDto
                        {
                            FullName = a.Doctor.FullName
                        }
                    })
                    .ToListAsync();

                return appointments;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error fetching payments with patient and appointment details.", ex);
            }
        }



        public async Task<bool> IsAppointmentExists(int doctorId, DateTime startDateTime, DateTime endDateTime)
        {
            try
            {
                return await _dbContext.Appointments.AnyAsync(app =>
                    app.DoctorID == doctorId &&
                    app.StartDateTime.Date == startDateTime.Date && // Ensure it's the same date
                    (
                        (startDateTime.TimeOfDay >= app.StartDateTime.TimeOfDay && startDateTime.TimeOfDay < app.EndDateTime.TimeOfDay) || // Overlaps at the start
                        (endDateTime.TimeOfDay > app.StartDateTime.TimeOfDay && endDateTime.TimeOfDay <= app.EndDateTime.TimeOfDay) ||   // Overlaps at the end
                        (startDateTime.TimeOfDay <= app.StartDateTime.TimeOfDay && endDateTime.TimeOfDay >= app.EndDateTime.TimeOfDay)  // Fully contains an existing appointment
                    ));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while checking for overlapping appointments.", ex);
            }
        }



    }
}
