using System.ComponentModel.DataAnnotations;

namespace HCAS.DL
{
    public class Patient
    {
        [Key]
        public int ID { get; set; } 

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } = string.Empty; //international format (E.164)

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        [MaxLength(10)]
        public string Gender { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        //Navigation Properties
        public virtual ICollection<Appointment>? Appointments { get; set; } = [];
        public virtual ICollection<Payment>? Payments { get; set; } = [];
    }
}