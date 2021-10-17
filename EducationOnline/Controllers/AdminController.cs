using DistanceLearning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DistanceLearning.Controllers {

    [Authorize(Roles = "admin")]
    public class AdminController : Controller {

        private DbManager db { get; set; }

        public AdminController() => db = new DbManager();

        // GET: Admin
        [Route("admin/management/courses")]
        public async Task<ActionResult> Courses(String search) {
            ViewBag.Groups = await db.GetGroupsAsync();
            ViewBag.CourseGroups = (await db.GetCourseGroupsAsync()).Select(x => new CourseGroupVM(x));
            if (!String.IsNullOrWhiteSpace(search)) {
                List<Course> courses = await db.SearchCoursesAsync(search);
                ViewBag.Search = search;
                return View(courses);
            }
            return View(await db.GetCoursesAsync());
        }

        [Route("admin/management/groups")]
        public async Task<ActionResult> Groups(String search) {
            ViewBag.Courses = await db.GetCoursesAsync();
            ViewBag.CourseGroups = (await db.GetCourseGroupsAsync()).Select(x => new CourseGroupVM(x));
            if (!String.IsNullOrWhiteSpace(search)) {
                List<Group> groups = await db.SearchGroupsAsync(search);
                ViewBag.Search = search;
                return View(groups);
            }
            return View(await db.GetGroupsAsync());
        }

        [Route("admin/management/users")]
        public async Task<ActionResult> Users(String search) {
            ViewBag.Groups = await db.GetGroupsAsync();
            ViewBag.UserGroups = (await db.GetUserGroupsAsync()).Select(x => new UserGroupVM(x));
            if (!String.IsNullOrWhiteSpace(search)) {
                List<User> users = await db.SearchUsersAsync(search);
                ViewBag.Search = search;
                return View(users);
            }
            return View(await db.GetUsersAsync());
        }

        [Route("admin/management/usergroups/{UserId}")]
        public async Task<ActionResult> UserGroups(int UserId) {
            ViewBag.User = new UserVM(await db.GetUserByIdAsync(UserId));
            ViewBag.Groups = await db.GetGroupsAsync();
            return View((await db.GetUserGroupsByUserEmailAsync(UserId)).Select(x => new UserGroupVM(x)));
        }

        [Route("admin/management/coursegroups/{CourseId}")]
        public async Task<ActionResult> CourseGroups(int CourseId) {
            ViewBag.Course = new CourseVM(await db.GetCourseByIdAsync(CourseId));
            ViewBag.Groups = await db.GetGroupsAsync();
            return View((await db.GetCourseGroupsByCourseIdAsync(CourseId)).Select(x => new CourseGroupVM(x)));
        }

        //group
        [HttpPost]
        public async Task<ActionResult> AddGroup(string GroupName) {
            await db.CreateGroup(GroupName);
            ViewBag.Courses = await db.GetCoursesAsync();
            ViewBag.CourseGroups = (await db.GetCourseGroupsAsync()).Select(x => new CourseGroupVM(x));
            return PartialView("Partial/Tables/_TableGroups", await db.GetGroupsAsync());
        }

        [HttpPost]
        public ActionResult EditGroup(Group Model) {
            return PartialView("Partial/Edit/_EditGroupRow", Model);
        }

        [HttpPost]
        public async Task<ActionResult> CancelGroup(Group Model) {
            ViewBag.Courses = await db.GetCoursesAsync();
            ViewBag.CourseGroups = (await db.GetCourseGroupsAsync()).Select(x => new CourseGroupVM(x));
            return PartialView("Partial/_GroupRow", Model);
        }

        [HttpPost]
        public async Task<ActionResult> SaveGroup(Group Model) {
            await db.UpdateGroupAsync(Model.GroupId, Model.Name);
            ViewBag.Courses = await db.GetCoursesAsync();
            ViewBag.CourseGroups = (await db.GetCourseGroupsAsync()).Select(x => new CourseGroupVM(x));
            return PartialView("Partial/_GroupRow", Model);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteGroup(int GroupId) {
            await db.DeleteGroupAsync(GroupId);
            return null;
        }

        //user
        [HttpPost]
        public ActionResult CancelUser(User model) => PartialView("Partial/_UserRow", model);

        [HttpPost]
        public async Task<ActionResult> AddUser(string Email) {
            await db.CreateUserAsync(Email);
            ViewBag.Groups = await db.GetGroupsAsync();
            ViewBag.UserGroups = (await db.GetUserGroupsAsync()).Select(x => new UserGroupVM(x));
            return PartialView("Partial/Tables/_TableUsers", await db.GetUsersAsync());
        }

        [HttpPost]
        public async Task<ActionResult> DeleteUser(int UserId) {
            await db.DeleteUserAsync(UserId);
            return null;
        }

        //userGroup
        [HttpPost]
        public async Task<ActionResult> EditUserGroup(UserGroupVM Model) {
            ViewBag.Groups = await db.GetGroupsAsync();
            await Model.Update();
            return PartialView("Partial/Edit/_EditUserGroupRow", Model);
        }

        [HttpPost]
        public async Task<ActionResult> SaveUserGroup(UserGroupVM Model, int WasGroupId) {
            await db.UpdateUserGroupAsync(Model.UserId, WasGroupId, Model.GroupId);
            await Model.Update();
            return PartialView("Partial/_UserGroupRow", Model);
        }

        [HttpPost]
        public async Task<ActionResult> CancelUserGroup(UserGroupVM Model) {
            await Model.Update();
            return PartialView("Partial/_UserGroupRow", Model);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteUserGroup(UserGroupVM Model) {
            await db.DeleteUserGroup(Model.Id);
            return null;
        }

        [HttpPost]
        public async Task<ActionResult> AddUserGroup(int UserId, int GroupId) {
            await db.CreateUserGroupAsync(UserId, GroupId);
            ViewBag.User = new UserVM(await db.GetUserByIdAsync(UserId));
            ViewBag.Groups = await db.GetGroupsAsync();
            return PartialView("Partial/Tables/_TableUserGroups", (await db.GetUserGroupsByUserEmailAsync(UserId)).Select(x => new UserGroupVM(x)));
        }

        //course
        [HttpPost]
        public async Task<ActionResult> AddCourse(string Name, string ShortName) {
            await db.CreateCourseAsync(Name, ShortName);
            ViewBag.Groups = await db.GetGroupsAsync();
            ViewBag.CourseGroups = (await db.GetCourseGroupsAsync()).Select(x => new CourseGroupVM(x));
            return PartialView("Partial/Tables/_TableCourses", await db.GetCoursesAsync());
        }

        [HttpPost]
        public ActionResult EditCourse(Course Model) => PartialView("Partial/Edit/_EditCourseRow", Model);

        [HttpPost]
        public async Task<ActionResult> CancelCourse(Course Model) {
            ViewBag.Groups = await db.GetGroupsAsync();
            ViewBag.CourseGroups = (await db.GetCourseGroupsAsync()).Select(x => new CourseGroupVM(x));
            return PartialView("Partial/_CourseRow", Model);
        }

        [HttpPost]
        public async Task<ActionResult> SaveCourse(Course Model) {
            await db.UpdateCourseAsync(Model.CourseId, Model.Name, Model.ShortName);
            ViewBag.Groups = await db.GetGroupsAsync();
            ViewBag.CourseGroups = (await db.GetCourseGroupsAsync()).Select(x => new CourseGroupVM(x));
            return PartialView("Partial/_CourseRow", Model);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteCourse(int CourseId) {
            await db.DeleteCourseAsync(CourseId);
            return null;
        }

        //courseGroup
        [HttpPost]
        public async Task<ActionResult> EditCourseGroup(CourseGroupVM Model) {
            ViewBag.Groups = await db.GetGroupsAsync();
            await Model.Update();
            return PartialView("Partial/Edit/_EditCourseGroupRow", Model);
        }

        [HttpPost]
        public async Task<ActionResult> SaveCourseGroup(CourseGroupVM Model, int WasGroupId) {
            await db.UpdateCourseGroupAsync(Model.CourseId, WasGroupId, Model.GroupId);
            await Model.Update();
            return PartialView("Partial/_CourseGroupRow", Model);
        }

        [HttpPost]
        public async Task<ActionResult> CancelCourseGroup(CourseGroupVM Model) {
            await Model.Update();
            return PartialView("Partial/_CourseGroupRow", Model);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteCourseGroup(CourseGroupVM Model) {
            await db.DeleteCourseGroupAsync(Model.Id);
            return null;
        }

        [HttpPost]
        public async Task<ActionResult> AddCourseGroup(int CourseId, int GroupId) {
            await db.CreateCourseGroupAsync(CourseId, GroupId);
            ViewBag.Course = new CourseVM(await db.GetCourseByIdAsync(CourseId));
            ViewBag.Groups = await db.GetGroupsAsync();
            return PartialView("Partial/Tables/_TableCourseGroups", (await db.GetCourseGroupsByCourseIdAsync(CourseId)).Select(x => new CourseGroupVM(x)));
        }
    }
}