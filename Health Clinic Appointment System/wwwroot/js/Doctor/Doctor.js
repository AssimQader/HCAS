

//validations
(function () {
    'use strict';

    //define a regex map 
    const regexMap = {
        PhoneNumber: /^\+2?[1-9]\d{1,14}$/, // E.164 format regex
        Email: /^[^\s@]+@[^\s@]+\.[^\s@]+$/, // Basic email format
    };

    //validate a single input field
    const validateInput = (input) => {
        const regex = regexMap[input.id]; // Get regex based on input ID
        if (regex)
        {
            if (regex.test(input.value)) {
                input.classList.add("is-valid");
                input.classList.remove("is-invalid");
            } else {
                input.classList.add("is-invalid");
                input.classList.remove("is-valid");
            }
        } else if (input.checkValidity()) {
            input.classList.add("is-valid");
            input.classList.remove("is-invalid");
        } else {
            input.classList.add("is-invalid");
            input.classList.remove("is-valid");
        }
    };


    //add real-time validation to all inputs
    const inputs = document.querySelectorAll("input, select");
    inputs.forEach((input) => {
        input.addEventListener("input", () => validateInput(input));
    });


    //handle form submission
    const forms = document.querySelectorAll('.needs-validation');
    forms.forEach((form) => {
        form.addEventListener('submit', (event) => {
            let isFormValid = true;

            //validate all inputs before submission
            inputs.forEach((input) => {
                validateInput(input);
                if (!input.classList.contains("is-valid")) {
                    isFormValid = false;
                }
            });

            if (!isFormValid || !form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }

            form.classList.add('was-validated');
        });
    });
})();




//after add new doctor succesfully: 
document.getElementById("addDoctorForm").addEventListener("submit", async function (event)
{
    event.preventDefault();

    const form = this;
    const formData = new FormData(form);

    try
    {
        //send the form data to the server using Fetch API
        const response = await fetch("/Doctor/Create", {
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
                document.getElementById("addDoctorModal").click(); //close modal
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

