namespace PFE.ExpenseTracker.Application.Common.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<GoogleUserInfo> VerifyGoogleTokenAsync(string idToken);
    }

    public class GoogleUserInfo
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;
    }
}
