using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using PisMirShow.Models.Account;
using PisMirShow.Services;
using PisMirShow.ViewModels;

namespace PisMirShow.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(PisDbContext dbContext, IWebHostEnvironment env, IToastNotification toastNotification) : base(dbContext, env, toastNotification)
        {
        }

        [HttpGet]
        public IActionResult Login(string redirectUrl)
        {
            ViewBag.RedirectURL = redirectUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string redirectURL)
        {
	        if (!ModelState.IsValid) return View(model);
	        try
            {
	            User user = await DbContext.Users.FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);

	            if (user != null)
	            {
		            await Authenticate(model.Login); // аутентификация
                    if (string.IsNullOrEmpty(redirectURL))
		                return RedirectToAction("Index", "Home");

                    return Redirect(redirectURL);
	            }
            }
            catch {
	            ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await DbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    if (model.Password == model.ConfirmPassword){
                        // добавляем пользователя в бд
                        await DbContext.Users.AddAsync(new User
                        {
	                        Email = model.Email,
	                        Password = model.Password,
	                        RegisterTime = DateTime.UtcNow,
	                        BirthdayDay = model.BirthdayDay,
	                        FirstName = model.FirstName,
	                        LastName = model.LastName,
	                        Department = model.Department,
	                        Login = model.Login,
	                        OfficePost = model.OfficePost,
	                        RoleId = 3,
	                        Phone = model.Phone
                        });
                        await DbContext.SaveChangesAsync();

                        await Authenticate(model.Login); // аутентификация

                        return RedirectToAction("Index", "Home");
                    }
                    ToastNotification.AddErrorToastMessage("Пароли не совпадают");
                }
                ToastNotification.AddErrorToastMessage("Пользователь с таким email уже зарегистрирован");
            }
            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            User user = await DbContext.Users.Include(u => u.Role).AsNoTracking().FirstOrDefaultAsync(u => u.Login == userName);
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
				new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name),
			};
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        private void DeleteFile(string deleteFilePath)
        {
            if (deleteFilePath != string.Empty)
            {
                if (System.IO.File.Exists(HostingEnv.WebRootPath + deleteFilePath))
                {
                    FileInfo file = new FileInfo(HostingEnv.WebRootPath + deleteFilePath);
                }
            }
        }

        private async Task<string> AddFile(IFormFile uploadedFile, string directory)
        {
            if (uploadedFile != null)
            {
                var fileName = $"\\files\\{directory}\\{DateTime.UtcNow.Ticks}{uploadedFile.FileName}";
                string path = HostingEnv.WebRootPath + $@"{fileName}";

                if (!Directory.Exists(HostingEnv.WebRootPath + $@"\files\{directory}\"))
                {
                    Directory.CreateDirectory(HostingEnv.WebRootPath + $@"\files\{directory}\");
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                return fileName.Replace('\\', '/');
            }
            return null;
        }

        [HttpPost]
        public async Task<string> ChangeAvatar(IFormFile file)
        {
			var user = GetCurrentUser();

			//if (!string.IsNullOrEmpty(user.Avatar))
			//{
			//    DeleteFile(user.Avatar);
			//}

			//user.Avatar = await AddFile(file, "avatars");

			//DbContext.SaveChanges();

			//return user.Avatar.Replace('\\', '/');

			var avatar = FileService.UploadFileInBd(file);

			user.AvatarBD = avatar;
			user.Avatar = $"/Account/GetAvatar?id={user.Id}";
			await DbContext.SaveChangesAsync();

            return user.Avatar;
        }

	    [HttpGet]
		public ActionResult GetAvatar(int id)
		{
			var bytes = DbContext.Users.FirstOrDefault(t => t.Id == id)?.AvatarBD;

			if (bytes != null)
			{
				byte[] image = bytes;
				return new FileContentResult(image, "image/jpg");
			}

			return BadRequest();
		}
	}
}