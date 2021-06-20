using DocumentEditing.Domain.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentEditing.Domain.Contracts.ViewModels
{
	public class ViewProjectModel
	{
		public bool IsOwner { get; set; }
		public ProjectModel Project { get; set; }
		public List<CommentaryModel> Commentaries { get; set; }
	}
}
