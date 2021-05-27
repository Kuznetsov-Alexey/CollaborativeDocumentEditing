using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentEditing.ViewModels
{
	/// <summary>
	/// For adding user to project, site needs to know project ID and User's email
	/// </summary>
	public class AddUserToProjectModel
	{
		public int ProjectId { get; set; }

		[Required]		
		[EmailAddress]
		public string UserEmail { get; set; }
	}
}
