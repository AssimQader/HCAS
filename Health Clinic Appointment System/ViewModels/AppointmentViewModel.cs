namespace Health_Clinic_Appointment_System.ViewModels
{
    public class AppointmentViewModel
    {
        public int AppointmentID { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public string AppointmentStatus { get; set; }
        public string PaymentStatus { get; set; }
    }
}
