using DocumentEditing.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentEditing.Domain.Implementations
{
	public class InviteSenderService : IInviteSenderService
	{
		public Task SendInvite(string userEmail, string userPassword, string projectName)
		{
			throw new NotImplementedException();
		}
	}
}
