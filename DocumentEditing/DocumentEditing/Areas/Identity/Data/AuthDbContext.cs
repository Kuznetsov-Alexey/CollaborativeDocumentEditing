using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentEditing.Areas.Identity.Data;
using DocumentEditing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DocumentEditing.Data
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //create relationship many-to-many between projects and users who can visit project page
            builder.Entity<Project>().HasMany(proj => proj.Visitors).WithMany(user => user.AvailableProjects);

            //create relationship one-to-many between project and users, who own this project
            builder.Entity<Project>().HasOne(proj => proj.ProjectOwner).WithMany(user => user.OwnProjects);
		}

        //context for commentary objects
        public DbSet<Commentary> Comments { get; set; }

        //context for project objects
        public DbSet<Project> Projects { get; set; }

        //context for files objects
        public DbSet<UploadedFile> Files { get; set; }
    }
}
