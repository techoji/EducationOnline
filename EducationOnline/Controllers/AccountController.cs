using DistanceLearning.Models;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;
using System.Net;

namespace DistanceLearning.Controllers {
    [Authorize]
    public class AccountController : Controller {

        private DbManager db;

        public AccountController() => db = new DbManager();

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Pages");
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl) {
            if (!ModelState.IsValid)
                return View(model);

            User user = await db.GetUserAsync(model.Email, model.Password);
            if (user != null) {
                FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);
                Session["user"] = new UserVM {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Photo = user.Photo
                };
                return RedirectToAction("Index", "Pages");
            }
            else {
                ModelState.AddModelError("", "Неудачная попытка входа.");
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<bool> AuthUser(string Email) {
            User user = await db.GetUserByEmailAsync(Email);
            Session["user"] = new UserVM {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Photo = user.Photo
            };
            FormsAuthentication.SetAuthCookie(Email, false);
            return true;
        }

        // GET: /Account/Logout
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Logout() {
            FormsAuthentication.SignOut();
            Session["user"] = null;
            return RedirectToAction("Index", "Pages");
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register() => View();

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model) {
            if (ModelState.IsValid) {
                DbManagerResult result = await db.CreateUser(model);
                if (result == DbManagerResult.Successful)
                    return View("Login", new LoginViewModel { Email = model.Email });
                else if (result == DbManagerResult.Exist) {
                    ModelState.AddModelError("Email", "Такая почта уже существует.");
                    return View(model);
                }
            }
            return View(model);
        }

        public async Task<ActionResult> Profile() {
            User user = await db.GetUserByEmailAsync(User.Identity.Name);
            ViewBag.Groups = await db.GetGroupsByUserEmailAsync(User.Identity.Name);
            ViewBag.Courses = await db.GetCoursesByUserEmailAsync(User.Identity.Name);
            ViewBag.Roles = await db.GetRoleByUserEmailAsync(User.Identity.Name);
            ViewBag.ShowCourseEvents = true;
            return View(user);
        }

        protected override void Dispose(bool disposing) {
            if (disposing && db != null) {
                db.Dispose();
                db = null;
            }

            base.Dispose(disposing);
        }

        //admin controller
        [HttpPost]
        [AllowAnonymous]
        public async Task<bool> GoogleAuth(string FirstName, string LastName, string Email, string Photo) {
            bool IsAuth = await db.GoogleAuthUserAsync(FirstName, LastName, Email, Photo);
            if (IsAuth)
                Session["user"] = new UserVM {
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Photo = Photo
                };

            return IsAuth;
        }
    }
}