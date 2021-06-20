using AutoMapper;
using DocumentEditing.DAL.Contracts.Enteties;
using DocumentEditing.Domain.Contracts;
using DocumentEditing.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentEditing.Web.Controllers
{
	public class ProjectController : Controller
	{
		private readonly ILogger<ProjectController> _logger;
		private readonly IMyUserManager _myManager;
		private readonly IFileManagerService _fileManager;
		private readonly IProjectService _projectService;
		private readonly UserManager<ApplicationUserEntity> _userManager;

		public ProjectController(ILogger<ProjectController> logger, IProjectService projectService, UserManager<ApplicationUserEntity> userManager, IMyUserManager myManager, IFileManagerService fileManager)
		{
			_myManager = myManager;
			_fileManager = fileManager;
			_logger = logger;
			_projectService = projectService;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index()
		{
			var myUser = await _myManager.GetUserByEmail("vlad@mail.ru");
			var userProjects = await _projectService.GetUserProjects(myUser.Id);

			return View(userProjects);
		}

		/// <summary>
		/// Show project's data
		/// </summary>
		/// <param name="projectId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> ViewProject(int projectId)
		{
			var currentUser = await _myManager.GetUserByEmail("vlad@mail.ru");

			var viewModel = await _projectService.GetProjectView(projectId, currentUser.Id);

			//check user in group of project members
			if (!viewModel.Project.Visitors.Any(v => v.Id == currentUser.Id))
			{
				return RedirectToAction(nameof(Index));
			}

			return View(viewModel);
		}

		[HttpGet]
		public async Task<IActionResult> DownloadFile(int fileId)
		{
			var fileResult = await _fileManager.GetFileResult(fileId);

			if (fileResult == null)
			{
				return new NotFoundResult();
			}

			return fileResult;
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new DocumentEditing.Web.Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
