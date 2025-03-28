using DigiNumberApplicationApi.TelegramBot.Core.Domian;
using DigiNumberApplicationApi.TelegramBot.Core.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DigiNumberApplicationApi.TelegramBot.Infrastructer.Repository;


public class SudoRepository : ISudoRepository
{
    private readonly Context _context;

    public SudoRepository(Context context)
    {
        _context = context;
    }

    public async Task Add(Sudo tclass)
    {
        await _context.Sudo.AddAsync(tclass);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Any(long chatId)
    {
        bool any = await _context.Sudo.AnyAsync(x => x.ChatId == chatId);
        return any;
    }

    public async Task<Sudo> Get(long chatId)
    {
        Sudo sudo = await _context.Sudo.SingleAsync(x => x.ChatId == chatId);
        return sudo;
    }

    public async Task<IEnumerable<Sudo>> GetAll()
    {
        IEnumerable<Sudo> sudos = await _context.Sudo.ToListAsync();
        return sudos;
    }

    public async ValueTask Remove(Sudo tclass)
    {
        _context.Sudo.Remove(tclass);
        await _context.SaveChangesAsync();
    }
}
