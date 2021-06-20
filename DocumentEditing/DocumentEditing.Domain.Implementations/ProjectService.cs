using AutoMapper;
using DocumentEditing.DAL.Contracts;
using DocumentEditing.DAL.Contracts.Enteties;
using DocumentEditing.Domain.Contracts;
using DocumentEditing.Domain.Contracts.Models;
using DocumentEditing.Domain.Contracts.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentEditing.Domain.Implementations
{
	public class ProjectService : IProjectService
	{
		private IDbRepository _repository;
		private IMapper _mapper;

		public ProjectService(IDbRepository repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		public async Task AddCommentaryToProject(CommentaryModel commentary, int projectId)
		{
			var project = await _repository.Get<ProjectEntity>().FirstOrDefaultAsync(p => p.Id == projectId);
			var commentEntity = _mapper.Map<CommentaryEntity>(commentary);

			project.Commentaries.Add(commentEntity);

			if(commentEntity.AttachedFile != null)
			{
				project.CurrentFile = commentEntity.AttachedFile;
			}

			await _repository.SaveChangesAsync();
		}

		public async Task AddProject(ProjectModel project)
		{
			var mappedProject = _mapper.Map<ProjectEntity>(project);

			await _repository.Add(mappedProject);
			await _repository.SaveChangesAsync();
		}

		public async Task AddUserToProject(int projectId, string userId)
		{
			var project = await _repository.Get<ProjectEntity>().FirstOrDefaultAsync(p => p.Id == projectId);
			var user = await _repository.Get<ApplicationUserEntity>().FirstOrDefaultAsync(user => user.Id == userId);

			project.Visitors.Add(user);

			await _repository.SaveChangesAsync();
		}

		public async Task FinishProject(int projectId)
		{
			var project = await _repository.Get<ProjectEntity>().FirstOrDefaultAsync(p => p.Id == projectId);

			project.IsProjectFinished = true;
			await _repository.SaveChangesAsync();
		}

		public async Task<ProjectModel> GetProject(int projectId)
		{
			var project = await _repository.Get<ProjectEntity>().FirstOrDefaultAsync(p => p.Id == projectId);
			var projectModel = _mapper.Map<ProjectModel>(project);

			return projectModel;
		}

		public async Task<ViewProjectModel> GetProjectView(int projectId, string userId)
		{
			ViewProjectModel viewProject = new ViewProjectModel();

			var project = await _repository.Get<ProjectEntity>()
				.Include(p => p.Visitors)
				.Include(p => p.CurrentFile)
				.FirstOrDefaultAsync(p => p.Id == projectId);

			var mappedProject = _mapper.Map<ProjectModel>(project);

			var commentaries = await _repository.Get<CommentaryEntity>()
				.Include(c => c.AttachedFile)
				.Where(c => c.ProjectId == projectId)
				.OrderByDescending(c => c.CommentDate)
				.ToListAsync();

			var mappedComments = commentaries.Select(comment => _mapper.Map<CommentaryModel>(comment)).ToList();

			viewProject.Project = mappedProject;
			viewProject.Commentaries = mappedComments;
			viewProject.IsOwner = project.ProjectOwnerId == userId ? true : false;

			return viewProject;
		}

		public async Task<ViewUserProjectsModel> GetUserProjects(string userId)
		{
			var viewUserProjects = new ViewUserProjectsModel();

			var invitedProject = await _repository.Get<ProjectEntity>()
				.Include(p => p.Commentaries)
				.Where(p => p.Visitors.Any(user => user.Id == userId) && p.ProjectOwnerId != userId)
				.ToListAsync();

			invitedProject = invitedProject.OrderByDescending(p => p.Commentaries.LastOrDefault().CommentDate).ToList();

			var mappedProjects = invitedProject.Select(p => _mapper.Map<ProjectModel>(p)).ToList();


			var personalProjects = await _repository.Get<ProjectEntity>()
				.Include(p => p.Commentaries)
				.Where(p => p.ProjectOwnerId == userId)
				.ToListAsync();

			personalProjects = personalProjects.OrderByDescending(p => p.Commentaries.LastOrDefault().CommentDate).ToList();

			var mappedPersProjects = personalProjects.Select(p => _mapper.Map<ProjectModel>(p)).ToList();

			viewUserProjects.InvitedProjects = mappedProjects;
			viewUserProjects.PersonalProjects = mappedPersProjects;

			return viewUserProjects;
		}
	}
}
