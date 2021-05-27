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

namespace DocumentEditing.Controllers
{	
	[Authorize]
	public class ProjectsController : Controller
	{
		private readonly AuthDbContext _context;

		private UserManager<ApplicationUser> _userManager;
		private readonly IWebHostEnvironment _hostingEnvironment;
		private readonly IProject _projectManager;
		private readonly IInviteSender _inviteSender;
		private readonly IFileManager _fileManager;

		public ProjectsController(AuthDbContext context,
									UserManager<ApplicationUser> userManager,
									IWebHostEnvironment hostingEnvironment,
									IProject projectManager,
									IInviteSender inviteSender,
									IFileManager fileManager)
		{
			_userManager = userManager;
			_hostingEnvironment = hostingEnvironment;
			_context = context;
			_projectManager = projectManager;
			_inviteSender = inviteSender;
			_fileManager = fileManager;
		}

		/// <summary>
		/// Page that represent all available projects for current user
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
		/// Action for adding new project member
		/// </summary>
		/// <param name="projectId">Project to add user</param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> AddUserToProject(int projectId)
		{ 
			var currentUser = await _userManager.GetUserAsync(User);
			var project = await _projectManager.GetProject(projectId);

			//var projectt = await _context.Projects.Where(p => p.Id == projectId).FirstOrDefaultAsync();

			//check user, he must be owner of project
			if (project.ProjectOwnerId != currentUser.Id)
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
		/// Add user to project after post request
		/// </summary>
		/// <param name="model">Model from html form</param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> AddUserToProject(AddUserToProjectModel model)
		{
			var project = await _projectManager.GetProject(model.ProjectId);
			var user = await _userManager.Users.Where(u => u.Email == model.UserEmail).FirstOrDefaultAsync();

			if(user == null)
			{
				string userPassword = GeneratePassword(5);
				await _userManager.CreateAsync(new ApplicationUser { Email = model.UserEmail, UserName = model.UserEmail }, userPassword);
				user = await _userManager.Users.Where(u => u.Email == model.UserEmail).FirstOrDefaultAsync();
				//_inviteSender.SendInvite(user.Email, userPassword, project.Name);				
			}

			await _projectManager.AddUserToProject(project.Id, user.Email);

			return RedirectToAction(nameof(ViewProject), new { projectId = model.ProjectId });
		}

		/// <summary>
		/// Simple password genarator, return password that consists of digit,
		/// get one argue - length of password (4- min, 10 - max)
		/// </summary>
		/// <param name="passwordLenght">lenght of password</param>
		/// <returns></returns>
		private string GeneratePassword(int passwordLenght)
		{
			Random rand = new Random();

			int minLenght = 4;
			int maxLength = 10;

			if (passwordLenght < minLenght)
				passwordLenght = minLenght;

			if (passwordLenght > maxLength)
				passwordLenght = maxLength;

			int botRange = 10;

			for (int i = 2; i < passwordLenght; i++)
				botRange *= 10;

			int topRange = botRange * 10 - 1;

			int value = rand.Next(botRange, topRange);
			return value.ToString();
		}

		

		//------------       Download file from page    ------------//

		/// <summary>
		/// Action for file downloading
		/// </summary>
		/// <param name="fileName">Short file name</param>
		/// <param name="filePath">Full path to file</param>
		/// <returns></returns>
		public async Task<IActionResult> DownloadFile(int userFileId)
		{
			var userFileData = await _fileManager.GetUserFile(userFileId);
			var memory = await _fileManager.GetMemoryStream(userFileId);
						
			////todo: create service for work with files
			FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
						
			provider.TryGetContentType(userFileData.FileName, out string fileType);			

			return File(memory, fileType, userFileData.FileName);
		}

		//------------  Create new project ------------//

		/// <summary>
		/// Show form for creation new project
		/// </summary>
		/// <returns></returns>
		public IActionResult AddProject()
		{
			return View();
		}

		/// <summary>
		/// Post action for creating new project
		/// </summary>
		/// <param name="model">got model from form</param>
		/// <returns></returns>
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
			var currentUser = await _userManager.GetUserAsync(User);
			var viewModel = await _projectManager.GetProjectView(projectId, currentUser.Id);

			//check user in visitors list
			if (viewModel == null)
			{
				return RedirectToAction(nameof(Index));
			}
			
			return View(viewModel);
		}

		/// <summary>
		/// Add commentary to project
		/// </summary>
		/// <param name="userText">Commentary text</param>
		/// <param name="attachedFile">Uploaded file</param>
		/// <param name="projectId">Id of project</param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> AddCommentary(string userText, IFormFile attachedFile, int projectId)
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
				Text = userText,
				ProjectId = projectId,
				CommentOwner = currentUser
			};

			if(attachedFile != null)
			{
				var userFile = await _fileManager.UploadFile(attachedFile);
				comment.AttachedFile = userFile;
			}

			await _projectManager.AddCommentary(comment, projectId);			
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
			await _projectManager.FinishProject(projectId, currentUser.Id);			

			return RedirectToAction(nameof(ViewProject), new { projectId = projectId });
		}
	}
}
