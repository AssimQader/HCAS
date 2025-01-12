namespace Health_Clinic_Appointment_System.ViewModels
{
    public class PaymentViewModel
    {
        public int PaymentID { get; set; }
        public string PatientName { get; set; }
        public string PatientPhoneNumber { get; set; }
        public DateTime AppointmentStartDateTime { get; set; }
        public DateTime AppointmentEndDateTime { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }

    }
}
