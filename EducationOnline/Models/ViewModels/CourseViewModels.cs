using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DistanceLearning.Models
{
	public class CourseVM : IEquatable<CourseVM>
	{
		public int Id { get; set; }
		[Display(Name = "Название курса")]
		public string Name { get; set; }
		[Display(Name = "Аббревиатура курса")]
		public string ShortName { get; set; }

		public CourseVM(Course course)
		{
			Id = course.CourseId;
			Name = course.Name;
			ShortName = course.ShortName;
		}

		public CourseVM(int Id) => this.Id = Id;

		public CourseVM GetCourse()
		{
			if (Id != 0)
				using (DbManager db = new DbManager())
					return new CourseVM(db.GetCourseById(Id));
			else return null;
		}

		public async Task<CourseVM> GetCourseAsync()
		{
			if (Id != 0)
				using (DbManager db = new DbManager())
					return new CourseVM(await db.GetCourseByIdAsync(Id));
			else return null;
		}

		public List<TestCourseTopicVM> GetTests()
		{
			if (Id != 0)
				using (DbManager db = new DbManager())
					return db.GetTestsCourseTopicByCourseId(Id).Select(x => new TestCourseTopicVM(x)).ToList();
			else return null;
		}

		public List<CourseTopicFileVM> GetFiles()
		{
			if (Id != 0)
				using (DbManager db = new DbManager())
					return db.GetCourseTopicFilesByCourseId(Id).Select(x => new CourseTopicFileVM(x)).ToList();
			else return null;
		}

		public override bool Equals(object obj) => Equals(obj as CourseVM);

		public bool Equals(CourseVM other)
		{
			return other != null &&
				   Id == other.Id &&
				   Name == other.Name &&
				   ShortName == other.ShortName;
		}

		public override int GetHashCode()
		{
			var hashCode = 673720172;
			hashCode = hashCode * -1521134295 + Id.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ShortName);
			return hashCode;
		}

		public static bool operator ==(CourseVM vM1, CourseVM vM2)
		{
			return EqualityComparer<CourseVM>.Default.Equals(vM1, vM2);
		}

		public static bool operator !=(CourseVM vM1, CourseVM vM2)
		{
			return !(vM1 == vM2);
		}
	}

	public class FileVM
	{
		public int Id { get; set; }
		public int CourseId { get; set; }
		public int FileTypeId { get; set; }
		public string Name { get; set; }
		public DateTime ClosingIn { get; set; }
		public bool IsLabFile
		{
			get
			{
				if (Id != 0)
					using (DbManager db = new DbManager())
					{
						IEnumerable<int> labTypes = db.GetFileTypesLab().Select(x => x.FileTypeId);
						return labTypes.Contains(FileTypeId);
					}
				return false;
			}
		}

		public FileVM(File file)
		{
			Id = file.FileId;
			CourseId = file.CourseId;
			FileTypeId = file.FileTypeId;
			Name = file.Name;
			ClosingIn = file.ClosingIn;
		}

		public FileVM(int Id) => this.Id = Id;

		public FileVM GetFile()
		{
			if (Id != 0)
				using (DbManager db = new DbManager())
					return new FileVM(db.GetFileById(Id));
			else return null;
		}

		public async Task<FileVM> GetFileAsync()
		{
			if (Id != 0)
				using (DbManager db = new DbManager())
					return new FileVM(await db.GetFileByIdAsync(Id));
			else return null;
		}

		public FileTypeVM GetFileType() => new FileTypeVM(FileTypeId).GetFileType();
		public CourseVM GetCourse() => new CourseVM(CourseId).GetCourse();
		public async Task<CourseVM> GetCourseAsync() => await new CourseVM(CourseId).GetCourseAsync();

		public FileLocationVM GetFileLocation()
		{
			if (Id != 0)
				using (DbManager db = new DbManager())
					return new FileLocationVM(db.GetFileLocationByFileId(Id));
			else return null;
		}
	}

	public class FileLocationVM
	{

		public int FileLocationId { get; set; }
		public int FileId { get; set; }
		public String FileLocation { get; set; }
		public String AttachedFileLocation { get; set; }

		public FileLocationVM(FilesLocation fileLocation)
		{
			FileLocationId = fileLocation.FileLocationId;
			FileId = fileLocation.FileId;
			FileLocation = fileLocation.FileLocation;
			AttachedFileLocation = fileLocation.AttachedFileLocation;
		}

		public FileVM GetFile() => new FileVM(FileId).GetFile();
		public async Task<FileVM> GetFileAsync() => await new FileVM(FileId).GetFileAsync();
	}

	public class UserCourseVM
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int CourseId { get; set; }
		public int ColorHue { get; set; }
		public int DonePercent { get; set; }

		public UserCourseVM(UserCourse userCourse)
		{
			Id = userCourse.UserCourseId;
			UserId = userCourse.UserId;
			CourseId = userCourse.CourseId;
			ColorHue = userCourse.ColorHue;
			DonePercent = userCourse.DonePercent;
		}

		public UserVM GetUser() => new UserVM(UserId).GetUser();
		public async Task<UserVM> GetUserAsync() => await new UserVM(UserId).GetUserAsync();
		public CourseVM GetCourse() => new CourseVM(CourseId).GetCourse();
		public async Task<CourseVM> GetCourseAsync() => await new CourseVM(CourseId).GetCourseAsync();
	}

	public class UserCourseFileVM
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int CourseId { get; set; }
		public int FileId { get; set; }
		public int? CourseTopicId { get; set; }
		public int CourseTopicFileId { get; set; }
		public UserVM User { get; private set; }
		public CourseVM Course { get; private set; }
		public FileVM File { get; private set; }
		public CourseTopicVM CourseTopic { get; private set; }
		public CourseTopicFileVM CourseTopicFile { get; private set; }

		public UserCourseFileVM() { }

		public UserCourseFileVM(UserCourseFile userCourseFile)
		{
			Id = userCourseFile.UserCourseFileId;
			UserId = userCourseFile.UserId;
			CourseId = userCourseFile.CourseId;
			FileId = userCourseFile.FileId;
			CourseTopicId = userCourseFile.CourseTopicId;
			CourseTopicFileId = userCourseFile.CourseTopicFileId;
		}

		public UserVM GetUser() => new UserVM(UserId).GetUser();
		public async Task<UserVM> GetUserAsync() => await new UserVM(UserId).GetUserAsync();
		public CourseVM GetCourse() => new CourseVM(CourseId).GetCourse();
		public async Task<CourseVM> GetCourseAsync() => await new CourseVM(CourseId).GetCourseAsync();
		public FileVM GetFile() => new FileVM(FileId).GetFile();
		public async Task<FileVM> GetFileAsync() => await new FileVM(FileId).GetFileAsync();
		public CourseTopicVM GetCourseTopic() => new CourseTopicVM((int)CourseTopicId).GetCourseTopic();
		public async Task<CourseTopicVM> GetCourseTopicAsync() => await new CourseTopicVM((int)CourseTopicId).GetCourseTopicAsync();
		public CourseTopicFileVM GetCourseTopicFile() => new CourseTopicFileVM((int)CourseTopicFileId).GetCourseTopicFile();
		public async Task<CourseTopicFileVM> GetCourseTopicFileAsync() => await new CourseTopicFileVM((int)CourseTopicFileId).GetCourseTopicFileAsync();

		public void Update()
		{
			User = GetUser();
			Course = GetCourse();
			File = GetFile();
			CourseTopic = GetCourseTopic();
			CourseTopicFile = GetCourseTopicFile();
		}
	}

	public class CourseGroupVM
	{
		public int Id { get; set; }
		public int CourseId { get; set; }
		public int GroupId { get; set; }
		public CourseVM Course { get; set; }
		public GroupVM Group { get; set; }

		public CourseGroupVM() { }

		public CourseGroupVM(CourseGroup courseGroup)
		{
			Id = courseGroup.CourseGroupId;
			CourseId = courseGroup.CourseId;
			GroupId = courseGroup.GroupId;
		}

		public CourseVM GetCourse() => new CourseVM(CourseId).GetCourse();
		public async Task<CourseVM> GetCourseAsync() => await new CourseVM(CourseId).GetCourseAsync();
		public GroupVM GetGroup() => new GroupVM(GroupId).GetGroup();
		public async Task<GroupVM> GetGroupAsync() => await new GroupVM(GroupId).GetGroupAsync();

		public async Task Update()
		{
			Course = await GetCourseAsync();
			Group = await GetGroupAsync();
		}
	}

	public class CourseTopicVM
	{
		public int Id { get; set; }
		public int CourseId { get; set; }
		public string SectionName { get; set; }
		public string Topic { get; set; }
		public string Description { get; set; }
		public bool IsHidden { get; set; }

		public CourseTopicVM(int Id) => this.Id = Id;

		public CourseTopicVM(CourseTopic courseTopic)
		{
			Id = courseTopic.CourseTopicId;
			CourseId = courseTopic.CourseId;
			SectionName = courseTopic.SectionName;
			Topic = courseTopic.Topic;
			Description = courseTopic.Description;
			IsHidden = courseTopic.IsHidden;
		}

		public CourseTopicVM GetCourseTopic()
		{
			if (Id != 0)
				using (DbManager db = new DbManager())
					return new CourseTopicVM(db.GetCourseTopicById(Id));
			else return null;
		}

		public async Task<CourseTopicVM> GetCourseTopicAsync()
		{
			if (Id != 0)
				using (DbManager db = new DbManager())
					return new CourseTopicVM(await db.GetCourseTopicByIdAsync(Id));
			else return null;
		}

		public CourseVM GetCourse() => new CourseVM(CourseId).GetCourse();
		public async Task<CourseVM> GetCourseAsync() => await new CourseVM(CourseId).GetCourseAsync();
	}

	public class FileTypeVM
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		public FileTypeVM(int Id) => this.Id = Id;

		public FileTypeVM(FileType fileType)
		{
			Id = fileType.FileTypeId;
			Name = fileType.Name;
			Description = fileType.Description;
		}

		public FileTypeVM GetFileType()
		{
			if (Id != 0)
				using (DbManager db = new DbManager())
					return new FileTypeVM(db.GetFileTypeById(Id));
			else return null;
		}

		public async Task<FileTypeVM> GetFileTypeAsync()
		{
			if (Id != 0)
				using (DbManager db = new DbManager())
					return new FileTypeVM(await db.GetFileTypeByIdAsync(Id));
			else return null;
		}
	}

	public class CourseTopicFileVM
	{
		public int Id { get; set; }
		public int? CourseTopicId { get; set; }
		public int? FileId { get; set; }
		public int CourseId { get; set; }
		public bool IsVariant { get; set; }
		public String Name { get; set; }

		public CourseTopicFileVM(CourseTopicFile courseTopicFile)
		{
			if (courseTopicFile == null)
				return;
			Id = courseTopicFile.CourseTopicFileId;
			CourseTopicId = courseTopicFile.CourseTopicId;
			FileId = courseTopicFile.FileId;
			IsVariant = courseTopicFile.IsVariant;
			CourseId = courseTopicFile.CourseId;
			Name = courseTopicFile.Name;
		}

		public CourseTopicFileVM(int Id) => this.Id = Id;

		public CourseTopicVM GetCourseTopic() => new CourseTopicVM((int)CourseTopicId).GetCourseTopic();
		public async Task<CourseTopicVM> GetCourseTopicAsync() => await new CourseTopicVM((int)CourseTopicId).GetCourseTopicAsync();
		public FileVM GetFile() => new FileVM((int)FileId).GetFile();
		public async Task<FileVM> GetFileAsync() => await new FileVM((int)FileId).GetFileAsync();

		public CourseTopicFileVM GetCourseTopicFile()
		{
			using (DbManager db = new DbManager())
				return new CourseTopicFileVM(db.GetCourseTopicFileById(Id));
		}

		public async Task<CourseTopicFileVM> GetCourseTopicFileAsync()
		{
			using (DbManager db = new DbManager())
				return new CourseTopicFileVM(await db.GetCourseTopicFileByIdAsync(Id));
		}
	}

	public class StatisticVM
	{
		public int Id { get; set; }
		public int CourseTopicFileId { get; set; }
		public int UserId { get; set; }
		public int CourseId { get; set; }
		public int? CourseTopicId { get; set; }
		public int FileId { get; set; }
		public DateTime? PerformedAt { get; set; }
		public UserVM User { get; set; }
		public CourseTopicFileVM CourseTopicFile { get; set; }
		public CourseVM Course { get; set; }
		public CourseTopicVM CourseTopic { get; set; }
		public FileVM File { get; set; }

		public StatisticVM(Statistic statistic)
		{
			Id = statistic.StatisticId;
			UserId = statistic.UserId;
			CourseTopicFileId = statistic.CourseTopicFileId;
			CourseTopicId = statistic.CourseTopicId;
			CourseId = statistic.CourseId;
			FileId = statistic.FileId;
			PerformedAt = statistic.PerformedAt;
		}

		public UserVM GetUser() => new UserVM(UserId).GetUser();
		public async Task<UserVM> GetUserAsync() => await new UserVM(UserId).GetUserAsync();
		public CourseVM GetCourse() => new CourseVM(CourseId).GetCourse();
		public async Task<CourseVM> GetCourseAsync() => await new CourseVM(CourseId).GetCourseAsync();
		public CourseTopicVM GetCourseTopic() => new CourseTopicVM((int)CourseTopicId).GetCourseTopic();
		public async Task<CourseTopicVM> GetCourseTopicAsync() => await new CourseTopicVM((int)CourseTopicId).GetCourseTopicAsync();
		public FileVM GetFile() => new FileVM(FileId).GetFile();
		public async Task<FileVM> GetFileAsync() => await new FileVM(FileId).GetFileAsync();
		public async Task<CourseTopicFileVM> GetCourseTopicFileAsync() => await new CourseTopicFileVM(CourseTopicFileId).GetCourseTopicFileAsync();
		public CourseTopicFileVM GetCourseTopicFile() => new CourseTopicFileVM(CourseTopicFileId).GetCourseTopicFile();

		public void Update()
		{
			User = GetUser();
			CourseTopicFile = GetCourseTopicFile();
			Course = GetCourse();
			if (CourseTopicId != null)
				CourseTopic = GetCourseTopic();
			File = GetFile();
		}
	}

	public class CourseTopicFileTaskVM
	{
		public int Id { get; set; }
		public int CourseTopicFileId { get; set; }
		public int CourseId { get; set; }
		public int? CourseTopicId { get; set; }
		public int FileId { get; set; }
		public CourseTopicFileVM CourseTopicFile { get; set; }
		public CourseVM Course { get; set; }
		public CourseTopicVM CourseTopic { get; set; }
		public FileVM File { get; set; }

		public CourseTopicFileTaskVM(CourseTopicFileTask courseTopicFileTask)
		{
			if (courseTopicFileTask == null)
				return;
			Id = courseTopicFileTask.CourseTopicFileTaskId;
			CourseTopicFileId = courseTopicFileTask.CourseTopicFileId;
			CourseTopicId = courseTopicFileTask.CourseTopicId;
			CourseId = courseTopicFileTask.CourseId;
			FileId = courseTopicFileTask.FileId;
		}

		public CourseTopicFileTaskVM(int Id) => this.Id = Id;

		public CourseVM GetCourse() => new CourseVM(CourseId).GetCourse();
		public async Task<CourseVM> GetCourseAsync() => await new CourseVM(CourseId).GetCourseAsync();
		public CourseTopicVM GetCourseTopic() => new CourseTopicVM((int)CourseTopicId).GetCourseTopic();
		public async Task<CourseTopicVM> GetCourseTopicAsync() => await new CourseTopicVM((int)CourseTopicId).GetCourseTopicAsync();
		public FileVM GetFile() => new FileVM(FileId).GetFile();
		public async Task<FileVM> GetFileAsync() => await new FileVM(FileId).GetFileAsync();
		public async Task<CourseTopicFileVM> GetCourseTopicFileAsync() => await new CourseTopicFileVM(CourseTopicFileId).GetCourseTopicFileAsync();
		public CourseTopicFileVM GetCourseTopicFile() => new CourseTopicFileVM(CourseTopicFileId).GetCourseTopicFile();

		public void Update()
		{
			CourseTopicFile = GetCourseTopicFile();
			Course = GetCourse();
			if (CourseTopicId != null)
				CourseTopic = GetCourseTopic();
			File = GetFile();
		}
	}

	public class UserTopicFileDoneVM
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int CourseId { get; set; }
		public int FileId { get; set; }
		public int CourseTopicFileId { get; set; }

		public UserTopicFileDoneVM(int Id) => this.Id = Id;

		public UserTopicFileDoneVM(UserTopicFileDone userTopicFileDone)
		{
			Id = userTopicFileDone.UserTopicFileDoneId;
			UserId = userTopicFileDone.UserId;
			CourseId = userTopicFileDone.CourseId;
			FileId = userTopicFileDone.FileId;
			CourseTopicFileId = userTopicFileDone.CourseTopicFileId;
		}

		public UserVM GetUser() => new UserVM(UserId).GetUser();
		public async Task<UserVM> GetUserAsync() => await new UserVM(UserId).GetUserAsync();
		public CourseVM GetCourse() => new CourseVM(CourseId).GetCourse();
		public async Task<CourseVM> GetCourseAsync() => await new CourseVM(CourseId).GetCourseAsync();
		public CourseTopicFileVM GetCourseTopicFile() => new CourseTopicFileVM(CourseTopicFileId).GetCourseTopicFile();
		public async Task<CourseTopicFileVM> GetCourseTopicFileAsync() => await new CourseTopicFileVM(CourseTopicFileId).GetCourseTopicFileAsync();

		public UserTopicFileDoneVM GetTopicFile()
		{
			if (Id != 0)
				using (DbManager db = new DbManager())
					return new UserTopicFileDoneVM(db.GetUserTopicFileDoneById(Id));
			else return null;
		}

		public async Task<UserTopicFileDoneVM> GetTopicFileAsync()
		{
			if (Id != 0)
				using (DbManager db = new DbManager())
					return new UserTopicFileDoneVM(await db.GetUserTopicFileDoneByIdAsync(Id));
			else return null;
		}
	}
}