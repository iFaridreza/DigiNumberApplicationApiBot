using DigiNumberApplicationApi.TelegramBot.Core.Domian;
using DigiNumberApplicationApi.TelegramBot.Core.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DigiNumberApplicationApi.TelegramBot.Infrastructer.Repository;

public class VirtualNumberRepository : IVirtualNumberRepository
{
    private readonly Context _context;

    public VirtualNumberRepository(Context context)
    {
        _context = context;
    }

    public async Task Add(VirtualNumber tclass)
    {
        await _context.VirtualNumber.AddAsync(tclass);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Any(string number)
    {
        bool any = await _context.VirtualNumber.AnyAsync(x => x.Number == number);
        return any;
    }

    public async Task<VirtualNumber> Get(string number)
    {
        VirtualNumber virtualNumber = await _context.VirtualNumber.SingleAsync(x => x.Number == number);
        return virtualNumber;
    }

    public async Task<IEnumerable<VirtualNumber>> GetAll(EStatusOrder eStatusOrder)
    {
        IEnumerable<VirtualNumber> virtualNumbers = await _context.VirtualNumber.Where(x => x.EStatusOrder == eStatusOrder).ToListAsync();
        return virtualNumbers;
    }

    public async Task<IEnumerable<VirtualNumber>> GetAll()
    {
        IEnumerable<VirtualNumber> virtualNumbers = await _context.VirtualNumber.ToListAsync();
        return virtualNumbers;
    }

    public async ValueTask Update(VirtualNumber tclass)
    {
        _context.VirtualNumber.Update(tclass);
        await _context.SaveChangesAsync();
    }
}
