using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DistanceLearning.Models {
    public class UserVM {
        public int Id { get; set; }
        [Display(Name = "Почта")]
        public string Email { get; set; }
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        public string Photo { get; set; }

        public UserVM(User user) {
            Id = user.UserId;
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Password = user.Password;
            Photo = user.Photo;
        }

        public UserVM() { }

        public UserVM(int Id) => this.Id = Id;

        public UserVM GetUser() {
            if (Id != 0)
                using (DbManager db = new DbManager())
                    return new UserVM(db.GetUserById(Id));
            else return null;
        }

        public async Task<UserVM> GetUserAsync() {
            if (Id != 0)
                using (DbManager db = new DbManager())
                    return new UserVM(await db.GetUserByIdAsync(Id));
            else return null;
        }
    }

    public class RoleVM {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public RoleVM(Role role) {
            Id = role.RoleId;
            Name = role.Name;
            Description = role.Description;
        }

        public RoleVM(int Id) => this.Id = Id;

        public RoleVM GetRole() {
            if (Id != 0)
                using (DbManager db = new DbManager())
                    return new RoleVM(db.GetRoleById(Id));
            else return null;
        }

        public async Task<RoleVM> GetRoleAsync() {
            if (Id != 0)
                using (DbManager db = new DbManager())
                    return new RoleVM(await db.GetRoleByIdAsync(Id));
            else return null;
        }
    }

    public class UserRoleVM {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public UserRoleVM(UserRole userRole) {
            Id = userRole.UserRoleId;
            UserId = userRole.UserId;
            RoleId = userRole.RoleId;
        }

        public UserVM GetUser() => new UserVM(UserId).GetUser();
        public async Task<UserVM> GetUserAsync() => await new UserVM(UserId).GetUserAsync();
        public RoleVM GetRole() => new RoleVM(RoleId).GetRole();
        public async Task<RoleVM> GetRoleAsync() => await new RoleVM(RoleId).GetRoleAsync();
    }

    public class GroupVM {
        public int Id { get; set; }
        [Display(Name = "Название группы")]
        public string Name { get; set; }
        public GroupVM Group { get; private set; }

        public GroupVM(Group group) {
            Id = group.GroupId;
            Name = group.Name;
        }

        public GroupVM() { }

        public GroupVM(int Id) => this.Id = Id;

        public GroupVM GetGroup() {
            if (Id != 0)
                using (DbManager db = new DbManager())
                    return new GroupVM(db.GetGroupById(Id));
            else return null;
        }

        public async Task<GroupVM> GetGroupAsync() {
            if (Id != 0)
                using (DbManager db = new DbManager())
                    return new GroupVM(await db.GetGroupByIdAsync(Id));
            else return null;
        }

        public async Task Update() => Group = await GetGroupAsync();
    }

    public class UserGroupVM {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public UserVM User { get; set; }
        public GroupVM Group { get; set; }

        public UserGroupVM() { }

        public UserGroupVM(UserGroup userGroup) {
            Id = userGroup.UserGroupId;
            UserId = userGroup.UserId;
            GroupId = userGroup.GroupId;
        }

        public UserVM GetUser() => new UserVM(UserId).GetUser();
        public async Task<UserVM> GetUserAsync() => await new UserVM(UserId).GetUserAsync();
        public GroupVM GetGroup() => new GroupVM(GroupId).GetGroup();
        public async Task<GroupVM> GetGroupAsync() => await new GroupVM(GroupId).GetGroupAsync();

        public async Task Update() {
            User = await GetUserAsync();
            Group = await GetGroupAsync();
        }
    }

    public class TeacherVM {
        public int TeacherId { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }

        public TeacherVM(Teacher teacher) {
            TeacherId = teacher.TeacherId;
            UserId = teacher.UserId;
            CourseId = teacher.CourseId;
        }
    }
}