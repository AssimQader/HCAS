using HCAS.DL;
using HCAS.DTO;
using HCAS.Services.PatientServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Health_Clinic_Appointment_System.Controllers
{
    [Authorize(Roles = "Admin")] //only users with the "Admin" role can access this controller
    public class PatientController : Controller
    {
        private readonly IPatientServices _patientService;

        public PatientController(IPatientServices patientService)
        {
            _patientService = patientService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                List<PatientDto> patients = await _patientService.GetAll();
                return View(patients);
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
        public async Task<IActionResult> Create(PatientDto patientDto)
        {
            if (!ModelState.IsValid)
                return View(patientDto);

            try
            {
                int patientId = await _patientService.AddPatient(patientDto);
                return Json(new
                { 
                    success = true, 
                    message = "Patient added successfully!" ,
                    id = patientId
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
                var patient = await _patientService.GetById(id);
                if (patient == null)
                {
                    return NotFound();
                }

                return Json(new
                {
                    fullName = patient.FullName,
                    email = patient.Email,
                    phoneNumber = patient.PhoneNumber,
                    gender = patient.Gender
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Edit(PatientDto patientDto)
        {
            if (!ModelState.IsValid)
                return View(patientDto);

            try
            {
                bool result = await _patientService.UpdatePatient(patientDto);
                if (!result)
                {
                    return Json(new 
                    { 
                        success = false, 
                        message = $"Faild to update the patient {patientDto.FullName}'s details! please try again." 
                    });
                }

                return Json(new
                {
                    success = true,
                    message = "Patient's data updated successfully!",
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An unexpected error occurred: {ex.Message}" }); //return json to the error method of Ajax call in js file
                throw;
            }
        }



        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool result = await _patientService.DeletePatient(id);
                
                if (result)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Patient removed successfully."
                    });
                }

                return Json(new
                {
                    success = false,
                    message = "An error occurred!"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }



        [HttpGet]
        public async Task<IActionResult> CheckPhoneExists2(string phoneNum)
        {
            try
            {
                bool isExists = await _patientService.IsPhoneNumberExists(phoneNum);

                if (isExists)
                {
                    return Json(new { exists = true, message = "A user with the same phone number already exists!" }); //return json object of two properties: exists, message
                }

                return Json(new { exists = false, message = "" });

            }
            catch (Exception ex)
            {
                return Json(new { exists = false, message = $"An error occurred while checking the phone number: {ex.Message}" });
            }
        }


        [HttpGet]
        public async Task<IActionResult> CheckPhoneExists(string phoneNum)
        {
            try
            {
                int patientId = await _patientService.GetPatientIdByPhoneNumber(phoneNum);

                if (patientId != 0)
                {
                    return Json(new
                    {
                        exists = true,
                        patientID = patientId,
                        message = "Phone number exists in the system."
                    });
                }

                return Json(new
                {
                    exists = false,
                    patientId = 0,
                    message = "Phone number not found."
                });
            }
            catch (Exception ex)
            {
                return Json(new { exists = false, message = $"An error occurred: {ex.Message}" });
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetAllWithAppointments()
        {
            try
            {
                List<PatientDto> patients = await _patientService.GetAllWithAppointments();

                return Json(new
                {
                    success = true,
                    details = patients
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An unexpected error occurred: {ex.Message}" });
                throw;
            }
        }


    }
}
