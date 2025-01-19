

//Validate Patient Data//
(function () {
    'use strict';

    // Validation rules and messages
    const regexMap = {
        PhoneNumber: {
            regex: /^\+2[0-9]\d{10}$/, // E.164 format with minimum length 13 (e.g., +201061103073)
            message: "Phone numbers must start with +2 and have exactly 13 characters!"
        },
        Email: {
            regex: /^[^\s@]+@[^\s@]+\.[^\s@]+$/, // Basic email format
            message: "Please enter a valid email address!"
        },
    };


    /**
     * Validate input field based on regex or built-in validation.
     * @param {HTMLInputElement} input - Input element to validate.
     */
    const validateInput = async (input) => {
        const feedbackDiv = input.nextElementSibling; // Get the invalid-feedback div
        const validationRule = regexMap[input.id]; // Get validation rule based on input Id

        if (validationRule) {
            // Check against regex
            if (validationRule.regex.test(input.value)) {
                // If the input is PhoneNumber, perform an additional server-side check
                if (input.id === "PhoneNumber") {
                    const exists = await checkPhoneExists(input.value, feedbackDiv);
                    if (!exists) {
                        input.classList.add("is-valid");
                        input.classList.remove("is-invalid");
                        feedbackDiv.textContent = ""; // Clear any existing feedback
                    }
                }
                else {
                    input.classList.add("is-valid");
                    input.classList.remove("is-invalid");
                    feedbackDiv.textContent = ""; // Clear any existing feedback
                }
            }
            else {
                input.classList.add("is-invalid");
                input.classList.remove("is-valid");
                feedbackDiv.textContent = validationRule.message; // Set dynamic feedback message
            }
        }
        else if (input.checkValidity()) {
            input.classList.add("is-valid");
            input.classList.remove("is-invalid");
            feedbackDiv.textContent = ""; // Clear any existing feedback
        }
        else {
            input.classList.add("is-invalid");
            input.classList.remove("is-valid");
            feedbackDiv.textContent = input.validationMessage; // Default browser feedback
        }
    };

    /**
     * Check if a phone number exists in the database via server-side validation.
     * @param {string} phoneNumber - Phone number to check.
     * @param {HTMLElement} feedbackDiv - Div to display validation message.
     * @returns {boolean} - Returns true if the phone number exists, otherwise false.
     */
    const checkPhoneExists = async (phoneNumber, feedbackDiv) => {
        try {
            const response = await fetch(`/Patient/CheckPhoneExists?phoneNum=${encodeURIComponent(phoneNumber)}`);
            const result = await response.json();

            if (result.exists) {
                feedbackDiv.textContent = "A user with this phone number already exists!";
                feedbackDiv.style.display = "block";
                return true; // Phone number exists
            } else {
                feedbackDiv.textContent = "";
                feedbackDiv.style.display = "none";
                return false; // Phone number does not exist
            }
        } catch (error) {
            console.error("Error checking phone number:", error);
            feedbackDiv.textContent = "An error occurred while validating the phone number.";
            feedbackDiv.style.display = "block";
            return true; // Assume the phone exists to prevent issues
        }
    };


    /**
     * Add blur event listeners to all inputs for real-time validation.
     */
    const inputs = document.querySelectorAll("input, select");
    inputs.forEach((input) => {
        input.addEventListener("blur", () => validateInput(input));
    });



    /**
     * Handle form submission by validating all inputs.
     */
    const forms = document.querySelectorAll('.needs-validation');
    forms.forEach((form) => {
        form.addEventListener('submit', (event) => {
            let isFormValid = true;

            inputs.forEach((input) => {
                validateInput(input); // Validate each input
                if (!input.classList.contains("is-valid")) {
                    isFormValid = false; // Mark the form invalid if any input fails validation
                }
            });

            if (!isFormValid || !form.checkValidity()) {
                event.preventDefault(); // Prevent form submission
                event.stopPropagation();
            }

            form.classList.add('was-validated');
        });
    });
})();



$("#goCustomCalenderDateBtn").on("click", function () {
    calendar.gotoDate($("#customCalenderDate").val());
    $("#closeCustomCalenderModelBtn").click();
});


var calendar;
let startDateTime;
let endDateTime;
document.addEventListener('DOMContentLoaded', function ()
{
    const specializationSelect = document.getElementById("Specialization");
    const doctorSelect = document.getElementById("DoctorsList");
    const dateInput = document.getElementById("Date");
    const startTimeInput = document.getElementById("StartTime");
    const endTimeInput = document.getElementById("EndTime");
    const errorMessageDiv = document.getElementById("errorMessage");

    const submitBtn = document.getElementById("submitBtn");


    startTimeInput.disabled = true;
    endTimeInput.disabled = true;


    const showError = (message) => {
        errorMessageDiv.textContent = message;
        errorMessageDiv.style.display = "block";
    };

    const clearError = () => {
        errorMessageDiv.textContent = "";
        errorMessageDiv.style.display = "none";
    };



    // Fetch doctors based on specialization
    specializationSelect.addEventListener("change", async function () {
        doctorSelect.innerHTML = '<option value="" selected disabled>Select Doctor</option>';
        clearError();

        try {
            const response = await fetch(`/Doctor/GetDocBySpecialization?spec=${specializationSelect.value}`);
            const doctors = await response.json();

            doctors.data.forEach((doctor) => {
                const option = document.createElement("option");
                option.value = doctor.id;
                option.textContent = doctor.fullName;
                doctorSelect.appendChild(option);
            });
        } catch (error) {
            showError("Error fetching doctors.");
        }
    });


    let DayName = '';
    // Handle date selection
    dateInput.addEventListener("change", async function () {
        const selectedDate = new Date(dateInput.value);
        const dayName = selectedDate.toLocaleDateString("en-US", { weekday: "long" });
        DayName = dayName;
        const doctorId = doctorSelect.value;

        if (!doctorId) {
            showError("Please select a doctor first.");
            return;
        }

        try {
            const response = await fetch(`/DoctorSchedule/IsAvailableDay?id=${doctorId}&day=${dayName}`);
            const result = await response.json();

            if (!result.exists) {
                showError(`Dr. ${doctorSelect.options[doctorSelect.selectedIndex].text} is not available on ${dayName}! Please re-check the Schedule.`);
                startTimeInput.disabled = true;
                endTimeInput.disabled = true;
            } else {
                clearError();
                startTimeInput.disabled = false;
                endTimeInput.disabled = false;
            }
        } catch (error) {
            showError("Error checking doctor's availability.");
        }
    });



    // Handle "Add" button click
    submitBtn.addEventListener("click", async function ()
    {
        let doctorId = doctorSelect.value;
        const doctorName = doctorSelect.options[doctorSelect.selectedIndex]?.text;
        const selectedDate = dateInput.value;
        const startTime = startTimeInput.value;
        const endTime = endTimeInput.value;

        if (!doctorId || !selectedDate || !startTime || !endTime) {
            showError("Please fill in all required fields!");
            return;
        }

        const formattedStartTime = convertTo24Hour(startTime) + ":00";
        const formattedEndTime = convertTo24Hour(endTime) + ":00";

        try
        {
            // 1- Check if the slot is available
            const slotResponse = await fetch(`/DoctorSchedule/CheckSlot?docId=${doctorId}&day=${DayName}&st=${formattedStartTime}&et=${formattedEndTime}`);
            const slotResult = await slotResponse.json();

            if (!slotResult.exists) {
                showError(`Dr. ${doctorName} is not available within the selected time period: ${startTime} - ${endTime} at ${DayName}!  Please re-check the Schedule.`);
                return;
            }

            // 2- Check if the appointment already exists
            startDateTime = `${selectedDate} ${formattedStartTime}.0000000`;
            endDateTime = `${selectedDate} ${formattedEndTime}.0000000`;

            const appointmentResponse = await fetch(`/Appointment/IsExists?docId=${doctorId}&sdt=${startDateTime}&edt=${endDateTime}`);
            const appointmentResult = await appointmentResponse.json();

            if (appointmentResult.exists) {
                showError(`Dr. ${doctorName} already has an appointment at the selected date and time! Please re-check Dr. ${doctorName}'s Appointments.`);
                return;
            }

            //if everything is fine, display success message
            clearError();



            //Save patient and appointment to database logic//
            const appointmentFormData = new FormData();
            appointmentFormData.append("DoctorID", parseInt(doctorId));
            appointmentFormData.append("StartDateTime", startDateTime);
            appointmentFormData.append("EndDateTime", endDateTime);
            appointmentFormData.append("Status", "Confirmed");
            appointmentFormData.append("PaymentStatus", "Pending");
            

            await SavePatientAndAppointmentToDB(appointmentFormData);
        }
        catch (error) {
            showError("An error occurred while checking the appointment.");
        }
    });


    var calendarEl = document.getElementById('calendar');
    var calendar = new FullCalendar.Calendar(calendarEl, {
        expandRows: true,
        customButtons: {
            customDateRange: {
                text: 'Go to Date!',
                click: function () {
                    $("#showSelectCalendarDateModalBtn").click();
                }
            }
        },
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay,customDateRange'
        },
        initialView: 'dayGridMonth',
        navLinks: true, //clickable day/week names to navigate views
        editable: true,
        selectable: true,
        nowIndicator: true,
        dayMaxEvents: true, //show "more" link when too many events
        eventTimeFormat: {
            hour: 'numeric',
            minute: '2-digit',
            meridiem: 'short',
            hour12: true,
        },
        eventColor: '#28a745', //green color for events
        eventClick: function (info)
        {
            const modalContent = `
                <div>
                    <strong>Doctor:</strong> ${info.event.extendedProps.docName}<br>
                    <strong>Patient:</strong> ${info.event.extendedProps.patientName}<br>
                    <strong>Payment:
                        <span class="badge ${info.event.extendedProps.paymentStatus === "Paid" ? "bg-success" : "bg-warning"}">
                                 ${info.event.extendedProps.paymentStatus}
                       </span>
                    </strong><br>
                    <strong>Start:</strong> ${new Date(info.event.start).toLocaleString()}<br>
                    <strong>End:</strong> ${new Date(info.event.end).toLocaleString()}<br>
                </div>
            `;
            Swal.fire({
                title: "Appointment Details",
                html: modalContent,
                icon: "info",
                confirmButtonText: "Close",
            });
        }
    });


    //function to fetch and render appointments on the calendar
    async function renderCalendar() {
        try
        {
            const response = await fetch('/Appointment/GetAllAppointments');
            const appointments = await response.json();

            if (appointments && Array.isArray(appointments))
            {
                appointments.forEach(appointment => {
                    const startFormatted = formatTimeTo12Hour(appointment.startDateTime);
                    const endFormatted = formatTimeTo12Hour(appointment.endDateTime);

                    calendar.addEvent({
                        title: `${startFormatted} - ${endFormatted}`, //example: Dr. John: 10:00 AM - 11:00 AM
                        start: appointment.startDateTime,
                        end: appointment.endDateTime,
                        allDay: false,
                        color: '#28a745', //green bullet
                        extendedProps: {
                            patientName: appointment.patientName,
                            docName: appointment.doctorName,
                            paymentStatus: appointment.paymentStatus,
                        }
                    });
                });
            } else {
                console.error("No appointments found or invalid data format.");
            }
        } catch (error) {
            console.error("Error fetching appointments:", error);
            Swal.fire({
                title: 'Error!',
                text: 'Failed to load appointments. Please try again later.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
        }
    }

    calendar.render();
    renderCalendar();
});



// Function to format time in 12-hour format with AM/PM
function formatTimeTo12Hour(dateTime) {
    const date = new Date(dateTime);
    let hours = date.getHours();
    const minutes = String(date.getMinutes()).padStart(2, '0');
    const ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12 || 12; //convert 0 to 12 for 12-hour format
    return `${hours}:${minutes} ${ampm}`;
}



//save appointment to databse (Add NEw & Edit)
async function SavePatientAndAppointmentToDB(appointmentFormData)
{
    const form = document.getElementById("addPatientForm");
    const formData = new FormData(form);

    try
    {
        if (_AppointmentID === 0) //means it is add new appointmnt process
        {
            const PatientResponse = await fetch("/Patient/Create", {
                method: "POST",
                body: formData,
            });

            const PatientResult = await PatientResponse.json();

            if (PatientResult.success) {
                let patientId = PatientResult.id;
                appointmentFormData.append("PatientID", parseInt(patientId));

                const AppointmentResponse = await fetch("/Appointment/Create", {
                    method: "POST",
                    body: appointmentFormData,
                });

                const AppointmentResult = await AppointmentResponse.json();

                if (AppointmentResult.success) {
                    Swal.fire({
                        icon: "success",
                        title: "Success!",
                        text: AppointmentResult.message,
                        confirmButtonText: "OK",
                    }).then(() => {
                        // Reset form, close modal, and reload page
                        form.reset();
                        clearAndCloseAppointmentForm();
                        location.reload();
                    });
                } else {
                    Swal.fire({
                        icon: "error",
                        title: "Error!",
                        text: AppointmentResult.message,
                        confirmButtonText: "OK",
                    });
                }
            } else {
                Swal.fire({
                    icon: "error",
                    title: "Error!",
                    text: PatientResult.message,
                    confirmButtonText: "OK",
                });
            }
        }
        else  // If _AppointmentID !== 0, update the existing appointment
        {
           
            const updateFormData = new FormData();
            updateFormData.append("ID", parseInt(_AppointmentID));
            updateFormData.append("PatientID", _PatientID);

            //get the DoctorID from the selected option in DoctorsList
            const selectedDoctorId = document.getElementById("DoctorsList").value;
            updateFormData.append("DoctorID", parseInt(selectedDoctorId));

            updateFormData.append("StartDateTime", startDateTime);
            updateFormData.append("EndDateTime", endDateTime);
            updateFormData.append("Status", "Confirmed");
            updateFormData.append("PaymentStatus", "Pending");

            const UpdateResponse = await fetch("/Appointment/Edit", {
                method: "POST",
                body: updateFormData,
            });

            const UpdateResult = await UpdateResponse.json();

            if (UpdateResult.success) {
                Swal.fire({
                    icon: "success",
                    title: "Success!",
                    text: UpdateResult.message,
                    confirmButtonText: "OK",
                }).then(() => {
                    form.reset();
                    clearAndCloseAppointmentForm();
                    location.reload();
                });
            } else {
                Swal.fire({
                    icon: "error",
                    title: "Error!",
                    text: UpdateResult.message,
                    confirmButtonText: "OK",
                });
            }
        }
    } catch (error) {
        Swal.fire({
            icon: "error",
            title: "Error!",
            text: "An unexpected error occurred!",
            confirmButtonText: "OK",
        });
        console.error("Error:", error);
    }
}


/*------------------------------------------------------------------------*/
/*------------------------------------------------------------------------*/


//change between calender and table view
$("input[type=radio][name=viewOption-radio]").on("change", function () {
    if ($("#viewAsTable").is(":checked"))
    {
        $("#tableView").show();
        $("#calenderView").hide();
    }
    else
    {
        $("#tableView").hide();
        $("#calenderView").show();
    }

});





let _AppointmentID = 0;
let _PatientID = 0;
let _DoctorID = 0;
//show create/edit appointment card when click on Add New or Edit buttons.
function showHideAddEventForm(appointmentId)
{
    _AppointmentID = appointmentId;
    if (appointmentId === 0)
    {
        document.getElementById("submitBtn").textContent = "Add";

        $('#FullName').prop('disabled', false);
        $('#Email').prop('disabled', false);
        $('#PhoneNumber').prop('disabled', false);
        $('#Gender').prop('disabled', false);

        // Clear the form for creating a new appointment
        $('#Specialization').val('');
        $('#DoctorsList').empty().append('<option value="" selected disabled>Select Doctor</option>');
        $('#Date').val('');
        $('#StartTime').val('');
        $('#EndTime').val('');
        $('#FullName').val('');
        $('#Email').val('');
        $('#PhoneNumber').val('');
        $('#Gender').val('');
    }
    else
    {
        document.getElementById("submitBtn").textContent = "Save Changes";
        // Fetch appointment details and populate the form
        fetch(`/Appointment/Edit?id=${appointmentId}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Failed to fetch appointment data.');
                }
                return response.json();
            })
            .then(responseData => {
                if (!responseData.success) {
                    throw new Error('Appointment not found or an error occurred.');
                }

                const data = responseData.data;

                _PatientID = data.patientID;
                _DoctorID = data.doctorID;

                // Populate the form fields with fetched data
                $('#Specialization').val(data.doctor.specialization);
                $('#DoctorsList').empty().append(`<option value="${data.doctor.id}" selected>${data.doctor.fullName}</option>`);
                $('#Date').val(data.startDateTime.split('T')[0]); // Extract the date in YYYY-MM-DD format
                $('#StartTime').val(formatTimeTo24Hour(data.startDateTime)); // Format time to HH:mm
                $('#EndTime').val(formatTimeTo24Hour(data.endDateTime)); // Format time to HH:mm

                $('#FullName').val(data.patient.fullName).prop('disabled', true);
                $('#Email').val(data.patient.email).prop('disabled', true);
                $('#PhoneNumber').val(data.patient.phoneNumber).prop('disabled', true);
                $('#Gender').val(data.patient.gender).prop('disabled', true);
            })
            .catch(error => {
                console.error('Error fetching appointment data:', error);
                alert('Failed to fetch appointment data. Please try again.');
            });


    }

    if ($('#addAppointmentForm').is(':hidden')) {
        $("#addAppointmentForm").show();
    }
}






// Function to format time in 24-hour format
function formatTimeTo24Hour(dateTime) {
    const date = new Date(dateTime);
    const hours = String(date.getHours()).padStart(2, '0'); // Ensure 2-digit hours
    const minutes = String(date.getMinutes()).padStart(2, '0'); // Ensure 2-digit minutes
    return `${hours}:${minutes}`;
}


// Convert startTime and endTime to 24-hour format
const convertTo24Hour = (time12h) => {
    const [time, modifier] = time12h.split(' ');
    let [hours, minutes] = time.split(':').map(Number);

    if (modifier === 'PM' && hours < 12) {
        hours += 12;
    } else if (modifier === 'AM' && hours === 12) {
        hours = 0;
    }

    return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}`;
};





//close create appointment card.
function clearAndCloseAppointmentForm() {
    $("#Specialization").val("select specialization").change();
    $("#DoctorsList").val("select Doctor").change();
    $("#Date").val("");
    $("#StartTime").val("");
    $("#EndTime").val("");
    $("#addAppointmentForm").toggle("hide");
}



/*---------------------------------------------------------------------------------*/

// Delete Appointment logic
document.addEventListener("DOMContentLoaded", function ()
{
    const deleteButtons = document.querySelectorAll(".delete-appointment-btn");

    deleteButtons.forEach((button) => {
        button.addEventListener("click", async function () {
            const appointmentId = this.getAttribute("data-id");

            const confirmation = await Swal.fire({
                title: "Are you sure?",
                text: "You are about to remove this appointment.",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "Yes",
                cancelButtonText: "Cancel",
            });

            if (confirmation.isConfirmed)
            {
                try
                {
                    const response = await fetch(`/Appointment/Delete?id=${appointmentId}`, {
                        method: "GET",
                    });

                    const result = await response.json();

                    if (result.success) {
                        await Swal.fire({
                            title: "Deleted!",
                            text: result.message,
                            icon: "success",
                            confirmButtonText: "OK",
                        });

                        location.reload();
                    }
                    else
                    {
                        Swal.fire({
                            title: "Error!",
                            text: result.message,
                            icon: "error",
                            confirmButtonText: "OK",
                        });
                    }
                }
                catch (error)
                {
                    Swal.fire({
                        title: "Error!",
                        text: "An unexpected error occurred while trying to delete the appointment!",
                        icon: "error",
                        confirmButtonText: "OK",
                    });
                }
            }
        });
    });
});

