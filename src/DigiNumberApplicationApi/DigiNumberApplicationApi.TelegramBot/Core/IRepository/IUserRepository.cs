using DigiNumberApplicationApi.TelegramBot.Core.Domian;

namespace DigiNumberApplicationApi.TelegramBot.Core.IRepository;

public interface IUserRepository : IBaseUser<User>
{
    ValueTask Update(User user);
}