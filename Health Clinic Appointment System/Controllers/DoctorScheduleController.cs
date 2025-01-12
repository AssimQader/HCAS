using HCAS.DL;
using HCAS.DTO;
using HCAS.Services.DoctorScheduleServices;
using HCAS.Services.DoctorServices;
using Microsoft.AspNetCore.Mvc;

namespace Health_Clinic_Appointment_System.Controllers
{
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
    }
}
