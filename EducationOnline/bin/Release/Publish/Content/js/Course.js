$('#CourseContainer').ready(function () {
    $('#CourseContainer').load('/pages/GetCourseTopicFile', { CourseId: $('#CourseContainer').attr('data-course'), IsEdit: $('#CourseContainer').attr('data-edit') });
});

