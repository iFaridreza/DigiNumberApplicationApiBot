namespace DigiNumberApplicationApi.TelegramBot.StepUser;

public interface IUserStepRepository
{
    Task Create(UserStep userSession);
    Task<bool> Any(long chatId);
    Task<UserStep> Get(long chatId);
    Task Remove(UserStep userSession);
    Task<IEnumerable<UserStep>> GetAll();
}