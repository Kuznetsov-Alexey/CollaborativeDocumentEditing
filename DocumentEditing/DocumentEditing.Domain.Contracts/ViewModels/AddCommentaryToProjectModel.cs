using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentEditing.Domain.Contracts.ViewModels
{
	public class AddCommentaryToProjectModel
	{
		public int ProjectId { get; set; }

		public IFormFile AttachedFile { get; set; }
		
		public string Text { get; set; }
	}
}
