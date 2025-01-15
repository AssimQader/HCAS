using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCAS.DL
{
    public class Appointment
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int PatientID { get; set; } // Foreign Key

        [Required]
        public int DoctorID { get; set; } // Foreign Key

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public DateTime EndDateTime { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; //default value for first appointment e.g., pending, confirmed, canceled

        [Required]
        [MaxLength(20)]
        public string PaymentStatus { get; set; } = "Pending"; //default value untill patient come to clinic and pay

        [MaxLength(200)]
        public string? Notes { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        //Navigation Properties
        [ForeignKey("PatientID")]
        public virtual Patient? Patient { get; set; } = default!;

        [ForeignKey("DoctorID")]
        public virtual Doctor? Doctor { get; set; } = default!;

        public virtual Payment? Payment { get; set; } = default!; //one to one with payment: (put fk in one of the tables, not both, so I put it in Payment Table as "AppointmentID")
    }
}
