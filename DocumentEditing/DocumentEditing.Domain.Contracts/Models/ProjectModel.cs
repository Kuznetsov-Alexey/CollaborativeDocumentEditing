﻿using System.Collections.Generic;

namespace DocumentEditing.Domain.Contracts.Models
{
	public class ProjectModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public bool IsProjectFinished { get; set; }
		public ApplicationUserModel ProjectOwner { get; set; }
		public UserFileModel CurrentFile { get; set; }		

		public List<ApplicationUserModel> Visitors { get; set; } = new List<ApplicationUserModel>();
		public List<CommentaryModel> Commentaries { get; set; } = new List<CommentaryModel>();
	}
}