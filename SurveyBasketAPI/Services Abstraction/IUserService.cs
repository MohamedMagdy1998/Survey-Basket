using SurveyBasketAPI.DTOs.Account;
using SurveyBasketAPI.Result_Pattern;

namespace SurveyBasketAPI.Services_Abstraction;

public interface IUserService
{
    public Task<Result<UserProfileResponse>> GetUserProfileAsync(string userId);
    public Task<Result> UpdateUserProfileAsync(string userId, UpdateProfileRequest request);

    public Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
}
