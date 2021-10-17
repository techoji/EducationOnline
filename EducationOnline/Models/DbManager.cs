using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DistanceLearning.Models.TeacherUpdates;

namespace DistanceLearning.Models
{

	public class Db : DbContext
	{

		//USERS
		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }
		public DbSet<Group> Groups { get; set; }
		public DbSet<UserGroup> UserGroups { get; set; }
		public DbSet<Teacher> Teachers { get; set; }

		//COURSES
		public DbSet<Course> Courses { get; set; }
		public DbSet<CourseTopicFile> CourseTopicFiles { get; set; }
		public DbSet<CourseTopicFileTask> CourseTopicFileTasks { get; set; }
		public DbSet<CourseGroup> CourseGroups { get; set; }
		public DbSet<UserCourse> UserCourse { get; set; }
		public DbSet<UserCourseFile> UserCourseFiles { get; set; }
		public DbSet<CourseTopic> CourseTopics { get; set; }
		public DbSet<File> Files { get; set; }
		public DbSet<FilesLocation> FilesLocation { get; set; }
		public DbSet<FileType> FileTypes { get; set; }
		public DbSet<UserTopicFileDone> UserTopicFileDones { get; set; }
		public DbSet<Statistic> Statistics { get; set; }

		//TESTS
		public DbSet<Test> Tests { get; set; }
		public DbSet<TestQuestion> TestQuestions { get; set; }
		public DbSet<TestAnswer> TestAnswers { get; set; }
		public DbSet<TestCorrectAnswer> TestCorrectAnswers { get; set; }
		public DbSet<TestCourseTopic> TestCourseTopics { get; set; }
		public DbSet<UserTest> UserTests { get; set; }
		public DbSet<UserTestAnswer> UserTestAnswers { get; set; }
	}

	public class DbManager : IDisposable
	{

		private Db db { get; set; }

		public DbManager() => db = new Db();

		public async Task<bool> HaveUserAccessToCourseAsync(string UserEmail, int CourseId)
		{
			User user = await GetUserByEmailAsync(UserEmail);
			List<UserGroup> userGroups = await GetUserGroupsByUserEmailAsync(user.UserId);
			Course course = await GetCourseByIdAsync(CourseId);
			List<Group> courseGroups = GetGroupsByCourseId(course.CourseId);

			bool groupExist = false;
			foreach (Group courseGroup in courseGroups)
			{
				foreach (UserGroup userGroup in userGroups)
				{
					if (courseGroup.GroupId == userGroup.GroupId)
					{
						groupExist = true;
						break;
					}
				}
				if (groupExist)
					break;
			}

			return groupExist;
		}

		public async Task<bool> HaveUserAccessToCourseAsync(string UserEmail, String CourseShortName) => await HaveUserAccessToCourseAsync(UserEmail, db.Courses.FirstOrDefault(c => c.ShortName == CourseShortName).CourseId);

		public async Task<bool> HaveUserAccessToCourseTopicFile(string UserEmail, int CourseTopicFileId)
		{
			User user = GetUserByEmail(UserEmail);
			return await db.Statistics.AnyAsync(s => s.UserId == user.UserId && s.CourseTopicFileId == CourseTopicFileId);
		}

		//GET
		//user
		public User GetUserById(int Id) => db.Users.FirstOrDefault(u => u.UserId == Id);
		public User GetUserByEmail(string UserEmail) => db.Users.FirstOrDefault(u => u.Email == UserEmail);
		public async Task<User> GetUserByIdAsync(int Id) => await db.Users.FirstOrDefaultAsync(u => u.UserId == Id);
		public async Task<User> GetUserByEmailAsync(string UserEmail) => await db.Users.FirstOrDefaultAsync(u => u.Email == UserEmail);
		public async Task<User> GetUserAsync(string UserEmail, string Password) => await db.Users.FirstOrDefaultAsync(u => u.Email == UserEmail && u.Password == Password);
		public async Task<List<User>> GetUsersAsync() => await db.Users.ToListAsync();
		public async Task<List<User>> GetUsersByGroupId(int GroupId)
		{
			List<int> usersId = await db.UserGroups.Where(u => u.GroupId == GroupId).Select(x => x.UserId).ToListAsync();
			return await db.Users.Where(u => usersId.Contains(u.UserId)).ToListAsync();
		}
		public async Task<List<User>> GetUsersByCourseId(int CourseId)
		{
			IQueryable<int> usersId = db.UserCourse.Where(u => u.CourseId == CourseId).Select(x => x.UserId);
			return await db.Users.Where(u => usersId.Contains(u.UserId)).ToListAsync();
		}
		//role
		public Role GetRoleById(int Id) => db.Roles.FirstOrDefault(u => u.RoleId == Id);
		public async Task<Role> GetRoleByIdAsync(int Id) => await db.Roles.FirstOrDefaultAsync(u => u.RoleId == Id);
		public async Task<List<Role>> GetRoleByUserEmailAsync(String UserEmail)
		{
			User user = GetUserByEmail(UserEmail);
			return await db.UserRoles.Where(u => u.UserId == user.UserId).Select(x => x.Role).ToListAsync();
		}
		//userRole
		public List<UserRole> GetUserRoles(int UserId) => db.UserRoles.Where(r => r.UserId == UserId).ToList();
		//course
		public List<Course> GetUserCoursesByUserEmail(string UserEmail)
		{
			if (String.IsNullOrWhiteSpace(UserEmail))
				return new List<Course>();
			else if (IsTeacherByUserEmail(UserEmail))
				return GetTeacherCoursesByEmail(UserEmail);
			else
				return GetCoursesByUserEmail(UserEmail).ToList();
		}
		public async Task<List<Course>> GetUserCoursesByUserEmailAsync(string UserEmail)
		{
			if (String.IsNullOrWhiteSpace(UserEmail))
				return new List<Course>();
			else if (IsTeacherByUserEmail(UserEmail))
				return await GetTeacherCoursesByEmailAsync(UserEmail);
			else
				return await GetCoursesByUserEmailAsync(UserEmail);
		}
		public List<Course> GetCoursesByUserEmail(string UserEmail)
		{
			int userId = GetUserByEmail(UserEmail).UserId;
			List<UserGroup> userGroups = GetUserGroupsByUserId(userId);
			return GetCoursesByUserGroups(userGroups);
		}
		public async Task<List<Course>> GetCoursesByUserEmailAsync(string UserEmail)
		{
			if (await IsTeacherByEmailAsync(UserEmail))
				return await GetTeacherCoursesByEmailAsync(UserEmail);

			int userId = (await GetUserByEmailAsync(UserEmail)).UserId;
			List<UserGroup> userGroups = await GetUserGroupsByUserEmailAsync(userId);
			return await GetCoursesByUserGroupsAsync(userGroups);
		}
		public async Task<Course> GetCourseByIdAsync(int CourseId) => await db.Courses.FirstOrDefaultAsync(c => c.CourseId == CourseId);
		public Course GetCourseById(int CourseId) => db.Courses.FirstOrDefault(c => c.CourseId == CourseId);
		public List<Course> GetCoursesByUserGroups(List<UserGroup> UserGroups)
		{
			List<int> GroupsId = UserGroups.Select(x => x.GroupId).ToList();
			return db.CourseGroups.Where(c => GroupsId.Contains(c.GroupId)).Select(x => x.Course).ToList();
		}
		public async Task<List<Course>> GetCoursesByUserGroupsAsync(List<UserGroup> UserGroups)
		{
			List<int> GroupsId = UserGroups.Select(x => x.GroupId).ToList();
			return await db.CourseGroups.Where(c => GroupsId.Contains(c.GroupId)).Select(x => x.Course).ToListAsync();
		}
		public async Task<Course> GetCourseByShortNameAsync(string ShortName) => await db.Courses.FirstOrDefaultAsync(c => c.ShortName == ShortName);
		public async Task<List<Course>> GetCoursesAsync() => await db.Courses.ToListAsync();
		public async Task<List<Course>> GetCoursesByGroupIdAsync(int GroupId)
		{
			List<int> coursesId = await db.CourseGroups.Where(c => c.GroupId == GroupId).Select(x => x.CourseId).ToListAsync();
			return await db.Courses.Where(u => coursesId.Contains(u.CourseId)).ToListAsync();
		}
		public async Task<Course> GetCourseByCourseTopicFileIdAsync(int CourseTopicFileId)
		{
			CourseTopicFile courseTopicFile = await db.CourseTopicFiles.FirstOrDefaultAsync(c => c.CourseTopicFileId == CourseTopicFileId);
			return await GetCourseByIdAsync(courseTopicFile.CourseId);
		}
		//courseTopicFile
		public CourseTopicFile GetCourseByCourseTopicFileId(int CourseTopicFileId) => db.CourseTopicFiles.FirstOrDefault(c => c.CourseTopicFileId == CourseTopicFileId);
		public CourseTopicFile GetCourseTopicFileById(int CourseTopicFileId) => db.CourseTopicFiles.FirstOrDefault(c => c.CourseTopicFileId == CourseTopicFileId);
		public async Task<CourseTopicFile> GetCourseTopicFileByIdAsync(int CourseTopicFileId) => await db.CourseTopicFiles.FirstOrDefaultAsync(c => c.CourseTopicFileId == CourseTopicFileId);
		public List<CourseTopicFile> GetCourseTopicFilesByCourseId(int CourseId) => db.CourseTopicFiles.Where(c => c.CourseId == CourseId).ToList();
		public async Task<List<CourseTopicFile>> GetCourseTopicFilesByCourseIdAsync(int CourseId) => await db.CourseTopicFiles.Where(c => c.CourseId == CourseId).ToListAsync();
		//courseTopicFileTask
		public async Task<List<CourseTopicFileTask>> GetCourseTopicFileTasksByCourseTopicFileId(int CourseTopicFileId) => await db.CourseTopicFileTasks.Where(c => c.CourseTopicFileId == CourseTopicFileId).ToListAsync();
		//courseTopic
		public CourseTopic GetCourseTopicById(int CourseTopicId) => db.CourseTopics.FirstOrDefault(u => u.CourseTopicId == CourseTopicId);
		public async Task<CourseTopic> GetCourseTopicByIdAsync(int CourseTopicId) => await db.CourseTopics.FirstOrDefaultAsync(u => u.CourseTopicId == CourseTopicId);
		public async Task<List<CourseTopic>> GetCourseTopicByCourseIdAsync(int CourseId) => await db.CourseTopics.Where(u => u.CourseId == CourseId).ToListAsync();
		public async Task<CourseTopic> GetCourseTopicByCourseTopicFileIdAsync(int CourseTopicFileId)
		{
			CourseTopicFile courseTopicFile = await db.CourseTopicFiles.FirstOrDefaultAsync(c => c.CourseTopicFileId == CourseTopicFileId);
			if (courseTopicFile.CourseTopicId == null)
				return null;
			return await GetCourseTopicByIdAsync((int)courseTopicFile.CourseTopicId);
		}
		//file
		public File GetFileById(int FileId) => db.Files.FirstOrDefault(f => f.FileId == FileId);
		public async Task<File> GetFileByIdAsync(int FileId) => await db.Files.FirstOrDefaultAsync(f => f.FileId == FileId);
		public List<File> GetFilesByUserEmail(String UserEmail, bool IsHiddenFiles = false)
		{
			IEnumerable<int> coursesId = GetUserCoursesByUserEmail(UserEmail).Select(x => x.CourseId).Distinct();

			if (!IsHiddenFiles)
			{
				IEnumerable<CourseTopicFile> courseTopicFiles;
				UserCourseFile userCourseFile;
				List<File> files = new List<File>();
				foreach (int courseId in coursesId)
				{
					courseTopicFiles = GetCourseTopicFilesByCourseId(courseId);
					foreach (CourseTopicFile courseTopicFile in courseTopicFiles)
					{
						if (courseTopicFile.CourseTopicId == null || !GetCourseTopicById((int)courseTopicFile.CourseTopicId).IsHidden)
						{
							if (courseTopicFile.FileId == null)
							{
								userCourseFile = GetUserCourseFileByParams(UserEmail, courseTopicFile.CourseTopicFileId);
								files.Add(GetFileById(userCourseFile.FileId));
							}
							else
								files.Add(GetFileById((int)courseTopicFile.FileId));
						}
					}
				}
				return files;
			}

			List<int> filesId = db.Files.Where(t => coursesId.Contains(t.CourseId)).Select(x => x.FileId).ToList();
			return db.Files.Where(t => filesId.Contains(t.FileId)).ToList();
		}
		public bool IsFileChecked(int FileId, int UserId)
		{
			if (IsFileInCourseTopic(FileId))
				return db.UserTopicFileDones.Any(f => f.FileId == FileId && f.UserId == UserId);
			else
				return true;
		}
		public List<File> GetFilesByCourseId(int CourseId) => db.Files.Where(f => f.CourseId == CourseId).ToList();
		public List<File> GetLabFilesByCourseId(int CourseId)
		{
			IEnumerable<int> labFileTypesId = GetFileTypesLab().Select(x => x.FileTypeId);
			return db.Files.Where(f => f.CourseId == CourseId && labFileTypesId.Contains(f.FileTypeId)).ToList();
		}
		public async Task<List<File>> GetFilesAsync() => await db.Files.ToListAsync();
		public async Task<File> GetFileByCourseTopicFileIdAsync(String UserEmail, int CourseTopicFileId)
		{
			User user = GetUserByEmail(UserEmail);
			CourseTopicFile courseTopicFile = await GetCourseTopicFileByIdAsync(CourseTopicFileId);
			if (courseTopicFile == null)
				return null;
			if (courseTopicFile.IsVariant)
			{
				UserCourseFile userCourseFile = await GetUserCourseFileByParamsAsync(user.UserId, courseTopicFile.CourseTopicFileId);
				if (userCourseFile != null)
					return await GetFileByIdAsync(userCourseFile.FileId);
				else
					return null;
			}
			else
				return await GetFileByIdAsync((int)courseTopicFile.FileId);
		}
		//fileLocation
		public FilesLocation GetFileLocationByFileId(int FileId) => db.FilesLocation.FirstOrDefault(f => f.FileId == FileId);
		public async Task<List<FilesLocation>> GetFileLocationsAsync() => await db.FilesLocation.ToListAsync();
		public List<FilesLocation> GetFileLocationsByCourseId(int CourseId)
		{
			IEnumerable<int> filesId = GetFilesByCourseId(CourseId).Select(x => x.FileId);
			return db.FilesLocation.Where(f => filesId.Contains(f.FileId)).ToList();
		}
		//fileType
		public FileType GetFileTypeById(int FileTypeId) => db.FileTypes.FirstOrDefault(u => u.FileTypeId == FileTypeId);
		public async Task<FileType> GetFileTypeByIdAsync(int FileTypeId) => await db.FileTypes.FirstOrDefaultAsync(u => u.FileTypeId == FileTypeId);
		public List<FileType> GetFileTypes() => db.FileTypes.ToList();
		public List<FileType> GetFileTypesLab() => db.FileTypes.Where(f => f.Name.ToLower().Contains("lab")).ToList();
		//userGroup
		public List<UserGroup> GetUserGroupsByUserId(int UserId) => db.UserGroups.Where(u => u.UserId == UserId).ToList();
		public async Task<List<UserGroup>> GetUserGroupsByUserEmailAsync(int UserId) => await db.UserGroups.Where(u => u.UserId == UserId).ToListAsync();
		public async Task<List<UserGroup>> GetUserGroupsAsync() => await db.UserGroups.ToListAsync();
		//group
		public Group GetGroupById(int GroupId) => db.Groups.FirstOrDefault(c => c.GroupId == GroupId);
		public async Task<Group> GetGroupByIdAsync(int GroupId) => await db.Groups.FirstOrDefaultAsync(c => c.GroupId == GroupId);
		public List<Group> GetGroupsByUserGroups(List<UserGroup> UserGroups)
		{
			List<int> userGroupsId = UserGroups.Select(x => x.GroupId).ToList();
			return db.Groups.Where(g => userGroupsId.Contains(g.GroupId)).ToList();
		}
		public List<Group> GetGroupsByCourseId(int CourseId) => db.CourseGroups.Where(g => g.CourseId == CourseId).Select(x => x.Group).ToList();
		public async Task<List<Group>> GetGroupsByUserIdAsync(int UserId)
		{
			List<int> groupsId = (await GetUserGroupsByUserEmailAsync(UserId)).Select(x => x.GroupId).ToList();
			return db.Groups.Where(g => groupsId.Contains(g.GroupId)).ToList();
		}
		public async Task<List<Group>> GetGroupsByUserEmailAsync(String UserEmail)
		{
			User user = GetUserByEmail(UserEmail);
			List<int> groupsId = (await GetUserGroupsByUserEmailAsync(user.UserId)).Select(x => x.GroupId).ToList();
			return db.Groups.Where(g => groupsId.Contains(g.GroupId)).ToList();
		}
		public async Task<List<Group>> GetGroupsAsync() => await db.Groups.ToListAsync();
		//courseGroup
		public List<CourseGroup> GetCourseGroupsByGroupId(int GroupId) => db.CourseGroups.Where(g => g.GroupId == GroupId).ToList();
		public async Task<List<CourseGroup>> GetCourseGroupsAsync() => await db.CourseGroups.ToListAsync();
		public async Task<List<CourseGroup>> GetCourseGroupsByCourseIdAsync(int CourseId) => await db.CourseGroups.Where(u => u.CourseId == CourseId).ToListAsync();
		//userCourse
		public UserCourse GetUserCourse(String UserEmail, int CourseId)
		{
			User user = GetUserByEmail(UserEmail);
			if (user == null)
				return null;
			CreateUserCourse(UserEmail, CourseId);
			return db.UserCourse.FirstOrDefault(c => c.UserId == user.UserId && c.CourseId == CourseId);
		}
		public async Task<UserCourse> GetUserCourseAsync(String UserEmail, int CourseId)
		{
			User user = GetUserByEmail(UserEmail);
			await CreateUserCourseAsync(UserEmail, CourseId);
			return await db.UserCourse.FirstOrDefaultAsync(c => c.UserId == user.UserId && c.CourseId == CourseId);
		}
		public async Task<UserCourse> GetUserCourseAsync(int UserId, int CourseId)
		{
			await CreateUserCourseAsync(UserId, CourseId);
			return await db.UserCourse.FirstOrDefaultAsync(c => c.UserId == UserId && c.CourseId == CourseId);
		}
		//userCourseFile
		public async Task<UserCourseFile> GetUserCourseFileById(int UserCourseFileId) => await db.UserCourseFiles.FirstOrDefaultAsync(u => u.UserCourseFileId == UserCourseFileId);
		public async Task<List<UserCourseFile>> GetUserCourseFilesAsync(String UserEmail, int CourseId)
		{
			User user = GetUserByEmail(UserEmail);
			if (user == null)
				return null;
			return await db.UserCourseFiles.Where(u => u.UserId == user.UserId && u.CourseId == CourseId).ToListAsync();
		}
		public async Task<List<UserCourseFile>> GetUserCourseFilesByParams(int CourseId, int CourseTopicId) => await db.UserCourseFiles.Where(u => u.CourseId == CourseId && u.CourseTopicId == CourseTopicId).ToListAsync();
		public UserCourseFile GetUserCourseFileByParams(String UserEmail, int CourseTopicFileId)
		{
			User user = GetUserByEmail(UserEmail);
			return db.UserCourseFiles.FirstOrDefault(u => u.CourseTopicFileId == CourseTopicFileId && u.UserId == user.UserId);
		}
		public async Task<UserCourseFile> GetUserCourseFileByParamsAsync(int UserId, int CourseTopicFileId) => await db.UserCourseFiles.FirstOrDefaultAsync(u => u.CourseTopicFileId == CourseTopicFileId && u.UserId == UserId);
		//statistics
		public async Task<Statistic> GetStatisticsById(int StatisticId) => await db.Statistics.FirstOrDefaultAsync(s => s.StatisticId == StatisticId);
		public async Task<List<Statistic>> GetStatisticsByCourseTopicFileId(int CourseTopicFileId) => await db.Statistics.Where(s => s.CourseTopicFileId == CourseTopicFileId).ToListAsync();
		//teacher
		public async Task<bool> IsTeacherByEmailAsync(string Email)
		{
			User user = await GetUserByEmailAsync(Email);
			Role teacherRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "Teacher");
			if (user == null)
				return false;
			return await db.UserRoles.AnyAsync(r => r.UserId == user.UserId && r.RoleId == teacherRole.RoleId);
		}
		public bool IsTeacherByUserEmail(string Email)
		{
			User user = GetUserByEmail(Email);
			Role teacherRole = db.Roles.FirstOrDefault(r => r.Name == "Teacher");
			if (user == null)
				return false;
			return db.UserRoles.Any(r => r.UserId == user.UserId && r.RoleId == teacherRole.RoleId);
		}
		public async Task<bool> HaveTeacherAccessToCourseByEmailAsync(string Email, int CourseId)
		{
			User user = await GetUserByEmailAsync(Email);
			return await db.Teachers.AnyAsync(t => t.UserId == user.UserId && t.CourseId == CourseId);
		}
		public List<Course> GetTeacherCoursesByEmail(string Email)
		{
			User user = GetUserByEmail(Email);
			List<int> coursesId = db.Teachers.Where(t => t.UserId == user.UserId).Select(x => x.CourseId).ToList();
			if (coursesId.Count > 0)
				return db.Courses.Where(c => coursesId.Contains(c.CourseId)).ToList();
			else
				return null;
		}
		public async Task<List<Course>> GetTeacherCoursesByEmailAsync(string Email)
		{
			User user = await GetUserByEmailAsync(Email);
			List<int> coursesId = db.Teachers.Where(t => t.UserId == user.UserId).Select(x => x.CourseId).ToList();
			if (coursesId.Count > 0)
				return db.Courses.Where(c => coursesId.Contains(c.CourseId)).ToList();
			else
				return null;
		}
		//test
		public Test GetTestById(int TestId) => db.Tests.FirstOrDefault(t => t.TestId == TestId);
		public async Task<Test> GetTestByIdAsync(int TestId) => await db.Tests.FirstOrDefaultAsync(t => t.TestId == TestId);
		public List<Test> GetTestsByUserId(String UserEmail)
		{
			List<int> coursesId = GetUserCoursesByUserEmail(UserEmail).Select(x => x.CourseId).ToList();
			List<int> testsId = db.Tests.Where(t => coursesId.Contains(t.CourseId)).Select(x => x.TestId).ToList();
			List<Test> tests = new List<Test>();

			foreach (int item in testsId)
				tests.Add(GetTestById(item));
			return tests;
		}
		public bool IsTestChecked(int TestId, int UserId)
		{
			UserTest userTest = db.UserTests.FirstOrDefault(u => u.TestId == TestId && u.UserId == UserId);
			if (userTest != null)
				return userTest.IsChecked;
			return false;
		}
		//testCourseTopic
		public TestCourseTopic GetTestCourseTopicById(int TestCourseTopicId) => db.TestCourseTopics.FirstOrDefault(t => t.TestCourseTopicId == TestCourseTopicId);
		public async Task<TestCourseTopic> GetTestCourseTopicByIdAsync(int TestCourseTopicId) => await db.TestCourseTopics.FirstOrDefaultAsync(t => t.TestCourseTopicId == TestCourseTopicId);
		public List<TestCourseTopic> GetTestsCourseTopicByCourseId(int CourseId)
		{
			List<int> testsId = db.Tests.Where(c => c.CourseId == CourseId).Select(x => x.TestId).ToList();
			return db.TestCourseTopics.Where(c => testsId.Contains(c.TestId)).ToList();
		}
		public async Task<List<TestCourseTopic>> GetTestsCourseTopicByCourseIdAsync(int CourseId)
		{
			List<int> testsId = await db.Tests.Where(c => c.CourseId == CourseId).Select(x => x.TestId).ToListAsync();
			return await db.TestCourseTopics.Where(c => testsId.Contains(c.TestId)).ToListAsync();
		}
		public async Task<TestCourseTopic> GetTestsCourseTopicByTestIdAsync(int TestId) => await db.TestCourseTopics.FirstOrDefaultAsync(c => c.TestId == TestId);
		//testQuestion 
		public TestQuestion GetTestQuestionById(int TestQuestionId) => db.TestQuestions.FirstOrDefault(t => t.TestQuestionId == TestQuestionId);
		public async Task<TestQuestion> GetTestQuestionByIdAsync(int TestQuestionId) => await db.TestQuestions.FirstOrDefaultAsync(t => t.TestQuestionId == TestQuestionId);
		public async Task<TestQuestion> GetTestQuestionByTestParamsAsync(int TestId, int TestQuestionNumber) => await db.TestQuestions.FirstOrDefaultAsync(t => t.TestQuestionNumber == TestQuestionNumber && t.TestId == TestId);

		internal Task GetStatisticsByCourseTopicFileId(int? courseTopicId)
		{
			throw new NotImplementedException();
		}

		public async Task<List<TestQuestion>> GetTestQuestionsByTestIdAsync(int TestId) => await db.TestQuestions.Where(t => t.TestId == TestId).ToListAsync();
		//testAnswer
		public async Task<TestAnswer> GetTestAnswerByTestParamsAsync(int TestId, int TestAnswerNumber) => await db.TestAnswers.FirstOrDefaultAsync(t => t.TestId == TestId && t.TestAnswerNumber == TestAnswerNumber);
		public async Task<List<TestAnswer>> GetTestAnswersByTestParamsAsync(int TestId, int TestQuestionId) => await db.TestAnswers.Where(t => t.TestId == TestId && t.TestQuestionId == TestQuestionId).ToListAsync();
		//testCorrectAnswer
		public TestAnswer GetTestAnswerById(int TestAnswerId) => db.TestAnswers.FirstOrDefault(r => r.TestAnswerId == TestAnswerId);
		public async Task<TestAnswer> GetTestAnswerByIdAsync(int TestAnswerId) => await db.TestAnswers.FirstOrDefaultAsync(r => r.TestAnswerId == TestAnswerId);
		//testCorrectAnswer
		public TestCorrectAnswer GetTestCorrectAnswerById(int TestQuestionId) => db.TestCorrectAnswers.FirstOrDefault(t => t.TestCorrectAnswerId == TestQuestionId);
		public async Task<TestCorrectAnswer> GetTestCorrectAnswerByIdAsync(int TestQuestionId) => await db.TestCorrectAnswers.FirstOrDefaultAsync(t => t.TestCorrectAnswerId == TestQuestionId);
		//userCoursePercent
		public int GetUserCoursePercent(String UserEmail, int CourseId)
		{
			User user = GetUserByEmail(UserEmail);
			return db.UserCourse.FirstOrDefault(u => u.UserId == user.UserId && u.CourseId == CourseId).DonePercent;
		}
		public bool IsFileInCourseTopic(int FileId) => db.CourseTopicFiles.Any(c => c.FileId == FileId);
		//userTopicFileDone
		public UserTopicFileDone GetUserTopicFileDoneById(int UserTopicFileDoneId) => db.UserTopicFileDones.FirstOrDefault(t => t.UserTopicFileDoneId == UserTopicFileDoneId);
		public async Task<UserTopicFileDone> GetUserTopicFileDoneByIdAsync(int UserTopicFileDoneId) => await db.UserTopicFileDones.FirstOrDefaultAsync(t => t.UserTopicFileDoneId == UserTopicFileDoneId);
		public async Task<List<UserTopicFileDone>> GetUserTopicFileDoneByCourseIdAsync(String UserEmail, int CourseId)
		{
			User user = await GetUserByEmailAsync(UserEmail);
			return db.UserTopicFileDones.Where(t => t.CourseId == CourseId && t.UserId == user.UserId).ToList();
		}
		public UserTopicFileDone GetUserTopicFileDone(int CourseId, int CourseTopicFileId) => db.UserTopicFileDones.FirstOrDefault(u => u.CourseId == CourseId && u.CourseTopicFileId == CourseTopicFileId);
		public async Task<UserTopicFileDone> GetUserTopicFileDoneAsync(int CourseId, int CourseTopicFileId) => await db.UserTopicFileDones.FirstOrDefaultAsync(u => u.CourseId == CourseId && u.CourseTopicFileId == CourseTopicFileId);
		//userTest
		public async Task<UserTest> GetUserTestAsync(String UserEmail, int CourseId, int TestId, int TestCourseTopicId)
		{
			User user = await GetUserByEmailAsync(UserEmail);
			await UpdateUserTestAsync(user.UserId, CourseId, TestId, TestCourseTopicId);
			return db.UserTests.FirstOrDefault(u => u.UserId == user.UserId && u.CourseId == CourseId && u.TestCourseTopicId == TestCourseTopicId);
		}
		public async Task<List<UserTest>> GetUserTestByCourseIdAsync(String UserEmail, int CourseId)
		{
			User user = await GetUserByEmailAsync(UserEmail);
			return db.UserTests.Where(u => u.CourseId == CourseId && u.UserId == user.UserId).ToList();
		}
		//userTestAnswer
		public async Task<List<UserTestAnswer>> GetUserTestAnswersByTestParamsAsync(int TestId, int QuestionId) => await db.UserTestAnswers.Where(u => u.TestId == TestId && u.TestQuestionId == QuestionId).ToListAsync();
		public async Task<List<UserTestAnswer>> GetUserTestAnswersByTestParamsAsync(int TestId) => await db.UserTestAnswers.Where(u => u.TestId == TestId).ToListAsync();

		//ADMIN
		public async Task<bool> IsAdminByEmailAsync(string UserEmail)
		{
			User user = await GetUserByEmailAsync(UserEmail);
			Role role = db.Roles.FirstOrDefault(r => r.Name == "admin");
			if (user != null && role != null)
				return db.UserRoles.Any(u => u.UserId == user.UserId && u.RoleId == role.RoleId);
			return false;
		}
		public async Task<bool> GoogleAuthUserAsync(string FirstName, string LastName, string Email, string Photo)
		{
			User user = await db.Users.FirstOrDefaultAsync(u => u.Email == Email);
			if (user != null)
			{
				user.FirstName = FirstName;
				user.LastName = LastName;
				user.Photo = Photo;
				await db.SaveChangesAsync();
				return true;
			}
			return false;
		}


		//UPDATE
		public async Task UpdateTestAnswerAsync(int UserId, int TestId, int TestQuestionId, List<int> Answers)
		{
			UserTest userTest = await db.UserTests.FirstOrDefaultAsync(u => u.TestId == TestId && u.UserId == UserId);
			if (userTest != null && userTest.IsStarted && userTest.ClosingIn <= DateTime.Now)
				return;

			List<UserTestAnswer> userAnswers = await db.UserTestAnswers.Where(u =>
				   u.UserId == UserId &&
				   u.TestId == TestId &&
				   u.TestQuestionId == TestQuestionId).ToListAsync();

			userAnswers.ForEach(u =>
			{
				if (Answers == null || !Answers.Contains(u.TestAnswerId))
					db.UserTestAnswers.Remove(u);
			});

			if (Answers != null)
				foreach (int answerId in Answers)
				{
					UserTestAnswer userAnswer = await db.UserTestAnswers.FirstOrDefaultAsync(u =>
						u.UserId == UserId &&
						u.TestId == TestId &&
						u.TestQuestionId == TestQuestionId &&
						u.TestAnswerId == answerId);
					if (userAnswer == null)
					{
						userAnswer = new UserTestAnswer
						{
							UserId = UserId,
							TestId = TestId,
							TestQuestionId = TestQuestionId,
							TestAnswerId = answerId
						};
						db.UserTestAnswers.Add(userAnswer);
					}
					else
						userAnswer.TestAnswerId = answerId;
				}

			await db.SaveChangesAsync();
		}

		public async Task UpdateUserCourseColorAsync(String UserEmail, int CourseId, int ColorHue)
		{
			UserCourse userCourse = await GetUserCourseAsync(UserEmail, CourseId);
			if (userCourse == null)
				await CreateUserCourseAsync(UserEmail, CourseId);
			userCourse.ColorHue = ColorHue;
			await db.SaveChangesAsync();
		}

		public async Task UpdateUserTestAsync(int UserId, int CourseId, int TestId, int TestCourseTopicId, bool IsChecked = default(bool), bool IsDone = default(bool), bool IsStarted = default(bool))
		{
			UserTest userTest = db.UserTests.FirstOrDefault(u => u.UserId == UserId && u.CourseId == CourseId && u.TestCourseTopicId == TestCourseTopicId);
			Test test = db.Tests.FirstOrDefault(t => t.TestId == TestId);
			if (userTest == null)
			{
				userTest = new UserTest()
				{
					UserId = UserId,
					CourseId = CourseId,
					TestCourseTopicId = TestCourseTopicId,
					TestId = TestId
				};
				db.UserTests.Add(userTest);
			}
			if (IsChecked != default(bool))
				userTest.IsChecked = IsChecked;
			if (IsDone != default(bool))
				userTest.IsDone = IsDone;
			if (IsStarted != default(bool))
			{
				userTest.ClosingIn = DateTime.Now.Add(TimeSpan.FromMinutes(test.TimeMinutes)).Add(TimeSpan.FromSeconds(1));
				userTest.IsStarted = IsStarted;
			}

			await db.SaveChangesAsync();
		}

		public async Task UpdateFileCheckedAsync(int CourseId, int FileId, int CourseTopicFileId, int UserId, bool IsChecked)
		{
			User user = await GetUserByIdAsync(UserId);
			if (IsChecked)
			{
				UserTopicFileDone userTopicFileDone = new UserTopicFileDone()
				{
					CourseId = CourseId,
					UserId = UserId,
					CourseTopicFileId = CourseTopicFileId,
					FileId = FileId
				};
				db.UserTopicFileDones.Add(userTopicFileDone);

			}
			else
				db.UserTopicFileDones.Remove(GetUserTopicFileDone(CourseId, CourseTopicFileId));
			await db.SaveChangesAsync();

			List<UserTopicFileDone> userTopicFilesDone = db.UserTopicFileDones.Where(u => u.UserId == UserId && u.CourseId == CourseId).ToList();
			List<UserTest> userTests = db.UserTests.Where(u => u.UserId == UserId && u.CourseId == CourseId && u.IsChecked).ToList();

			int filesCount = GetCountFilesInCourseByUserEmail(CourseId);
			//int filesCount = GetFilesByUserEmail(user.Email).Count;
			//filesCount += db.Tests.Where(f => f.CourseId == CourseId).Count();
			int userCheckedCount = userTopicFilesDone.Count + userTests.Count;

			UserCourse userCourse = await GetUserCourseAsync(UserId, CourseId);
			userCourse.DonePercent = filesCount == 0 ? 100 : 100 * userCheckedCount / filesCount;

			await db.SaveChangesAsync();
		}

		public async Task UpdateTestCheckedAsync(int CourseId, int TestId, int TestCourseTopicId, int UserId, bool IsChecked = false)
		{
			UserTest test = db.UserTests.FirstOrDefault(u => u.TestCourseTopicId == TestCourseTopicId && u.UserId == UserId && u.CourseId == CourseId);
			if (test == null)
			{
				test = new UserTest
				{
					CourseId = CourseId,
					TestCourseTopicId = TestCourseTopicId,
					UserId = UserId,
					TestId = TestId
				};
				db.UserTests.Add(test);
			}
			test.IsChecked = IsChecked;
			await db.SaveChangesAsync();

			List<UserTopicFileDone> userTopicFilesDone = db.UserTopicFileDones.Where(u => u.UserId == UserId && u.CourseId == CourseId).ToList();
			List<UserTest> userTests = db.UserTests.Where(u => u.UserId == UserId && u.CourseId == CourseId && u.IsChecked).ToList();

			int filesCount = GetCountFilesInCourseByUserEmail(CourseId);
			int userCheckedCount = userTopicFilesDone.Count + userTests.Count;

			UserCourse userCourse = await GetUserCourseAsync(UserId, CourseId);
			userCourse.DonePercent = filesCount == 0 ? 100 : 100 * userCheckedCount / filesCount;

			await db.SaveChangesAsync();
		}

		private int GetCountFilesInCourseByUserEmail(int CourseId) => db.CourseTopicFiles.Where(c => c.CourseId == CourseId).Count() + db.Tests.Where(c => c.CourseId == CourseId).Count();

		public async Task UpdateStatisticPerformedAsync(String UserEmail, int CourseTopicFileId)
		{
			Statistic statistic = await db.Statistics.FirstOrDefaultAsync(u => u.User.Email == UserEmail && u.CourseTopicFileId == CourseTopicFileId);
			if (statistic != null && statistic.PerformedAt == null)
			{
				statistic.PerformedAt = DateTime.Now;
				await db.SaveChangesAsync();
			}
		}

		public async Task UpdateStatisticPerformedResetAsync(int StatisticId)
		{
			Statistic statistic = await db.Statistics.FirstOrDefaultAsync(u => u.StatisticId == StatisticId);
			if (statistic != null)
			{
				statistic.PerformedAt = null;
				await db.SaveChangesAsync();
			}
		}

		//admin
		public async Task UpdateGroupAsync(int GroupId, string GroupName)
		{
			Group group = db.Groups.FirstOrDefault(g => g.GroupId == GroupId);
			if (group != null)
			{
				group.Name = GroupName;
				await db.SaveChangesAsync();
			}
		}

		public async Task UpdateUserGroupAsync(int UserId, int WasGroupId, int GroupId)
		{
			bool IsUserGroup = db.UserGroups.Any(u => u.UserId == UserId && u.GroupId == GroupId);
			if (!IsUserGroup)
			{
				UserGroup userGroup = db.UserGroups.FirstOrDefault(g => g.UserId == UserId && g.GroupId == WasGroupId);
				if (userGroup != null)
				{
					userGroup.GroupId = GroupId;
					await db.SaveChangesAsync();
				}
			}
		}

		public async Task UpdateCourseAsync(int CourseId, String Name, String ShortName)
		{
			Course course = db.Courses.FirstOrDefault(c => c.CourseId == CourseId);
			if (course != null)
			{
				course.Name = Name;
				course.ShortName = ShortName;
				await db.SaveChangesAsync();
			}
		}

		public async Task UpdateCourseGroupAsync(int CourseId, int WasGroupId, int GroupId)
		{
			bool IsCourseGroup = db.CourseGroups.Any(u => u.CourseId == CourseId && u.GroupId == GroupId);
			if (!IsCourseGroup)
			{
				CourseGroup courseGroup = db.CourseGroups.FirstOrDefault(g => g.CourseId == CourseId && g.GroupId == WasGroupId);
				if (courseGroup != null)
				{
					courseGroup.GroupId = GroupId;
					await db.SaveChangesAsync();
				}
			}
		}

		public async Task<bool> UpdateCourseTopicVisibilityAsync(int CourseId, int CourseTopicId)
		{
			CourseTopic courseTopic = await db.CourseTopics.FirstOrDefaultAsync(c => c.CourseId == CourseId && c.CourseTopicId == CourseTopicId);
			if (courseTopic != null)
			{
				courseTopic.IsHidden = !courseTopic.IsHidden;
				await db.SaveChangesAsync();
				return courseTopic.IsHidden;
			}
			return false;
		}

		public async Task UpdateUserCourseFileAsync(Statistic model, bool IsRandom)
		{
			UserCourseFile userCourseFile = await GetUserCourseFileByParamsAsync(model.UserId, model.CourseTopicFileId);
			Statistic statistic = await GetStatisticsById(model.StatisticId);

			if (IsRandom)
			{
				List<int> filesId = (await GetCourseTopicFileTasksByCourseTopicFileId(model.CourseTopicFileId)).Select(x => x.FileId).ToList();
				filesId.Remove(userCourseFile.FileId);
				int fileId = filesId[new Random().Next(0, filesId.Count())];
				statistic.FileId = fileId;
				userCourseFile.FileId = fileId;
			}
			else
			{
				statistic.FileId = model.FileId;
				userCourseFile.FileId = model.FileId;
			}

			await db.SaveChangesAsync();
		}

		//search
		public async Task<List<User>> SearchUsersAsync(string search)
		{
			List<User> users = new List<User>();
			string[] searchSplit = search.Split(' ');
			List<User> findUsers;
			if (searchSplit.Length > 1)
			{
				string split1 = searchSplit[0];
				string split2 = searchSplit[1];
				findUsers = await db.Users.Where(u => u.FirstName.Contains(split1) && u.LastName.Contains(split2) || u.FirstName.Contains(split2) && u.LastName.Contains(split1)).ToListAsync();
			}
			else
			{
				findUsers = db.Users.Where(u => u.FirstName.Contains(search) || u.LastName.Contains(search) || u.Email.Contains(search)).ToList();
				List<int> groupsId = db.Groups.Where(g => g.Name.Contains(search)).Select(x => x.GroupId).ToList();
				if (groupsId.Count > 0)
					foreach (int groupId in groupsId)
						foreach (User user in await GetUsersByGroupId(groupId))
							users.Add(user);
			}

			foreach (User user in findUsers)
				users.Add(user);

			return users;
		}
		public async Task<List<Group>> SearchGroupsAsync(string search) => await db.Groups.Where(u => u.Name.Contains(search)).ToListAsync();
		public async Task<List<Course>> SearchCoursesAsync(string search)
		{
			List<Course> courses = new List<Course>();
			List<Course> findCourses = await db.Courses.Where(c => c.Name.Contains(search) || c.ShortName.Contains(search)).ToListAsync();
			List<int> groupsId = await db.Groups.Where(g => g.Name.Contains(search)).Select(x => x.GroupId).ToListAsync();
			if (groupsId.Count > 0)
				foreach (int groupId in groupsId)
					foreach (Course course in await GetCoursesByGroupIdAsync(groupId))
						courses.Add(course);

			foreach (Course course in findCourses)
				courses.Add(course);

			return courses;
		}
		public async Task<List<Statistic>> SearchStatisticsAsync(int CourseTopicFileId, string search)
		{
			string[] searchSplit = search.Split(' ');

			if (searchSplit.Length > 1)
			{
				string split1 = searchSplit[0];
				string split2 = searchSplit[1];
				return await db.Statistics.Where(s =>
				s.CourseTopicFileId == CourseTopicFileId && (s.User.FirstName.Contains(split1) && s.User.LastName.Contains(split2) ||
				s.User.FirstName.Contains(split2) && s.User.LastName.Contains(split1) ||
				s.File.Name.Contains(search)))
				.ToListAsync();
			}
			else
				return await db.Statistics
					.Where(s => s.CourseTopicFileId == CourseTopicFileId && (s.User.FirstName.Contains(search) ||
						s.User.LastName.Contains(search) ||
						s.User.Email.Contains(search) ||
						s.File.Name.Contains(search)))
					.ToListAsync();
		}

		//CREATE
		public async Task<DbManagerResult> CreateUser(RegisterViewModel User)
		{
			if (db.Users.Any(u => u.Email == User.Email))
				return DbManagerResult.Exist;
			else
			{
				User newUser = new User
				{
					Email = User.Email,
					FirstName = User.FirstName,
					LastName = User.LastName,
					Password = User.Password
				};
				db.Users.Add(newUser);
				await db.SaveChangesAsync();
				return DbManagerResult.Successful;
			}
		}

		public async Task CreateLaboratoryFile(UploadFileModel Model, string FileLocation, string AttachedFileLocation)
		{
			File file = new File
			{
				Name = Model.FileName,
				FileTypeId = Model.FileType,
				//ClosingIn = Model.ClosingIn,
				CourseId = Model.CourseId
			};
			db.Files.Add(file);
			await db.SaveChangesAsync();

			FilesLocation fileLocation = new FilesLocation
			{
				FileId = file.FileId,
				FileLocation = FileLocation,
				AttachedFileLocation = AttachedFileLocation
			};
			db.FilesLocation.Add(fileLocation);

			await db.SaveChangesAsync();
		}

		public async Task<DbManagerResult> CreateTopic(TopicModel model)
		{
			if (String.IsNullOrEmpty(model.SectionName))
				return DbManagerResult.Error;
			db.CourseTopics.Add(new CourseTopic
			{
				CourseId = model.CourseId,
				Topic = model.Topic,
				SectionName = model.SectionName,
				Description = model.Description,
				IsHidden = model.IsHidden
			});
			await db.SaveChangesAsync();
			return DbManagerResult.Successful;
		}

		public async Task CreateTopicFile(UploadFileToTheCourse model)
		{
			List<User> users = await GetUsersByCourseId(model.CourseId);
			foreach (int fileId in model.FilesId)
			{
				CourseTopicFile courseTopicFile = new CourseTopicFile
				{
					FileId = fileId,
					CourseTopicId = model.CourseTopicId,
					IsVariant = model.IsRandom,
					CourseId = model.CourseId
				};
				db.CourseTopicFiles.Add(courseTopicFile);
				await db.SaveChangesAsync();

				foreach (User user in users)
				{
					db.Statistics.Add(new Statistic
					{
						CourseTopicFileId = courseTopicFile.CourseTopicFileId,
						UserId = user.UserId,
						CourseId = model.CourseId,
						CourseTopicId = model.CourseTopicId,
						FileId = fileId,
						PerformedAt = null
					});
				}
			}
			await db.SaveChangesAsync();
		}

		public async Task CreateTopicFileRandom(UploadFileToTheCourse model)
		{
			int[] filesId = Shuffle(model.FilesId);
			List<User> users = await GetUsersByCourseId(model.CourseId);
			users = Shuffle(users);

			CourseTopicFile courseTopicFile = new CourseTopicFile
			{
				CourseTopicId = model.CourseTopicId,
				FileId = null,
				CourseId = model.CourseId,
				IsVariant = true,
				Name = model.Name
			};
			db.CourseTopicFiles.Add(courseTopicFile);
			await db.SaveChangesAsync();

			foreach (int id in model.FilesId)
			{
				CourseTopicFileTask courseTopicFileTask = new CourseTopicFileTask
				{
					CourseTopicId = model.CourseTopicId,
					CourseId = model.CourseId,
					CourseTopicFileId = courseTopicFile.CourseTopicFileId,
					FileId = id
				};
				db.CourseTopicFileTasks.Add(courseTopicFileTask);
			}
			await db.SaveChangesAsync();

			int userId, fileId;
			for (int i = 0; i < users.Count; i++)
			{
				userId = users[i].UserId;
				fileId = filesId[i % filesId.Length];

				db.UserCourseFiles.Add(new UserCourseFile
				{
					UserId = users[i].UserId,
					CourseId = model.CourseId,
					FileId = fileId,
					CourseTopicId = model.CourseTopicId,
					CourseTopicFileId = courseTopicFile.CourseTopicFileId
				});

				db.Statistics.Add(new Statistic
				{
					CourseTopicFileId = courseTopicFile.CourseTopicFileId,
					UserId = users[i].UserId,
					CourseId = model.CourseId,
					CourseTopicId = model.CourseTopicId,
					FileId = fileId,
					PerformedAt = null
				});
			}
			await db.SaveChangesAsync();
		}

		public List<User> Shuffle(List<User> list)
		{
			Random rng = new Random();
			List<User> users = list;
			int n = users.Count;
			User value;
			int k;
			while (n > 1)
			{
				n--;
				k = rng.Next(n + 1);
				value = users[k];
				users[k] = users[n];
				users[n] = value;
			}
			return users;
		}

		public int[] Shuffle(int[] list)
		{
			Random rng = new Random();
			int[] users = list;
			int n = users.Length;
			int value;
			int k;
			while (n > 1)
			{
				n--;
				k = rng.Next(n + 1);
				value = users[k];
				users[k] = users[n];
				users[n] = value;
			}
			return users;
		}

		private DbManagerResult CreateUserCourse(String UserEmail, int CourseId)
		{
			User user = GetUserByEmail(UserEmail);
			UserCourse userCourse = db.UserCourse.FirstOrDefault(u => u.UserId == user.UserId && u.CourseId == CourseId);
			if (userCourse == null)
			{
				db.UserCourse.Add(new UserCourse
				{
					UserId = user.UserId,
					CourseId = CourseId,
					ColorHue = 220,
					DonePercent = 0
				});
				db.SaveChanges();
				return DbManagerResult.Successful;
			}
			return DbManagerResult.Exist;
		}

		private async Task<DbManagerResult> CreateUserCourseAsync(String UserEmail, int CourseId)
		{
			User user = await GetUserByEmailAsync(UserEmail);
			UserCourse userCourse = db.UserCourse.FirstOrDefault(u => u.UserId == user.UserId && u.CourseId == CourseId);
			if (userCourse == null)
			{
				db.UserCourse.Add(new UserCourse
				{
					UserId = user.UserId,
					CourseId = CourseId,
					ColorHue = 220,
					DonePercent = 0
				});
				await db.SaveChangesAsync();
				return DbManagerResult.Successful;
			}
			return DbManagerResult.Exist;
		}

		private async Task<DbManagerResult> CreateUserCourseAsync(int UserId, int CourseId)
		{
			UserCourse userCourse = db.UserCourse.FirstOrDefault(u => u.UserId == UserId && u.CourseId == CourseId);
			if (userCourse == null)
			{
				db.UserCourse.Add(new UserCourse
				{
					UserId = UserId,
					CourseId = CourseId,
					ColorHue = 220,
					DonePercent = 0
				});
				await db.SaveChangesAsync();
				return DbManagerResult.Successful;
			}
			return DbManagerResult.Exist;
		}

		public async Task CreateUserGroupAsync(int UserId, int GroupId)
		{ ///////
			bool IsUserGroup = db.UserGroups.Any(u => u.UserId == UserId && u.GroupId == GroupId);
			if (!IsUserGroup)
			{
				db.UserGroups.Add(new UserGroup
				{
					UserId = UserId,
					GroupId = GroupId
				});
				await db.SaveChangesAsync();

				await GiveNewUserTasks(UserId);
			}
		}

		private async Task GiveNewUserTasks(int UserId)
		{
			List<Course> userCourses = GetCoursesByUserGroups(GetUserGroupsByUserId(UserId));
			List<CourseTopicFile> courseTopicFiles;
			List<CourseTopicFileTask> courseTopicFileTasks;
			CourseTopicFileTask courseTopicFileTask;
			Random random = new Random();
			int fileId = 0;
			foreach (Course course in userCourses)
			{
				courseTopicFiles = GetCourseTopicFilesByCourseId(course.CourseId);
				foreach (CourseTopicFile courseTopicFile in courseTopicFiles)
				{
					if (await db.UserCourseFiles.AnyAsync(u => u.UserId == UserId && u.CourseTopicFileId == courseTopicFile.CourseTopicFileId) ||
						await db.Statistics.AnyAsync(s => s.UserId == UserId && s.CourseTopicFileId == courseTopicFile.CourseTopicFileId))
						continue;

					if (courseTopicFile.FileId == null)
					{
						courseTopicFileTasks = await db.CourseTopicFileTasks.Where(c => c.CourseTopicFileId == courseTopicFile.CourseTopicFileId).ToListAsync();
						courseTopicFileTask = courseTopicFileTasks[random.Next(0, courseTopicFileTasks.Count)];

						db.UserCourseFiles.Add(new UserCourseFile()
						{
							UserId = UserId,
							CourseId = courseTopicFile.CourseId,
							FileId = courseTopicFileTask.FileId,
							CourseTopicId = courseTopicFile.CourseTopicId,
							CourseTopicFileId = courseTopicFile.CourseTopicFileId
						});

						fileId = courseTopicFileTask.FileId;
					}
					else
						fileId = (int)courseTopicFile.FileId;

					db.Statistics.Add(new Statistic()
					{
						UserId = UserId,
						CourseId = courseTopicFile.CourseId,
						FileId = fileId,
						CourseTopicId = courseTopicFile.CourseTopicId,
						CourseTopicFileId = courseTopicFile.CourseTopicFileId,
						PerformedAt = null
					});
					await db.SaveChangesAsync();
				}
			}
		}

		public async Task CreateGroup(String GroupName)
		{
			if (!await db.Groups.AnyAsync(u => u.Name == GroupName))
			{
				db.Groups.Add(new Group
				{
					Name = GroupName
				});
				await db.SaveChangesAsync();
			}
		}

		public async Task CreateUserAsync(String Email)
		{
			if (!await db.Users.AnyAsync(u => u.Email == Email))
			{
				User user = new User
				{
					Email = Email
				};
				db.Users.Add(user);
				await db.SaveChangesAsync();

				Role studentRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "Student");
				db.UserRoles.Add(new UserRole
				{
					UserId = user.UserId,
					RoleId = studentRole.RoleId
				});
				await db.SaveChangesAsync();
			}
		}

		public async Task CreateCourseAsync(String Name, String ShortName)
		{
			if (!await db.Courses.AnyAsync(u => u.Name == Name) && !String.IsNullOrWhiteSpace(Name))
			{
				db.Courses.Add(new Course
				{
					Name = Name,
					ShortName = ShortName
				});
				await db.SaveChangesAsync();
			}
		}

		public async Task CreateCourseGroupAsync(int CourseId, int GroupId)
		{
			if (!await db.CourseGroups.AnyAsync(u => u.CourseId == CourseId && u.GroupId == GroupId))
			{
				db.CourseGroups.Add(new CourseGroup
				{
					CourseId = CourseId,
					GroupId = GroupId
				});
				await db.SaveChangesAsync();
			}
		}

		//DELETE
		public async Task DeleteFile(int FileId)
		{
			File file = await db.Files.FirstOrDefaultAsync(f => f.FileId == FileId);
			FilesLocation fileLocation = await db.FilesLocation.FirstOrDefaultAsync(f => f.FileId == FileId);
			if (file != null)
			{
				db.Files.Remove(file);
				await db.SaveChangesAsync();
				if (fileLocation != null)
				{
					System.IO.File.Delete(fileLocation.FileLocation);
					if (fileLocation.AttachedFileLocation != null)
						System.IO.File.Delete(fileLocation.AttachedFileLocation);
				}
			}
		}

		public async Task DeleteCourseTopic(int CourseTopicId)
		{
			CourseTopic courseTopic = await db.CourseTopics.FirstOrDefaultAsync(c => c.CourseTopicId == CourseTopicId);
			if (courseTopic != null)
			{
				db.CourseTopics.Remove(courseTopic);
				await db.SaveChangesAsync();
			}
		}

		public async Task DeleteCourseTopicFile(int CourseTopicFileId)
		{
			CourseTopicFile courseTopicFile = await db.CourseTopicFiles.FirstOrDefaultAsync(c => c.CourseTopicFileId == CourseTopicFileId);
			if (courseTopicFile != null)
			{
				List<CourseTopicFileTask> courseTopicFileTasks = await db.CourseTopicFileTasks.Where(c => c.CourseTopicFileId == courseTopicFile.CourseTopicFileId).ToListAsync();
				foreach (CourseTopicFileTask courseTopicFileTask in courseTopicFileTasks)
					db.CourseTopicFileTasks.Remove(courseTopicFileTask);

				db.CourseTopicFiles.Remove(courseTopicFile);
				await db.SaveChangesAsync();
			}
		}

		public async Task DeleteUserGroup(int UserGroupId)
		{
			UserGroup userGroup = await db.UserGroups.FirstOrDefaultAsync(u => u.UserGroupId == UserGroupId);
			if (userGroup != null)
			{
				db.UserGroups.Remove(userGroup);
				await db.SaveChangesAsync();
			}

			List<Course> Courses = await GetCoursesByGroupIdAsync(userGroup.GroupId);

			foreach (Course course in Courses)
			{
				IQueryable<UserCourseFile> userCourseFiles = db.UserCourseFiles.Where(u => u.UserId == userGroup.UserId && u.CourseId == course.CourseId);
				foreach (UserCourseFile userCourseFile in userCourseFiles)
					db.UserCourseFiles.Remove(userCourseFile);

				IQueryable<Statistic> statistics = db.Statistics.Where(u => u.UserId == userGroup.UserId && u.CourseId == course.CourseId);
				foreach (Statistic statistic in statistics)
					db.Statistics.Remove(statistic);
			}
			await db.SaveChangesAsync();
		}

		public async Task DeleteGroupAsync(int GroupId)
		{
			Group group = await db.Groups.FirstOrDefaultAsync(g => g.GroupId == GroupId);
			if (group != null)
			{
				db.Groups.Remove(group);
				await db.SaveChangesAsync();
			}
		}

		public async Task DeleteUserAsync(int UserId)
		{
			User user = await db.Users.FirstOrDefaultAsync(u => u.UserId == UserId);
			if (user != null)
			{
				db.Users.Remove(user);
				await db.SaveChangesAsync();
			}
		}

		public async Task DeleteCourseAsync(int CourseId)
		{
			Course course = await db.Courses.FirstOrDefaultAsync(c => c.CourseId == CourseId);
			if (course != null)
			{
				db.Courses.Remove(course);
				await db.SaveChangesAsync();
			}
		}

		public async Task DeleteCourseGroupAsync(int CourseGroupId)
		{
			CourseGroup courseGroup = await db.CourseGroups.FirstOrDefaultAsync(c => c.CourseGroupId == CourseGroupId);
			if (courseGroup != null)
			{
				db.CourseGroups.Remove(courseGroup);
				await db.SaveChangesAsync();
			}
		}

		public async Task DeleteStatisticAsync(int StatisticId)
		{
			Statistic statistic = await db.Statistics.FirstOrDefaultAsync(g => g.StatisticId == StatisticId);
			if (statistic != null)
			{
				db.Statistics.Remove(statistic);
				await db.SaveChangesAsync();

				if (!db.Statistics.Any(s => s.CourseTopicFileId == statistic.CourseTopicFileId))
				{
					CourseTopicFile courseTopicFile = db.CourseTopicFiles.FirstOrDefault(c => c.CourseTopicFileId == statistic.CourseTopicFileId);
					db.CourseTopicFiles.Remove(courseTopicFile);
					await db.SaveChangesAsync();
				}

				UserCourseFile userCourseFile = await db.UserCourseFiles.FirstOrDefaultAsync(u => u.CourseTopicFileId == statistic.CourseTopicFileId && u.UserId == statistic.UserId);
				if (userCourseFile != null)
				{
					db.UserCourseFiles.Remove(userCourseFile);
					await db.SaveChangesAsync();
				}
			}
		}

		public void Dispose() => db.Dispose();
	}

	public enum DbManagerResult
	{
		Successful, Error, Exist
	}
}