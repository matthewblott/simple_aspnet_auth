window.addEventListener('load', function (event) {
  localStorage.clear(); //if the user refreshes the page the tokens are discarded

  document.getElementById('btCreate').addEventListener('click', onAddNewUserClicked);
  document.getElementById('btLogin').addEventListener('click', onLoginClicked);
  document.getElementById('btLogout').addEventListener('click', onLogoutClicked);
  document.getElementById('btAuthenticatedRequest').addEventListener('click', onPerformAuthenticatedRequestClicked);
  document.getElementById('btRevoke').addEventListener('click', onRevokeClicked);
});

function writeFeedback(feedback) {
  document.getElementById('feedbackContainer').textContent = feedback;
}

function onAddNewUserClicked() {
  var name = document.getElementById('newName').value;
  var password = document.getElementById('password').value;
  var repassword = document.getElementById('repassword').value;

  if (name === '') {
    writeFeedback("Name is required");
    return;
  }

  if (password !== repassword) {
    writeFeedback("Passwords don't match");
    return;
  }

  fetch('/api/signup', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/x-www-form-urlencoded'
    },
    body: `name=${name}&password=${password}`
  }).then(response => {
    if (response.ok)
      writeFeedback(`User ${name} was created, you can now login`);
    else
      writeFeedback("Error creating new name, make sure you haven't used this name before");
  });
}

async function onLoginClicked() {
  var name = document.getElementById('name').value;
  var password = document.getElementById('loginPassword').value;

  try {
    await login(name, password);
    writeFeedback('');
    document.getElementById('signedInAs').textContent = name;
  } catch {
    writeFeedback('Wrong name/password');
  }
}

async function login(name, password) {
  var loginResponse = await fetch('/api/login', {
    method: 'POST',
    body: "name=" + name + "&password=" + password,
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
  });
  if (loginResponse.ok) {
    var tokens = await loginResponse.json();
    saveJwtToken(tokens.token);
    saveRefreshToken(tokens.refreshToken);
  } else {
    throw new Error("Failed to login");
  }
}

function onLogoutClicked() {
  localStorage.clear();
  document.getElementById('signedInAs').textContent = 'Anonymous';
  writeFeedback('');
}

function getJwtToken() {
  return localStorage.getItem('token');
}
function saveJwtToken(token) {
  localStorage.setItem('token', token);
}

function saveRefreshToken(refreshToken) {
  localStorage.setItem('refreshToken', refreshToken);
}

function getRefreshToken() {
  return localStorage.getItem('refreshToken');
}

async function onPerformAuthenticatedRequestClicked() {
  writeFeedback('');
  var response = await fetchWithCredentials('/api/test');
  if (response.ok) {
    writeFeedback(await response.text());
  } else {
    writeFeedback(`Request failed with status code: ${response.status}`);
  }
}

async function onRevokeClicked() {
  writeFeedback('');
  var revokeResponse = await revoke();
  if (revokeResponse.ok) {
    writeFeedback('Refresh token was revoked, when the access token (JWT) expires authenticated requests will start to fail');
  } else {
    writeFeedback(`Revoke failed with status code: ${revokeResponse.status}`);
  }
}

async function fetchWithCredentials(url, options) {
  var jwtToken = getJwtToken();
  options = options || {};
  options.headers = options.headers || {};
  options.headers['Authorization'] = 'Bearer ' + jwtToken;
  var response = await fetch(url, options);
  if (response.ok) {
    return response;
  }

  if (response.status === 401 && response.headers.has('Token-Expired')) {
    var refreshToken = getRefreshToken();

    var refreshResponse = await refresh(jwtToken, refreshToken);
    if (!refreshResponse.ok) {
      return response;
    }
    var jsonRefreshResponse = await refreshResponse.json();

    saveJwtToken(jsonRefreshResponse.token);
    saveRefreshToken(jsonRefreshResponse.refreshToken);
    return await fetchWithCredentials(url, options); //repeat the original request
  } else { //status is not 401 and/or there's no Token-Expired header
    return response; //return the original 401 response
  }
}

function refresh(jwtToken, refreshToken) {
  return fetch('token/refresh', {
    method: 'POST',
    body: `token=${encodeURIComponent(jwtToken)}&refreshToken=${encodeURIComponent(getRefreshToken())}`,
    headers: {
      'Content-Type': 'application/x-www-form-urlencoded'
    }
  });
}

function revoke() {
  return fetchWithCredentials('token/revoke', {
    method: 'POST'
  });
}