using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DistanceLearning.Models.TeacherUpdates {
    public class UploadFileModel {

        public HttpPostedFileBase File { get; set; }
        public HttpPostedFileBase AttachedFile { get; set; }
        public String FileName { get; set; }
        public String Course { get; set; }
        public int CourseId { get; set; }
        public DateTime ClosingIn { get; set; }
        public int FileType { get; set; }


    }
}