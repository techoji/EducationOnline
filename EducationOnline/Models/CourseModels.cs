using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DistanceLearning.Models {

    [Table("Courses")]
    public class Course {
        [Key]
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
    }

    [Table("Files")]
    public class File {
        [Key]
        public int FileId { get; set; }
        public int CourseId { get; set; }
        public int FileTypeId { get; set; }
        public string Name { get; set; }
        public DateTime ClosingIn { get; set; }

        [ForeignKey("FileTypeId")]
        public virtual FileType FileType { get; set; }
    }

    [Table("FilesLocation")]
    public class FilesLocation {
        [Key]
        public int FileLocationId { get; set; }
        public int FileId { get; set; }
        public String FileLocation { get; set; }
        public String AttachedFileLocation { get; set; }

        [ForeignKey("FileId")]
        public virtual File File { get; set; }
    }

    [Table("UserCourses")]
    public class UserCourse {
        [Key]
        public int UserCourseId { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public int ColorHue { get; set; }
        public int DonePercent { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
    }

    [Table("UserCourseFiles")]
    public class UserCourseFile {
        [Key]
        public int UserCourseFileId { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public int FileId { get; set; }
        public int? CourseTopicId { get; set; }
        public int CourseTopicFileId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
        [ForeignKey("FileId")]
        public virtual File File { get; set; }
        [ForeignKey("CourseTopicId")]
        public virtual CourseTopic CourseTopic { get; set; }
        [ForeignKey("CourseTopicFileId")]
        public virtual CourseTopicFile CourseTopicFile { get; set; }
    }

    [Table("Statistics")]
    public class Statistic {
        [Key]
        public int StatisticId { get; set; }
        public int CourseTopicFileId { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public int? CourseTopicId { get; set; }
        public int FileId { get; set; }
        public DateTime? PerformedAt { get; set; }

        [ForeignKey("CourseTopicFileId")]
        public virtual CourseTopicFile CourseTopicFile { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
        [ForeignKey("CourseTopicId")]
        public virtual CourseTopic CourseTopic { get; set; }
        [ForeignKey("FileId")]
        public virtual File File { get; set; }

    }

    [Table("CourseGroups")]
    public class CourseGroup {
        [Key]
        public int CourseGroupId { get; set; }
        public int CourseId { get; set; }
        public int GroupId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }
    }

    [Table("CourseTopics")]
    public class CourseTopic {
        [Key]
        public int CourseTopicId { get; set; }
        public int CourseId { get; set; }
        public string SectionName { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public bool IsHidden { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
    }

    [Table("CourseTopicFiles")]
    public class CourseTopicFile {
        [Key]
        public int CourseTopicFileId { get; set; }
        public int? CourseTopicId { get; set; }
        public int? FileId { get; set; }
        public int CourseId { get; set; }
        public bool IsVariant { get; set; }
        public String Name { get; set; }

        [ForeignKey("CourseTopicId")]
        public virtual CourseTopic CourseTopic { get; set; }
        [ForeignKey("FileId")]
        public virtual File File { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
    }

    [Table("CourseTopicFileTasks")]
    public class CourseTopicFileTask {
        [Key]
        public int CourseTopicFileTaskId { get; set; }
        public int CourseTopicFileId { get; set; }
        public int CourseId { get; set; }
        public int? CourseTopicId { get; set; }
        public int FileId { get; set; }

        [ForeignKey("CourseTopicFileId")]
        public virtual CourseTopicFile CourseTopicFile { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
        [ForeignKey("CourseTopicId")]
        public virtual CourseTopic CourseTopic { get; set; }
        [ForeignKey("FileId")]
        public virtual File File { get; set; }
    }

    [Table("FileTypes")]
    public class FileType {
        [Key]
        public int FileTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    [Table("UserTopicFilesDone")]
    public class UserTopicFileDone {
        [Key]
        public int UserTopicFileDoneId { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public int FileId { get; set; }
        public int CourseTopicFileId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
        [ForeignKey("CourseTopicFileId")]
        public virtual CourseTopicFile CourseTopicFile { get; set; }
    }
}