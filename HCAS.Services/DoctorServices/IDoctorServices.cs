﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCAS.DTO;

namespace HCAS.Services.DoctorServices
{
    public interface IDoctorServices
    {
        Task<List<DoctorDto>> GetAll();
        Task<List<DoctorDto>> GetAllWithSchedules();
        Task<DoctorDto> GetById(int id);
        Task<bool> AddDoctor(DoctorDto doctorDto);
        Task<bool> UpdateDoctor(DoctorDto doctorDto);
        Task<bool> DeleteDoctor(int id);
        Task<List<AppointmentDto>> GetAppointmentsByDoctorId(int doctorId);
        Task<List<PatientDto>> GetPatientsByDoctorId(int doctorId);
        Task<bool> IsPhoneNumberExists(string phoneNum);
        Task<List<DoctorScheduleDto>> GetDocScheduleByDocId(int doctorId);
        Task<List<DoctorDto>> GetDocBySpecialization(string specialization);
    }
}
