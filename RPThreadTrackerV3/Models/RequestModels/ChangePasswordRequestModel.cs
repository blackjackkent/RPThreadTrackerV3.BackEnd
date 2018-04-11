namespace RPThreadTrackerV3.Models.RequestModels
{
    public class ChangePasswordRequestModel
    {
	    public string CurrentPassword { get; set; }
		public string NewPassword { get; set; }
		public string ConfirmNewPassword { get; set; }
    }
}
