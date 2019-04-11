using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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

        public string GetFullName()
        {
            return FirstName + LastName;
        }
    }


}
