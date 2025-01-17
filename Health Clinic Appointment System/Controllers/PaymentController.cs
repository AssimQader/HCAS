using HCAS.DL;
using HCAS.DTO;
using HCAS.Services.Mapperly;
using HCAS.Services.PaymentServices;
using Health_Clinic_Appointment_System.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Health_Clinic_Appointment_System.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentServices _paymentService;

        public PaymentController(IPaymentServices paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                List<PaymentDto> payments = await _paymentService.GetPaymentDetailsData();
                List<PaymentViewModel> paymentDetails = [];

                foreach (PaymentDto payment in payments)
                {
                    paymentDetails.Add(new PaymentViewModel()
                    {
                        PatientName = payment.Patient.FullName,
                        PatientPhoneNumber = payment.Patient.PhoneNumber,
                        AppointmentStartDateTime = payment.Appointment.StartDateTime,
                        AppointmentEndDateTime = payment.Appointment.EndDateTime,
                        Amount = payment.Amount,
                        PaymentDate = (DateTime)payment.PaymentDate,
                        PaymentID = payment.ID,
                        PaymentMethod = payment.PaymentMethod
                    });
                }

                return View(paymentDetails);
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
        public async Task<IActionResult> Create(PaymentDto paymentDto)
        {
            if (!ModelState.IsValid)
                return View(paymentDto);

            try
            {
                paymentDto.PaymentDate = DateTime.Now;
                bool result = await _paymentService.AddPayment(paymentDto);

                if (result)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Payment done successfully.",
                    });
                }


                return Json(new
                {
                    success = false,
                    message = "Payment Failed!",
                });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An unexpected error occurred: {ex.Message}" });
                throw;
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                PaymentDto payment = await _paymentService.GetById(id);
                return View(payment);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PaymentDto paymentDto)
        {
            if (!ModelState.IsValid)
                return View(paymentDto);

            try
            {
                await _paymentService.UpdatePayment(paymentDto);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _paymentService.DeletePayment(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetPaymentDetails()
        {
            try
            {
                List<PaymentDto> payments = await _paymentService.GetPaymentDetailsData();
                List<PaymentViewModel> paymentDetails = [];

                foreach (PaymentDto payment in payments)
                {
                    paymentDetails.Add(new PaymentViewModel()
                    {
                        PatientName = payment.Patient.FullName,
                        PatientPhoneNumber = payment.Patient.PhoneNumber,
                        AppointmentStartDateTime = payment.Appointment.StartDateTime,
                        AppointmentEndDateTime = payment.Appointment.EndDateTime,
                        Amount = payment.Amount,
                        PaymentDate = (DateTime)payment.PaymentDate,
                        PaymentID = payment.ID,
                        PaymentMethod = payment.PaymentMethod
                    });
                }

                return Json(new
                {
                    success = true,
                    paymentData = paymentDetails
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An unexpected error occurred: {ex.Message}" }); 
                throw;
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetPaymentByAppointmentId(int id)
        {
            try
            {
                List<PaymentDto> payments = await _paymentService.GetPaymentDetailsData();
                List<PaymentViewModel> paymentDetails = [];

                foreach (PaymentDto payment in payments)
                {
                    paymentDetails.Add(new PaymentViewModel()
                    {
                        PatientName = payment.Patient.FullName,
                        PatientPhoneNumber = payment.Patient.PhoneNumber,
                        AppointmentStartDateTime = payment.Appointment.StartDateTime,
                        AppointmentEndDateTime = payment.Appointment.EndDateTime,
                        Amount = payment.Amount,
                        PaymentDate = (DateTime)payment.PaymentDate,
                        PaymentID = payment.ID,
                        PaymentMethod = payment.PaymentMethod
                    });
                }

                return Json(new
                {
                    success = true,
                    paymentData = paymentDetails
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
