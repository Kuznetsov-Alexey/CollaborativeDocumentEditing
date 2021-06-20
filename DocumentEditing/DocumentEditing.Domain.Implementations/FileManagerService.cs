using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DocumentEditing.DAL.Contracts;
using DocumentEditing.DAL.Contracts.Enteties;
using DocumentEditing.Domain.Contracts;
using DocumentEditing.Domain.Contracts.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace DocumentEditing.Domain.Implementations
{
	public class FileManagerService : IFileManagerService
	{
		private IMapper _mapper;
		private IWebHostEnvironment _webHostEnvironment;
		private IDbRepository _repository;

		public FileManagerService(IWebHostEnvironment webHostEnvironment, IDbRepository repository, IMapper mapper)
		{
			_mapper = mapper;
			_webHostEnvironment = webHostEnvironment;
			_repository = repository;
		}

		public async Task<FileResult> GetFileResult(int fileId)
		{
			var userFile = await _repository.Get<UserFileEntity>().FirstOrDefaultAsync(f => f.Id == fileId);

			if (userFile == null)
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

		public async Task<UserFileModel> UploadFile(IFormFile fileToUpload)
		{
			string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "UserFiles");

			//generate unique file name
			string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileToUpload.FileName;

			//get full path to file
			string filePath = Path.Combine(uploadsFolder, uniqueFileName);

			//copy file to server folder
			await fileToUpload.CopyToAsync(new FileStream(filePath, FileMode.Create));

			UserFileModel userFile = new UserFileModel
			{
				FileName = fileToUpload.FileName,
				PathToFile = filePath
			};

			var mappedFile = _mapper.Map<UserFileEntity>(userFile);

			await _repository.Add(mappedFile);
			await _repository.SaveChangesAsync();

			return userFile;
		}
	}
}
