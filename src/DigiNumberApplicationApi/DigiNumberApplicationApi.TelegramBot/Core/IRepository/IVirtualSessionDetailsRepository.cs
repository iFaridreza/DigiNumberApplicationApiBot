using DigiNumberApplicationApi.TelegramBot.Core.Domian;

namespace DigiNumberApplicationApi.TelegramBot.Core.IRepository;

public interface IVirtualSessionDetailsRepository
{
    Task Add(VirtualSessionDetails virtualSessionDetails);
    Task<VirtualSessionDetails> Get();
    Task<bool> Any();
}