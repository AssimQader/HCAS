﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCAS.DL;
using HCAS.DTO;
using HCAS.Services.Mapperly;
using Microsoft.EntityFrameworkCore;

namespace HCAS.Services.DoctorScheduleServices
{
    public class DoctorScheduleServices : IDoctorScheduleServices
    {
        private readonly HCASDbContext _dbContext;

        public DoctorScheduleServices(HCASDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        //get shcedules with doctors data(full name)
        public async Task<List<DoctorScheduleDto>> GetAll()
        {
            try
            {
                List<DoctorScheduleDto> schedules = await _dbContext.DoctorsSchedules
                    .Include(ds => ds.Doctor)
                    .Select(ds => new DoctorScheduleDto
                    {
                        DayOfWeek = ds.DayOfWeek,
                        StartTime = DateTime.Today.Add(ds.StartTime).ToString("hh:mm tt"),
                        EndTime = DateTime.Today.Add(ds.EndTime).ToString("hh:mm tt"),
                        ID = ds.ID,
                        Doctor = new DoctorDto
                        {
                            FullName = ds.Doctor.FullName
                        }
                    })
                    .ToListAsync();
                if (schedules == null || schedules.Count == 0)
                    return [];

                return schedules;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error fetching all doctor schedules.", ex);
            }
        }

        public async Task<DoctorScheduleDto> GetById(int id)
        {
            try
            {
                DoctorSchedule schedule = await _dbContext.DoctorsSchedules.FindAsync(id);
                return schedule == null
                    ? throw new KeyNotFoundException($"Schedule with ID {id} not found.")
                    : Mapper.Map(schedule);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error fetching schedule by ID {id}.", ex);
            }
        }

        public async Task<bool> AddSchedule(DoctorScheduleDto scheduleDto)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(scheduleDto);

                DoctorSchedule schedule = Mapper.Map(scheduleDto);
                await _dbContext.DoctorsSchedules.AddAsync(schedule);
                int affectedRows = await _dbContext.SaveChangesAsync();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error adding doctor schedule.", ex);
            }
        }

        public async Task<bool> UpdateSchedule(DoctorScheduleDto scheduleDto)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(scheduleDto);

                DoctorSchedule schedule = await _dbContext.DoctorsSchedules.FindAsync(scheduleDto.ID)
                    ?? throw new KeyNotFoundException($"Schedule with ID {scheduleDto.ID} not found.");

                schedule.DayOfWeek = scheduleDto.DayOfWeek;
                schedule.StartTime = TimeSpan.Parse(DateTime.ParseExact(scheduleDto.StartTime, "hh:mm tt", CultureInfo.InvariantCulture).ToString("HH:mm:ss"));
                schedule.EndTime = TimeSpan.Parse(DateTime.ParseExact(scheduleDto.EndTime, "hh:mm tt", CultureInfo.InvariantCulture).ToString("HH:mm:ss"));



                _dbContext.DoctorsSchedules.Update(schedule);
                int affectedRows = await _dbContext.SaveChangesAsync();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating schedule with ID {scheduleDto.ID}.", ex);
            }
        }

        public async Task<bool> DeleteSchedule(int id)
        {
            try
            {
                DoctorSchedule schedule = await _dbContext.DoctorsSchedules.FindAsync(id)
                    ?? throw new KeyNotFoundException($"Schedule with ID {id} not found.");

                _dbContext.DoctorsSchedules.Remove(schedule);
                int affectedRows = await _dbContext.SaveChangesAsync();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting schedule with ID {id}.", ex);
            }
        }


        // Fetches all schedules for a specific doctor
        public async Task<List<DoctorScheduleDto>> GetSchedulesByDoctorId(int doctorId)
        {
            try
            {
                List<DoctorSchedule> schedules = await _dbContext.DoctorsSchedules
                    .Where(s => s.DoctorID == doctorId)
                    .ToListAsync();

                if (schedules == null || schedules.Count == 0)
                    return [];

                return Mapper.Map(schedules);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error fetching schedules for doctor with ID {doctorId}.", ex);
            }
        }


        public async Task<bool> IsDoctorScheduledOnDay(int doctorId, string dayOfWeek)
        {
            try
            {
                return await _dbContext.DoctorsSchedules
                    .AnyAsync(ds => ds.DoctorID == doctorId && ds.DayOfWeek.Equals(dayOfWeek));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error checking doctor schedule availability.", ex);
            }
        }


        public async Task<bool> IsSlotAvailable(int doctorId, string day, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                //check if the provided slot is strictly within the existing schedule for the doctor
                return await _dbContext.DoctorsSchedules
                    .AnyAsync(ds =>
                        ds.DoctorID == doctorId &&
                        ds.DayOfWeek == day &&
                        startTime >= ds.StartTime && 
                        endTime <= ds.EndTime
                    );
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error checking doctor slot availability.", ex);
            }
        }


    }
}
