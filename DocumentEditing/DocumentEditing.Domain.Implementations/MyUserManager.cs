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
		private UserManager<ApplicationUserEntity> _userSource;
		

		public MyUserManager(UserManager<ApplicationUserEntity> userSource, IMapper mapper)
		{
			_mapper = mapper;
			_userSource = userSource;
		}

		public async Task CreateUser(ApplicationUserModel user, string password)
		{
			var mappedUser = _mapper.Map<ApplicationUserEntity>(user);

			await _userSource.CreateAsync(mappedUser, password);
		}

		public async Task<ApplicationUserModel> GetCurrentUser(ClaimsPrincipal userClaims)
		{
			var user = await _userSource.GetUserAsync(userClaims);
			
			return _mapper.Map<ApplicationUserModel>(user);
		}

		public async Task<ApplicationUserModel> GetUserByEmail(string email)
		{
			var user = await _userSource.FindByEmailAsync(email);

			return _mapper.Map<ApplicationUserModel>(user);
		}
	}
}
