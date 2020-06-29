using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.WebEncoders;
using Newtonsoft.Json;
using NToastNotify;
using PisMirShow.Services;
using PisMirShow.SignalR;

namespace PisMirShow
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddRazorPages()
				.AddRazorRuntimeCompilation();

			services.AddEntityFrameworkNpgsql().AddDbContext<PisDbContext>(opt =>
				opt.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

			//services.AddDbContext<PisDbContext>(options =>
			//	options
			//		.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
			//		);

			services.Configure<WebEncoderOptions>(options =>
			{
				options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
			});

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					options.LoginPath = new PathString("/Account/Login");
					options.ReturnUrlParameter = "redirectUrl";
					options.AccessDeniedPath = new PathString("/Home/Index");
				});

			services.Configure<CookiePolicyOptions>(options =>
			{
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});
			services.AddSignalR();

			services.AddSingleton<FileService>();

			services.AddMvc() 
				.AddNewtonsoftJson(options =>
				{
					options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				})
				.AddNToastNotifyToastr(new ToastrOptions
				{
					ProgressBar = false,
					PositionClass = ToastPositions.TopRight
				});
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseNToastNotify();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					"default",
					"{controller=Home}/{action=Index}/{id?}");

				endpoints.MapControllerRoute(
					"Home",
					"{action=Index}/{id?}",
					new { controller = "Home" });

				endpoints.MapHub<ChatHub>("/chat-hub");
				endpoints.MapHub<Dialogs>("/dialog-hub");
			});
		}
	}
}
