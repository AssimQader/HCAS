using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.DL
{
    public class Payment
    {
        [Key]
        public int ID { get; set; }
        
        [Required]
        public int AppointmentID { get; set; } // Foreign Key

        [Required]
        public int PatientID { get; set; } // Foreign Key

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(20)]
        public string PaymentMethod { get; set; } = "Cash"; //default

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;



        //Navigation Properties
        [ForeignKey("AppointmentID")]
        public virtual Appointment? Appointment { get; set; } = default!;

        [ForeignKey("PatientID")]
        public virtual Patient? Patient { get; set; } = default!; //one to one with appointment
    }
}
