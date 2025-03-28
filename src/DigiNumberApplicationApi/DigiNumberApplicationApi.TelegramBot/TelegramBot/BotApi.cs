using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.DependencyInjection;
using DigiNumberApplicationApi.TelegramBot.Common;
using DigiNumberApplicationApi.TelegramBot.Core.IRepository;
using DigiNumberApplicationApi.TelegramBot.Api;
using DigiNumberApplicationApi.TelegramBot.Cashe;
using DigiNumberApplicationApi.TelegramBot.StepUser;
using DigiNumberApplicationApi.TelegramBot.Api.Models;
using DigiNumberApplicationApi.TelegramBot.Infrastructer.Repository;
using User = DigiNumberApplicationApi.TelegramBot.Core.Domian.User;
using System.Runtime.InteropServices;
using DigiNumberApplicationApi.TelegramBot.Core.Domian;

namespace DigiNumberApplicationApi.TelegramBot.TelegramBot;
public class BotApi
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;
    private readonly TelegramBotClient _telegramBotClient;
    private readonly AppSettings _appSettings;

    public BotApi(IServiceProvider serviceProvider, ILogger logger, TelegramBotClient telegramBotClient)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _telegramBotClient = telegramBotClient;
        _appSettings = Utils.GetAppSettings();
    }

    public void Listen()
    {
        _telegramBotClient.OnMessage += OnMessage;
        _telegramBotClient.OnUpdate += OnUpdate;
        _logger.Information($"- Bot {_appSettings.UsernameBot} Run");
    }

    private async Task OnUpdate(Update update)
    {
        try
        {
            if (update.CallbackQuery is null || update.CallbackQuery.Message is null || update.CallbackQuery.Message.Chat.Type != ChatType.Private)
            {
                return;
            }


            int messageId = update.CallbackQuery.Message.MessageId;
            long chatId = update.CallbackQuery.Message.Chat.Id;
            string? data = update.CallbackQuery.Data;

            if (string.IsNullOrEmpty(data))
            {
                return;
            }

            ISudoRepository sudoRepository = _serviceProvider.GetRequiredService<ISudoRepository>();
            IUserRepository userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            IUserStepRepository userStepRepository = _serviceProvider.GetRequiredService<IUserStepRepository>();

            bool anySudo = await sudoRepository.Any(chatId);

            if (anySudo)
            {
                if (data.Equals("AddCountry"))
                {
                    _logger.Information($"Sudo {chatId} افزودن کشور ➕");
                }

                return;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "❌ Error");
        }
    }

    private async Task OnMessage(Message message, UpdateType type)
    {
        try
        {
            long chatId = message.Chat.Id;

            await _telegramBotClient.SendChatAction(chatId, ChatAction.Typing);

            string? text = message.Text;

            if (string.IsNullOrEmpty(text)) return;

            int messageId = message.MessageId;

            switch (text)
            {
                case "/start":
                    {
                        await OnStart(message, chatId);
                    }
                    break;
                case "👤 حساب کاربری":
                    {
                        await OnInfo(message, chatId);
                    }
                    break;
                case "♻️ استعلام شماره":
                    {

                    }
                    break;
                case "🛍 خرید شماره مجازی":
                    {

                    }
                    break;
                case "🔖 قوانین":
                    {
                        await OnRules(message, chatId);
                    }
                    break;
                case "☎️ پشتیبانی":
                    {
                        await OnSupport(message, chatId);
                    }
                    break;
                case "📋 لیست کشور":
                    {
                        await OnListCountry(message, chatId);
                    }
                    break;
                default:
                    {
                        IUserStepRepository userStepRepository = _serviceProvider.GetRequiredService<IUserStepRepository>();
                        IUserRepository userRepository = _serviceProvider.GetRequiredService<IUserRepository>();

                        bool anyStep = await userStepRepository.Any(chatId);

                        if (!anyStep)
                        {
                            return;
                        }

                        UserStep userStep = await userStepRepository.Get(chatId);

                        if (userStep.Step == "Phone")
                        {
                            string phone = text.Replace(" ", "");

                            bool isValid = Utils.IsValidPhone(phone);

                            if (!isValid)
                            {
                                _logger.Information($"User {chatId} Phone Invalid");

                                await userStepRepository.Remove(userStep);
                                await _telegramBotClient.SendMessage(chatId, ReplyText._invalidPhone, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeUser());
                                return;
                            }

                            bool isVerifyUser = await ApplicationApi.VerifyUserAsync(chatId, phone);

                            if (isVerifyUser)
                            {
                                User user = await userRepository.Get(chatId);

                                user.PhoneNumber = phone;
                                user.IsVerify = true;

                                await userRepository.Update(user);
                                await _telegramBotClient.SendMessage(chatId, ReplyText._verifySucsess, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeUser());

                                return;
                            }

                            RegisterUser registerUser = await ApplicationApi.RegisterUserOTPAsync(chatId, text);

                            if (!registerUser.Status)
                            {
                                _logger.Information($"User {chatId} RegisterUser Status False");

                                await _telegramBotClient.SendMessage(chatId, ReplyText._timeLater, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeUser());
                                return;
                            }

                            CashSession cashSession = new();
                            cashSession.keyValuePairs.Add("CodeToken", registerUser.CodeToken);
                            cashSession.keyValuePairs.Add("Phone", phone);
                            CashManager.AddOrUpdate(chatId, cashSession);

                            await userStepRepository.Remove(userStep);
                            await userStepRepository.Create(new()
                            {
                                ChatId = chatId,
                                ExpierTime = DateTime.Now.AddMinutes(_appSettings.TimeOutMinute),
                                Step = "Code"
                            });

                            _logger.Information($"User {chatId} Send Otp Code");

                            await _telegramBotClient.SendMessage(chatId, ReplyText._codeOtp, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.Back());
                        }
                        else if (userStep.Step == "Code")
                        {
                            string code = text.Replace(" ", "");

                            bool isValid = int.TryParse(code, out _);

                            if (!isValid || code.Length != 5)
                            {
                                _logger.Information($"User {chatId} Code Invalid");

                                await userStepRepository.Remove(userStep);
                                await _telegramBotClient.SendMessage(chatId, ReplyText._invalidCode, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeUser());
                                return;
                            }

                            CashSession? cashSession = CashManager.Get(chatId);

                            if (cashSession is null)
                            {
                                await _telegramBotClient.SendMessage(chatId, ReplyText._timeLater, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeUser());
                                return;
                            }

                            cashSession.keyValuePairs.TryGetValue("CodeToken", out string? codeToken);
                            cashSession.keyValuePairs.TryGetValue("Phone", out string? phone);

                            if (string.IsNullOrEmpty(codeToken) || string.IsNullOrEmpty(phone))
                            {
                                await _telegramBotClient.SendMessage(chatId, ReplyText._timeLater, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeUser());
                                return;
                            }

                            bool verifyState = await ApplicationApi.VerifyUserOTPAsync(chatId, phone, code, codeToken);

                            if (!verifyState)
                            {
                                await _telegramBotClient.SendMessage(chatId, ReplyText._invalidCode, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeUser());
                                return;
                            }

                            User user = await userRepository.Get(chatId);

                            user.PhoneNumber = phone;
                            user.IsVerify = true;

                            await userRepository.Update(user);

                            await userStepRepository.Remove(userStep);
                            CashManager.Remove(chatId);

                            _logger.Information($"User {chatId} Verify Sucsess");

                            await _telegramBotClient.SendMessage(chatId, ReplyText._verifySucsess, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeUser());
                        }
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "❌ Error");
        }
    }

    private async Task OnListCountry(Message message, long chatId)
    {
        int messageId = message.Id;

        ISudoRepository sudoRepository = _serviceProvider.GetRequiredService<ISudoRepository>();
        IVirtualNumberDetailsRepository virtualNumberDetailsRepository = _serviceProvider.GetRequiredService<IVirtualNumberDetailsRepository>();

        bool anySudo = await sudoRepository.Any(chatId);

        if (!anySudo)
        {
            return;
        }

        _logger.Information($"Sudo {chatId} 📋 لیست کشور");

        IEnumerable<VirtualNumberDetails> countrys = await virtualNumberDetailsRepository.GetAll();

        await _telegramBotClient.SendMessage(chatId, ReplyText._countryList, replyParameters: messageId, replyMarkup: ReplyKeyboard.CountryList(countrys));
    }

    private async Task OnRules(Message message, long chatId)
    {
        int messageId = message.Id;

        IUserRepository userRepository = _serviceProvider.GetRequiredService<IUserRepository>();

        bool anyUser = await userRepository.Any(chatId);

        if (!anyUser)
        {
            return;
        }

        _logger.Information($"User {chatId} 🔖 قوانین");

        User user = await userRepository.Get(chatId);

        if (!user.IsVerify)
        {
            IUserStepRepository userStepRepository = _serviceProvider.GetRequiredService<IUserStepRepository>();

            _logger.Information($"User {chatId} Not Verify");

            bool anyStep = await userStepRepository.Any(chatId);

            if (anyStep)
            {
                UserStep userStep = await userStepRepository.Get(chatId);
                await userStepRepository.Remove(userStep);
            }

            await userStepRepository.Create(new()
            {
                ChatId = chatId,
                ExpierTime = DateTime.Now.AddMinutes(_appSettings.TimeOutMinute),
                Step = "Phone"
            });

            await _telegramBotClient.SendMessage(chatId, ReplyText._sendPhone, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeUser());

            return;
        }

        await _telegramBotClient.SendMessage(chatId, ReplyText._rules, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.Support(_appSettings.UsernameSupport));

    }

    private async Task OnSupport(Message message, long chatId)
    {
        int messageId = message.Id;

        IUserRepository userRepository = _serviceProvider.GetRequiredService<IUserRepository>();

        bool anyUser = await userRepository.Any(chatId);

        if (!anyUser)
        {
            return;
        }

        _logger.Information($"User {chatId} ☎️ پشتیبانی");

        User user = await userRepository.Get(chatId);

        if (!user.IsVerify)
        {
            IUserStepRepository userStepRepository = _serviceProvider.GetRequiredService<IUserStepRepository>();

            _logger.Information($"User {chatId} Not Verify");

            bool anyStep = await userStepRepository.Any(chatId);

            if (anyStep)
            {
                UserStep userStep = await userStepRepository.Get(chatId);
                await userStepRepository.Remove(userStep);
            }

            await userStepRepository.Create(new()
            {
                ChatId = chatId,
                ExpierTime = DateTime.Now.AddMinutes(_appSettings.TimeOutMinute),
                Step = "Phone"
            });

            await _telegramBotClient.SendMessage(chatId, ReplyText._sendPhone, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeUser());

            return;
        }

        await _telegramBotClient.SendMessage(chatId, ReplyText._support, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.Support(_appSettings.UsernameSupport));
    }

    private async Task OnInfo(Message message, long chatId)
    {
        int messageId = message.Id;

        IUserRepository userRepository = _serviceProvider.GetRequiredService<IUserRepository>();

        bool anyUser = await userRepository.Any(chatId);

        if (!anyUser)
        {
            return;
        }

        _logger.Information($"User {chatId} 👤 حساب کاربری");

        User user = await userRepository.Get(chatId);

        if (!user.IsVerify)
        {
            IUserStepRepository userStepRepository = _serviceProvider.GetRequiredService<IUserStepRepository>();

            _logger.Information($"User {chatId} Not Verify");

            bool anyStep = await userStepRepository.Any(chatId);

            if (anyStep)
            {
                UserStep userStep = await userStepRepository.Get(chatId);
                await userStepRepository.Remove(userStep);
            }

            await userStepRepository.Create(new()
            {
                ChatId = chatId,
                ExpierTime = DateTime.Now.AddMinutes(_appSettings.TimeOutMinute),
                Step = "Phone"
            });

            await _telegramBotClient.SendMessage(chatId, ReplyText._sendPhone, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeUser());

            return;
        }

        WalletUser walletUser = await ApplicationApi.WalletUserAsync(chatId);
        await _telegramBotClient.SendMessage(chatId, string.Format(ReplyText._informashion, chatId, walletUser.Wallet), parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeUser());
    }

    private async Task OnStart(Message message, long chatId)
    {
        int messageId = message.Id;

        ISudoRepository sudoRepository = _serviceProvider.GetRequiredService<ISudoRepository>();

        bool anySudo = await sudoRepository.Any(chatId);

        if (anySudo)
        {
            _logger.Information($"Sudo {chatId} /start Bot");

            await _telegramBotClient.SendMessage(chatId, ReplyText._start, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeSudo());
            return;
        }

        IUserRepository userRepository = _serviceProvider.GetRequiredService<IUserRepository>();

        bool anyUser = await userRepository.Any(chatId);

        if (!anyUser)
        {
            await userRepository.Add(new()
            {
                ChatId = chatId,
                IsVerify = false
            });

            _logger.Information($"User {chatId} Add To Database");
        }

        await _telegramBotClient.SendMessage(chatId, ReplyText._start, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeUser());
    }
}
