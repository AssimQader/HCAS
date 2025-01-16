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
            if (!ModelState.IsValid)
                return View(doctorDto);

            try
            {
                await _doctorService.AddDoctor(doctorDto);
                return Json(new { success = true, message = "Doctor added successfully!" }); //return json to the success of error methods of Ajax call in js file
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
                return View(doctor);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DoctorDto doctorDto)
        {
            if (!ModelState.IsValid)
                return View(doctorDto);

            try
            {
                await _doctorService.UpdateDoctor(doctorDto);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
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
        public async Task<IActionResult> CheckPhoneExists(string phoneNum)
        {
            try
            {
                bool isExists = await _doctorService.IsPhoneNumberExists(phoneNum);

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
    }
}
