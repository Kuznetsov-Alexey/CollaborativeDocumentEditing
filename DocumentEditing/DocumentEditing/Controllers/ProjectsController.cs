using DocumentEditing.Areas.Identity.Data;
using DocumentEditing.Areas.Interfaces;
using DocumentEditing.Areas.Services;
using DocumentEditing.Data;
using DocumentEditing.Models;
using DocumentEditing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace DocumentEditing.Controllers
{	
	[Authorize]
	public class ProjectsController : Controller
	{
		private UserManager<ApplicationUser> _userManager;		
		private readonly IProject _projectManager;
		private readonly IInviteSender _inviteSender;
		private readonly IFileManager _fileManager;

		public ProjectsController(UserManager<ApplicationUser> userManager,									
									IProject projectManager,
									IInviteSender inviteSender,
									IFileManager fileManager)
		{
			_userManager = userManager;					
			_projectManager = projectManager;
			_inviteSender = inviteSender;
			_fileManager = fileManager;
		}

		/// <summary>
		/// Represent all available projects for current user
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> Index()
		{
			var currentUser = await _userManager.GetUserAsync(User);
			var userProjects = await _projectManager.GetUserProjects(currentUser.Id);

			return View(userProjects);
		}


		//------------       Add new member to project    ------------//



		/// <summary>
		/// Show page for adding user to project
		/// </summary>
		/// <param name="projectId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> AddUserToProject(int projectId)
		{ 
			var currentUser = await _userManager.GetUserAsync(User);
			var project = await _projectManager.GetProject(projectId);			

			//check user, he must be owner of project
			if (project.ProjectOwner != currentUser)
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
			var currentUser = await _userManager.GetUserAsync(User);

			//get neccessary data from DB
			var project = await _projectManager.GetProject(model.ProjectId);			

			//for adding another persons, user must be a project owner
			if(project.ProjectOwner != currentUser)
			{
				return RedirectToAction(nameof(ViewProject), new { projectId = model.ProjectId });
			}

			//todo: add identity.options.User.Require.UniqueEmail
			var user = await _userManager.FindByEmailAsync(model.UserEmail);

			//create user if he doesn't exist
			if(user == null)
			{
				user = new ApplicationUser
				{
					Email = model.UserEmail,
					UserName = model.UserEmail
				};

				//todo:check migration with new method in ApplicationUser
				string userPassword = user.GeneratePassword(5);				

				await _userManager.CreateAsync(user, userPassword);


				//Sending isn't working, because current email is blocked by @mail.ru
				//await _inviteSender.SendInvite(user.Email, userPassword, project.Name);
			}

			await _projectManager.AddUserToProject(project.Id, user.Email);

			return RedirectToAction(nameof(ViewProject), new { projectId = model.ProjectId });
		}

		//------------       Download file from page    ------------//

		[HttpGet]
		public async Task<IActionResult> DownloadFile(int fileId)
		{
			var fileResult = await _fileManager.GetFileResult(fileId);

			if(fileResult == null)
			{
				return new NotFoundResult();
			}

			return fileResult;
		}

		//------------  Create new project ------------//

		[HttpGet]
		public IActionResult AddProject()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> AddProject(AddProjectModel model)
		{
			var currentUser = await _userManager.GetUserAsync(User);

			if (!ModelState.IsValid)
			{
				return View(model);
			}
			
			var attachedFile = await _fileManager.UploadFile(model.UploadedFile);

			//create new project and save in database
			Project project = new Project
			{
				Name = model.Name,
				ProjectOwner = currentUser,
				Visitors = new List<ApplicationUser> { currentUser },
				CurrentFile = attachedFile
			};			

			Commentary comment = new Commentary
			{
				Text = "Initial file",
				AttachedFile = attachedFile,
				CommentDate = DateTime.Now,
				CommentOwner = currentUser,
				Project = project
			};

			await _projectManager.AddProject(project, comment);

			//redirect to overall projects page
			return RedirectToAction(nameof(Index));			
		}


		//------------  Show and edit project ------------//

		/// <summary>
		/// Show project's data
		/// </summary>
		/// <param name="projectId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> ViewProject(int projectId)
		{		
			//todo: change buttons style on view
			var currentUser = await _userManager.GetUserAsync(User);
			var viewModel = await _projectManager.GetProjectView(projectId, currentUser.Id);

			if(!viewModel.Project.Visitors.Contains(currentUser))
			{
				return RedirectToAction(nameof(Index));
			}
			
			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> AddCommentaryToProject(string commentText, IFormFile attachedFile, int projectId)
		{
			var currentUser = await _userManager.GetUserAsync(User);
			var project = await _projectManager.GetProject(projectId);

			if(!project.Visitors.Contains(currentUser))
			{
				return RedirectToAction(nameof(ViewProject), new { projectId = projectId });
			}

			//create commentary object
			var comment = new Commentary { 
				CommentDate = DateTime.Now,
				Text = commentText,
				ProjectId = projectId,
				CommentOwner = currentUser
			};

			if(attachedFile != null)
			{
				var userFile = await _fileManager.UploadFile(attachedFile);
				comment.AttachedFile = userFile;
			}

			await _projectManager.AddCommentaryToProject(comment, projectId);			
			return RedirectToAction(nameof(ViewProject), new { projectId = projectId });			
		}

		/// <summary>
		/// Close project for editing
		/// </summary>
		/// <param name="projectId">Id of project to close</param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> FinishProject(int projectId)
		{
			//get current user
			var currentUser = await _userManager.GetUserAsync(User);
			var project = await _projectManager.GetProject(projectId);

			if(project.ProjectOwnerId == currentUser.Id)
			{
				await _projectManager.FinishProject(projectId, currentUser.Id);
			}

			return RedirectToAction(nameof(ViewProject), new { projectId = projectId });
		}
	}
}
