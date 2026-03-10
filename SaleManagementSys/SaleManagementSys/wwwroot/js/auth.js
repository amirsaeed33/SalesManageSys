(function () {
    var container = document.getElementById('authContainer');
    var signUp = document.getElementById('signUp');
    var signIn = document.getElementById('signIn');
    var loginForm = document.getElementById('loginForm');
    var registerForm = document.getElementById('registerForm');
    var loginError = document.getElementById('loginError');
    var registerError = document.getElementById('registerError');

    // 🔴 NEW: Google button elements
    var googleButtons = document.querySelectorAll('.auth-btn-google');

    if (!container) return;

    // Password show/hide toggle
    function initPasswordToggles() {
        var toggles = container.querySelectorAll('.auth-password-toggle');
        toggles.forEach(function (btn) {
            btn.addEventListener('click', function () {
                var targetId = btn.getAttribute('data-target');
                var input = targetId ? document.getElementById(targetId) : null;
                if (!input) return;
                var showIcon = btn.querySelector('.auth-password-toggle-show');
                var hideIcon = btn.querySelector('.auth-password-toggle-hide');
                var isPassword = input.type === 'password';
                input.type = isPassword ? 'text' : 'password';
                if (showIcon) showIcon.style.display = isPassword ? 'none' : 'inline-block';
                if (hideIcon) hideIcon.style.display = isPassword ? 'inline-block' : 'none';
                btn.setAttribute('aria-label', isPassword ? 'Hide password' : 'Show password');
                btn.setAttribute('title', isPassword ? 'Hide password' : 'Show password');
            });
        });
    }
    initPasswordToggles();

    // 🔴 NEW: Check for Google auth response in URL
    function checkGoogleAuthResponse() {
        var urlParams = new URLSearchParams(window.location.search);
        var googleAuth = urlParams.get('googleAuth');

        if (googleAuth === 'success') {
            showGlobalMessage('Successfully logged in with Google!', 'success');
            setTimeout(function () {
                window.location.href = '/Dashboard/Index';
            }, 1500);
        } else if (googleAuth === 'error') {
            showGlobalMessage('Google login failed. Please try again.', 'error');
        } else if (googleAuth === 'exists') {
            showGlobalMessage('Email already registered. Please login with your password.', 'warning');
        }
    }

    // 🔴 NEW: Show message in the active panel
    function showGlobalMessage(message, type) {
        // Try to show in login panel first
        if (container.classList.contains('active')) {
            // Register panel is active
            if (registerError) {
                registerError.textContent = message;
                registerError.style.display = 'block';
                registerError.className = 'auth-message auth-message-' + type;

                setTimeout(function () {
                    registerError.style.display = 'none';
                }, 5000);
            }
        } else {
            // Login panel is active
            if (loginError) {
                loginError.textContent = message;
                loginError.style.display = 'block';
                loginError.className = 'auth-message auth-message-' + type;

                setTimeout(function () {
                    loginError.style.display = 'none';
                }, 5000);
            }
        }
    }

    function showError(el, msg) {
        if (!el) return;
        el.textContent = msg || 'Something went wrong.';
        el.style.display = 'block';
        el.className = 'auth-message auth-message-error';
    }

    function hideError(el) {
        if (!el) return;
        el.style.display = 'none';
        el.textContent = '';
    }

    // 🔴 NEW: Google button click handlers
    function initGoogleButtons() {
        googleButtons.forEach(function (btn) {
            btn.addEventListener('click', function (e) {
                // Prevent double-click and show loading state
                btn.classList.add('loading');
                var originalHtml = btn.innerHTML;
                btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Connecting...';

                // Store original content to restore if needed
                btn.setAttribute('data-original-html', originalHtml);
            });
        });
    }

    // 🔴 NEW: Handle Google auth errors
    window.handleGoogleError = function () {
        googleButtons.forEach(function (btn) {
            btn.classList.remove('loading');
            var originalHtml = btn.getAttribute('data-original-html');
            if (originalHtml) {
                btn.innerHTML = originalHtml;
            }
        });
        showGlobalMessage('Google authentication failed. Please try again.', 'error');
    };

    if (signUp) {
        signUp.addEventListener('click', function (e) {
            e.preventDefault();
            hideError(registerError);
            container.classList.add('active');
        });
    }
    if (signIn) {
        signIn.addEventListener('click', function (e) {
            e.preventDefault();
            hideError(loginError);
            container.classList.remove('active');
        });
    }

    if (registerForm) {
        registerForm.addEventListener('submit', function (e) {
            e.preventDefault();
            hideError(registerError);
            var firstName = document.getElementById('regFirstName').value.trim();
            var lastName = document.getElementById('regLastName').value.trim();
            var username = document.getElementById('regUsername').value.trim();
            var email = document.getElementById('regEmail').value.trim();
            var password = document.getElementById('regPassword').value;
            if (!firstName || !lastName || !username || !email || !password) {
                showError(registerError, 'Please fill all fields.');
                return;
            }
            if (password.length < 6) {
                showError(registerError, 'Password must be at least 6 characters.');
                return;
            }
            var btn = document.getElementById('registerBtn');
            if (btn) btn.disabled = true;
            fetch('/auth/register', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ firstName: firstName, lastName: lastName, username: username, email: email, password: password })
            })
                .then(function (res) { return res.json().then(function (data) { return { ok: res.ok, data: data }; }); })
                .then(function (result) {
                    var d = result.data;
                    if (result.ok && d && (d.success === true || d.Success === true)) {
                        if (d.token) localStorage.setItem('authToken', d.token);
                        else if (d.Token) localStorage.setItem('authToken', d.Token);
                        if (d.username) localStorage.setItem('authUsername', d.username);
                        else if (d.Username) localStorage.setItem('authUsername', d.Username);
                        if (d.email) localStorage.setItem('authEmail', d.email);
                        else if (d.Email) localStorage.setItem('authEmail', d.Email);

                        // 🔴 NEW: Store auth provider
                        localStorage.setItem('authProvider', d.authProvider || 'Local');

                        window.location.href = '/Dashboard/Index';
                        return;
                    }

                    // 🔴 NEW: Check if email already exists with Google
                    if (d && d.message && d.message.includes('Google')) {
                        showError(registerError, d.message);
                        // Show Google login option
                        setTimeout(function () {
                            container.classList.remove('active'); // Switch to login panel
                        }, 2000);
                    } else {
                        showError(registerError, (d && (d.message || d.Message)) || 'Registration failed.');
                    }
                })
                .catch(function () {
                    showError(registerError, 'Network error. Try again.');
                })
                .finally(function () {
                    if (btn) btn.disabled = false;
                });
        });
    }

    if (loginForm) {
        loginForm.addEventListener('submit', function (e) {
            e.preventDefault();
            hideError(loginError);
            var username = document.getElementById('loginUsername').value.trim();
            var password = document.getElementById('loginPassword').value;
            if (!username || !password) {
                showError(loginError, 'Enter username and password.');
                return;
            }
            var btn = document.getElementById('loginBtn');
            if (btn) btn.disabled = true;
            fetch('/auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username: username, password: password })
            })
                .then(function (res) { return res.json().then(function (data) { return { ok: res.ok, data: data }; }); })
                .then(function (result) {
                    var d = result.data;
                    if (result.ok && d && (d.success === true || d.Success === true)) {
                        if (d.token) localStorage.setItem('authToken', d.token);
                        else if (d.Token) localStorage.setItem('authToken', d.Token);
                        if (d.username != null) localStorage.setItem('authUsername', d.username);
                        else if (d.Username != null) localStorage.setItem('authUsername', d.Username);
                        if (d.email) localStorage.setItem('authEmail', d.email);
                        else if (d.Email) localStorage.setItem('authEmail', d.Email);

                        // 🔴 NEW: Store auth provider
                        localStorage.setItem('authProvider', d.authProvider || 'Local');

                        window.location.href = '/Dashboard/Index';
                        return;
                    }

                    // 🔴 NEW: Check if user exists with Google
                    if (d && d.message && d.message.includes('Google')) {
                        showError(loginError, d.message);
                        // Highlight Google button
                        document.querySelectorAll('.auth-btn-google').forEach(function (btn) {
                            btn.style.animation = 'googlePulse 1.5s infinite';
                        });
                    } else {
                        showError(loginError, (d && (d.message || d.Message)) || 'Invalid username or password.');
                    }
                })
                .catch(function () {
                    showError(loginError, 'Network error. Try again.');
                })
                .finally(function () {
                    if (btn) btn.disabled = false;
                });
        });
    }

    // 🔴 NEW: Initialize Google button handlers and check response
    initGoogleButtons();
    checkGoogleAuthResponse();

    // 🔴 NEW: Add keyboard shortcut (optional) - Press 'g' for Google login
    document.addEventListener('keydown', function (e) {
        if (e.key === 'g' || e.key === 'G') {
            if (!container.classList.contains('active')) {
                // Login panel active
                window.location.href = '/auth/google';
            }
        }
    });

    // 🔴 NEW: Handle browser back button after Google auth
    window.addEventListener('pageshow', function (event) {
        if (event.persisted) {
            // Page loaded from cache (back button)
            googleButtons.forEach(function (btn) {
                btn.classList.remove('loading');
                var originalHtml = btn.getAttribute('data-original-html');
                if (originalHtml) {
                    btn.innerHTML = originalHtml;
                }
            });
        }
    });
})();