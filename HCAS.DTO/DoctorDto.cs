namespace HCAS.DTO
{
    public record DoctorDto
    {
        public int ID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;


        //schedules Information
        public List<DoctorScheduleDto> DoctorSchedules { get; set; } = [];
    }

}
