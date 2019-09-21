using System.ComponentModel.DataAnnotations;

namespace PisMirShow.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Неверный или уже существующий Логин")]
        public string Login { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string OfficePost { get; set; }

        public string Department { get; set; }

		[MaxLength(12)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Неверный дата")]
        public string BirthdayDay { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        public string ConfirmPassword { get; set; }
    }
}
