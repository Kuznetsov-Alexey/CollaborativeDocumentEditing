using DocumentEditing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentEditing.ViewModels
{
	/// <summary>
	/// For showing project info. Different possibilities for owner and for visitiors
	/// </summary>
	public class ViewProject
	{
		//is current user a project owner
		public bool IsOwner { get; set; }

		public Project Project { get; set; }		

		public ICollection<Commentary> Commentaries { get; set; }
	}
}
