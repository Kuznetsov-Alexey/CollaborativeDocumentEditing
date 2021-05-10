using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentEditing.ViewModels
{
	/// <summary>
	/// Each project must has name and uploaded file, so this model
	/// has field for each of them  
	/// </summary>
	public class AddProjectModel
	{
		[Required]
		[Column(TypeName = "varchar(200)")]
		public string Name { get; set; }

		[Required]
		public IFormFile UploadedFile { get; set; }
	}
}
