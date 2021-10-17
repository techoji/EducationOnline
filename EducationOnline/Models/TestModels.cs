using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DistanceLearning.Models {

    [Table("Tests")]
    public class Test {
        [Key]
        public int TestId { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public int TimeMinutes { get; set; }
        public int Attempts { get; set; }
        public DateTime ClosingIn{ get; set; }
    }

    [Table("TestQuestions")]
    public class TestQuestion {
        [Key]
        public int TestQuestionId { get; set; }
        public int TestQuestionNumber { get; set; }
        public int TestId { get; set; }
        public string Question { get; set; }
        public bool IsMultiplyAnswer { get; set; }

        [ForeignKey("TestId")]
        public virtual Test Test { get; set; }
    }

    [Table("TestAnswers")]
    public class TestAnswer {
        [Key]
        public int TestAnswerId { get; set; }
        public int TestAnswerNumber { get; set; }
        public int TestId { get; set; }
        public int TestQuestionId { get; set; }
        public string Answer { get; set; }

        [ForeignKey("TestId")]
        public virtual Test Test { get; set; }
        [ForeignKey("TestQuestionId")]
        public virtual TestQuestion TestQuestion { get; set; }
    }

    [Table("TestCorrectAnswers")]
    public class TestCorrectAnswer {
        [Key]
        public int TestCorrectAnswerId { get; set; }
        public int TestId { get; set; }
        public int TestQuestionId { get; set; }
        public int TestAnswerId { get; set; }

        [ForeignKey("TestId")]
        public virtual Test Test { get; set; }
        [ForeignKey("TestQuestionId")]
        public virtual TestQuestion TestQuestion { get; set; }
        [ForeignKey("TestAnswerId")]
        public virtual TestAnswer TestAnswer { get; set; }
    }

    [Table("TestCourseTopics")]
    public class TestCourseTopic {
        [Key]
        public int TestCourseTopicId { get; set; }
        public int TestId { get; set; }
        public int? CourseTopicId { get; set; }

        [ForeignKey("TestId")]
        public virtual Test Test { get; set; }
        [ForeignKey("CourseTopicId")]
        public virtual CourseTopic CourseTopic { get; set; }
    }

    [Table("UserTests")]
    public class UserTest {
        [Key]
        public int UserTestId { get; set; }
        public int UserId { get; set; }
        public int TestId { get; internal set; }
        public int TestCourseTopicId { get; set; }
        public int CourseId { get; set; }
        public bool IsChecked { get; set; }
        public bool IsDone { get; set; }
        public bool IsStarted { get; set; }
        public DateTime ClosingIn { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("TestId")]
        public virtual Test Test { get; set; }
        [ForeignKey("TestCourseTopicId")]
        public virtual TestCourseTopic TestCourseTopic { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
    }

    [Table("UserTestAnswers")]
    public class UserTestAnswer {
        [Key]
        public int UserTestAnswerId { get; set; }
        public int UserId { get; set; }
        public int TestId { get; set; }
        public int TestQuestionId { get; set; }
        public int TestAnswerId { get; set; }
      
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("TestId")]
        public virtual Test Test { get; set; }
        [ForeignKey("TestQuestionId")]
        public virtual TestQuestion TestQuestion { get; set; }
        [ForeignKey("TestAnswerId")]
        public virtual TestAnswer TestAnswer { get; set; }
    }
}