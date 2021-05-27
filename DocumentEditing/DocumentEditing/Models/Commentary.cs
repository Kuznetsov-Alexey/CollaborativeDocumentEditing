using DocumentEditing.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentEditing.Models
{
	/// <summary>
	/// Represantation of commentary object
	/// </summary>
	public class Commentary
	{
		//comment ID
		public int Id { get; set; }

		//field for link between tables
		public int ProjectId { get; set; }

		//project what comment belong to
		public Project Project { get; set; }

		//field for link between tables
		public string CommentOwnerId { get; set; }

		//commentary owner
		public ApplicationUser CommentOwner { get; set; }

		//commentary's text
		public string Text { get; set; }

		//field for link between tables
		public int? AttachedFileId { get; set; } 

		//file attached to commentary
		public UserFile AttachedFile { get; set; }

		//time of creation of comment
		public DateTime CommentDate { get; set; }

	}
}
