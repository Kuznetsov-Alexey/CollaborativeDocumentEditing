using AutoMapper;
using DocumentEditing.DAL.Contracts.Enteties;
using DocumentEditing.Domain.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentEditing.Web.Profiles
{
	public class LeadProfile : Profile
	{
		public LeadProfile()
		{
			CreateMap<ApplicationUserEntity, ApplicationUserModel>();
			CreateMap<ApplicationUserModel, ApplicationUserEntity>();

			CreateMap<CommentaryEntity, CommentaryModel>();
			CreateMap<CommentaryModel, CommentaryEntity>();

			CreateMap<ProjectEntity, ProjectModel>();
			CreateMap<ProjectModel, ProjectEntity>();

			CreateMap<UserFileEntity, UserFileModel>();
			CreateMap<UserFileModel, UserFileEntity>();
		}
	}
}
