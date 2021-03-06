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
		public int Id { get; set; }

		//short file name, without unique part
		public string FileName { get; set; }

		//full path to file
		public string PathToFile { get; set; }
	}
}
