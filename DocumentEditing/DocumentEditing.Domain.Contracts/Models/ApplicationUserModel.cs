using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentEditing.Domain.Contracts.Models
{
	public class ApplicationUserModel
	{
		public string Id { get; set; }
		public string Email { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }

		public List<ProjectModel> OwnProjects { get; set; } = new List<ProjectModel>();
		public List<ProjectModel> AvailableProjects { get; set; } = new List<ProjectModel>();
	}
}
