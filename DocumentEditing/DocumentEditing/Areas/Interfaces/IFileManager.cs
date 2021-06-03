using DocumentEditing.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentEditing.Areas.Interfaces
{
	public interface IFileManager
	{

		Task<UserFile> UploadFile(IFormFile userFile);

		/// <summary>
		/// Get file result according to file id
		/// I chose FileResult class to realize child classes such as PhysicalFileResult
		/// </summary>
		/// <param name="fileId"></param>
		/// <returns></returns>
		Task<FileResult> GetFileResult(int fileId);
	}
}
