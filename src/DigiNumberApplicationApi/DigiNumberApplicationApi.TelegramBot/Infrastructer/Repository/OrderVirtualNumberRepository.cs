using DigiNumberApplicationApi.TelegramBot.Core.Domian;
using DigiNumberApplicationApi.TelegramBot.Core.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DigiNumberApplicationApi.TelegramBot.Infrastructer.Repository;
public class OrderVirtualNumberRepository : IOrderVirtualNumberRepository
{
    private readonly Context _context;

    public OrderVirtualNumberRepository(Context context)
    {
        _context = context;
    }

    public async Task Add(OrderVirtualNumber tclass)
    {
        await _context.OrderVirtualNumber.AddAsync(tclass);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<OrderVirtualNumber>> GetAll()
    {
        IEnumerable<OrderVirtualNumber> orderVirtualNumbers = await _context.OrderVirtualNumber.ToListAsync();
        return orderVirtualNumbers;
    }

    public async ValueTask Update(OrderVirtualNumber orderVirtualNumber)
    {
        _context.OrderVirtualNumber.Update(orderVirtualNumber);
        await _context.SaveChangesAsync();
    }
}