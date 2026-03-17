using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SurveyBasketAPI.DTOs.Account;
using SurveyBasketAPI.Entities;
using SurveyBasketAPI.Result_Pattern;
using SurveyBasketAPI.Services_Abstraction;

namespace SurveyBasketAPI.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> UserManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        UserManager = userManager;
    }

    public async Task<Result<UserProfileResponse>> GetUserProfileAsync(string userId)
    {

        var user = await UserManager.Users
            .Where(u => u.Id == userId)
            .ProjectToType<UserProfileResponse>()
            .SingleAsync();


        if (user == null) 
        {
            throw new Exception("User not found");
        }

        return Result.Success(user);

    }


    public async Task<Result> UpdateUserProfileAsync(string userId, UpdateProfileRequest request)
    {
        //var user = await UserManager.FindByIdAsync(userId);
       
        //if (user == null)
        //{
        //    throw new Exception("User not found");
        //}

        //user = request.Adapt(user);

        //var result = await UserManager.UpdateAsync(user);

            var user = await UserManager.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(u => u.FirstName, request.FirstName)
                    .SetProperty(u => u.LastName, request.LastName));

        return Result.Success();
    }


    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await UserManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }
        var result = await UserManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault();

            return Result.Failure(new Error(error!.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        return Result.Success();
    }
}
