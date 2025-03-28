using DigiNumberApplicationApi.TelegramBot.Core.Domian;
using DigiNumberApplicationApi.TelegramBot.Core.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DigiNumberApplicationApi.TelegramBot.Infrastructer.Repository;

public class UserRepository : IUserRepository
{
    private readonly Context _context;

    public UserRepository(Context context)
    {
        _context = context;
    }

    public async Task Add(User tclass)
    {
        await _context.User.AddAsync(tclass);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Any(long chatId)
    {
        bool any = await _context.User.AnyAsync(x => x.ChatId == chatId);
        return any;
    }

    public async Task<User> Get(long chatId)
    {
        User user = await _context.User.SingleAsync(x => x.ChatId == chatId);
        return user;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        IEnumerable<User> users = await _context.User.ToListAsync();
        return users;
    }

    public async ValueTask Remove(User tclass)
    {
        _context.User.Remove(tclass);
        await _context.SaveChangesAsync();
    }

    public async ValueTask Update(User user)
    {
        _context.User.Update(user);
        await _context.SaveChangesAsync();
    }
}
