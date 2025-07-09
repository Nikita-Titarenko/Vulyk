function handleCredentialResponse(response) {
    fetch('/Account/GoogleSignIn', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ IdToken: response.credential })
    }).then(response => {
        if (response.redirected) {
            window.location.href = response.url;
        }
    })
}