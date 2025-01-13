﻿using HCAS.DTO;
using HCAS.Services.PatientServices;
using Microsoft.AspNetCore.Mvc;

namespace Health_Clinic_Appointment_System.Controllers
{
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
                await _patientService.AddPatient(patientDto);
                return Json(new { success = true, message = "Patient added successfully!" }); //return json to the success method of Ajax call in js file
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
                PatientDto patient = await _patientService.GetById(id);
                return View(patient);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PatientDto patientDto)
        {
            if (!ModelState.IsValid)
                return View(patientDto);

            try
            {
                // Update patient
                await _patientService.UpdatePatient(patientDto);
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
                await _patientService.DeletePatient(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
