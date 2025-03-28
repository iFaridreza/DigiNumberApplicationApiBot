namespace DigiNumberApplicationApi.TelegramBot.StepUser;
public class UserStep
{
    internal long Id { get; set; }
    internal long ChatId { get; set; }
    internal string Step { get; set; } = string.Empty;
    internal DateTime ExpierTime { get; init; }
}
