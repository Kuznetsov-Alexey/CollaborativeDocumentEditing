using DocumentEditing.Domain.Contracts;
using DocumentEditing.Domain.Contracts.Models;
using DocumentEditing.Domain.Contracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentEditing.Domain.Implementations
{
	class ProjectService : IProjectService
	{
		public Task AddCommentaryToProject(CommentaryModel commentary, int projectId)
		{
			throw new NotImplementedException();
		}

		public Task AddProject(ProjectModel project)
		{
			throw new NotImplementedException();
		}

		public Task AddUserToProject(int projectId, string userId)
		{
			throw new NotImplementedException();
		}

		public Task FinishProject(int projectId, string userId)
		{
			throw new NotImplementedException();
		}

		public Task<ProjectModel> GetProject(int projectId)
		{
			throw new NotImplementedException();
		}

		public Task<ViewProjectModel> GetProjectView(int projectId, string userId)
		{
			throw new NotImplementedException();
		}

		public Task<ViewUserProjectsModel> GetUserProjects(string userId)
		{
			throw new NotImplementedException();
		}
	}
}
