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

		Task<UserFile> GetUserFile(int fileId);

		Task<MemoryStream> GetMemoryStream(int fileId);
	}
}
