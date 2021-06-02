using DocumentEditing.Areas.Interfaces;
using DocumentEditing.Data;
using DocumentEditing.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentEditing.Areas.Services
{
	/// <summary>
	/// File manager for working with server storage
	/// </summary>
	public class LocalFileManager : IFileManager
	{
		private readonly AuthDbContext _dbContext;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public LocalFileManager(IWebHostEnvironment webHostEnvironment, AuthDbContext dbContext)
		{
			_dbContext = dbContext;
			_webHostEnvironment = webHostEnvironment;
		}

		public async Task<FileResult> GetFileResult(int fileId)
		{
			var userFile = await _dbContext.Files.FindAsync(fileId);

			if(userFile == null)
			{
				return null;
			}	
			
			var contentType = GetContentType(userFile.PathToFile);			

			var result = new PhysicalFileResult(userFile.PathToFile, contentType);
			result.FileDownloadName = userFile.FileName;

			return result;
		}
		
		private string GetContentType(string fileName)
		{			
			var provider = new FileExtensionContentTypeProvider();
			provider.TryGetContentType(fileName, out string fileType);
			return fileType;
		}

		public async Task<UserFile> UploadFile(IFormFile fileToUpload)
		{
			string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "UserFiles");

			//generate unique file name
			string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileToUpload.FileName;

			//get full path to file
			string filePath = Path.Combine(uploadsFolder, uniqueFileName);

			//copy file to server folder
			await fileToUpload.CopyToAsync(new FileStream(filePath, FileMode.Create));

			UserFile userFile = new UserFile
			{
				FileName = fileToUpload.FileName,
				PathToFile = filePath
			};

			_dbContext.Files.Add(userFile);
			await _dbContext.SaveChangesAsync();		

			return userFile;
		}
	}
}
