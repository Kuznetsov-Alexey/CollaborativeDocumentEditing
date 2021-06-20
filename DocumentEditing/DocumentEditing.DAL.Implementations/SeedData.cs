using DocumentEditing.DAL.Contracts;
using DocumentEditing.DAL.Contracts.Enteties;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocumentEditing.DAL.Implementations
{
	public static class SeedData 
	{
		public async static void Initialize(IServiceProvider serviceProvider)
		{
			var repository = serviceProvider.GetService<IDbRepository>();

			var context = serviceProvider.GetService<MyDbContext>();
			
			//если нет ни проекта ни польлзователя
			if (!repository.Get<ProjectEntity>().Any())
			{
				var testFile = repository.Get<UserFileEntity>().FirstOrDefault();
				var user = repository.Get<ApplicationUserEntity>().FirstOrDefault();

				//create new project and save in database
				ProjectEntity project = new ProjectEntity
				{
					Name = "Test",
					ProjectOwner = user,
					Visitors = new List<ApplicationUserEntity> { user },
					CurrentFile = testFile
				};

				CommentaryEntity comment = new CommentaryEntity
				{
					Text = "Initial file",
					AttachedFile = testFile,
					CommentDate = DateTime.Now,
					CommentOwner = user,
					Project = project
				};

				await repository.Add(comment);
				project.Commentaries.Add(comment);
				await repository.Add(project);

				context.SaveChanges();				
			}
		}
	}
}
