using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MerchShop.Models.DataAccessLayer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using MerchShop.Models.DomainModels;

namespace MerchShop
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
			services.AddControllersWithViews();

			// Add Entity Framework Core DbContext
			services.AddDbContext<MerchShopContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

			// Configure Identity SignInManager
			services.AddIdentity<AppUser, IdentityRole>(options =>
		options.SignIn.RequireConfirmedAccount = false)
				.AddEntityFrameworkStores<MerchShopContext>()
				.AddSignInManager<SignInManager<AppUser>>()
				.AddUserManager<UserManager<AppUser>>()
                .AddDefaultTokenProviders();

			//        services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>,
			//AdditionalUserClaimsPrincipalFactory>();

			// Configure cookie policy
			services.Configure<CookiePolicyOptions>(options =>
			{
				options.MinimumSameSitePolicy = SameSiteMode.Strict;
				options.HttpOnly = HttpOnlyPolicy.Always;
				options.Secure = CookieSecurePolicy.Always;
			});

			// Configure authentication
			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie();

			services.AddAuthorization(options =>
			{
				// Add authorization policies if needed
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

			app.UseRouting();

			app.UseAuthentication(); // Enable authentication
			app.UseAuthorization(); // Enable authorization

			app.UseCookiePolicy(); // Apply cookie policy

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Account}/{action=Login}/{id?}");
			});
		}
	}
}
