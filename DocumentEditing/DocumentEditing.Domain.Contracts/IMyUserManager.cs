using DocumentEditing.Domain.Contracts.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentEditing.Domain.Contracts
{
	public interface IMyUserManager
	{
		Task<ApplicationUserModel> GetUserByClaimsAsync(System.Security.Claims.ClaimsPrincipal user);

		Task<ApplicationUserModel> GetUserByEmail(string email);

		Task<IdentityResult> CreateUser(ApplicationUserModel user);

		Task SignIn(ApplicationUserModel user);

		Task<SignInResult> PasswordSignIn(ApplicationUserModel user, bool rememberMe);
		
		string GeneratePassword(int lenght);
		Task SignOut();
	}
}
