using DocumentEditing.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentEditing.Models
{
	/// <summary>
	/// Representation of user file on server
	/// </summary>
	public class UserFile
	{
		//uploaded file ID
		public int Id { get; set; }

		//short file name, without unique part
		public string FileName { get; set; }

		//full path to file
		public string PathToFile { get; set; }

		//field for link between tables
		//public string FileOwnerId { get; set; }

		//file owner
		//public ApplicationUser FileOwner { get; set; }

	}
}
