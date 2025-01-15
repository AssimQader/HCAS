using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.DTO
{
    public record PaymentDto
    {
        public int ID { get; set; }
        public int AppointmentID { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }

        public AppointmentDto? Appointment { get; set; } = default!;
        public PatientDto? Patient { get; set; } = default!; 
    }
}
