using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.DTO
{
    public record DoctorScheduleDto
    {
        public int? ID { get; set; }
        public string DayOfWeek { get; set; }
        public string StartTime { get; set; } 
        public string EndTime { get; set; }

        public int? DocId { get; set; }

        public DoctorDto? Doctor { get; set; }
    }
}
