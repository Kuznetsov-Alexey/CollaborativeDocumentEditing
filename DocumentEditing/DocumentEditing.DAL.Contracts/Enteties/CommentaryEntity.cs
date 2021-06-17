using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentEditing.DAL.Contracts.Enteties
{
	/// <summary>
	/// Represantation of commentary object
	/// </summary>
	public class CommentaryEntity
	{		
		public int Id { get; set; }

		//project what comment belong to
		public ProjectEntity Project { get; set; }
		public int ProjectId { get; set; }

		//commentary owner
		public ApplicationUserEntity CommentOwner { get; set; }
		public string CommentOwnerId { get; set; }

		//commentary's text
		public string Text { get; set; }

		//file attached to commentary
		public UserFileEntity AttachedFile { get; set; }
		public int? AttachedFileId { get; set; }

		//time of comment creation
		public DateTime CommentDate { get; set; }

	}
}
