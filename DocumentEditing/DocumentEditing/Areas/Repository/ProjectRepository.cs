using DocumentEditing.Areas.Services;
using DocumentEditing.Data;
using DocumentEditing.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DocumentEditing.Areas.Identity.Data;

namespace DocumentEditing.Areas.Repository
{
	public class ProjectRepository : IProject
	{
		AuthDbContext _dbContext;

		public ProjectRepository(AuthDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddUserToProject(int projectId, string userId)
		{
			var project = await _dbContext.Projects.Where(p => p.Id == projectId).FirstOrDefaultAsync();
			var user = await _dbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

			project.Visitors.Add(user);

			await _dbContext.SaveChangesAsync();

		}

		public async Task<Models.Project> GetProject(int projectId)
		{
			return await _dbContext.Projects.Where(p => p.Id == projectId).FirstOrDefaultAsync();
		}

		public async Task<ViewProject> GetProjectView(int projectId, string userId)
		{
			ViewProject viewProject = new ViewProject();

			var project =  await _dbContext.Projects.Include(p => p.CurrentFile)
														.Include(p => p.Visitors)
														.Where(p => p.Id == projectId)
														.FirstOrDefaultAsync();

			var commentaries = await _dbContext.Comments.Include(c => c.AttachedFile)
											.Where(c => c.ProjectId == projectId)
											.OrderByDescending(c => c.CommentDate)
											.ToListAsync();

			viewProject.Project = project;
			viewProject.Commentaries = commentaries;
			viewProject.IsOwner = project.ProjectOwnerId == userId ? true : false;

			return viewProject;
		}

		public async Task<ViewUserProjects> GetUserProjects(string userId)
		{
			

			var invitedProjects = await _dbContext.Projects.Include(p => p.Commentaries)
														.Where(p => p.Visitors.Any(user => user.Id == userId) && p.ProjectOwner.Id != userId)
														.ToListAsync();
			invitedProjects = invitedProjects.OrderByDescending(p => p.Commentaries.LastOrDefault().CommentDate).ToList();

			var personalProjects = await _dbContext.Projects.Include(p => p.Commentaries)
											.Where(p => p.ProjectOwnerId == userId)
											.ToListAsync();

			personalProjects = personalProjects.OrderByDescending(p => p.Commentaries.LastOrDefault().CommentDate).ToList();

			ViewUserProjects viewProjects = new ViewUserProjects
			{
				InvitedProjects = invitedProjects,
				PersonalProjects = personalProjects
			};

			return viewProjects;
		}
	}
}
