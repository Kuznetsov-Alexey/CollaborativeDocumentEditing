using DocumentEditing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentEditing.ViewModels
{
	/// <summary>
	/// For showing available project 
	/// </summary>
	public class ViewUserProjectsModel
	{
		//projects that user created
		public List<Project> PersonalProjects { get; set; }

		//projects where user was invited
		public List<Project> InvitedProjects { get; set; }
	}
}
