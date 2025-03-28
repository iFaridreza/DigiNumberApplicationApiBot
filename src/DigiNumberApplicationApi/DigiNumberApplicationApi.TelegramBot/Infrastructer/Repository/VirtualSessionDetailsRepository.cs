using DigiNumberApplicationApi.TelegramBot.Core.Domian;
using DigiNumberApplicationApi.TelegramBot.Core.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DigiNumberApplicationApi.TelegramBot.Infrastructer.Repository;

public class VirtualSessionDetailsRepository : IVirtualSessionDetailsRepository
{
    private readonly Context _context;

    public VirtualSessionDetailsRepository(Context context)
    {
        _context = context;
    }

    public async Task Add(VirtualSessionDetails virtualSessionDetails)
    {
        await _context.VirtualSessionDetails.AddAsync(virtualSessionDetails);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Any()
    {
        bool any = await _context.VirtualSessionDetails.AnyAsync();
        return any;
    }

    public async Task<VirtualSessionDetails> Get()
    {
        VirtualSessionDetails virtualSessionDetails = await _context.VirtualSessionDetails.SingleAsync();
        return virtualSessionDetails;
    }
}
