using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentEditing.DAL.Contracts.Enteties;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DocumentEditing.DAL.Implementations
{
    public class AuthDbContext : IdentityDbContext<ApplicationUserEntity>
    {
       
        public DbSet<CommentaryEntity> Comments { get; set; }        
        public DbSet<ProjectEntity> Projects { get; set; }        
        public DbSet<UserFileEntity> Files { get; set; }


        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<T> DbSet<T>() where T : class
		{
            return Set<T>();
		}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //create relationship many-to-many between projects and users who can visit project page
            builder.Entity<ProjectEntity>().HasMany(proj => proj.Visitors).WithMany(user => user.AvailableProjects);

            //create relationship one-to-many between project and users, who own this project
            builder.Entity<ProjectEntity>().HasOne(proj => proj.ProjectOwner).WithMany(user => user.OwnProjects);
		}

        
    }
}
