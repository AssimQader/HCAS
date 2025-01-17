// -------------------- Input Validations & Check phone number existence ----------------- //
(function () {
    'use strict';

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

    let currentPatientId = null; //track the current patient being edited in edit process




    /**
     * Validate input field based on regex or built-in validation.
     * @param {HTMLInputElement} input -
     */
    const validateInput = async (input) => {
        const feedbackDiv = input.nextElementSibling; //get the invalid-feedback div
        const validationRule = regexMap[input.id]; //get validation rule based on input Id(Email or PhoneNumber)

        if (validationRule)
        {
            if (validationRule.regex.test(input.value))
            {
                //if the input is PhoneNumber, perform an additional server-side check
                if (input.id === "PhoneNumber")
                {
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
                feedbackDiv.textContent = validationRule.message; //set dynamic feedback message
            }
        }
        else if (input.checkValidity())
        {
            input.classList.add("is-valid");
            input.classList.remove("is-invalid");
            feedbackDiv.textContent = ""; 
        }
        else
        {
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
            const response = await fetch(`/Patient/CheckPhoneExists?phoneNum=${encodeURIComponent(phoneNumber)}`);
            const result = await response.json();

            if (result.exists && (!currentPatientId || result.patientID !== parseInt(currentPatientId)))
            {
                feedbackDiv.textContent = "A user with this phone number already exists!";
                feedbackDiv.style.display = "block";
                return true;
            }
            else {
                feedbackDiv.textContent = "";
                feedbackDiv.style.display = "none";
                return false;
            }
        }
        catch (error) {
            console.error("Error checking phone number:", error);
            feedbackDiv.textContent = "An error occurred while validating the phone number.";
            feedbackDiv.style.display = "block";
            return true;
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
                validateInput(input); 

                if (!input.classList.contains("is-valid"))
                {
                    isFormValid = false;
                }
            });

            if (!isFormValid || !form.checkValidity()) {
                event.preventDefault(); //prevent form submission
                event.stopPropagation();
            }

            form.classList.add('was-validated');
        });
    });





    /**
     * Handle adding or editing a patient.
     */
    document.getElementById("addPatientForm").addEventListener("submit", async function (event)
    {
        event.preventDefault();

        const form = this;
        const formData = new FormData(form);

        if (currentPatientId) //f it has a value that means user in EDIT proccess
        {
            formData.append("ID", currentPatientId); //append the ID to send with patient object to database
        }

        try {
            const url = currentPatientId ? `/Patient/Edit` : `/Patient/Create`;
            const response = await fetch(url, {
                method: "POST",
                body: formData
            });

            const result = await response.json();

            if (result.success)
            {
                Swal.fire({
                    icon: "success",
                    title: "Success!",
                    text: result.message,
                    confirmButtonText: "OK",
                }).then(() => {
                    form.reset();
                    currentPatientId = null; //reset currentPatientId
                    document.getElementById("addPatientModal").click();
                    location.reload();
                });
            } else {
                Swal.fire({
                    icon: "error",
                    title: "Error!",
                    text: result.message,
                    confirmButtonText: "OK",
                });
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
    });




    /**
     * Handle populating the modal for editing a patient.
     */
    document.querySelectorAll(".edit-patient-btn").forEach(button => {
        button.addEventListener("click", async function () {
            currentPatientId = this.getAttribute("data-id");

            try {
                const response = await fetch(`/Patient/Edit?id=${currentPatientId}`);
                const patient = await response.json();

                if (response.ok && patient)
                {
                    document.getElementById("FullName").value = patient.fullName || "";
                    document.getElementById("Email").value = patient.email || "";
                    document.getElementById("PhoneNumber").value = patient.phoneNumber || "";
                    document.getElementById("Gender").value = patient.gender || "";

                    //update modal title and submit button texts
                    document.getElementById("addPatientModalLabel").textContent = "Edit Patient";
                    document.querySelector("#addPatientForm button[type='submit']").textContent = "Save Changes";

                    const modal = new bootstrap.Modal(document.getElementById("addPatientModal"));
                    modal.show();
                } else {
                    throw new Error("Failed to fetch patient data.");
                }
            } catch (error) {
                Swal.fire({
                    icon: "error",
                    title: "Error!",
                    text: "An error occurred while fetching patient data.",
                    confirmButtonText: "OK",
                });
                console.error("Error fetching patient data:", error);
            }
        });
    });
})();



document.getElementById('addPatientModal').addEventListener('hidden.bs.modal', function ()
{
    const backdrop = document.querySelector('.modal-backdrop');
    if (backdrop) {
        backdrop.remove();
    }

    document.activeElement.blur();

    //reset the modal content
    const form = document.getElementById('addPatientForm');
    form.reset();
    form.classList.remove('was-validated');
    document.getElementById('addPatientModalLabel').textContent = 'Add New Patient';
});




// Delete Patient Logic
document.addEventListener("DOMContentLoaded", function ()
{
    const deleteButtons = document.querySelectorAll(".delete-patient-btn");

    deleteButtons.forEach((button) => {
        button.addEventListener("click", async function () {
            const patientId = this.getAttribute("data-id");

            const confirmation = await Swal.fire({
                title: "Are you sure?",
                text: "You are about to remove this patient.",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "Yes",
                cancelButtonText: "Cancel",
            });

            if (confirmation.isConfirmed)
            {
                try
                {
                    const response = await fetch(`/Patient/Delete?id=${patientId}`, {
                        method: "GET",
                    });

                    const result = await response.json();

                    if (result.success)
                    {
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
                        text: "An unexpected error occurred while trying to delete the patient!",
                        icon: "error",
                        confirmButtonText: "OK",
                    });
                }
            }
        });
    });
});
