using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCAS.DL;
using HCAS.DTO;
using HCAS.Services.Mapperly;
using Microsoft.EntityFrameworkCore;

namespace HCAS.Services.PatientServices
{
    public class PatientServices : IPatientServices
    {
        private readonly HCASDbContext _dbContext;

        public PatientServices(HCASDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PatientDto>> GetAll()
        {
            try
            {
                List<Patient> patients = await _dbContext.Patients.ToListAsync();
                if (patients == null || patients.Count == 0)
                    return [];

                return Mapper.Map(patients);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error fetching all patients.", ex);
            }
        }

        public async Task<PatientDto> GetById(int id)
        {
            try
            {
                Patient patient = await _dbContext.Patients.FindAsync(id);
                return patient == null ? throw new KeyNotFoundException($"Patient with ID {id} not found.") : Mapper.Map(patient);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error fetching patient by ID {id}.", ex);
            }
        }

        public async Task<int> AddPatient(PatientDto patientDto)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(patientDto);

                Patient patient = Mapper.Map(patientDto);
                await _dbContext.Patients.AddAsync(patient);
                int affectedRows = await _dbContext.SaveChangesAsync();
                return patient.ID;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error adding patient.", ex);
            }
        }

        public async Task<bool> UpdatePatient(PatientDto patientDto)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(patientDto);

                Patient patient = await _dbContext.Patients.FindAsync(patientDto.ID)
                    ?? throw new KeyNotFoundException($"Patient with ID {patientDto.ID} not found.");
               
                patient.FullName = patientDto.FullName;
                patient.Email = patientDto.Email;
                patient.PhoneNumber = patientDto.PhoneNumber;
                patient.Gender = patientDto.Gender;

                _dbContext.Patients.Update(patient);
                int affectedRows = await _dbContext.SaveChangesAsync();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating patient with ID {patientDto.ID}.", ex);
            }
        }

        public async Task<bool> DeletePatient(int id)
        {
            try
            {
                Patient patient = await _dbContext.Patients.FindAsync(id) 
                    ?? throw new KeyNotFoundException($"Patient with ID {id} not found.");

                _dbContext.Patients.Remove(patient);
                int affectedRows = await _dbContext.SaveChangesAsync();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting patient with ID {id}.", ex);
            }
        }


        public async Task<List<AppointmentDto>> GetAppointmentsByPatientId(int patientId)
        {
            try
            {
                List<Appointment> appointments = await _dbContext.Appointments
                    .Where(a => a.PatientID == patientId)
                    .ToListAsync();

                if (appointments == null || appointments.Count == 0)
                    return [];

                return Mapper.Map(appointments);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error fetching appointments for patient with ID {patientId}.", ex);
            }
        }




        public async Task<bool> IsPhoneNumberExists(string phoneNumber)
        {
            try
            {
                bool result = await _dbContext.Patients.AnyAsync(d => d.PhoneNumber == phoneNumber);
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error checking phone number existence: {ex.Message}", ex);
            }
        }

        public async Task<int> GetPatientIdByPhoneNumber(string phoneNumber)
        {
            try
            {
                var patientId = await _dbContext.Patients
                    .Where(d => d.PhoneNumber == phoneNumber)
                    .Select(d => d.ID)
                    .FirstOrDefaultAsync();

                return patientId == 0 ? 0 : patientId;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error fetching patient ID by phone number: {ex.Message}", ex);
            }
        }

    }
}
