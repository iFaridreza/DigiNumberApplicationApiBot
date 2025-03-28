using DigiNumberApplicationApi.TelegramBot.Core.Domian;
using DigiNumberApplicationApi.TelegramBot.Core.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DigiNumberApplicationApi.TelegramBot.Infrastructer.Repository;


public class VirtualNumberDetailsRepository : IVirtualNumberDetailsRepository
{
    private readonly Context _context;

    public VirtualNumberDetailsRepository(Context context)
    {
        _context = context;
    }

    public async Task Add(VirtualNumberDetails virtualNumberDetails)
    {
        await _context.VirtualNumberDetails.AddAsync(virtualNumberDetails);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Any(string countryCode)
    {
        bool any = await _context.VirtualNumberDetails.AnyAsync(x => x.CountryCode == countryCode);
        return any;
    }

    public async Task<VirtualNumberDetails> Get(string countryCode)
    {
        VirtualNumberDetails virtualNumberDetails = await _context.VirtualNumberDetails.SingleAsync(x => x.CountryCode == countryCode);
        return virtualNumberDetails;
    }

    public async Task<IEnumerable<VirtualNumberDetails>> GetAll()
    {
        IEnumerable<VirtualNumberDetails> virtualNumberDetails = await _context.VirtualNumberDetails.ToListAsync();
        return virtualNumberDetails;
    }

    public async ValueTask Remove(VirtualNumberDetails virtualNumberDetails)
    {
        _context.VirtualNumberDetails.Remove(virtualNumberDetails);
        await _context.SaveChangesAsync();
    }

    public async ValueTask Update(VirtualNumberDetails virtualNumberDetails)
    {
        _context.VirtualNumberDetails.Update(virtualNumberDetails);
        await _context.SaveChangesAsync();
    }
}

