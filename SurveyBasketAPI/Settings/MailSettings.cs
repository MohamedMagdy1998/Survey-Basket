namespace SurveyBasketAPI.Settings;

public class MailSettings
{
    public string Mail { get; set; } = null!;
    public int Port { get; set; }
    public string Host { get; set; } = string.Empty;
    public string DisplayName { get; set; } = null!;
    public string Password { get; set; } = null!;
}
