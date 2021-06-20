using DocumentEditing.Domain.Contracts.Models;
using DocumentEditing.Domain.Contracts.ViewModels;
using System.Threading.Tasks;

namespace DocumentEditing.Domain.Contracts
{
	public interface IProjectService
	{
		Task AddProject(ProjectModel project);

		Task<ViewUserProjectsModel> GetUserProjects(string userId);

		Task<ProjectModel> GetProject(int projectId);

		Task FinishProject(int projectId);

		Task AddUserToProject(int projectId, string userId);

		Task AddCommentaryToProject(CommentaryModel commentary, int projectId);

		Task<ViewProjectModel> GetProjectView(int projectId, string userId);
	}
}
