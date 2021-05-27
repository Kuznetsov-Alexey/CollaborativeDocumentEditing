using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentEditing.Areas.Interfaces
{
	public interface IInviteSender
	{
		void SendInvite(string userEmail, string userPassword, string projectName);
	}
}
