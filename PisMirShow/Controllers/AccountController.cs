using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using PisMirShow.Models;
using PisMirShow.ViewModels;

namespace PisMirShow.Controllers
{
    public class AccountController : BaseController
    {
        private readonly PisDbContext _dbContext;

        public AccountController(PisDbContext dbContext, IHostingEnvironment env, IToastNotification toastNotification) : base(dbContext, env, toastNotification)
        {
	        _dbContext = dbContext;
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
	            User user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);

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
                User user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    // добавляем пользователя в бд
                    _dbContext.Users.Add(new User
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
                    await _dbContext.SaveChangesAsync();

                    await Authenticate(model.Login); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            User user = await _dbContext.Users.Include(u => u.Role).AsNoTracking().FirstOrDefaultAsync(u => u.Login == userName);
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
				new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name)
			};
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}