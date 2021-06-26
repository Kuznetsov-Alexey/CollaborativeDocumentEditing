using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DocumentEditing.Domain.Contracts.ViewModels
{
	public class RegisterViewModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }


		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		public string ConfirmPassword { get; set; }

	}
}
