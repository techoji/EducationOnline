var TestId = $('.test').attr('data-testId');
var CourseId = $('.test').attr('data-courseId');
var TestCourseTopicId = $('.test').attr('data-testCourseTopicId');

if ($('.testStarted').length)
    $('.testStarted').ready(function () {

        var testTimeEnd = $('#testClosingInResult').val();
        var totalSec = GetTestMinutes(testTimeEnd);

        $('.test-options').addClass('testStarted');
        $('.test-options li').removeClass('notActive');

        if (totalSec <= 60)
            DangerTimer(totalSec);
        if (totalSec <= 0)
            return;

        var seconds = totalSec % 60;
        var minutes = Math.floor(totalSec / 60);
        $('#timer').text((minutes.toString().length == 1 ? '0' + minutes : minutes) + ':' + (seconds.toString().length == 1 ? '0' + seconds : seconds));

        var timer = setInterval(function UpdateTimer() {
            totalSec = GetTestMinutes($('#testClosingInResult').val());

            seconds = totalSec % 60;
            minutes = Math.floor(totalSec / 60);

            if (totalSec <= 60)
                DangerTimer(totalSec);

            $('#timer').text((minutes.toString().length == 1 ? '0' + minutes : minutes) + ':' + (seconds.toString().length == 1 ? '0' + seconds : seconds));
            if (totalSec <= 0) {
                clearInterval(timer);
                SaveAnswerResult();
                $.post('/Pages/TestStop', { TestId, CourseId, TestCourseTopicId }, function (e) {
                    window.location.href = '/pages/course/' + CourseId;
                })
            }
        }, 1000);
    });
else {
    var minutes = $('#testTimeResult').val();
    $('#testTime').text(minutes);
    $('#testAttempts').text($('#testAttemptsResult').val());
    $('#testQuestions').text($('#testQuestionsResult').val());
    $('#timer').ready(function () {
        $('#timer').text((minutes.toString().length == 1 ? '0' + minutes : minutes) + ':' + '00');
    });
}

$('#StopTest').click(function (e) {
    e.preventDefault();
    if (confirm("Вы уверены что хотите закончить тест?"))
        $(this).parent('form').submit();
});

var lightColor = 'hsl(' + $('.test').attr('data-colorHue') + ', 100%, 98.5%)';
var darkedColor = 'hsl(' + $('.test').attr('data-colorHue') + ', 53%, 52%)';

$('.courseNav').mouseenter(function () {
    $(this).css('background-color', lightColor);
})
$('.courseNav').mouseleave(function () {
    $(this).css('background-color', 'transparent');
})

function GetTestMinutes(date) {
    var timeTest = new Date(date);
    var timeNow = new Date();
    return Math.floor((timeTest - timeNow) / 1000);
}

function DangerTimer(totalSec) {
    $('#timer').css('color', '#C54242');
    if (totalSec % 2 == 0 || totalSec <= 0)
        $('.test-options').addClass('dangerTime');
    else
        $('.test-options').removeClass('dangerTime');
}


//TEST
$(".testNavigation input[type='submit']").click(function () {
    SaveAnswerResult();
});

$(".testQuestionNavigation").click(function () {
    SaveAnswerResult();
});

function SaveAnswerResult() {
    var TestQuestionId = $('.questionBox').attr('data-testQuestionId');
    var Answers = [];

    $('.answers input:checkbox:checked').each(function () {
        Answers.push($(this).val());
    });

    $('.answers input:radio:checked').each(function () {
        Answers.push($(this).val());
    });

    $.post('/Pages/SubmitTestAnswer', { TestId, TestQuestionId, Answers });
}