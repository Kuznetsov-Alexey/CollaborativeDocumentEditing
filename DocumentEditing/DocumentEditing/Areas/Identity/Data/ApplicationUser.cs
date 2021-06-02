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

		/// <summary>
		/// Simple password genarator, return password that consists of digit.
		/// Gets one argue - length of password (4- min, 10 - max)
		/// </summary>
		/// <param name="passwordLenght">lenght of password</param>
		/// <returns></returns>
		public string GeneratePassword(int passwordLenght)
		{
			Random rand = new Random();

			int minLenght = 4;
			int maxLength = 10;

			if (passwordLenght < minLenght)
				passwordLenght = minLenght;

			if (passwordLenght > maxLength)
				passwordLenght = maxLength;

			int botRange = 10;

			for (int i = 2; i < passwordLenght; i++)
				botRange *= 10;

			int topRange = botRange * 10 - 1;

			int value = rand.Next(botRange, topRange);
			return value.ToString();
		}

	}
}
