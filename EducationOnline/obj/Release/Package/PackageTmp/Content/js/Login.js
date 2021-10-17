function onSignIn(googleAuth) {

    var Email = googleAuth.getBasicProfile().getEmail();
    var FirstName = googleAuth.getBasicProfile().getGivenName();
    var LastName = googleAuth.getBasicProfile().getFamilyName();
    var Photo = googleAuth.getBasicProfile().getImageUrl();

    $.post('/Account/GoogleAuth', { Email, FirstName, LastName, Photo }, function (IsAccess) {
        if (IsAccess == 'True') {
            $.post('/Account/AuthUser', { Email }, function (IsAuth) {
                if (IsAuth == 'True') {
                    window.location.href = '/';
                    //window.location.reload();
                }
            });
        } else {
            alert("У вас нет доступа к данному ресурсу");
            signOut();
            window.location.reload();
        }
    });
}

function signOut() {
    var auth2 = gapi.auth2.getAuthInstance();
    auth2.signOut().then(function () {
        $.get('/Account/Logout', {}, function () {
            window.location.reload();
        });
    });
}

function onLoad() {
    gapi.load('auth2', function () {
        gapi.auth2.init();
    });
}