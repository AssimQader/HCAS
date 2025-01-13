

// -------------------- Input Validations & Check phone number exsistance ----------------- //
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

        if (validationRule)
        {
            // Check against regex
            if (validationRule.regex.test(input.value)) {
                // If the input is PhoneNumber, perform an additional server-side check
                if (input.id === "PhoneNumber")
                {
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


// -------------------- Alerts after add new patient ----------------- //
document.getElementById("addPatientForm").addEventListener("submit", async function (event)
{
    event.preventDefault();

    const form = this;
    const formData = new FormData(form);

    try
    {
        //send the form data to the server using Fetch API
        const response = await fetch("/Patient/Create", {
            method: "POST",
            body: formData
        });

        const result = await response.json();

        if (result.success)
        {
            // Show success alert
            Swal.fire({
                icon: "success",
                title: "Success!",
                text: result.message,
                confirmButtonText: "OK",
            }).then(() => {
                //reload the page, reset the form and close the modal
                form.reset();
                document.getElementById("addPatientModal").click(); //close modal
                location.reload();
            });
        } else {
            // Show error alert
            Swal.fire({
                icon: "error",
                title: "Error!",
                text: result.message,
                confirmButtonText: "OK",
            });
        }
    }
    catch (error)
    {
        Swal.fire({
            icon: "error",
            title: "Error!",
            text: "An unexpected error occurred!",
            confirmButtonText: "OK",
        });
        console.error("Error:", error);
    }
});

