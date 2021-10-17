using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DistanceLearning.Models.TeacherUpdates {
    public class DeleteCourseTopicFileModel {

        public int FileId { get; set; }
        public int CourseId { get; set; }
        public int? CourseTopicId { get; set; }
        public bool IsVariant { get; set; }

    }
}