using HCAS.DL;
using HCAS.DTO;
using HCAS.Services.AppointmentServices;
using Health_Clinic_Appointment_System.ViewModels;
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
                List<AppointmentDto> appointments = await _appointmentService.GetAppointmentDoctorPatientDetails();
                List<AppointmentViewModel> AppointmentDetails = [];

                foreach (AppointmentDto appointment in appointments)
                {
                    AppointmentDetails.Add(new AppointmentViewModel()
                    {
                        PatientName = appointment.Patient.FullName,
                        DoctorName = appointment.Doctor.FullName,
                        AppointmentStatus = appointment.Status,
                        StartDateTime = appointment.StartDateTime,
                        EndDateTime = appointment.EndDateTime,
                        PaymentStatus = appointment.PaymentStatus,
                        AppointmentID = appointment.ID
                    });
                }

                return View(AppointmentDetails);
            }
            catch (Exception)
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
                return Json(new
                {
                    success = true,
                    message = "Appointment is booked for the patient successfully!"
                }); //return json to the success method of Ajax call in js file
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An unexpected error occurred: {ex.Message}" }); //return json to the error method of Ajax call in js file
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


        [HttpGet]
        public async Task<IActionResult> IsExists(int docId, DateTime sdt, DateTime edt)
        {
            try
            {
                bool isExists = await _appointmentService.IsAppointmentExists(docId, sdt, edt);

                if (isExists)
                {
                    return Json(new
                    {
                        exists = true,
                        message = "An appointment already exists for the doctor!"
                    });
                }

                return Json(new
                {
                    exists = false,
                    message = "No appointment conflicts found."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exists = false,
                    message = $"An error occurred while checking for appointment conflicts: {ex.Message}"
                });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            try
            {
                List<AppointmentDto> appointments = await _appointmentService.GetAppointmentDoctorPatientDetails();

                var appointmentDetails = appointments.Select(appointment => new
                {
                    PatientName = appointment.Patient.FullName,
                    DoctorName = appointment.Doctor.FullName,
                    StartDateTime = appointment.StartDateTime.ToString("yyyy-MM-ddTHH:mm:ss"), // ISO format for FullCalendar
                    EndDateTime = appointment.EndDateTime.ToString("yyyy-MM-ddTHH:mm:ss"),     // ISO format for FullCalendar
                    AppointmentID = appointment.ID,
                    PaymentStatus = appointment.PaymentStatus,
                }).ToList();

                return Json(appointmentDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while fetching appointments.", details = ex.Message });
            }
        }


    }
}
