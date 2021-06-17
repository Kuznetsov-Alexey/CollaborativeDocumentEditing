namespace DocumentEditing.Domain.Contracts.Models
{
	public class UserFileModel
	{
		public int Id { get; set; }
		public string FileName { get; set; }
		public string PathToFile { get; set; }
	}
}