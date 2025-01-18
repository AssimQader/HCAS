using HCAS.DL;
using HCAS.DTO;
using HCAS.Services.DoctorScheduleServices;
using HCAS.Services.DoctorServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Health_Clinic_Appointment_System.Controllers
{
    [Authorize(Roles = "Admin")] //only users with the "Admin" role can access this controller
    public class DoctorScheduleController : Controller
    {
        private readonly IDoctorScheduleServices _doctorScheduleService;

        public DoctorScheduleController(IDoctorScheduleServices doctorScheduleService)
        {
            _doctorScheduleService = doctorScheduleService;
        }


        public async Task<IActionResult> Index()
        {
            try
            {
                List<DoctorScheduleDto> schedules = await _doctorScheduleService.GetAll();
                return View(schedules);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(DoctorScheduleDto doctorScheduleDto)
        {
            if (!ModelState.IsValid)
                return View(doctorScheduleDto);

            try
            {
                await _doctorScheduleService.AddSchedule(doctorScheduleDto);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                DoctorScheduleDto schedule = await _doctorScheduleService.GetById(id);
                return View(schedule);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DoctorScheduleDto doctorScheduleDto)
        {
            if (!ModelState.IsValid)
                return View(doctorScheduleDto);

            try
            {
                await _doctorScheduleService.UpdateSchedule(doctorScheduleDto);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _doctorScheduleService.DeleteSchedule(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpGet]
        public async Task<IActionResult> IsAvailableDay(int id, string day)
        {
            try
            {
                bool isAvailable = await _doctorScheduleService.IsDoctorScheduledOnDay(id, day);

                if (isAvailable)
                {
                    return Json(new { exists = true, message = $"The doctor is scheduled on {day}." });
                }

                return Json(new { exists = false, message = $"The doctor is not scheduled on {day}." });
            }
            catch (Exception ex)
            {
                return Json(new { exists = false, message = $"An error occurred while checking the schedule: {ex.Message}" });
            }
        }



        [HttpGet]
        public async Task<IActionResult> CheckSlot(int docId, string day, string st, string et)
        {
            try
            {
                //convert input start and end times to TimeSpan
                TimeSpan startTime = TimeSpan.Parse(st);
                TimeSpan endTime = TimeSpan.Parse(et);

                bool isSlotAvailable = await _doctorScheduleService.IsSlotAvailable(docId, day, startTime, endTime);

                if (isSlotAvailable)
                {
                    return Json(new { exists = true, message = "The selected slot is available." });
                }

                return Json(new { exists = false, message = "The selected slot overlaps with an existing schedule." });
            }
            catch (Exception ex)
            {
                return Json(new { exists = false, message = $"An error occurred while checking the slot: {ex.Message}" });
            }
        }


    }
}
