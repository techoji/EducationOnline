using DistanceLearning.Models;
using DistanceLearning.Models.TeacherUpdates;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DistanceLearning.Controllers
{

	[Authorize]
	public class PagesController : Controller
	{

		private DbManager db { get; set; }

		public PagesController() => db = new DbManager();

		[AllowAnonymous]
		public ActionResult Index()
		{
			ViewBag.ShowCourseEvents = true;
			return View();
		}

		public ActionResult Courses()
		{
			ViewBag.ShowCourseEvents = true;
			return View("Courses/Courses");
		}

		//[OutputCache(Duration = 300)]
		public async Task<ActionResult> GetCourses()
		{
			User user = await db.GetUserByEmailAsync(User.Identity.Name);
			if (user == null)
				return PartialView("Courses/_UserCourses", new List<CourseVM>());

			bool IsTeacher = await db.IsTeacherByEmailAsync(User.Identity.Name);
			TempData["IsTeacher"] = IsTeacher;
			List<CourseVM> coursesVM = (await db.GetUserCoursesByUserEmailAsync(User.Identity.Name)).Select(x => new CourseVM(x)).Distinct().ToList();
			ViewBag.User = new UserVM(user);
			ViewBag.UserGroups = db.GetUserGroupsByUserId(user.UserId).Select(x => new UserGroupVM(x)).ToList();
			return PartialView("Courses/_UserCourses", coursesVM);
		}

		[AllowAnonymous]
		public ActionResult Error()
		{
			ViewBag.ShowCourseEvents = true;
			return View();
		}

		[Route("pages/course/{CourseId}/{edit?}")]
		public async Task<ActionResult> Course(int CourseId, String edit)
		{
			bool IsTeacher = await db.IsTeacherByEmailAsync(User.Identity.Name);
			bool IsHaveAccess = false;

			if (IsTeacher)
			{
				ViewBag.CourseEdit = edit;
				ViewBag.IsTeacher = true;
				IsHaveAccess = await db.HaveTeacherAccessToCourseByEmailAsync(User.Identity.Name, CourseId);
				Session["FileTypes"] = Session["FileTypes"] == null ? db.GetFileTypesLab() : Session["FileTypes"];
			}
			else
			{
				if (!String.IsNullOrWhiteSpace(edit))
					return RedirectToAction("Error");
				IsHaveAccess = await db.HaveUserAccessToCourseAsync(User.Identity.Name, CourseId);
			}

			if (IsHaveAccess)
			{
				ViewBag.ShowCourseEvents = true;
				ViewBag.UserCourseColor = (await db.GetUserCourseAsync(User.Identity.Name, CourseId)).ColorHue;

				return View("Courses/Course", new CourseVM(await db.GetCourseByIdAsync(CourseId)));
			}
			return RedirectToAction("Error");
		}

		[Authorize(Roles = "teacher")]
		[Route("pages/statistics/{CourseTopicFileId}/{search?}")]
		public async Task<ActionResult> Statistics(int CourseTopicFileId, String search)
		{
			CourseTopicFile courseTopicFile = await db.GetCourseTopicFileByIdAsync(CourseTopicFileId);
			if (courseTopicFile == null)
				return RedirectToAction("Error");
			bool IsTeacher = await db.IsTeacherByEmailAsync(User.Identity.Name);
			if (!IsTeacher)
				return RedirectToAction("Error");
			bool IsAccess = await db.HaveTeacherAccessToCourseByEmailAsync(User.Identity.Name, courseTopicFile.CourseId);
			if (!IsAccess)
				return RedirectToAction("Error");
			if ((await db.GetStatisticsByCourseTopicFileId(CourseTopicFileId)).Count == 0)
				return RedirectToAction("Error");

			ViewBag.UserCourseColor = (await db.GetUserCourseAsync(User.Identity.Name, courseTopicFile.CourseId)).ColorHue;
			ViewBag.Course = await db.GetCourseByCourseTopicFileIdAsync(CourseTopicFileId);
			ViewBag.CourseTopic = await db.GetCourseTopicByCourseTopicFileIdAsync(CourseTopicFileId);
			ViewBag.CourseTopicFileId = CourseTopicFileId;

			if (!String.IsNullOrWhiteSpace(search))
			{
				List<StatisticVM> statistics = (await db.SearchStatisticsAsync(CourseTopicFileId, search)).Select(x => new StatisticVM(x)).ToList();
				ViewBag.Search = search;
				return View(statistics);
			}

			return View((await db.GetStatisticsByCourseTopicFileId(CourseTopicFileId)).Select(x => new StatisticVM(x)).ToList());
		}

		[Authorize(Roles = "teacher")]
		public async Task<ActionResult> EditStatistic(int id)
		{
			StatisticVM statistic = new StatisticVM(await db.GetStatisticsById(id));
			statistic.Update();
			List<Models.File> files = new List<Models.File>();
			foreach (CourseTopicFileTask courseTopicFileTask in await db.GetCourseTopicFileTasksByCourseTopicFileId(statistic.CourseTopicFileId))
				files.Add(await db.GetFileByIdAsync(courseTopicFileTask.FileId));
			ViewBag.Files = files;
			return PartialView("Partial/Edit/_EditStatisticRow", statistic);
		}

		[Authorize(Roles = "teacher")]
		public async Task<ActionResult> CancelStatistic(int id)
		{
			StatisticVM statistic = new StatisticVM(await db.GetStatisticsById(id));
			statistic.Update();
			return PartialView("Partial/_StatisticRow", statistic);
		}

		[Authorize(Roles = "teacher")]
		public async Task<ActionResult> SaveStatistic(Statistic model)
		{
			await db.UpdateUserCourseFileAsync(model, model.FileId == 0 ? true : false);
			StatisticVM statistic = new StatisticVM(await db.GetStatisticsById(model.StatisticId));
			statistic.Update();
			return PartialView("Partial/_StatisticRow", statistic);
		}

		[Authorize(Roles = "teacher")]
		public async Task<ActionResult> ResetStatistic(int id)
		{
			await db.UpdateStatisticPerformedResetAsync(id);
			StatisticVM statistic = new StatisticVM(await db.GetStatisticsById(id));
			statistic.Update();
			return PartialView("Partial/_StatisticRow", statistic);
		}

		[Authorize(Roles = "teacher")]
		public async Task<ActionResult> DeleteStatistic(int id)
		{
			await db.DeleteStatisticAsync(id);
			return null;
		}

		public async Task<ActionResult> Test(int id, int? question)
		{
			TestCourseTopicVM testCourseTopic = new TestCourseTopicVM(await db.GetTestsCourseTopicByTestIdAsync(id));
			TestVM test = await testCourseTopic.GetTestAsync();
			CourseVM course = test.GetCourse();
			UserTestVM userTest = new UserTestVM(await db.GetUserTestAsync(User.Identity.Name, course.Id, id, testCourseTopic.Id));
			List<TestQuestion> Questions = await db.GetTestQuestionsByTestIdAsync(id);

			if (userTest.IsStarted && userTest.ClosingIn <= DateTime.Now || userTest.IsDone || !userTest.IsStarted && question != null)
				return RedirectToAction("Error");

			ViewBag.Test = test;
			ViewBag.UserTest = userTest;
			ViewBag.Course = course;
			ViewBag.Questions = Questions;
			ViewBag.UserCourse = new UserCourseVM(await db.GetUserCourseAsync(User.Identity.Name, course.Id));

			if (!userTest.IsStarted && question == null)
			{
				ViewBag.TimeMinutes = test.TimeMinutes;
				ViewBag.Attempts = test.Attempts;
			}
			else if (userTest.IsStarted)
			{
				question = question == null ? 1 : question;

				List<TestAnswer> answers = await db.GetTestAnswersByTestParamsAsync(id, (int)question);
				List<UserTestAnswer> userTestAllAnswers = await db.GetUserTestAnswersByTestParamsAsync(id);

				ViewBag.Question = new TestQuestionVM(await db.GetTestQuestionByTestParamsAsync(id, (int)question));
				ViewBag.Answers = answers.Select(x => new TestAnswerVM(x)).ToList();
				ViewBag.UserAnswers = userTestAllAnswers.Select(x => new UserTestAnswerVM(x)).ToList();

				return View("Test", testCourseTopic);
			}
			return View("Test", testCourseTopic);
		}

		[Authorize(Roles = "teacher")]
		[Route("pages/files/{CourseTopicFileId}")]
		public async Task<ActionResult> TaskFiles(int CourseTopicFileId)
		{
			CourseTopicFile courseTopicFile = await db.GetCourseTopicFileByIdAsync(CourseTopicFileId);
			if (courseTopicFile == null)
				return RedirectToAction("Error");
			bool IsTeacher = await db.IsTeacherByEmailAsync(User.Identity.Name);
			if (!IsTeacher)
				return RedirectToAction("Error");
			bool IsAccess = await db.HaveTeacherAccessToCourseByEmailAsync(User.Identity.Name, courseTopicFile.CourseId);
			if (!IsAccess)
				return RedirectToAction("Error");
			List<CourseTopicFileTask> CourseTopicFileTasks = await db.GetCourseTopicFileTasksByCourseTopicFileId(CourseTopicFileId);
			ViewBag.UserCourseColor = (await db.GetUserCourseAsync(User.Identity.Name, courseTopicFile.CourseId)).ColorHue;
			return View(CourseTopicFileTasks.Select(x => new CourseTopicFileTaskVM(x)).ToList());
		}

		[HttpPost]
		public async Task SetFileChecked(int CourseId, int FileId, int CourseTopicFileId, bool IsChecked)
		{
			User user = await db.GetUserByEmailAsync(User.Identity.Name);
			await db.UpdateFileCheckedAsync(CourseId, FileId, CourseTopicFileId, user.UserId, IsChecked);
		}

		[HttpPost]
		public async Task SetTestChecked(int CourseId, int TestId, int TestCourseTopicId, bool IsChecked)
		{
			User user = await db.GetUserByEmailAsync(User.Identity.Name);
			await db.UpdateTestCheckedAsync(CourseId, TestId, TestCourseTopicId, user.UserId, IsChecked);
		}

		[HttpPost]
		public async Task<ActionResult> TestStart(StartTest model)
		{
			await db.UpdateUserTestAsync(model.UserId, model.CourseId, model.TestId, model.TestCourseTopicId, IsStarted: true);
			TestCourseTopicVM testCourseTopic = new TestCourseTopicVM(await db.GetTestsCourseTopicByTestIdAsync(model.TestId));
			return RedirectToAction("test", "pages", new { id = testCourseTopic.TestId, question = model.TestQuestionNumber });
		}

		[HttpPost]
		public async Task<ActionResult> TestStop(int TestId, int CourseId, int TestCourseTopicId)
		{
			User user = await db.GetUserByEmailAsync(User.Identity.Name);
			await db.UpdateUserTestAsync(user.UserId, CourseId, TestId, TestCourseTopicId, IsDone: true, IsChecked: true);
			return RedirectToAction("course", "pages", new { id = CourseId });
		}

		[HttpPost]
		public async Task UpdateTestEnd(int TestId, int CourseId, int TestCourseTopicId)
		{
			User user = await db.GetUserByEmailAsync(User.Identity.Name);
			await db.UpdateUserTestAsync(user.UserId, CourseId, TestId, TestCourseTopicId, IsDone: true);
		}

		public ActionResult UpdateCourseEvents(bool IsUpdateEvents)
		{
			ViewBag.UpdateEvents = IsUpdateEvents;
			return PartialView("Courses/_CourseEvents", "Pages");
		}

		[HttpPost]
		public async Task SubmitTestAnswer(int TestId, int TestQuestionId, List<int> Answers)
		{
			User user = await db.GetUserByEmailAsync(User.Identity.Name);
			await db.UpdateTestAnswerAsync(user.UserId, TestId, TestQuestionId, Answers);
		}

		[HttpPost]
		public async Task UpdateUserCourseColor(int CourseId, int ColorHue) => await db.UpdateUserCourseColorAsync(User.Identity.Name, CourseId, ColorHue);

		public async Task<ActionResult> GetCourseTopicFile(int CourseId, bool IsEdit)
		{
			TempData["IsTeacher"] = await db.IsTeacherByEmailAsync(User.Identity.Name);
			TempData["IsEdit"] = IsEdit;
			ViewBag.User = await db.GetUserByEmailAsync(User.Identity.Name);
			ViewBag.CourseTopics = (await db.GetCourseTopicByCourseIdAsync(CourseId)).Where(u => !string.IsNullOrWhiteSpace(u.SectionName)).Select(x => new CourseTopicVM(x)).ToList();
			ViewBag.UserTests = (await db.GetUserTestByCourseIdAsync(User.Identity.Name, CourseId)).Select(x => new UserTestVM(x)).ToList();
			ViewBag.UserTopicFilesDone = (await db.GetUserTopicFileDoneByCourseIdAsync(User.Identity.Name, CourseId)).Select(x => new UserTopicFileDoneVM(x)).ToList();
			ViewBag.CourseTopicFiles = (await db.GetCourseTopicFilesByCourseIdAsync(CourseId)).Select(x => new CourseTopicFileVM(x)).ToList();
			ViewBag.UserCourseFiles = (await db.GetUserCourseFilesAsync(User.Identity.Name, CourseId)).Select(x => new UserCourseFileVM(x)).ToList();
			ViewBag.TestsCourseTopic = (await db.GetTestsCourseTopicByCourseIdAsync(CourseId)).Select(x => new TestCourseTopicVM(x)).ToList();
			ViewBag.UserCourseColor = (await db.GetUserCourseAsync(User.Identity.Name, CourseId)).ColorHue;
			Course course = await db.GetCourseByIdAsync(CourseId);
			return PartialView("Courses/_GetCourseTopicFile", new CourseVM(course));
		}

		[HttpPost]
		[Authorize(Roles = "teacher")]
		public async Task<ActionResult> UploadFiles(UploadFileModel model)
		{
			String fileName, fileLoc, attachedFileLoc;

			if (model?.File != null)
			{ //File
				String fileDir = Server.MapPath($"~/Content/Laboratory/{model.Course}");
				if (!Directory.Exists(fileDir))
					Directory.CreateDirectory(fileDir);

				fileName = "image_" + Guid.NewGuid().ToString() + $"_{model.File.FileName}";
				fileLoc = Server.MapPath($"~/Content/Laboratory/{model.Course}/") + fileName;
				if (model.File.ContentLength > 0 && model.File.ContentLength < 200000000) // 200mb maximum
					model.File.SaveAs(fileLoc);

				if (model.FileType == 4 && model.AttachedFile != null)
				{ //Excel 
					String attachedFileName = "excel_" + Guid.NewGuid().ToString() + $"_{model.AttachedFile.FileName}";
					attachedFileLoc = Server.MapPath($"~/Content/Laboratory/{model.Course}/") + attachedFileName;
					if (model.AttachedFile.ContentLength > 0 && model.AttachedFile.ContentLength < 200000000) // 200mb maximum
						model.AttachedFile.SaveAs(attachedFileLoc);
					await db.CreateLaboratoryFile(model, fileLoc, attachedFileLoc);
				}
				else if (model.FileType == 5)
				{
					new Thread(async () =>
					{ //new thread for image compress 
						using (Image image = Image.FromFile(fileLoc))
						{
							if (image.Width > 1080)
								using (CompressImage compress = new CompressImage(new Bitmap(image), image.Width, 1080))
								{  // image compress
									compress.OnCompressing += (e) => { HttpContext.Cache["UploadPercent"] = e.CompressingPercent; };
									compress.OnCompressStart += () => { HttpContext.Cache["UploadPercent"] = 0; };
									compress.OnCompressEnd += () => { HttpContext.Cache["UploadPercent"] = 100; };
									compress.Resize();

									String attachedFileName = "small_" + Guid.NewGuid().ToString() + $"_{model.File.FileName}";
									attachedFileLoc = Server.MapPath($"~/Content/Laboratory/{model.Course}/") + attachedFileName;
									compress.OutputImage.Save(attachedFileLoc);
									await db.CreateLaboratoryFile(model, fileLoc, attachedFileLoc);
								}
							else
							{
								HttpContext.Cache["UploadPercent"] = 100;
								await Task.Delay(1000);
								await db.CreateLaboratoryFile(model, fileLoc, fileLoc);
							}
						}
					}).Start();
				}

				Session["Message"] = "Файл успешно добавлен";
			}
			return await GetCourseTopicFile(model.CourseId, true);
		}

		[HttpPost]
		public int? GetFileCompressPercent() => HttpContext.Cache["UploadPercent"] as int?;

		[HttpPost]
		public void DeleteFileCompressPercent() => HttpContext.Cache["UploadPercent"] = 0;

		[HttpPost]
		[Authorize(Roles = "teacher")]
		public async Task<ActionResult> AddTopic(TopicModel model)
		{
			DbManagerResult result = await db.CreateTopic(model);
			if (result == DbManagerResult.Successful)
				Session["Message"] = "Глава успешно добавлена";
			return await GetCourseTopicFile(model.CourseId, true);
		}

		[HttpPost]
		[Authorize(Roles = "teacher")]
		public async Task<ActionResult> UploadFileToTheCourseTopic(UploadFileToTheCourse model)
		{
			if (model.IsRandom)
			{
				await db.CreateTopicFileRandom(model);
			}
			else
			{
				await db.CreateTopicFile(model);
			}
			ViewBag.UpdateEvents = true;
			return await GetCourseTopicFile(model.CourseId, true);
		}

		[HttpPost]
		[Authorize(Roles = "teacher")]
		public async Task<ActionResult> DeleteFile(DeleteFileModel model)
		{
			await db.DeleteFile(model.FileId);
			Session["Message"] = "Файл успешно удален";
			ViewBag.UpdateEvents = true;
			return await GetCourseTopicFile(model.CourseId, true);
		}

		[HttpPost]
		[Authorize(Roles = "teacher")]
		public async Task<ActionResult> DeleteFileFromCourseTopic(int CourseTopicFileId, int CourseId)
		{
			await db.DeleteCourseTopicFile(CourseTopicFileId);
			ViewBag.UpdateEvents = true;
			return await GetCourseTopicFile(CourseId, true);
		}

		[HttpPost]
		[Authorize(Roles = "teacher")]
		public async Task<ActionResult> DeleteCourseTopic(DeleteCourseTopicModel model)
		{
			await db.DeleteCourseTopic(model.CourseTopicId);
			Session["Message"] = "Глава успешно удалена";
			ViewBag.UpdateEvents = true;
			return await GetCourseTopicFile(model.CourseId, true);
		}

		[HttpPost]
		[Authorize(Roles = "teacher")]
		public async Task<ActionResult> UpdateTeacherFileList(int FileTypeId, int CourseId)
		{
			IEnumerable<Models.File> files = await db.GetFilesAsync();
			List<FilesLocation> filesLocations = await db.GetFileLocationsAsync();
			ViewBag.FileLocations = db.GetFileLocationsByCourseId(CourseId).Select(x => new FileLocationVM(x));
			ViewBag.LabTypes = db.GetFileTypesLab();

			if (FileTypeId == 0)
				return PartialView("Courses/_FileList", files.Where(f => filesLocations.Any(l => l.FileId == f.FileId)));
			else
				return PartialView("Courses/_FileList", files.Where(f => f.FileTypeId == FileTypeId));
		}

		[HttpPost]
		[Authorize(Roles = "teacher")]
		public async Task<String> UpdateCourseTopicVisibility(int CourseId, int CourseTopicId)
		{
			bool IsHidden = await db.UpdateCourseTopicVisibilityAsync(CourseId, CourseTopicId);
			return IsHidden ? "visibility_off" : "visibility";
		}

		[Authorize(Roles = "teacher")]
		public async Task<ActionResult> ExportData(int CourseId, int CourseTopicFileId)
		{
			bool IsTeacher = await db.IsTeacherByEmailAsync(User.Identity.Name);
			if (!IsTeacher)
				return RedirectToAction("Error");
			bool IsAccess = await db.HaveTeacherAccessToCourseByEmailAsync(User.Identity.Name, CourseId);
			if (!IsAccess)
				return RedirectToAction("Error");

			ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
			using (ExcelPackage package = new ExcelPackage())
			{
				ExcelWorksheet ws = package.Workbook.Worksheets.Add("Statistics");

				List<StatisticVM> statistics = (await db.GetStatisticsByCourseTopicFileId(CourseTopicFileId)).Select(x => new StatisticVM(x)).ToList();
				foreach (StatisticVM item in statistics)
					item.Update();

				int rows = statistics.Count + 1;
				ws.Cells["A1"].LoadFromDataTable(ToDataTable(statistics), true);
				ws.Cells[$"A1:F{rows}"].AutoFitColumns();
				ws.Cells[$"A1:F{rows}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

				ws.Cells["A1:F1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
				ws.Cells["A1:F1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 225, 242));
				ws.Cells["A1:F1"].Style.Font.Bold = true;
				ws.Cells[$"A1:F{rows}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				ws.Cells[$"A1:F{rows}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
				ws.Cells[$"A1:F{rows}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
				ws.Cells[$"A1:F{rows}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
				ws.Cells[$"A1:F{rows}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

				MemoryStream stream = new MemoryStream();
				package.SaveAs(stream);

				string fileName = "Статистика";
				StatisticVM statistic = statistics.FirstOrDefault();
				if (statistic != null)
					fileName = $"{statistic.Course.Name} ({statistic.CourseTopic.SectionName} - {(statistic.CourseTopicFile.IsVariant ? statistic.CourseTopicFile.Name : statistic.File.Name)}) - Статистика.xlsx";

				string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

				stream.Position = 0;
				return File(stream, contentType, fileName);
			}
		}

		private DataTable ToDataTable(List<StatisticVM> data)
		{
			DataTable table = new DataTable();
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(List<UserCourseFileVM>));
			table.Columns.Add("Фамилия");
			table.Columns.Add("Имя");
			table.Columns.Add("Курс");
			table.Columns.Add("Тема курса");
			table.Columns.Add("Файл");
			table.Columns.Add("Статус");
			foreach (StatisticVM item in data)
			{
				DataRow row = table.NewRow();
				row["Фамилия"] = String.IsNullOrWhiteSpace(item.User.LastName) ? "" : item.User.LastName;
				row["Имя"] = String.IsNullOrWhiteSpace(item.User.FirstName) ? "" : item.User.FirstName;
				row["Курс"] = item.Course.Name;
				row["Тема курса"] = item.CourseTopic.SectionName;
				row["Файл"] = item.File.Name;
				row["Статус"] = item.PerformedAt == null ? "Не пройдено" : item.PerformedAt.Value.ToString("d MMMM в HH:mm", new System.Globalization.CultureInfo("ru-RU"));
				table.Rows.Add(row);
			}

			return table;
		}
	}
}