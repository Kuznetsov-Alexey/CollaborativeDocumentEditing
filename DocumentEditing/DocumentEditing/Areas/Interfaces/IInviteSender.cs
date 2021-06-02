using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentEditing.Areas.Interfaces
{
	/// <summary>
	/// Send invite to project with login data 
	/// </summary>
	public interface IInviteSender
	{
		Task SendInvite(string userEmail, string userPassword, string projectName);
	}
}
