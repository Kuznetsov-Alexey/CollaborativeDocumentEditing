using DocumentEditing.Domain.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentEditing.Domain.Contracts.ViewModels
{
	/// <summary>
	/// For showing available project 
	/// </summary>
	public class ViewUserProjectsModel
	{
		//projects that user created
		public List<ProjectModel> PersonalProjects { get; set; }

		//projects where user was invited
		public List<ProjectModel> InvitedProjects { get; set; }
	}
}
