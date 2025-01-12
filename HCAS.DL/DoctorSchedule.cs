using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.DL
{
    public class DoctorSchedule
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int DoctorID { get; set; } //FK

        [Required]
        [MaxLength(10)]
        public string DayOfWeek { get; set; } = string.Empty;

        [Required]
        public TimeSpan StartTime { get; set; } //start time of the available slot

        [Required]
        public TimeSpan EndTime { get; set; } //end time of the available slot

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;



        //Navigation Property
        [ForeignKey("DoctorID")]
        public virtual Doctor? Doctor { get; set; }
    }
}
