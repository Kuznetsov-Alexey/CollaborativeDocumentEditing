using DocumentEditing.Domain.Contracts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DocumentEditing.Domain.Contracts
{
	public interface IFileManagerService
	{
		Task<UserFileModel> UploadFile(IFormFile userFile);

		/// <summary>
		/// Get file result according to file id
		/// I chose FileResult class to realize child classes such as PhysicalFileResult
		/// </summary>
		/// <param name="fileId"></param>
		/// <returns></returns>
		Task<FileResult> GetFileResult(int fileId);
	}
}
