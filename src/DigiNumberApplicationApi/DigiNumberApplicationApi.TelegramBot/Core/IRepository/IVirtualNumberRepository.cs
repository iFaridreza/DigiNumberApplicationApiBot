using DigiNumberApplicationApi.TelegramBot.Core.Domian;

namespace DigiNumberApplicationApi.TelegramBot.Core.IRepository;

public interface IVirtualNumberRepository
{
    Task Add(VirtualNumber tclass);
    Task<VirtualNumber> Get(string number);
    Task<bool> Any(string number);
    Task<IEnumerable<VirtualNumber>> GetAll();
    ValueTask Update(VirtualNumber tclass);
    Task<IEnumerable<VirtualNumber>> GetAll(EStatusOrder eStatusOrder);
}