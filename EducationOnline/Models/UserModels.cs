using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DistanceLearning.Models {

    [Table("Users")]
    public class User {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Photo { get; set; }
    }

    [Table("Roles")]
    public class Role {
        [Key]
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    [Table("UserRoles")]
    public class UserRole {
        [Key]
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }

    [Table("Groups")]
    public class Group {
        [Key]
        public int GroupId { get; set; }
        public string Name { get; set; }
    }

    [Table("UserGroups")]
    public class UserGroup {
        [Key]
        public int UserGroupId { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }
    }

    [Table("Teachers")]
    public class Teacher {
        [Key]
        public int TeacherId { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
    }
}