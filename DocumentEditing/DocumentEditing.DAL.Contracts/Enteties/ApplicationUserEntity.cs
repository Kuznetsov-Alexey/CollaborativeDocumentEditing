using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

namespace DocumentEditing.DAL.Contracts.Enteties
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUserEntity : IdentityUser
    {      

        //owned project for link between objects
        public ICollection<ProjectEntity> OwnProjects { get; set; } = new List<ProjectEntity>();

        //available for wathcing projects for link between objects
        public ICollection<ProjectEntity> AvailableProjects { get; set; } = new List<ProjectEntity>();
	}
}
