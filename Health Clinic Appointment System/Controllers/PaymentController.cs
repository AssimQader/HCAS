using HCAS.DTO;
using HCAS.Services.PaymentServices;
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
                List<PaymentDto> payments = await _paymentService.GetAll();
                return View(payments);
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
