using DocumentEditing.Areas.Identity.Data;
using DocumentEditing.Data;
using DocumentEditing.Models;
using DocumentEditing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

		public ProjectsController(AuthDbContext context,
									UserManager<ApplicationUser> userManager,
									IWebHostEnvironment hostingEnvironment)
		{
			_userManager = userManager;
			_hostingEnvironment = hostingEnvironment;
			_context = context;
		}

		/// <summary>
		/// Page that represent all available projects for current user
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> Index()
		{
			var currentUser = await _userManager.GetUserAsync(User);

			//get all projects where user is invited
			var invitedProjects = _context.Projects.Include(p => p.Commentaries)
											.Where(p => p.Visitors.Contains(currentUser) && p.ProjectOwnerId != currentUser.Id)
											.ToList<Project>();

			//sorting by last commentary date
			var invitedProjectsSorted = invitedProjects.OrderByDescending(p => p.Commentaries.LastOrDefault().CommentDate).ToList();

			//get all projects what belong to user 
			var personalProjects = _context.Projects.Include(p => p.Commentaries)
											.Where(p => p.ProjectOwnerId == currentUser.Id)
											.ToList<Project>();

			//sorting by last commentary date
			var personalProjectsSorted = personalProjects.OrderByDescending(p => p.Commentaries.LastOrDefault().CommentDate).ToList();


			ViewUserProjectsModel model = new ViewUserProjectsModel
			{
				InvitedProjects = invitedProjectsSorted,
				PersonalProjects = personalProjectsSorted
			};

			return View(model);
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
			var project = await _context.Projects.Where(p => p.Id == projectId).FirstOrDefaultAsync();

			//check user, he must be owner of project
			if (project.ProjectOwnerId != currentUser.Id)
			{
				return RedirectToAction(nameof(Index));
			}

			AddUserToProjectModel model = new AddUserToProjectModel();
			model.ProjectId = projectId;

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
			var project = await _context.Projects.Where(project => project.Id == model.ProjectId).FirstOrDefaultAsync();
			var user = await _context.Users.Where(u => u.Email == model.UserEmail).FirstOrDefaultAsync();

			project.Visitors.Add(user);

			_context.SaveChanges();

			return RedirectToAction(nameof(ViewProject), new { projectId = model.ProjectId });
		}

		/// <summary>
		/// Send email with creating new user
		/// </summary>
		/// <param name="model">Model from html form</param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> SendMessageToEmail(AddUserToProjectModel model)
		{
			var currentProject = await _context.Projects.Where(p => p.Id == model.ProjectId).FirstOrDefaultAsync();

			//check of entered email 
			if(!IsEmailCorrectAndUnique(model.UserEmail))
			{
				return RedirectToAction(nameof(AddUserToProject), new { projectId = model.ProjectId });
			}

			//create new user
			ApplicationUser user = new ApplicationUser
			{
				Email = model.UserEmail,
				UserName = model.UserEmail
			};

			//get new password
			string password = GeneratePassword(5);

			//add new user to database
			await _userManager.CreateAsync(user, password);

			//add created user to project
			await AddUserToProject(model);

			//send invite Email
			await Task.Run(() => SendEmail(model.UserEmail, password, currentProject.Name));

			return RedirectToAction(nameof(ViewProject), new { id = model.ProjectId });
		}

		/// <summary>
		/// Simple email checking
		/// </summary>
		/// <param name="email">string to check</param>
		/// <returns></returns>
		private bool IsEmailValid(string email)
		{
			if (email.LastIndexOf("@") == -1)
				return false;

			int firstPos = email.LastIndexOf("@");

			if (email.LastIndexOf(".") < firstPos)
				return false;

			return true;
		}

		/// <summary>
		/// Check email in users database
		/// </summary>
		/// <param name="userEmail">string to search</param>
		/// <returns></returns>
		public IActionResult IsUserExist(string userEmail)
		{
			var matchUser = _context.Users.Where(u => u.Email == userEmail).FirstOrDefault();
			if (matchUser == null)
			{
				return Json(false);
			}

			return Json(true);
		}

		/// <summary>
		/// Check email, by format and existing in database
		/// </summary>
		/// <param name="userEmail">string to check</param>
		/// <returns></returns>
		public bool IsEmailCorrectAndUnique(string userEmail)
		{
			if (!IsEmailValid(userEmail))
			{
				return false;
			}

			var jsResult = ((JsonResult)IsUserExist(userEmail)).Value;

			_ = bool.TryParse(jsResult.ToString(), out bool userExist);

			if(userExist)
			{
				return false;
			}
			else
			{
				return true;
			}			
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

		/// <summary>
		/// Create message and send to email
		/// </summary>
		/// <param name="email">Email to send message</param>
		/// <param name="password">Password that user will have</param>
		/// <param name="projectName">Name of project, where user was invited</param>
		private void SendEmail(string email, string password, string projectName)
		{
			//data of mail.ru account
			//sender email - example_inviter@mail.ru
			//sender password - 123567Zz

			//create MailMessage
			MailAddress sender = new MailAddress("example_inviter@mail.ru", "Ivan");
			MailAddress recipient = new MailAddress(email);
			MailMessage msg = new MailMessage(sender, recipient);			

			//topic of letter
			msg.Subject = "Project invite";

			//link to site's URL
			var request = HttpContext.Request;
			var projectLink = $"{request.Scheme}://{request.Host.ToUriComponent()}";

			//body of message
			msg.Body = $"<h4>You were invited to project - {projectName}</h4>" +
						$"<a href=\"{ projectLink}\">Link to site</a> " +
						$"<p>Your log in data:</p>" +
						$"Email: {email}<br/>" +
						$"Password: {password}";

			msg.IsBodyHtml = true;

			//create SMTP client to sent message with right host data
			SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
			smtp.EnableSsl = true;

			//sender's personal data
			smtp.Credentials = new NetworkCredential("example_inviter@mail.ru", "123567Zz");
			smtp.Send(msg);
		}


		//------------       Download file from page    ------------//

		/// <summary>
		/// Action for file downloading
		/// </summary>
		/// <param name="fileName">Short file name</param>
		/// <param name="filePath">Full path to file</param>
		/// <returns></returns>
		public async Task<IActionResult> DownloadFile(string fileName, string filePath)
		{
			//save file in memory
			var memory = new MemoryStream();
			using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				await stream.CopyToAsync(memory);
			}

			memory.Position = 0;

			return File(memory, GetContentType(filePath), Path.GetFileName(fileName));
		}

		/// <summary>
		/// Get content type to know mime-type
		/// </summary>
		/// <param name="path">File name for searching extension</param>
		/// <returns></returns>
		private string GetContentType(string path)
		{
			//get dictionary of mimetypes
			var types = GetMimeTypes();

			//get file's extension
			var ext = Path.GetExtension(path).ToLowerInvariant();

			//try to get mime-type from dictionary
			string mimeType = null;
			try
			{
				mimeType = types[ext];
			}
			catch
			{
				mimeType = "application/octet-stream";
			}

			//return mime-type that matches with extension
			return mimeType;
		}

		//Get dictionary of mime-types
		private Dictionary<string, string> GetMimeTypes()
		{
			return new Dictionary<string, string>
			{
				{".txt", "text/plain"},
				{".pdf", "application/pdf"},
				{".doc", "application/vnd.ms-word"},
				{".docx", "application/vnd.ms-word"},
				{".xls", "application/vnd.ms-excel"},
				{".xlsx", "application/vnd.openxmlformats"},
				{".html", "text/html"},


				{".png", "image/png"},
				{".jpg", "image/jpeg"},
				{".jpeg", "image/jpeg"},
				{".gif", "image/gif"},
				{".csv", "text/csv"}
			};
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
			//get current user
			var currentUser = _context.Users.Where(u => u.Id == _userManager.GetUserId(User)).FirstOrDefault();

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			//folder for uploading files
			string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "UserFiles");

			//create unique file name
			string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.UploadedFile.FileName;

			//get full path tp file
			string filePath = Path.Combine(uploadsFolder, uniqueFileName);

			//put file in server folder
			await model.UploadedFile.CopyToAsync(new FileStream(filePath, FileMode.Create));

			//create object of uploaded file
			UploadedFile upFile = new UploadedFile
			{
				FileName = model.UploadedFile.FileName,
				PathToFile = filePath,
				FileOwner = currentUser
			};

			//save changes in database
			_context.Files.Add(upFile);
			await _context.SaveChangesAsync();

			//create new project and save in database
			Project project = new Project
			{
				Name = model.Name,
				ProjectOwner = currentUser,
				Visitors = new List<ApplicationUser> { currentUser },
				CurrentFile = upFile
			};
			_context.Projects.Add(project);
			await _context.SaveChangesAsync();

			//add initial commentary with uploaded file
			Commentary comment = new Commentary
			{
				Text = "Initial file",
				AttachedFile = upFile,
				CommentDate = DateTime.Now,
				CommentOwner = currentUser,
				Project = project
			};

			_context.Comments.Add(comment);
			await _context.SaveChangesAsync();

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
		public async Task<IActionResult> ViewProject(int? projectId)
		{
			if (projectId == null)
			{
				return RedirectToAction(nameof(Index));
			}
			//get current user
			var currentUser = await _userManager.GetUserAsync(User);

			//get project by ID
			var project = _context.Projects.Include(p => p.Visitors)
											.Include(p => p.CurrentFile)
											//.Include(p => p.Commentaries)
											.Where(p => p.Id == projectId)
											.FirstOrDefault();

			var comments = await _context.Comments.Include(c => c.AttachedFile)
											.Where(c => c.ProjectId == projectId)
											.OrderByDescending(c => c.CommentDate)
											.ToListAsync();

			//check user in visitors list
			if (project.Visitors.Select(user => user.Id).Where(id => id == currentUser.Id).FirstOrDefault() == null)
			{
				return RedirectToAction(nameof(Index));
			}

			//create view model 
			ViewProjectModel viewModel = new ViewProjectModel
			{				
				Project = project,
				Commentaries = comments
			};

			//is current user project owner
			if(project.ProjectOwnerId == currentUser.Id)
			{
				viewModel.IsOwner = true;
			}
			else
			{
				viewModel.IsOwner = false;
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
			//get current user
			var currentUser = _context.Users.Where(u => u.Id == _userManager.GetUserId(User)).FirstOrDefault();

			//create commentary object
			var comment = new Commentary { 
				CommentDate = DateTime.Now,
				Text = userText,
				ProjectId = projectId,
				CommentOwner = currentUser
			};			

			//if attached file
			if (attachedFile != null)
			{
				//get current project
				var project = await _context.Projects.Where(p => p.Id == projectId).FirstOrDefaultAsync();

				//folder for uploading files
				string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "UserFiles");

				//generate unique file name
				string uniqueFileName = Guid.NewGuid().ToString() + "_" + attachedFile.FileName;

				//get full path to file
				string filePath = Path.Combine(uploadsFolder, uniqueFileName);

				//copy file to folder
				attachedFile.CopyTo(new FileStream(filePath, FileMode.Create));

				//create object of uploaded file
				UploadedFile upFile = new UploadedFile
				{
					FileName = attachedFile.FileName,
					PathToFile = filePath,					
					FileOwner = currentUser
				};

				//save changes in database
				_context.Files.Add(upFile);
				await _context.SaveChangesAsync();

				project.CurrentFile = upFile;
				comment.AttachedFile = upFile;
			}
			//add comment to context
			_context.Comments.Add(comment);
			await _context.SaveChangesAsync();

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

			//get current project
			var project = await _context.Projects.Where(p => p.Id == projectId).FirstOrDefaultAsync();

			//if user isn't project owner - redirect
			if (project.ProjectOwnerId != currentUser.Id)
			{
				return RedirectToAction(nameof(Index));
			}

			//set project status "Finished" and save changes
			project.IsProjectFinished = true;
			_context.SaveChanges();

			return RedirectToAction(nameof(ViewProject), new { projectId = projectId });
		}
	}
}
