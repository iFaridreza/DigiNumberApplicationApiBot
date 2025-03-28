using Microsoft.EntityFrameworkCore;

namespace DigiNumberApplicationApi.TelegramBot.StepUser;

public class UserStepRepository : IUserStepRepository
{
    private readonly SchedulerContext _schedulerContext;

    public UserStepRepository(SchedulerContext schedulerContext)
    {
        _schedulerContext = schedulerContext;
    }

    public async Task<bool> Any(long chatId)
    {
        bool any = await _schedulerContext.UserStep.AnyAsync(x => x.ChatId == chatId);
        return any;
    }

    public async Task Create(UserStep userStep)
    {
        await _schedulerContext.UserStep.AddAsync(userStep);
        await _schedulerContext.SaveChangesAsync();
    }

    public async Task<UserStep> Get(long chatId)
    {
        UserStep userStep = await _schedulerContext.UserStep.SingleAsync(x => x.ChatId == chatId);
        return userStep;
    }

    public async Task<IEnumerable<UserStep>> GetAll()
    {
        IEnumerable<UserStep> userSessions = await _schedulerContext.UserStep.ToListAsync();
        return userSessions;
    }

    public async Task Remove(UserStep userSession)
    {
        _schedulerContext.UserStep.Remove(userSession);
        await _schedulerContext.SaveChangesAsync();
    }
}
