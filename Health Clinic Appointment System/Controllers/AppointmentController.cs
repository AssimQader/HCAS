using HCAS.DTO;
using HCAS.Services.AppointmentServices;
using Microsoft.AspNetCore.Mvc;

namespace Health_Clinic_Appointment_System.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentServices _appointmentService;

        public AppointmentController(IAppointmentServices appointmentService)
        {
            _appointmentService = appointmentService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                List<AppointmentDto> appointments = await _appointmentService.GetAll();
                return View(appointments);
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
        public async Task<IActionResult> Create(AppointmentDto appointmentDto)
        {
            if (!ModelState.IsValid)
                return View(appointmentDto);

            try
            {
                await _appointmentService.AddAppointment(appointmentDto);
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
                AppointmentDto appointment = await _appointmentService.GetById(id);
                return View(appointment);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Edit(AppointmentDto appointmentDto)
        {
            if (!ModelState.IsValid)
                return View(appointmentDto);

            try
            {
                await _appointmentService.UpdateAppointment(appointmentDto);
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
                await _appointmentService.DeleteAppointment(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
