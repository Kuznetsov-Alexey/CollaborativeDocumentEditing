using DocumentEditing.DAL.Implementations;
using DocumentEditing.Domain.Contracts.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DocumentEditing.Domain.Contracts;
using DocumentEditing.Domain.Implementations;
using DocumentEditing.DAL.Contracts;
using DocumentEditing.DAL.Contracts.Enteties;

namespace DocumentEditing.Web
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
			services.AddDbContext<MyDbContext>(options =>
			{
				options.UseSqlServer(Configuration.GetConnectionString("AuthDbContextConnection"),
					assembly =>
						assembly.MigrationsAssembly("DocumentEditing.DAL.Implementations"));
			});
			//services.AddIdentity<ApplicationUserModel, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>();

			services.AddIdentity<ApplicationUserEntity, IdentityRole>(options =>
			{
				//turn off email confirming
				options.SignIn.RequireConfirmedAccount = false;
				options.User.RequireUniqueEmail = true;
				

				//change password requirenments 
				options.Password.RequireLowercase = false;
				options.Password.RequireUppercase = false;
				options.Password.RequiredLength = 4;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireDigit = false;

			}).AddEntityFrameworkStores<MyDbContext>();

			services.AddControllersWithViews();

			services.AddAutoMapper(typeof(Startup));

			services.AddScoped<IDbRepository, DbRepository>();

			services.AddTransient<IInviteSenderService, InviteSenderService>();
			services.AddTransient<IFileManagerService, FileManagerService>();
			services.AddTransient<IMyUserManager, MyUserManager>();			
			services.AddTransient<IProjectService, ProjectService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseDeveloperExceptionPage();
			app.UseHttpsRedirection();

			app.UseStaticFiles();
			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "project",
					pattern: "{controller=Project}/{action=Index}/{id?}");
			});
		}
	}
}
