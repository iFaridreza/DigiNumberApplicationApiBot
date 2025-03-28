namespace DigiNumberApplicationApi.TelegramBot.Core.IRepository;

public interface IBaseUser<Tclass> where Tclass : class
{
    Task Add(Tclass tclass);
    Task<Tclass> Get(long chatId);
    Task<IEnumerable<Tclass>> GetAll();
    Task<bool> Any(long chatId);
    ValueTask Remove(Tclass tclass);
}
