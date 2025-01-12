using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCAS.DTO;

namespace HCAS.Services.AppointmentServices
{
    public interface IAppointmentServices
    {
        Task<List<AppointmentDto>> GetAll();
        Task<AppointmentDto> GetById(int id);
        Task<bool> AddAppointment(AppointmentDto appointmentDto);
        Task<bool> UpdateAppointment(AppointmentDto appointmentDto);
        Task<bool> DeleteAppointment(int id);
        Task<PatientDto> GetPatientByAppointmentId(int appointmentId);
        Task<DoctorDto> GetDoctorByAppointmentId(int appointmentId);
        Task<List<AppointmentDto>> GetAppointmentDoctorPatientDetails();
    }
}
