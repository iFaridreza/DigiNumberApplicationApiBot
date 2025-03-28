using DigiNumberApplicationApi.TelegramBot.Core.Domian;

namespace DigiNumberApplicationApi.TelegramBot.Core.IRepository;

public interface IVirtualNumberDetailsRepository
{
    Task Add(VirtualNumberDetails virtualNumberDetails);
    Task<bool> Any(string countryCode);
    Task<IEnumerable<VirtualNumberDetails>> GetAll();
    Task<VirtualNumberDetails> Get(string countryCode);
    ValueTask Remove(VirtualNumberDetails virtualNumberDetails);
    ValueTask Update(VirtualNumberDetails virtualNumberDetails);
}