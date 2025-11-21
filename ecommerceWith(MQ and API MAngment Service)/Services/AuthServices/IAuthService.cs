namespace Authentication_With_JWT.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterModel model, string? ReqUrl);
        Task<AuthModel> LoginAsync(LoginModel model);
        Task<string> AddRoleAsync(AddRoleModel model);
        Task<string> ConfirmEmail(string userId, string token);
        Task<string> ForgotPasswordConfermationModel(ForgotPasswordConfermationModel model);
        Task<string> DeleteAccount(string error, string email);
        Task<string> setAdminRole(string AdminID, string userEmail);
        Task<IReadOnlyList<UsersDto>> GetAllUsersAsync(string AdminID);

        Task<bool> isAdmin(string userId);
        Task<bool> isUser(string userId);
    }
}
