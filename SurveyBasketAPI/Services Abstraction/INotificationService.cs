namespace SurveyBasketAPI.Services_Abstraction;

public interface INotificationService
{
    Task SendNewPollsNotification(int? pollId = null);
}