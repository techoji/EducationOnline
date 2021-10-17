var courses = $('#Courses');

if (courses.length) {
    var loading = setTimeout(function () {
        courses.append('<h4>Загрузка курсов...</h4>');
    }, 1000);
    courses.load('/pages/GetCourses', function () {
        clearTimeout(loading);
        Context('.subject');
    });
}

$("input[type='color']").change(function () {

    var courseId = $(this).attr('data-courseId');

    var HSL = hexToHSL($(this).val().substring(1));

    var darkedColor = 'hsl(' + HSL['h'] + ',53%,52%)';
    var lightColor = 'hsl(' + HSL['h'] + ',100%, 98.5%)';

    $(".subject[data-courseId='" + courseId + "']").attr('data-colorHue', HSL['h']);

    $(".subject[data-courseId='" + courseId + "']").find('.imgBox').css('background-color', lightColor);
    $(".subject[data-courseId='" + courseId + "']").find('.course-shortName').css('color', darkedColor);
    $(".subject[data-courseId='" + courseId + "']").find('.percentScale').css('background-color', darkedColor);
    $(".subject[data-courseId='" + courseId + "']").find('.donePercent').css('border-color', darkedColor);

    $.post('/pages/UpdateUserCourseColor', { CourseId: courseId, ColorHue: HSL['h'] });

    $.get('/pages/UpdateCourseEvents', { IsUpdateEvents: false }, function (e) {
        $('#CourseEvents').html(e);
    });
});

function rgbToHsl(r, g, b) {
    r /= 255, g /= 255, b /= 255;
    var max = Math.max(r, g, b), min = Math.min(r, g, b);
    var h, s, l = (max + min) / 2;

    if (max == min) {
        h = s = 0; // achromatic
    } else {
        var d = max - min;
        s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
        switch (max) {
            case r: h = (g - b) / d + (g < b ? 6 : 0); break;
            case g: h = (b - r) / d + 2; break;
            case b: h = (r - g) / d + 4; break;
        }
        h /= 6;
    }

    return [h, s, l];
}

function hexToHSL(hex) {
    var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    var r = parseInt(result[1], 16);
    var g = parseInt(result[2], 16);
    var b = parseInt(result[3], 16);

    r /= 255, g /= 255, b /= 255;
    var max = Math.max(r, g, b), min = Math.min(r, g, b);
    var h, s, l = (max + min) / 2;

    if (max == min) {
        h = s = 0; // achromatic
    } else {
        var d = max - min;
        s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
        switch (max) {
            case r: h = (g - b) / d + (g < b ? 6 : 0); break;
            case g: h = (b - r) / d + 2; break;
            case b: h = (r - g) / d + 4; break;
        }
        h /= 6;
    }

    s = s * 100;
    s = Math.round(s);
    l = l * 100;
    l = Math.round(l);
    h = Math.round(360 * h);

    var HSL = new Object();

    HSL['h'] = h;
    HSL['s'] = s;
    HSL['l'] = l;

    return HSL;
}
