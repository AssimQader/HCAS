

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
        const feedbackDiv = input.nextElementSibling;
        const validationRule = regexMap[input.id];

        if (validationRule) {
            if (validationRule.regex.test(input.value)) {
                if (input.id === "PhoneNumber") {
                    const exists = await checkPhoneExists(input.value, feedbackDiv);
                    if (!exists) {
                        input.classList.add("is-valid");
                        input.classList.remove("is-invalid");
                        feedbackDiv.textContent = "";
                    }
                } else {
                    input.classList.add("is-valid");
                    input.classList.remove("is-invalid");
                    feedbackDiv.textContent = "";
                }
            } else {
                input.classList.add("is-invalid");
                input.classList.remove("is-valid");
                feedbackDiv.textContent = validationRule.message;
            }
        } else if (input.checkValidity()) {
            input.classList.add("is-valid");
            input.classList.remove("is-invalid");
            feedbackDiv.textContent = "";
        } else {
            input.classList.add("is-invalid");
            input.classList.remove("is-valid");
            feedbackDiv.textContent = input.validationMessage;
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
            const response = await fetch(`/Doctor/CheckPhoneExists?phoneNum=${encodeURIComponent(phoneNumber)}`);
            const result = await response.json();

            if (result.exists) {
                feedbackDiv.textContent = "A user with this phone number already exists!";
                feedbackDiv.style.display = "block";
                return true;
            } else {
                feedbackDiv.textContent = "";
                feedbackDiv.style.display = "none";
                return false;
            }
        } catch (error) {
            console.error("Error checking phone number:", error);
            feedbackDiv.textContent = "An error occurred while validating the phone number.";
            feedbackDiv.style.display = "block";
            return true;
        }
    };



    /**
     * Add a new schedule row dynamically.
     */
    const addScheduleRow = () => {
        const container = document.getElementById("scheduleContainer");
        const row = document.createElement("div");
        row.classList.add("row", "g-3", "align-items-center", "mb-3");

        row.innerHTML = `
            <div class="col-md-4">
                <label class="form-label">Day of Week</label>
                <select class="form-select schedule-day" required>
                    <option value="" disabled selected>Select Day</option>
                    <option value="Monday">Monday</option>
                    <option value="Tuesday">Tuesday</option>
                    <option value="Wednesday">Wednesday</option>
                    <option value="Thursday">Thursday</option>
                    <option value="Friday">Friday</option>
                    <option value="Saturday">Saturday</option>
                    <option value="Sunday">Sunday</option>
                </select>
                <div class="invalid-feedback">Please select a day.</div>
            </div>
            <div class="col-md-3">
                <label class="form-label">Start Time</label>
                <input type="time" class="form-control schedule-start-time" required />
                <div class="invalid-feedback">Please select a valid start time.</div>
            </div>
            <div class="col-md-3">
                <label class="form-label">End Time</label>
                <input type="time" class="form-control schedule-end-time" required />
                <div class="invalid-feedback">Please select a valid end time.</div>
            </div>
            <div class="col-md-2 text-end">
                <button type="button" class="btn btn-danger remove-schedule-btn">Remove</button>
            </div>
        `;

        container.appendChild(row);
    };


    /**
     * Handle schedule removal.
     */
    document.getElementById("scheduleContainer").addEventListener("click", (event) => {
        if (event.target.classList.contains("remove-schedule-btn")) {
            event.target.closest(".row").remove();
        }
    });

    // Add a new schedule row on button click
    document.getElementById("addScheduleBtn").addEventListener("click", addScheduleRow);


    // Real-time validation for all inputs
    const inputs = document.querySelectorAll("input, select");
    inputs.forEach((input) => {
        input.addEventListener("blur", () => validateInput(input));
    });
})();






// Handle "View Schedule" button click
$(document).on("click", ".view-schedule-btn", async function ()
{
    const doctorId = $(this).data("id");
    const doctorName = $(this).data("name");

    $("#doctorName").text(`Schedule for Dr. ${doctorName}`);

    try
    {
        const response = await fetch(`/Doctor/GetSchedule?doctorId=${doctorId}`);
        const schedules = await response.json();

        //clear any existing rows in the modal's schedule table before append to prevent redundent data view
        $("#scheduleDetails").empty();

        //populate the modal with the fetched schedule
        if (schedules.data.length > 0)
        {
            schedules.data.forEach(schedule => {
                $("#scheduleDetails").append(`
                        <tr>
                            <td>${schedule.dayOfWeek}</td>
                            <td>${schedule.startTime}</td>
                            <td>${schedule.endTime}</td>
                        </tr>
                    `);
            });
        }
        else
        {
            $("#scheduleDetails").append(`
                    <tr>
                        <td colspan="3" class="text-center">No schedule available.</td>
                    </tr>
            `);
        }


        $("#scheduleModal").modal("show");
    }
    catch (error)
    {
        console.error("Error fetching schedule:", error);
        alert("Failed to load the schedule! Please try again.");
    }
});




// Form submission handler
document.getElementById("addDoctorForm").addEventListener("submit", async function (event) {
    event.preventDefault();

    const form = this;
    const formData = new FormData(form);

    // Collect all schedules data
    document.querySelectorAll("#scheduleContainer .row").forEach((row, index) => {
        const DayOfWeek = row.querySelector(".schedule-day").value;
        const StartTime = row.querySelector(".schedule-start-time").value + ":00"; // Ensure hh:mm:ss format
        const EndTime = row.querySelector(".schedule-end-time").value + ":00"; // Ensure hh:mm:ss format

        /*
          Key-Value Binding: The DoctorSchedules list is sent using keys that ASP.NET Core can map to the List<DoctorScheduleDto> property. 
          For example, the first schedule is sent as:
          DoctorSchedules[0].DayOfWeek = "Monday"
          DoctorSchedules[0].StartTime = "14:00:00"
          DoctorSchedules[0].EndTime = "17:00:00"
        */
        if (DayOfWeek && StartTime && EndTime) {
            formData.append(`DoctorSchedules[${index}].DayOfWeek`, DayOfWeek);
            formData.append(`DoctorSchedules[${index}].StartTime`, StartTime);
            formData.append(`DoctorSchedules[${index}].EndTime`, EndTime);
        }
    });

    try {
        const response = await fetch("/Doctor/Create", {
            method: "POST",
            body: formData,
        });
        const result = await response.json();

        if (result.success) {
            Swal.fire("Success!", "Doctor added successfully!", "success").then(() => {
                form.reset();
                document.getElementById("addDoctorModal").click();
                location.reload();
            });
        } else {
            Swal.fire("Error!", result.message, "error");
        }
    } catch (error) {
        Swal.fire("Error!", "An unexpected error occurred!", "error");
        console.error(error);
    }
});
