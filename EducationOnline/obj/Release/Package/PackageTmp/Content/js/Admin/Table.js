$('#Create').click(function (e) {
    e.preventDefault();
    $('#Add').show();
    $(this).hide();
});

$('#CancelCreate').click(function (e) {
    e.preventDefault();
    $('#Add').hide();
    $('#Create').show();
});
