using DocumentEditing.Models;
using DocumentEditing.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentEditing.Areas.Interfaces
{
	public interface IProject
	{
		Task AddProject(Project project, Commentary commentary);

		Task<ViewUserProjects> GetUserProjects(string userId);

		Task<Project> GetProject(int projectId);

		Task FinishProject(int projectId, string userId);

		Task AddUserToProject(int projectId, string userId);

		Task AddCommentary(Commentary commentary, int projectId);

		Task<ViewProject> GetProjectView(int projectId, string userId);
	}
}
