using HCAS.DL;
using HCAS.DTO;
using HCAS.Services.DoctorServices;
using Microsoft.AspNetCore.Mvc;

namespace Health_Clinic_Appointment_System.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IDoctorServices _doctorService;

        public DoctorController(IDoctorServices doctorService)
        {
            _doctorService = doctorService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                List<DoctorDto> doctors = await _doctorService.GetAll();
                return View(doctors);
            }
            catch (Exception ex)
            {
                throw;
            }
        }  


        [HttpGet]
        public IActionResult Create()
        {
            return View(new DoctorDto());
        }


        [HttpPost]
        public async Task<IActionResult> Create(DoctorDto doctorDto)
        {

            try
            {
                int docId = await _doctorService.AddDoctor(doctorDto);
                return Json(new
                {
                    success = true,
                    message = "Doctor added successfully!",
                    id = docId
                });
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
                DoctorDto doctor = await _doctorService.GetById(id);
                if (doctor == null)
                {
                    return NotFound();
                }

                return Json(new
                {
                    fullName = doctor.FullName,
                    email = doctor.Email,
                    phoneNumber = doctor.PhoneNumber,
                    specialization = doctor.Specialization,
                    doctorSchedules = doctor.DoctorSchedules
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Edit(DoctorDto doctorDto)
        {

            if (!ModelState.IsValid)
                return View(doctorDto);

            try
            {
                bool result = await _doctorService.UpdateDoctor(doctorDto);
                if (!result)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Faild to update Dr. {doctorDto.FullName}'s details! please try again."
                    });
                }

                return Json(new
                {
                    success = true,
                    message = "Doctor's data updated successfully!",
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
                bool result = await _doctorService.DeleteDoctor(id);
                if (result)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Doctor removed successfully."
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
        public async Task<IActionResult> GetDocBySpecialization(string spec)
        {
            try
            {
                List<DoctorDto> doctors = await _doctorService.GetDocBySpecialization(spec);

                if (doctors != null && doctors.Count != 0)
                {
                    return Json(new
                    {
                        success = true,
                        data = doctors,
                        message = "doctors fetched successfully."
                    });
                }

                return Json(new
                {
                    success = false,
                    data = new List<DoctorDto>(),
                    message = "No doctors found for the specified specialization!"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    data = new List<DoctorDto>(),
                    message = $"An error occurred while fetching doctors: {ex.Message}"
                });
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetSchedule(int doctorId)
            {
                try
                {
                    List<DoctorScheduleDto> doctorSchedules = await _doctorService.GetDocScheduleByDocId(doctorId);

                    if (doctorSchedules != null && doctorSchedules.Count != 0)
                    {
                        return Json(new
                        {
                            success = true,
                            data = doctorSchedules,
                            message = "Schedule fetched successfully."
                        });
                    }

                    return Json(new
                    {
                        success = false,
                        data = new List<DoctorScheduleDto>(), //return empty list when no schedules exist
                        message = "No schedule found for the specified doctor."
                    });
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        success = false,
                        data = new List<DoctorScheduleDto>(),
                        message = $"An error occurred while fetching schedule: {ex.Message}"
                    });
                }
            }


        public async Task<IActionResult> CheckPhoneExists(string phoneNum)
        {
            try
            {
                int doctorId = await _doctorService.GetDoctorIdByPhoneNumber(phoneNum);

                if (doctorId != 0)
                {
                    return Json(new
                    {
                        exists = true,
                        doctorID = doctorId,
                        message = "Phone number exists in the system."
                    });
                }

                return Json(new
                {
                    exists = false,
                    doctorID = 0,
                    message = "Phone number not found."
                });
            }
            catch (Exception ex)
            {
                return Json(new { exists = false, message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
