// ==========================================
// CLEAN CONNECT AUTH
// ==========================================

document.addEventListener("DOMContentLoaded", () => {

    // ==========================
    // SHOW / HIDE PASSWORD
    // ==========================

    const togglePassword = document.getElementById("togglePassword");
    const password = document.getElementById("password");

    if (togglePassword && password) {

        togglePassword.addEventListener("click", function () {

            const type = password.getAttribute("type") === "password"
                ? "text"
                : "password";

            password.setAttribute("type", type);

            const icon = this.querySelector("i");

            icon.classList.toggle("fa-eye");
            icon.classList.toggle("fa-eye-slash");

        });

    }

    // ==========================
    // LOGIN BUTTON LOADING
    // ==========================

    const loginForm = document.querySelector("form");
    const loginButton = document.querySelector(".login-btn");

    if (loginForm && loginButton) {

        loginForm.addEventListener("submit", function () {

            if (!loginForm.checkValidity()) {
                return;
            }

            loginButton.classList.add("loading");

            loginButton.disabled = true;

            loginButton.innerHTML = "Signing In";

        });

    }

    // Role selection

    const roleCards = document.querySelectorAll(".role-card");
    const selectedRole = document.getElementById("SelectedRole");

    roleCards.forEach(card => {

        card.addEventListener("click", () => {

            roleCards.forEach(c => c.classList.remove("active"));

            card.classList.add("active");

            if (selectedRole) {
                selectedRole.value = card.dataset.role;
            }

        });

    });

});