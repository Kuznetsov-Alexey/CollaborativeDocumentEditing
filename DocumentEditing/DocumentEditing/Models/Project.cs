using DocumentEditing.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentEditing.Models
{
	/// <summary>
	/// Represantation of project object
	/// </summary>
	public class Project
	{	
		public  int Id { get; set; }

		//project name
		[Column(TypeName="nvarchar(200)")]
		public string Name { get; set; }

		//project status
		public bool IsProjectFinished { get; set; }
				
		//project owner
		public ApplicationUser ProjectOwner { get; set; }
		public string ProjectOwnerId { get; set; }

		//last uploaded file to project
		public UserFile CurrentFile { get; set; }
		public int CurrentFileId { get; set; }

		//collection of users, who can visit page of project
		public ICollection<ApplicationUser> Visitors { get;set; } = new List<ApplicationUser>();

		//collection of commentaries
		public ICollection<Commentary> Commentaries { get; set; } = new List<Commentary>();

	}
}
