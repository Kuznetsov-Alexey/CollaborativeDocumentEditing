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
	public class LocalFileManager : IFileManager
	{
		public LocalFileManager(IWebHostEnvironment webHostEnvironment, AuthDbContext dbContext)
		{
			_dbContext = dbContext;
			_webHostEnvironment = webHostEnvironment;
		}

		private AuthDbContext _dbContext;
		private IWebHostEnvironment _webHostEnvironment;


		public async Task<MemoryStream> GetMemoryStream(int fileId)
		{
			var userFile = await _dbContext.Files.FindAsync(fileId);

			//save file in memory
			var memory = new MemoryStream();
			using (var stream = new FileStream(userFile.PathToFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				await stream.CopyToAsync(memory);
			}

			//todo: create service for work with files
			FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();

			string fileType;
			provider.TryGetContentType(userFile.PathToFile, out fileType);

			memory.Position = 0;

			return memory;
		}

		public async Task<UserFile> UploadFile(IFormFile fileToUpload)
		{
			string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "UserFiles");

			//generate unique file name
			string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileToUpload.FileName;

			//get full path to file
			string filePath = Path.Combine(uploadsFolder, uniqueFileName);

			//copy file to folder
			await fileToUpload.CopyToAsync(new FileStream(filePath, FileMode.Create));

			UserFile userFile = new UserFile
			{
				FileName = fileToUpload.FileName,
				PathToFile = filePath
			};

			_dbContext.Files.Add(userFile);

			return userFile;
		}

		public async Task<UserFile> GetUserFile(int fileId)
		{
			return await _dbContext.Files.FindAsync(fileId);
		}
	}
}
