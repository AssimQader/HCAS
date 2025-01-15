using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.DTO
{
    public record AppointmentDto
    {
        public int ID { get; set; }
        public int PatientID { get; set; }
        public int DoctorID { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string? Notes { get; set; } = string.Empty;

        public PatientDto? Patient { get; set; } = default!;
        public  DoctorDto? Doctor { get; set; } = default!;
    }
}
