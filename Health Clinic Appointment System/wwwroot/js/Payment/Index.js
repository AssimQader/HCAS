document.addEventListener("DOMContentLoaded", () =>
{
    const patientSelect = document.getElementById("patientSelect");
    const appointmentCardsContainer = document.getElementById("appointmentCardsContainer");
    const addPaymentBtn = document.getElementById("AddPaymentBtn");
    const modal = document.getElementById("manageAppointmentsModal");


    function closeModalAndReload() {
        modal.style.display = "none";
        location.reload();
    }


    // When the user clicks anywhere outside the modal, close it and reload the page
    window.onclick = function (event) {
        if (event.target === modal) {
            closeModalAndReload();
        }
    };


    addPaymentBtn.addEventListener("click", async () => {
        try {
            const response = await fetch("/Patient/GetAllWithAppointments");
            if (!response.ok) throw new Error("Failed to fetch patients.");

            const result = await response.json();

            // Populate the patient select dropdown
            result.details.forEach((patient) => {
                const option = document.createElement("option");
                option.value = patient.id;
                option.textContent = patient.fullName;
                patientSelect.appendChild(option);
            });

            // Reset the card container
            appointmentCardsContainer.innerHTML = `<p class="text-muted">Select a patient to view appointments.</p>`;

            // Handle patient selection
            patientSelect.addEventListener("change", function () {
                const patientId = this.value;

                // Find the selected patient's appointments
                const selectedPatient = result.details.find((patient) => patient.id == patientId);
                const appointments = selectedPatient?.appointments || [];

                // Clear existing cards
                appointmentCardsContainer.innerHTML = "";

                // Generate appointment cards
                if (appointments.length > 0)
                {
                    appointments.forEach((appointment) => {
                        const card = document.createElement("div");
                        card.className = "col-md-6";
                        card.innerHTML = `
                            <div class="card shadow-sm">
                                <div class="card-body" data-id="${appointment.id}">
                                    <h6 class="card-title text-primary">${appointment.doctor.fullName}</h6>
                                    <p class="card-text">
                                        <strong>Specialization:</strong> ${appointment.doctor.specialization}<br>
                                        <strong>Start:</strong> ${new Date(appointment.startDateTime).toLocaleString()}<br>
                                        <strong>End:</strong> ${new Date(appointment.endDateTime).toLocaleString()}<br>
                                        <strong>Payment Status:</strong> 
                                        <span class="badge ${appointment.paymentStatus === "Paid" ? "bg-success" : "bg-warning"}">
                                            ${appointment.paymentStatus}
                                        </span>
                                    </p>
                                    <div class="input-group mb-3">
                                        <span class="input-group-text">Fees</span>
                                        <input type="number" class="form-control fees-input" placeholder="Enter Amount" />
                                    </div>
                                    <button class="btn btn-success w-100 pay-btn">Pay</button>
                                </div>
                            </div>
                        `;


                        // Add event listener for Pay button
                        card.querySelector(".pay-btn").addEventListener("click", async () => {
                            const feesInput = card.querySelector(".fees-input");
                            const fees = parseFloat(feesInput.value); // Convert to a floating-point number
                            const appointmentId = parseInt(card.querySelector(".card-body").getAttribute("data-id"));

                            if (!fees) {
                                Swal.fire("Error", "Please enter a valid fee amount.", "error");
                                return;
                            }

                            try
                            {
                                // Step 1: Add payment
                                const formData = new FormData();
                                formData.append("AppointmentID", appointmentId);
                                formData.append("Amount", fees);
                                formData.append("PaymentMethod", "Cash");

                                const paymentResponse = await fetch("/Payment/Create", {
                                    method: "POST",
                                    body: formData,
                                });

                                const paymentResult = await paymentResponse.json();

                                if (!paymentResponse.ok || !paymentResult.success)
                                {
                                    Swal.fire("Error", "Payment Failed! Please try again.", "error");
                                    return;
                                }

                                // Step 2: Update payment status in Appointment table
                                const appointmentResponse = await fetch(`/Appointment/Edit?id=${appointmentId}`);
                                if (!appointmentResponse.ok )
                                {
                                    Swal.fire("Error", "Failed to update payment status in Appointments table!", "error");
                                    return;
                                }

                                // Step 3: Update UI
                                Swal.fire("Success", "Payment processed successfully.", "success");
                                card.querySelector(".badge").className = "badge bg-success";
                                card.querySelector(".badge").textContent = "Paid";
                            }
                            catch (error)
                            {
                                Swal.fire("Error", "An unexpected error occurred! Please try again.", "error");
                                console.error("Error processing payment:", error);
                            }
                        });

                        appointmentCardsContainer.appendChild(card);
                    });
                } else {
                    appointmentCardsContainer.innerHTML = `<p class="text-muted">No appointments available for this patient.</p>`;
                }
            });
        } catch (error) {
            console.error("Error fetching patient data:", error);
            Swal.fire("Error", "An error occurred while fetching patient data. Please try again later.", "error");
        }
    });

});
