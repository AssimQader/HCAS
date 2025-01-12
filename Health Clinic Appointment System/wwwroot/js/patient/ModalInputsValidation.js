

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



