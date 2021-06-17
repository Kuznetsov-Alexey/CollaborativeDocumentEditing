using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentEditing.Domain.Contracts.Models
{
	public class ApplicationUserModel
	{
		public string Id { get; set; }
		public string Email { get; set; }
		public string HashPassword { get; set; }

		public ICollection<ProjectModel> OwnProjects { get; set; } = new List<ProjectModel>();
		public ICollection<ProjectModel> AvailableProjects { get; set; } = new List<ProjectModel>();
	}
}
