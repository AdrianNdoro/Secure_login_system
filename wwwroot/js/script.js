document.addEventListener("DOMContentLoaded", function () {
    // Register Form Submission
    document.getElementById("registerForm")?.addEventListener("submit", async function (event) {
        event.preventDefault();

        const username = document.getElementById("username").value;
        const email = document.getElementById("email").value;
        const password = document.getElementById("password").value;

        const response = await fetch("/Auth/Register", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, email, password })
        });

        const result = await response.text();
        alert(result);
    });

    // Login Form Submission
    document.getElementById("loginForm")?.addEventListener("submit", async function (event) {
        event.preventDefault();

        const email = document.getElementById("email").value;
        const password = document.getElementById("password").value;

        const response = await fetch("/Auth/Login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email, password })
        });

        const result = await response.json(); // Expecting JSON response

        if (result.message === "Login successful") {
            sessionStorage.setItem("loggedInUser", result.username);
            alert("Login successful!");
            window.location.reload(); // Reload page to reflect login state
        } else {
            alert("Invalid credentials");
        }
    });

    // Logout Button Handling
    const logoutBtn = document.getElementById("logoutBtn");

    if (logoutBtn) {
        // Show the logout button if the user is logged in
        if (sessionStorage.getItem("loggedInUser")) {
            logoutBtn.style.display = "block";
        }

        // Logout Button Click Event
        logoutBtn.addEventListener("click", async function () {
            const response = await fetch("/Auth/Logout", { method: "POST" });

            if (response.ok) {
                sessionStorage.removeItem("loggedInUser"); // Clear session storage
                alert("Logged out successfully");
                window.location.reload(); // Reload to update UI
            } else {
                alert("Error logging out");
            }
        });
    }
});
