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
                List<PaymentDto> payments = await _paymentService.GetPaymentPatientAppointmentDetails();
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
                        PaymentDate = payment.PaymentDate,
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
                await _paymentService.AddPayment(paymentDto);
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
    }
}
