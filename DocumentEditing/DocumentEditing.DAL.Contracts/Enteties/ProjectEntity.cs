using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentEditing.DAL.Contracts.Enteties
{
	/// <summary>
	/// Represantation of project object
	/// </summary>
	public class ProjectEntity
	{	
		public  int Id { get; set; }

		//project name
		[Column(TypeName="nvarchar(200)")]
		public string Name { get; set; }

		//project status
		public bool IsProjectFinished { get; set; }
				
		//project owner
		public ApplicationUserEntity ProjectOwner { get; set; }
		public string ProjectOwnerId { get; set; }

		//last uploaded file to project
		public UserFileEntity CurrentFile { get; set; }
		public int CurrentFileId { get; set; }

		//collection of users, who can visit page of project
		public ICollection<ApplicationUserEntity> Visitors { get;set; } = new List<ApplicationUserEntity>();

		//collection of commentaries
		public ICollection<CommentaryEntity> Commentaries { get; set; } = new List<CommentaryEntity>();

	}
}
