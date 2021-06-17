using System;

namespace DocumentEditing.DAL.Contracts.Enteties
{
	public class ErrorViewModel
	{
		public string RequestId { get; set; }

		public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
	}
}
