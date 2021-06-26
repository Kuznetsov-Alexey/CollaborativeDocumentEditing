using AutoMapper;
using DocumentEditing.DAL.Contracts.Enteties;
using DocumentEditing.Domain.Contracts;
using DocumentEditing.Domain.Contracts.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DocumentEditing.Domain.Implementations
{
	public class MyUserManager : IMyUserManager
	{
		private IMapper _mapper;
		private UserManager<ApplicationUserEntity> _identityUserManager;
		private SignInManager<ApplicationUserEntity> _identitySignInManager;
		

		public MyUserManager(UserManager<ApplicationUserEntity> userSource, IMapper mapper, SignInManager<ApplicationUserEntity> signInManager)
		{
			_mapper = mapper;
			_identityUserManager = userSource;
			_identitySignInManager = signInManager;
		}

		public async Task SignOut()
		{
			await _identitySignInManager.SignOutAsync();
		}

		public async Task<SignInResult> PasswordSignIn(ApplicationUserModel userModel, bool rememberMe)
		{ 
			var result = await _identitySignInManager.PasswordSignInAsync(userModel.Email, userModel.Password, rememberMe, false);

			return result;
		}

		public async Task SignIn(ApplicationUserModel userModel)
		{
			var user = await _identityUserManager.FindByIdAsync(userModel.Id);

			await _identitySignInManager.SignInAsync(user, false);
		}

		public async Task<IdentityResult> CreateUser(ApplicationUserModel user)
		{			
			ApplicationUserEntity newUser = new ApplicationUserEntity
			{
				UserName = user.UserName,
				Email = user.Email
			};

			user.Id = newUser.Id;
			var result = await _identityUserManager.CreateAsync(newUser, user.Password);			

			return result;
		}

		public string GeneratePassword(int passwordLength)
		{
			Random rand = new Random();

			int minLenght = 4;
			int maxLength = 10;

			if (passwordLength < minLenght)
				passwordLength = minLenght;

			if (passwordLength > maxLength)
				passwordLength = maxLength;

			int botRange = 10;

			for (int i = 2; i < passwordLength; i++)
				botRange *= 10;

			int topRange = botRange * 10 - 1;

			int value = rand.Next(botRange, topRange);
			return value.ToString();
		}

		public async Task<ApplicationUserModel> GetUserByClaimsAsync(ClaimsPrincipal userClaims)
		{
			var user = await _identityUserManager.GetUserAsync(userClaims);
			
			return _mapper.Map<ApplicationUserModel>(user);
		}

		public async Task<ApplicationUserModel> GetUserByEmail(string email)
		{
			var user = await _identityUserManager.FindByEmailAsync(email);

			return _mapper.Map<ApplicationUserModel>(user);
		}

		
	}
}
