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
			var project = await _repository.Get<ProjectEntity>().Include(p => p.Commentaries).FirstOrDefaultAsync(p => p.Id == projectId);
			var comOwner = await _repository.Get<ApplicationUserEntity>().FirstOrDefaultAsync(u => u.Id == commentary.CommentOwner.Id);

			var commentToAdd = new CommentaryEntity 
			{
				Text = commentary.Text,				
				CommentDate = DateTime.Now,
				Project = project,
				CommentOwner = comOwner			
			};

			if(commentary.AttachedFile != null)
			{
				var attachedFile = await _repository.Get<UserFileEntity>().FirstOrDefaultAsync(f => f.Id == commentary.AttachedFile.Id);
				commentToAdd.AttachedFile = attachedFile;
				project.CurrentFile = attachedFile;
			}

			project.Commentaries.Add(commentToAdd);

			await _repository.SaveChangesAsync();
		}

		public async Task AddProject(ProjectModel project)
		{
			var projectFile = await _repository.Get<UserFileEntity>().FirstOrDefaultAsync(f => f.Id == project.CurrentFile.Id);
			var projectOwner = await _repository.Get<ApplicationUserEntity>().FirstOrDefaultAsync(u => u.Id == project.ProjectOwner.Id);
			var visitors = new List<ApplicationUserEntity> { projectOwner };

			var projectToAdd = new ProjectEntity 
			{
				Name = project.Name,
				CurrentFile = projectFile,
				ProjectOwner = projectOwner,
				Visitors = visitors			
			};

			await _repository.Add<ProjectEntity>(projectToAdd);

			CommentaryEntity comment = new CommentaryEntity
			{
				Text = "Initial file",
				AttachedFile = projectFile,
				CommentOwner = projectOwner,
				CommentDate = DateTime.Now,
				Project = projectToAdd
			};

			await _repository.Add<CommentaryEntity>(comment);			
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
			var project = await _repository.Get<ProjectEntity>().Include(p => p.Visitors).FirstOrDefaultAsync(p => p.Id == projectId);
			var projectModel = _mapper.Map<ProjectModel>(project);

			return projectModel;
		}

		public async Task<ProjectViewModel> GetProjectView(int projectId, string userId)
		{
			ProjectViewModel viewProject = new ProjectViewModel();

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

		public async Task<UserProjectsViewModel> GetUserProjects(string userId)
		{
			var viewUserProjects = new UserProjectsViewModel();

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

			personalProjects = personalProjects.OrderByDescending(p => p.Commentaries.LastOrDefault()?.CommentDate).ToList();

			var mappedPersProjects = personalProjects.Select(p => _mapper.Map<ProjectModel>(p)).ToList();

			viewUserProjects.InvitedProjects = mappedProjects;
			viewUserProjects.PersonalProjects = mappedPersProjects;

			return viewUserProjects;
		}
	}
}
