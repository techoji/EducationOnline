var countResult = 1;
var IsClicked = false;
var delay;
$('#CanvasTools').click(function (e) {
    if (!IsToolSelected) {
        alert("Инструмент не выбран");
        return;
    }

    var fileId = $(this).attr('data-file');

    if (!IsClicked) {
        $.post('/lab/SetStatisticPerformed', { CourseTopicFileId: $(this).attr('data-courseTopicFile') });
        IsClicked = true;
    }

    if (delay != null)
        return;

    var offset = $(this).offset();
    var X = e.pageX - offset.left;
    var Y = e.pageY - offset.top;

    $.post('/Lab/GetPixelInfo', { coorX: parseInt(X), coorY: parseInt(Y), fileId: fileId }, function (pixelData) {
        if (countResult > 4)
            for (var i = 5; i < countResult; i++)
                $('#resultData' + (i - 4)).remove();

        //if (IsDensitometerSelected) {
        //    $('#result').prepend(
        //        '<li id="resultData' + countResult + '" class="' + pixelData.Color +
        //        '"><div class="resultContainer"><div class="resultName"><h4>' + pixelData.Comment +
        //        '</h4><p class="tool">Денситометр</p></div><div class="resultData">' +
        //        '<p>' + pixelData.L + '<span>L</span>' +
        //        '<p>' + pixelData.A + '<span>A</span>' +
        //        '<p>' + pixelData.B + '<span>B</span>' +
        //        '</p></div></li>'
        //    );
        //}
        //else if (IsSpectrumSelected) {
        //    $('#result').prepend(
        //        '<li id="resultData' + countResult + '" class="' + pixelData.Color +
        //        '"><div class="resultContainer"><div class="resultName"><h4>' + pixelData.Comment +
        //        '</h4><p class="tool">Спектрофотометр</p></div><div class="resultData">' +
        //        '<p>' + pixelData.C + '<span>C</span>' +
        //        '<p>' + pixelData.M + '<span>M</span>' +
        //        '<p>' + pixelData.Y + '<span>Y</span>' +
        //        '<p>' + pixelData.V + '<span>V</span>' +
        //        '</p></div></div></li>'
        //    );
        //}
        if (IsSpectrumSelected) {
            $('#result').prepend(
                '<li id="resultData' + countResult + '">' +
                '<div class="resultContainer"><div class="resultName"><h4>' + pixelData.Comment +
                '</h4><p class="tool">Спектрофотометр</p></div><div class="resultData">' +
                '<p>' + pixelData.L + '<span>L</span>' +
                '<p>' + pixelData.A + '<span>A</span>' +
                '<p>' + pixelData.B + '<span>B</span>' +
                '</p></div></li>'
            );
        }
        else if (IsDensitometerSelected) {
            $('#result').prepend(
                '<li id="resultData' + countResult + '">' +
                '<div class="resultContainer"><div class="resultName"><h4>' + pixelData.Comment +
                '</h4><p class="tool">Денситометр</p></div><div class="resultData">' +
                '<p>' + pixelData.C + '<span>C</span>' +
                '<p>' + pixelData.M + '<span>M</span>' +
                '<p>' + pixelData.Y + '<span>Y</span>' +
                '<p>' + pixelData.V + '<span>V</span>' +
                '</p></div></div></li>'
            );
        }

        $('#resultData' + countResult).slideDown(200);
    });
    countResult++;

    if (delay == null)
        delay = setTimeout(function () {
            clearTimeout(delay);
            delay = null;
        }, 300);
});

$('#CanvasZoom').click(function (e) {
    if (!IsToolSelected) {
        alert("Инструмент не выбран");
        return;
    }

    if (delay != null)
        return;

    if (!IsClicked) {
        $.post('/lab/SetStatisticPerformed', { CourseTopicFileId: $(this).attr('data-courseTopicFile') });
        IsClicked = true;
    }

    var offset = $(this).offset();
    var X = e.pageX - offset.left;
    var Y = e.pageY - offset.top;

    $('#CropingImage').html(
        '<img src="' + '/laboratory/GetZoomingImage/' + parseInt(X) + '/' + parseInt(Y) + '/' + $('#PhotoLoc').val() + '" />'
    );

    if (delay == null)
        delay = setTimeout(function () {
            clearTimeout(delay);
            delay = null;
        }, 1200);
});

var IsDensitometerSelected = false;
var IsSpectrumSelected = false;
var IsZoomSelected = false;
var IsToolSelected = false;
$('.tools ul li').click(function () {
    IsDensitometerSelected = $(this).attr('id') == 'Densitometer';
    IsSpectrumSelected = $(this).attr('id') == 'Spectrum';
    IsZoomSelected = $(this).attr('id') == 'Zoom';
    var tools = $('.tools ul li');
    tools.not(this).removeClass('toolActive');
    $(this).toggleClass('toolActive');
    IsToolSelected = $(this).hasClass('toolActive');
    if (IsToolSelected)
        $('.canvasBox img').css('cursor', 'crosshair');
    else
        $('.canvasBox img').css('cursor', 'default');
});
