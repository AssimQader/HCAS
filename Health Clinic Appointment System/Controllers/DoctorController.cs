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


        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _doctorService.DeleteDoctor(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw;
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

    }
}
