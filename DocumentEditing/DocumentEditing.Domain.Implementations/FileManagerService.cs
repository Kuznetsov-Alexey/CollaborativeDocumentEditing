using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DocumentEditing.Domain.Contracts;
using DocumentEditing.Domain.Contracts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DocumentEditing.Domain.Implementations
{
	public class FileManagerService : IFileManagerService
	{
		public Task<FileResult> GetFileResult(int fileId)
		{
			throw new NotImplementedException();
		}

		public Task<UserFileModel> UploadFile(IFormFile userFile)
		{
			throw new NotImplementedException();
		}
	}
}
