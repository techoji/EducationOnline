using System;

namespace DistanceLearning.Models.TeacherUpdates {
    public class UploadFileToTheCourse {

        public int CourseId { get; set; }
        public int[] FilesId { get; set; }
        public int? CourseTopicId { get; set; }
        public bool IsRandom { get; set; }
        public String Name { get; set; }

    }
}