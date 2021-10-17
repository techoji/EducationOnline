$("input[type='checkbox'].fileCheckbox").click(function () {
    var CourseId = $(this).attr('data-courseId');
    var FileId = $(this).attr('data-fileId');
    var CourseTopicFileId = $(this).attr('data-courseTopicFileId');
    $.ajax({
        url: '/Pages/SetFileChecked',
        type: 'POST',
        data: { CourseId, FileId, CourseTopicFileId, IsChecked: $(this).is(':checked') },
        success: function () {
            $.ajax({
                url: '/Pages/UpdateCourseEvents',
                data: { IsUpdateEvents: true },
                type: 'POST',
                success: function (e) {
                    $('#CourseEvents').html(e);
                },
            });
        }
    });
});

$("input[type='checkbox'].testCheckbox").click(function () {
    var CourseId = $(this).attr('data-courseId');
    var TestId = $(this).attr('data-testId');
    var TestCourseTopicId = $(this).attr('data-testCourseTopicId');
    $.post(
        '/Pages/SetTestChecked',
        { TestCourseTopicId: TestCourseTopicId, TestId: TestId, CourseId: CourseId, IsChecked: $(this).is(':checked') },
        function () {
            $.post(
                '/Pages/UpdateCourseEvents',
                { IsUpdateEvents: true },
                function (e) {
                    $('#CourseEvents').html(e);
                }
            );
        }
    );
});

//teacher
$('#UploadFileBtn').click(function () {
    $('#FileUploadBtn').click();
});

$('#UploadExcelFileBtn').click(function () {
    $('#ExcelFileUploadBtn').click();
});

$('#FileUploadBtn').change(function (e) {
    var value = e.target.files[0].name;
    if (value) {
        $('#UploadFileText').text(value);
    }
});

$('#ExcelFileUploadBtn').change(function (e) {
    var value = e.target.files[0].name;
    if (value) {
        $('#UploadExcelFileText').text(value);
    }
});

$('#FileType').change(function () {
    var fileId = $(this).val();
    if (fileId == 40) {
        $('#ExcelUploadBlock').show();
    }
    else {
        $('#ExcelUploadBlock').hide();
    }
});

$('#IsRandom').change(function () {
    var IsRandom = $(this).val();
    if (IsRandom == 'true') {
        $('#CourseTopicFileName').show();
        $('#CourseTopicFileName').attr('required', true);
    }
    else {
        $('#CourseTopicFileName').hide();
        $('#CourseTopicFileName').removeAttr('required');
    }
});

function UpdateEvents() {
    $('#CourseEvents').load('/pages/UpdateCourseEvents', { IsUpdateEvents: true });
}

var interval;
function UploadStart() {
    if ($('#FileType').val() == 5) {
        var loadingPercent = $('#LoadingPercent');
        var loadingPercentText = $('#LoadingPercentText');
        var popup = $('#PopupUpload');
        popup.slideDown(300);

        interval = setInterval(function () {
            $.post('/pages/GetFileCompressPercent', {}, function (percent) {
                loadingPercent.css("width", percent + "%");
                loadingPercentText.text(percent + '%');
                if (percent == 100) {
                    popup.slideUp(300);
                    loadingPercent.css("width", 0 + "%");
                    loadingPercentText.text(0 + '%');
                    $.post('/pages/DeleteFileCompressPercent', {});
                    clearInterval(interval);

                    setTimeout(function () {
                        if (confirm("Изображение сжато. Для загрузки файла в курс необходимо перезагрузить страницу."))
                            location.reload();
                    }, 500);
                }
            });
        }, 1000);
    }
}

if ($('#FileContainerTopic').length) {
    $('#FileContainerTopic').load('/pages/UpdateTeacherFileList', { FileTypeId: 0, CourseId: $('#FileContainerTopic').attr('data-course') });

    $('#TypeSort').change(function () {
        $('#FileContainerTopic').load('/pages/UpdateTeacherFileList', { FileTypeId: $('#TypeSort').val(), CourseId: $('#FileContainerTopic').attr('data-course') });
    });
}

ContextCourse();