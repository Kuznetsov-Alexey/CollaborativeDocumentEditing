using AutoMapper;
using DocumentEditing.DAL.Contracts.Enteties;
using DocumentEditing.Domain.Contracts;
using DocumentEditing.Domain.Contracts.Models;
using DocumentEditing.Domain.Contracts.ViewModels;
using DocumentEditing.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
	
	[Authorize]
	public class ProjectController : Controller
	{
		private readonly ILogger<ProjectController> _logger;
		private readonly IInviteSenderService _inviteSender;
		private readonly IMyUserManager _myManager;
		private readonly IFileManagerService _fileManager;
		private readonly IProjectService _projectService;
		private readonly UserManager<ApplicationUserEntity> _userManager;

		public ProjectController(ILogger<ProjectController> logger, IProjectService projectService, UserManager<ApplicationUserEntity> userManager, IMyUserManager myManager, IFileManagerService fileManager,
			IInviteSenderService inviteSender)
		{
			_inviteSender = inviteSender;
			_myManager = myManager;
			_fileManager = fileManager;
			_logger = logger;
			_projectService = projectService;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index()
		{
			var myUser = await _myManager.GetUserByClaimsAsync(User);
			//var myUser = await _myManager.GetUserByEmail("vlad@mail.ru");
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
			var currentUser = await _myManager.GetUserByClaimsAsync(User);
			//var currentUser = await _myManager.GetUserByEmail("vlad@mail.ru");

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

		[HttpGet]
		public IActionResult AddProject()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> AddProject(AddProjectModel model)
		{
			var currentUser = await _myManager.GetUserByClaimsAsync(User);
			//var currentUser = await _myManager.GetUserByEmail("vlad@mail.ru");

			//var currentUser = await _myManager.GetUserByClaimsAsync(User);

			if (!ModelState.IsValid)
			{
				return View(model);
			}
			
			var attachedFile = await _fileManager.UploadFile(model.UploadedFile);

			//create new project and save in database
			ProjectModel project = new ProjectModel
			{
				Name = model.ProjectName,
				ProjectOwner = currentUser,				
				CurrentFile = attachedFile,
			};

			await _projectService.AddProject(project);

			//redirect to overall projects page
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		public async Task<IActionResult> AddCommentaryToProject(AddCommentaryToProjectModel commentaryModel)
		{
			var currentUser = await _myManager.GetUserByClaimsAsync(User);
			//var currentUser = await _myManager.GetUserByEmail("vlad@mail.ru");
			var project = await _projectService.GetProject(commentaryModel.ProjectId);

			if (!project.Visitors.Any(v => v.Id == currentUser.Id) || project.IsProjectFinished)
			{
				return RedirectToAction(nameof(ViewProject), new { projectId = commentaryModel.ProjectId });
			}

			//create commentary object
			var comment = new CommentaryModel
			{
				Text = commentaryModel.Text,
				CommentOwner = currentUser
			};

			if (commentaryModel.AttachedFile != null)
			{
				var userFile = await _fileManager.UploadFile(commentaryModel.AttachedFile);
				comment.AttachedFile = userFile;
			}

			await _projectService.AddCommentaryToProject(comment, commentaryModel.ProjectId);
			return RedirectToAction(nameof(ViewProject), new { projectId = commentaryModel.ProjectId });

		}

		[HttpGet]
		public async Task<IActionResult> FinishProject(int projectId)
		{
			//get current user
			var currentUser = await _myManager.GetUserByClaimsAsync(User);
			//var currentUser = await _myManager.GetUserByEmail("vlad@mail.ru");
			//var currentUser = await _userManager.GetUserAsync(User);
			var project = await _projectService.GetProject(projectId);

			if (project.ProjectOwner.Id == currentUser.Id)
			{
				await _projectService.FinishProject(projectId);
			}

			return RedirectToAction(nameof(ViewProject), new { projectId = projectId });
		}

		#region Add new member to project

		//------------       Add new member to project    ------------//

		/// <summary>
		/// Show page for adding user to project
		/// </summary>
		/// <param name="projectId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> AddUserToProject(int projectId)
		{
			var currentUser = await _myManager.GetUserByClaimsAsync(User);
			//var currentUser = await _myManager.GetUserByEmail("vlad@mail.ru");
			//var currentUser = await _userManager.GetUserAsync(User);
			var project = await _projectService.GetProject(projectId);

			//check user, he must be owner of project
			if (project == null || project.ProjectOwner.Id != currentUser.Id)
			{
				return RedirectToAction(nameof(Index));
			}

			AddUserToProjectModel model = new AddUserToProjectModel
			{
				ProjectId = project.Id
			};

			return View(model);
		}

		/// <summary>
		/// POST adding user to project by model
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> AddUserToProject(AddUserToProjectModel model)
		{
			

			var currentUser = await _myManager.GetUserByClaimsAsync(User);
			//var currentUser = await _myManager.GetUserByEmail("vlad@mail.ru");
			//get neccessary data from DB
			var project = await _projectService.GetProject(model.ProjectId);

			//for adding another persons, user must be a project owner
			if (project == null || project.ProjectOwner.Id != currentUser.Id)
			{
				return RedirectToAction(nameof(ViewProject), new { projectId = model.ProjectId });
			}

			var user = await _myManager.GetUserByEmail(model.UserEmail);

			//create user if he doesn't exist
			if (user == null)
			{
				string userPassword = _myManager.GeneratePassword(5);


				user = new ApplicationUserModel
				{
					Email = model.UserEmail,
					UserName = model.UserEmail,
					Password = userPassword

				};

				await _myManager.CreateUser(user);

				//Invite sending don't work, because current email is blocked by @mail.ru,
				//but you can see method realization
				//await _inviteSender.SendInvite(user.Email, userPassword, project.Name);
			}

			await _projectService.AddUserToProject(project.Id, user.Id);

			return RedirectToAction(nameof(ViewProject), new { projectId = model.ProjectId });
		}

		#endregion


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new DocumentEditing.Web.Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
