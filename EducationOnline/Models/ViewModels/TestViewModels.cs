using System;
using System.Threading.Tasks;

namespace DistanceLearning.Models {
    public class TestVM {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public int TimeMinutes { get; set; }
        public int Attempts { get; set; }
        public DateTime ClosingIn { get; set; }

        public TestVM(int Id) => this.Id = Id;

        public TestVM(Test test) {
            Id = test.TestId;
            CourseId = test.CourseId;
            Name = test.Name;
            TimeMinutes = test.TimeMinutes;
            Attempts = test.Attempts;
            ClosingIn = test.ClosingIn;
        }

        public TestVM GetTest() {
            if (Id != 0)
                using (DbManager db = new DbManager())
                    return new TestVM(db.GetTestById(Id));
            else return null;
        }

        public async Task<TestVM> GetTestAsync() {
            if (Id != 0)
                using (DbManager db = new DbManager())
                    return new TestVM(await db.GetTestByIdAsync(Id));
            else return null;
        }

        public CourseVM GetCourse() => new CourseVM(CourseId).GetCourse();
        public async Task<CourseVM> GetCourseAsync() => await new CourseVM(CourseId).GetCourseAsync();
    }

    public class TestQuestionVM {
        public int Id { get; set; }
        public int TestQuestionNumber { get; set; }
        public int TestId { get; set; }
        public string Question { get; set; }
        public bool IsMultiplyAnswer { get; set; }

        public TestQuestionVM(TestQuestion testQuestion) {
            Id = testQuestion.TestQuestionId;
            TestQuestionNumber = testQuestion.TestQuestionNumber;
            TestId = testQuestion.TestId;
            Question = testQuestion.Question;
            IsMultiplyAnswer = testQuestion.IsMultiplyAnswer;
        }

        public TestQuestionVM(int Id) => this.Id = Id;

        public TestQuestionVM GetTestQuestion() {
            if (Id != 0)
                using (DbManager db = new DbManager())
                    return new TestQuestionVM(db.GetTestQuestionById(Id));
            else return null;
        }

        public async Task<TestQuestionVM> GetTestQuestionAsync() {
            if (Id != 0)
                using (DbManager db = new DbManager())
                    return new TestQuestionVM(await db.GetTestQuestionByIdAsync(Id));
            else return null;
        }

        public TestVM GetTest() => new TestVM(TestId).GetTest();
        public async Task<TestVM> GetTestAsync() => await new TestVM(TestId).GetTestAsync();
    }

    public class TestAnswerVM {
        public int Id { get; set; }
        public int TestAnswerNumber { get; set; }
        public int TestId { get; set; }
        public int TestQuestionId { get; set; }
        public string Answer { get; set; }

        public TestAnswerVM(TestAnswer testAnswer) {
            Id = testAnswer.TestAnswerId;
            TestAnswerNumber = testAnswer.TestAnswerNumber;
            TestId = testAnswer.TestId;
            TestQuestionId = testAnswer.TestQuestionId;
            Answer = testAnswer.Answer;
        }

        public TestAnswerVM(int Id) => this.Id = Id;

        public TestAnswerVM GetTestAnswer() {
            if (Id != 0)
                using (DbManager db = new DbManager())
                    return new TestAnswerVM(db.GetTestAnswerById(Id));
            else return null;
        }

        public async Task<TestAnswerVM> GetTestAnswerAsync() {
            if (Id != 0)
                using (DbManager db = new DbManager())
                    return new TestAnswerVM(await db.GetTestAnswerByIdAsync(Id));
            else return null;
        }

        public TestVM GetTest() => new TestVM(TestId).GetTest();
        public async Task<TestVM> GetTestAsync() => await new TestVM(TestId).GetTestAsync();
        public TestQuestionVM GetQuestion() => new TestQuestionVM(TestQuestionId).GetTestQuestion();
        public async Task<TestQuestionVM> GetQuestionAsync() => await new TestQuestionVM(TestQuestionId).GetTestQuestionAsync();
    }

    public class TestCorrectAnswerVM {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int TestQuestionId { get; set; }
        public int TestAnswerId { get; set; }

        public TestCorrectAnswerVM(TestCorrectAnswer testCorrectAnswer) {
            Id = testCorrectAnswer.TestCorrectAnswerId;
            TestId = testCorrectAnswer.TestId;
            TestQuestionId = testCorrectAnswer.TestQuestionId;
            TestAnswerId = testCorrectAnswer.TestAnswerId;
        }

        public TestCorrectAnswerVM(int Id) => this.Id = Id;

        public TestVM GetTest() => new TestVM(TestId).GetTest();
        public async Task<TestVM> GetTestAsync() => await new TestVM(TestId).GetTestAsync();
        public TestQuestionVM GetTestQuestion() => new TestQuestionVM(TestQuestionId).GetTestQuestion();
        public async Task<TestQuestionVM> GetTestQuestionAsync() => await new TestQuestionVM(TestQuestionId).GetTestQuestionAsync();
        public TestAnswerVM GetTestAnswer() => new TestAnswerVM(TestAnswerId).GetTestAnswer();
        public async Task<TestAnswerVM> GetTestAnswerAsync() => await new TestAnswerVM(TestAnswerId).GetTestAnswerAsync();

        public TestCorrectAnswerVM GetTestCorrectAnswer() {
            if (Id != 0)
                using (DbManager db = new DbManager())
                    return new TestCorrectAnswerVM(db.GetTestCorrectAnswerById(Id));
            else return null;
        }

        public async Task<TestCorrectAnswerVM> GetTestCorrectAnswerAsync() {
            if (Id != 0)
                using (DbManager db = new DbManager())
                    return new TestCorrectAnswerVM(await db.GetTestCorrectAnswerByIdAsync(Id));
            else return null;
        }
    }

    public class TestCourseTopicVM {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int? CourseTopicId { get; set; }

        public TestCourseTopicVM(TestCourseTopic testCourseTopic) {
            Id = testCourseTopic.TestCourseTopicId;
            TestId = testCourseTopic.TestId;
            CourseTopicId = testCourseTopic.CourseTopicId;
        }

        public TestCourseTopicVM(int Id) => this.Id = Id;

        public TestCourseTopicVM GetTestCorrectAnswer() {
            if (Id != 0)
                using (DbManager db = new DbManager())
                    return new TestCourseTopicVM(db.GetTestCourseTopicById(Id));
            else return null;
        }

        public async Task<TestCourseTopicVM> GetTestCorrectAnswerAsync() {
            if (Id != 0)
                using (DbManager db = new DbManager())
                    return new TestCourseTopicVM(await db.GetTestCourseTopicByIdAsync(Id));
            else return null;
        }

        public TestVM GetTest() => new TestVM(TestId).GetTest();
        public async Task<TestVM> GetTestAsync() => await new TestVM(TestId).GetTestAsync();
        public CourseTopicVM GetTopicFiles() => new CourseTopicVM((int)CourseTopicId).GetCourseTopic();
        public async Task<CourseTopicVM> GetTopiGetTopicFileAsynccFiles() => await new CourseTopicVM((int)CourseTopicId).GetCourseTopicAsync();

        public TestCourseTopicVM GetTestCourseTopic() {
            if (Id != 0)
                using (DbManager db = new DbManager())
                    return new TestCourseTopicVM(db.GetTestCourseTopicById(Id));
            else return null;
        }

        public async Task<TestCourseTopicVM> GetTestCourseTopicAsync() {
            if (Id != 0)
                using (DbManager db = new DbManager())
                    return new TestCourseTopicVM(await db.GetTestCourseTopicByIdAsync(Id));
            else return null;
        }
    }

    public class UserTestVM {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TestId { get; set; }
        public int TestCourseTopicId { get; set; }
        public int CourseId { get; set; }
        public bool IsChecked { get; private set; }
        public bool IsDone { get; private set; }
        public bool IsStarted { get; private set; }
        public DateTime ClosingIn { get; set; }

        public UserTestVM(int Id) => this.Id = Id;

        public UserTestVM(UserTest userTest) {
            Id = userTest.UserTestId;
            UserId = userTest.UserId;
            TestId = userTest.TestId;
            TestCourseTopicId = userTest.TestCourseTopicId;
            CourseId = userTest.CourseId;
            IsChecked = userTest.IsChecked;
            IsDone = userTest.IsDone;
            IsStarted = userTest.IsStarted;
            ClosingIn = userTest.ClosingIn;
        }

        public UserVM GetUser() => new UserVM(UserId).GetUser();
        public async Task<UserVM> GetUserAsync() => await new UserVM(UserId).GetUserAsync();
        public CourseVM GetCourse() => new CourseVM(CourseId).GetCourse();
        public async Task<CourseVM> GetCourseAsync() => await new CourseVM(CourseId).GetCourseAsync();
        public TestCourseTopicVM GetTestCourseTopic() => new TestCourseTopicVM(TestCourseTopicId).GetTestCourseTopic();
        public async Task<TestCourseTopicVM> GetTestCourseTopicAsync() => await new TestCourseTopicVM(TestCourseTopicId).GetTestCourseTopicAsync();

        public UserTopicFileDoneVM GetTopicFile() {
            if (Id != 0)
                using (DbManager db = new DbManager())
                    return new UserTopicFileDoneVM(db.GetUserTopicFileDoneById(Id));
            else return null;
        }

        public async Task<UserTopicFileDoneVM> GetTopicFileAsync() {
            if (Id != 0)
                using (DbManager db = new DbManager())
                    return new UserTopicFileDoneVM(await db.GetUserTopicFileDoneByIdAsync(Id));
            else return null;
        }
    }

    public class UserTestAnswerVM {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TestId { get; set; }
        public int TestQuestionId { get; set; }
        public int TestAnswerId { get; set; }

        public UserTestAnswerVM(UserTestAnswer userTestAnswer) {
            Id = userTestAnswer.UserTestAnswerId;
            UserId = userTestAnswer.UserId;
            TestId = userTestAnswer.TestId;
            TestQuestionId = userTestAnswer.TestQuestionId;
            TestAnswerId = userTestAnswer.TestAnswerId;
        }

        public UserVM GetUser() => new UserVM(UserId).GetUser();
        public async Task<UserVM> GetUserAsync() => await new UserVM(UserId).GetUserAsync();
        public TestVM GetTest() => new TestVM(TestId).GetTest();
        public async Task<TestVM> GetTestAsync() => await new TestVM(TestId).GetTestAsync();
        public TestQuestionVM GetTestQuestion() => new TestQuestionVM(TestQuestionId).GetTestQuestion();
        public async Task<TestQuestionVM> GetTestQuestionAsync() => await new TestQuestionVM(TestQuestionId).GetTestQuestionAsync();
        public TestAnswerVM GetTestAnswer() => new TestAnswerVM(TestAnswerId).GetTestAnswer();
        public async Task<TestAnswerVM> GetTestAnswerAsync() => await new TestAnswerVM(TestAnswerId).GetTestAnswerAsync();
    }

    public class StartTest {
        public int UserId { get; set; }
        public int TestId { get; set; }
        public int CourseId { get; set; }
        public int TestCourseTopicId { get; set; }
        public int? TestQuestionNumber { get; set; }
    }
}