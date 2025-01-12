using HCAS.DL;
using HCAS.DTO;
using Riok.Mapperly.Abstractions;

namespace HCAS.Services.Mapperly
{
    [Mapper]
    public partial class Mapper
    {
        // --- DestenationClass Map (SourceClass) --- //

        public static partial Doctor Map(DoctorDto source);
        public static partial Patient Map(PatientDto source);
        public static partial Appointment Map(AppointmentDto source);
        public static partial Payment Map(PaymentDto source);
        public static partial DoctorSchedule Map(DoctorScheduleDto source);


        public static partial DoctorDto Map(Doctor source);
        public static partial PatientDto Map(Patient source);
        public static partial AppointmentDto Map(Appointment source);
        public static partial PaymentDto Map(Payment source);
        public static partial DoctorScheduleDto Map(DoctorSchedule source);


        public static partial List<Doctor> Map(List<DoctorDto> source);
        public static partial List<Patient> Map(List<PatientDto> source);
        public static partial List<Appointment> Map(List<AppointmentDto> source);
        public static partial List<Payment> Map(List<PaymentDto> source);
        public static partial List<DoctorSchedule> Map(List<DoctorScheduleDto> source);


        public static partial List<DoctorDto> Map(List<Doctor> source);
        public static partial List<PatientDto> Map(List<Patient> source);
        public static partial List<AppointmentDto> Map(List<Appointment> source);
        public static partial List<PaymentDto> Map(List<Payment> source);
        public static partial List<DoctorScheduleDto> Map(List<DoctorSchedule> source);




        //[MapperIgnoreTarget(nameof(Template.DateCreated))]
        //[MapperIgnoreTarget(nameof(Template.DateModified))]
        //public partial Template Map(TemplateDto source);
        //public partial TemplateDto Map(Template source);



        //---- INFOS ----//

        // Custom mapping for properties with different names
        //[MapProperty(nameof(SourceClass.OldName), nameof(DestinationClass.NewName))]
        //public partial DestinationClass Map(SourceClass source);



        // Ignore specific properties
        //[MapIgnore(nameof(DestinationClass.SomeProperty))]
        //public partial DestinationClass Map(SourceClass source);



        //Mapperly supports mapping collections. You can map a list of objects like this:
        // public partial IEnumerable<DestinationClass> Map(IEnumerable<SourceClass> source);


        /*
        Why Mapperly?
        - Compile-Time Generation: Unlike AutoMapper, Mapperly generates mapping code at compile-time, which makes it faster and avoids reflection at runtime.
        - Strongly-Typed: Since the mappings are generated in code, they are strongly-typed, making it easier to maintain and debug.
        - Simplicity: It removes the need for global configuration like in AutoMapper, making the mapping logic localized and clearer. 
        */

    }
}


