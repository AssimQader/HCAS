using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCAS.DL;
using HCAS.DTO;
using HCAS.Services.Mapperly;
using Microsoft.EntityFrameworkCore;

namespace HCAS.Services.DoctorServices
{
    public class DoctorServices : IDoctorServices
    {
        private readonly HCASDbContext _dbContext;

        public DoctorServices(HCASDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DoctorDto>> GetAll()
        {
            try
            {
                List<Doctor> doctors = await _dbContext.Doctors.ToListAsync();
                if (doctors == null || doctors.Count == 0)
                    return [];

                return Mapper.Map(doctors);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error fetching all doctors.", ex);
            }
        }

        public async Task<List<DoctorDto>> GetAllWithSchedules()
        {
            try
            {
                var doctors = await _dbContext.Doctors
                    .Include(d => d.DoctorSchedules)
                    .Select(d => new DoctorDto
                    {
                        ID = d.ID,
                        FullName = d.FullName,
                        Specialization = d.Specialization,
                        Email = d.Email,
                        PhoneNumber = d.PhoneNumber,
                        DoctorSchedules = d.DoctorSchedules.Select(s => new DoctorScheduleDto
                        {
                            DayOfWeek = s.DayOfWeek,
                            StartTime = DateTime.Today.Add(s.StartTime).ToString("hh:mm tt"),
                            EndTime = DateTime.Today.Add(s.EndTime).ToString("hh:mm tt")
                        }).ToList()
                    })
                    .ToListAsync();

                return doctors;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error fetching all doctors with schedules.", ex);
            }
        }

        public async Task<DoctorDto> GetById(int id)
        {
            try
            {
                Doctor doctor = await _dbContext.Doctors.FindAsync(id);
                return doctor == null 
                    ? throw new KeyNotFoundException($"Doctor with ID {id} not found.")
                    : Mapper.Map(doctor);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error fetching doctor by ID {id}.", ex);
            }
        }

        public async Task<bool> AddDoctor(DoctorDto doctorDto)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(doctorDto);

                Doctor doctor = Mapper.Map(doctorDto);
                await _dbContext.Doctors.AddAsync(doctor);
                int affectedRows = await _dbContext.SaveChangesAsync();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error adding doctor.", ex);
            }
        }

        public async Task<bool> UpdateDoctor(DoctorDto doctorDto)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(doctorDto);

                Doctor doctor = await _dbContext.Doctors.FindAsync(doctorDto.ID)
                    ?? throw new KeyNotFoundException($"Doctor with ID {doctorDto.ID} not found.");

                doctor.FullName = doctorDto.FullName;
                doctor.Email = doctorDto.Email;
                doctor.PhoneNumber = doctorDto.PhoneNumber;
                doctor.Specialization = doctorDto.Specialization;

                _dbContext.Doctors.Update(doctor);
                int affectedRows = await _dbContext.SaveChangesAsync();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating doctor with ID {doctorDto.ID}.", ex);
            }
        }

        public async Task<bool> DeleteDoctor(int id)
        {
            try
            {
                Doctor doctor = await _dbContext.Doctors.FindAsync(id)
                    ?? throw new KeyNotFoundException($"Doctor with ID {id} not found.");

                _dbContext.Doctors.Remove(doctor);
                int affectedRows = await _dbContext.SaveChangesAsync();

                //return true if rows were affected
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting doctor with ID {id}.", ex);
            }
        }


        //Fetches all appointments for a specific doctor
        public async Task<List<AppointmentDto>> GetAppointmentsByDoctorId(int doctorId)
        {
            try
            {
                List<Appointment> appointments = await _dbContext.Appointments
                    .Where(a => a.DoctorID == doctorId)
                    .ToListAsync();

                if (appointments == null || appointments.Count == 0)
                    return [];

                return Mapper.Map(appointments);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error fetching appointments for doctor with ID {doctorId}.", ex);
            }
        }


        //Fetches a distinct list of patients associated with the doctor's appointments
        public async Task<List<PatientDto>> GetPatientsByDoctorId(int doctorId)
        {
            try
            {
                var patients = await _dbContext.Appointments
                    .Include(a => a.Patient)
                    .Where(a => a.DoctorID == doctorId)
                    .Select(a => a.Patient)
                    .Distinct()
                    .ToListAsync();

                if (patients == null || patients.Count == 0)
                    return [];

                return Mapper.Map(patients);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error fetching patients for doctor with ID {doctorId}.", ex);
            }
        }


        public async Task<bool> IsPhoneNumberExists(string phoneNumber)
        {
            try
            {
                bool result = await _dbContext.Doctors.AnyAsync(d => d.PhoneNumber == phoneNumber);
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error checking phone number existence: {ex.Message}", ex);
            }
        }


        public async Task<List<DoctorScheduleDto>> GetDocScheduleByDocId(int doctorId)
        {
            try
            {
                var schedules = await _dbContext.DoctorsSchedules
                    .Where(ds => ds.DoctorID == doctorId)
                    .Select(ds => new DoctorScheduleDto
                    {
                        DayOfWeek = ds.DayOfWeek,
                        StartTime = DateTime.Today.Add(ds.StartTime).ToString("hh:mm tt"), // Convert TimeSpan to DateTime as "TimeSpan" does not support the "hh:mm tt" format with AM/PM directly. 
                        EndTime = DateTime.Today.Add(ds.EndTime).ToString("hh:mm tt")
                    })
                    .ToListAsync();

                return schedules;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error fetching all doctors with schedules.", ex);
            }
        }


        public async Task<List<DoctorDto>> GetDocBySpecialization(string specialization)
        {
            try
            {
                var schedules = await _dbContext.Doctors
                    .Where(d => d.Specialization == specialization)
                    .Select(d => new DoctorDto
                    {
                       ID = d.ID,
                       FullName = d.FullName
                    })
                    .ToListAsync();

                return schedules;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error fetching all doctors with schedules.", ex);
            }
        }




    }
}
