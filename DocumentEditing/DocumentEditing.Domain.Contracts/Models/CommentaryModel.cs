namespace DocumentEditing.Domain.Contracts.Models
{
	public class CommentaryModel
	{
		public int Id { get; set; }
		public ProjectModel Project { get; set; }
		public ApplicationUserModel CommentOwner { get; set; }
		public string Text { get; set; }
		public UserFileModel AttachedFile { get; set; }

		public System.DateTime CommentDate { get; set; }

	}
}