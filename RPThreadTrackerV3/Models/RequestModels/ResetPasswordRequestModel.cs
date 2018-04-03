namespace RPThreadTrackerV3.Models.RequestModels
{
    public class ResetPasswordRequestModel
    {
		public string Email { get; set; }
		public string Code { get; set; }
		public string NewPassword { get; set; }
		public string ConfirmNewPassword { get; set; }
    }
}
