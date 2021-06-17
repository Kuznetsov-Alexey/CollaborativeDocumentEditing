namespace DocumentEditing.Domain.Contracts
{
	public interface IInviteSenderService
	{
		System.Threading.Tasks.Task SendInvite(string userEmail, string userPassword, string projectName);
	}
}
