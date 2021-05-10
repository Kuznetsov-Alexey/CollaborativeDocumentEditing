using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using DocumentEditing.Models;
using Microsoft.AspNetCore.Identity;

namespace DocumentEditing.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        //owned project for link between objects
        public ICollection<Project> OwnProjects { get; set; } = new List<Project>();

        //available for wathcing projects for link between objects
        public ICollection<Project> AvailableProjects { get; set; } = new List<Project>();  
    }
}
