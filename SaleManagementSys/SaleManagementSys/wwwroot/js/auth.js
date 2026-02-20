(function () {
  var container = document.getElementById('authContainer');
  var signUp = document.getElementById('signUp');
  var signIn = document.getElementById('signIn');
  var loginForm = document.getElementById('loginForm');
  var registerForm = document.getElementById('registerForm');
  var loginError = document.getElementById('loginError');
  var registerError = document.getElementById('registerError');

  if (!container) return;

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
      var username = document.getElementById('regUsername').value.trim();
      var email = document.getElementById('regEmail').value.trim();
      var password = document.getElementById('regPassword').value;
      if (!username || !email || !password) {
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
        body: JSON.stringify({ username: username, email: email, password: password })
      })
        .then(function (res) { return res.json().then(function (data) { return { ok: res.ok, data: data }; }); })
        .then(function (result) {
          var d = result.data;
          if (result.ok && d && (d.success === true || d.Success === true)) {
            if (d.token) localStorage.setItem('authToken', d.token);
            else if (d.Token) localStorage.setItem('authToken', d.Token);
            window.location.href = '/Dashboard';
            return;
          }
          showError(registerError, (d && (d.message || d.Message)) || 'Registration failed.');
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
            window.location.href = '/Dashboard';
            return;
          }
          showError(loginError, (d && (d.message || d.Message)) || 'Invalid username or password.');
        })
        .catch(function () {
          showError(loginError, 'Network error. Try again.');
        })
        .finally(function () {
          if (btn) btn.disabled = false;
        });
    });
  }
})();
