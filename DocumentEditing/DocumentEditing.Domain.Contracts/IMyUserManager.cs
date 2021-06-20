using DocumentEditing.Domain.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentEditing.Domain.Contracts
{
	public interface IMyUserManager
	{
		Task<ApplicationUserModel> GetCurrentUser(System.Security.Claims.ClaimsPrincipal user);

		Task<ApplicationUserModel> GetUserByEmail(string email);

		Task CreateUser(ApplicationUserModel user, string password);
	}
}
