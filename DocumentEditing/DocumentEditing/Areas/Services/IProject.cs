using DocumentEditing.Models;
using DocumentEditing.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentEditing.Areas.Services
{
	public interface IProject
	{
		Task<ViewUserProjects> GetUserProjects(string userId);

		Task<Project> GetProject(int projectId);

		Task AddUserToProject(int projectId, string userId);

		Task<ViewProject> GetProjectView(int projectId, string userId);
	}
}
