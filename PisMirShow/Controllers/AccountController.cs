using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using PisMirShow.Models;
using PisMirShow.Models.Account;
using PisMirShow.ViewModels;

namespace PisMirShow.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public AccountController(PisDbContext dbContext, IHostingEnvironment env, IToastNotification toastNotification, IHostingEnvironment hostingEnvironment) : base(dbContext, env, toastNotification)
        {
            _hostingEnvironment = hostingEnvironment;

        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
	        if (!ModelState.IsValid) return View(model);
	        try
            {
	            User user = await DbContext.Users.FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);

	            if (user != null)
	            {
		            await Authenticate(model.Login); // аутентификация

		            return RedirectToAction("Index", "Home");
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
                        DbContext.Users.Add(new User
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
                            RoleId = 1,
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
                if (System.IO.File.Exists(_hostingEnvironment.WebRootPath + deleteFilePath))
                {
                    FileInfo file = new FileInfo(_hostingEnvironment.WebRootPath + deleteFilePath);
                }
            }
        }

        private async Task<string> AddFile(IFormFile uploadedFile, string directory)
        {
            if (uploadedFile != null)
            {
                var fileName = $"\\files\\{directory}\\{DateTime.UtcNow.Ticks}{uploadedFile.FileName}";
                string path = _hostingEnvironment.WebRootPath + $@"{fileName}";

                if (!Directory.Exists(_hostingEnvironment.WebRootPath + $@"\files\{directory}\"))
                {
                    Directory.CreateDirectory(_hostingEnvironment.WebRootPath + $@"\files\{directory}\");
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

            if (!string.IsNullOrEmpty(user.Avatar))
            {
                DeleteFile(user.Avatar);
            }

            user.Avatar = await AddFile(file, "avatars");

            DbContext.SaveChanges();

            return user.Avatar.Replace('\\', '/');
        }
    }
}