using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCAS.DL;
using HCAS.DTO;

namespace HCAS.Services.DoctorScheduleServices
{
    public interface IDoctorScheduleServices
    {
        Task<List<DoctorScheduleDto>> GetAll();
        Task<DoctorScheduleDto> GetById(int id);
        Task<bool> AddSchedule(DoctorScheduleDto schedule);
        Task<bool> UpdateSchedule(DoctorScheduleDto schedule);
        Task<bool> DeleteSchedule(int id);
        Task<List<DoctorScheduleDto>> GetSchedulesByDoctorId(int doctorId);

        Task<bool> IsDoctorScheduledOnDay(int doctorId, string dayOfWeek);
        Task<bool> IsSlotAvailable(int doctorId, string day, TimeSpan startTime, TimeSpan endTime);
    }
}
