//CONTEXT MENU
function Context(element) {
    var contextElement = $('#context-menu');
    $(element).contextmenu(function (event) {
        event.preventDefault();

        var colorHue = $(this).attr('data-colorHue');
        var darkedColor = 'hsl(' + colorHue + ',53%,52%)';
        var lightColor = 'hsl(' + colorHue + ',100%, 98.5%)';
        //var HaveLab = $(this).attr('data-lab') == 'True';

        contextElement.find('input').attr('data-courseId', $(this).attr('data-courseId'));
        contextElement.children('h4').css('color', 'hsl(' + colorHue + ', 53%, 52%)');

        //contextElement.find('#context-lab').css('display', HaveLab ? 'block' : 'none'); //LABORATORY

        contextElement.find('li').mouseenter(function () {
            $(this).css('color', darkedColor);
            $(this).css('background-color', lightColor);
        });
        contextElement.find('li').mouseleave(function () {
            $(this).css('color', 'gray');
            $(this).css('background-color', 'transparent');
        });

        contextElement.children('h4').text($(this).find('.course-shortName').text());
        contextElement.css('top', (event.clientY - 10) + 'px');
        contextElement.css('left', (event.clientX - contextElement.width() + 20) + 'px');
        contextElement.addClass('active');

        contextElement.mouseleave(function () {
            contextElement.removeClass('active');
        });

        document.addEventListener("scroll", function () {
            contextElement.removeClass('active');
        });
    });
}

function ContextCourse() {
    var contextElement = $('#context-menu');
    var colorHue = $('.topic-files').attr('data-colorHue');
    $('.topic-files li').contextmenu(function (event) {
        event.preventDefault();

        var darkedColor = 'hsl(' + colorHue + ',53%,52%)';
        var lightColor = 'hsl(' + colorHue + ',100%, 98.5%)';
        var lab = contextElement.find('#context-lab');
        var isTools = $(this).attr('data-fileType').includes('tools');
        //$.ajax({
        //    url: '/Pages/SetFileChecked',
        //    type: 'POST',
        //    data: { CourseId, FileId, CourseTopicFileId, IsChecked: $(this).is(':checked') },
        //    success: function () {
        //        $.ajax({
        //            url: '/Pages/UpdateCourseEvents',
        //            data: { IsUpdateEvents: true },
        //            type: 'POST',
        //            success: function (e) {
        //                $('#CourseEvents').html(e);
        //            },
        //        });
        //    }
        //});
        if (isTools) {
            lab.addClass('labtools');
            lab.removeClass('labzoom');
        }
        else {
            lab.addClass('labzoom');
            lab.removeClass('labtools');
        }
        lab.css('display', 'block');
        lab.find('a').attr('href', '/laboratory/' + (isTools ? 'tools' : 'zoom') + '/' + $(this).attr('data-courseTopicFile') + '/' + $(this).attr('data-file'))

        contextElement.find('li').mouseenter(function () {
            $(this).css('color', darkedColor);
            $(this).css('background-color', lightColor);
        });
        contextElement.find('li').mouseleave(function () {
            $(this).css('color', 'gray');
            $(this).css('background-color', 'transparent');
        });

        contextElement.children('h4').css('color', 'hsl(' + colorHue + ', 53%, 52%)');
        contextElement.children('h4').text($(this).find('.course-shortName').text());
        contextElement.css('top', (event.clientY - 10) + 'px');
        contextElement.css('left', (event.clientX - contextElement.width() + 20) + 'px');
        contextElement.addClass('active');

        contextElement.mouseleave(function () {
            contextElement.removeClass('active');
        });

        document.addEventListener("scroll", function () {
            contextElement.removeClass('active');
        });
    });
}

//$('#Menu').click(function (e) {
//    e.preventDefault();
//    $('.leftBox .box-container').toggleClass('menuActive');
//    $('.centerBox').toggleClass('menuActive');
//});

