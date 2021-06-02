using DocumentEditing.Areas.Identity.Data;
using DocumentEditing.Areas.Interfaces;
using DocumentEditing.Areas.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentEditing
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

	
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();

			services.AddTransient<IProject, ProjectRepository>();
			services.AddTransient<IInviteSender, EmailInviteSender>();
			services.AddTransient<IFileManager, LocalFileManager>();
			services.AddHttpContextAccessor();

			services.AddMvc(options =>
			{
				var policy = new AuthorizationPolicyBuilder()
								.RequireAuthenticatedUser()
								.Build();

				options.Filters.Add(new AuthorizeFilter(policy));

			}).AddXmlSerializerFormatters();

			services.AddAuthentication()
				.AddGoogle(options => {

					options.ClientId = "567847066979-sshnp0bg8c9r161p9e4ouji082c0g9o0.apps.googleusercontent.com";
					options.ClientSecret = "fYt_GZebYzsfFW1IAVU9pL_2";
				});
				

			services.AddRazorPages();
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
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Projects}/{action=Index}/{id?}");

				endpoints.MapRazorPages();
			});
		}
	}
}
