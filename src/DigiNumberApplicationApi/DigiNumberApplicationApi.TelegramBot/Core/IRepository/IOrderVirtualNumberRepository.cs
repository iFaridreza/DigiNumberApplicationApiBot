using DigiNumberApplicationApi.TelegramBot.Core.Domian;

namespace DigiNumberApplicationApi.TelegramBot.Core.IRepository;

public interface IOrderVirtualNumberRepository
{
    Task Add(OrderVirtualNumber tclass);
    Task<IEnumerable<OrderVirtualNumber>> GetAll();
    ValueTask Update(OrderVirtualNumber orderVirtualNumber);
}