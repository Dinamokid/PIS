using PisMirShow.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PisMirShow.Models
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime RegisterTime { get; set; }
        public DateTime? LastEnter { get; set; }
        public string OfficePost { get; set; }
        public string Department { get; set; }
        public string BirthdayDay { get; set; }
        public string Avatar { get; set; }

        public int? RoleId { get; set; }
        public virtual Role Role { get; set; }

        public virtual List<FileItem> Files { get; set; }

		[MaxLength(12)]
		public string Phone { get; set; }

        public string GetFullName()
        {
            return string.Join(" ", LastName, FirstName).Trim();
        }

        public User GetUserSafe()
        {
            return new User
            {
                Avatar = Avatar,
                BirthdayDay = BirthdayDay,
                Department = Department,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                Id = Id,
                Login = Login,
                OfficePost = OfficePost,
                Phone = Phone,
                RoleId = RoleId,
                Role = Role
            };
        }
    }


}
