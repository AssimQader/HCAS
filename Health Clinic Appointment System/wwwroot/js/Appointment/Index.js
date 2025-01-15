

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

    // Utility function to show error messages
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
        const doctorId = doctorSelect.value;
        const doctorName = doctorSelect.options[doctorSelect.selectedIndex]?.text;
        const selectedDate = dateInput.value;
        const startTime = startTimeInput.value;
        const endTime = endTimeInput.value;

        if (!doctorId || !selectedDate || !startTime || !endTime) {
            showError("Please fill in all required fields!");
            return;
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

        const formattedStartTime = convertTo24Hour(startTime) + ":00";
        const formattedEndTime = convertTo24Hour(endTime) + ":00";

        try
        {
            // 1- Check if the slot is available
            const slotResponse = await fetch(`/DoctorSchedule/CheckSlot?docId=${doctorId}&day=${DayName}&st=${formattedStartTime}&et=${formattedEndTime}`);
            const slotResult = await slotResponse.json();

            if (!slotResult.exists) {
                showError(`Dr. ${doctorName} is not available within the selected time period: from ${startTime} to ${endTime} ! Please re-check the Schedule.`);
                return;
            }

            // 2- Check if the appointment already exists
            const startDateTime = `${selectedDate} ${formattedStartTime}.0000000`;
            const endDateTime = `${selectedDate} ${formattedEndTime}.0000000`;

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
            

            await SavePatientToDB(appointmentFormData);
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
        dayMaxEvents: true, // Show "more" link when too many events
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
                    <strong>Payment:</strong> ${info.event.extendedProps.paymentStatus}<br>
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


    // Function to format time in 12-hour format with AM/PM
    function formatTimeTo12Hour(dateTime)
    {
        const date = new Date(dateTime);
        let hours = date.getHours();
        const minutes = String(date.getMinutes()).padStart(2, '0');
        const ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12 || 12; //convert 0 to 12 for 12-hour format
        return `${hours}:${minutes} ${ampm}`;
    }

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



async function SavePatientToDB(appointmentFormData)
{
    const form = document.getElementById("addPatientForm");
    const formData = new FormData(form);

    try {
        //send the form data to the server using Fetch API
        const PatientResponse = await fetch("/Patient/Create", {
            method: "POST",
            body: formData
        });

        const PatientResult = await PatientResponse.json();

        if (PatientResult.success)
        {
            let patientId = PatientResult.id;
            appointmentFormData.append("PatientID", parseInt(patientId));

            const AppointmentResponse = await fetch("/Appointment/Create", {
                method: "POST",
                body: appointmentFormData,
            });

            AppointmentResult = await AppointmentResponse.json();

            if (AppointmentResult.success)
            {
                // Show success alert
                Swal.fire({
                    icon: "success",
                    title: "Success!",
                    text: AppointmentResult.message,
                    confirmButtonText: "OK",
                }).then(() => {
                    //reload the page, reset the form and close the modal
                    form.reset();
                    clearAndCloseAppointmentForm();
                    location.reload();


                    ////display event on the calender:
                    //var startEvent = appointmentFormData["StartDateTime"];
                    //var endEvent = appointmentFormData["EndDateTime"];

                    //var evId = 1
                    //calendar.addEvent({
                    //    id: evId,
                    //    title: 'Appointment Title',
                    //    start: startEvent,
                    //    end: endEvent,
                    //    allDay: false
                    //});

                    ////// Store the event in local storage
                    ////const storedEvents = JSON.parse(localStorage.getItem('events')) || [];
                    ////storedEvents.push({ title: sceneTitleInput, start: startEvent, end: endEvent });
                    ////localStorage.setItem('events', JSON.stringify(storedEvents));
                });
            }
            else {
                // Show error alert
                Swal.fire({
                    icon: "error",
                    title: "Error!",
                    text: AppointmentResult.message,
                    confirmButtonText: "OK",
                });
            }
           
        }
        else
        {
            // Show error alert
            Swal.fire({
                icon: "error",
                title: "Error!",
                text: PatientResult.message,
                confirmButtonText: "OK",
            });
        }
    }
    catch (error) {
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



//show create appointment card when click on Add New.
function showHideAddEventForm()
{
    if ($('#addAppointmentForm').is(':hidden'))
    {
        $("#addAppointmentForm").show();
    }
}



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



//Add new event in databse//
function addEventAndCloseEventForm()
{
    if (!validateEvent()) {
        return; // Stop further execution if validation fails
    }

    var form = {};
    form["title"] = sceneTitleInput;
    form["startTime"] = $("#eventStartTime").val();
    form["durationTime"] = $("#eventDurationTime").val();
    form["sceneId"] = Number(sceneID);
    form["scheduleId"] = Number(scheduleID);

    //add event in db
    $.ajax({
        url: "/API/Schedule/Module/Event/AddNew",
        method: 'POST',
        dataType: "json",
        contentType: "application/json;",
        data: JSON.stringify(form),
        success: function (data) {
            $("#addEventForm").toggle("hide");


            //display event on the calender:
            var scheduleDate = $("#scheduleSelect :selected").attr("data-startDay").split('T')[0];
            var startEvent = scheduleDate + 'T' + form["startTime"];
            var endTime = addTwoTimes(form["startTime"], form["durationTime"]);
            var endEvent = scheduleDate + 'T' + endTime;

            var evId = data.id.toString();
            calendar.addEvent({
                id: evId,
                title: sceneTitleInput,
                start: startEvent,
                end: endEvent,
                allDay: false
            });

            // Store the event in local storage
            const storedEvents = JSON.parse(localStorage.getItem('events')) || [];
            storedEvents.push({ title: sceneTitleInput, start: startEvent, end: endEvent });
            localStorage.setItem('events', JSON.stringify(storedEvents));
        },
        error: function () {
            console.log("Event not saved in database!");
        }
    });
}


