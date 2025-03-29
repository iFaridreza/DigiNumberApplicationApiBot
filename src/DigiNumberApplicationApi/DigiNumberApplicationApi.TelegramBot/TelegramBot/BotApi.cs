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
using DigiNumberApplicationApi.TelegramBot.Core.Domian;
using System.IO.Compression;
using User = DigiNumberApplicationApi.TelegramBot.Core.Domian.User;

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
            IVirtualNumberDetailsRepository virtualNumberDetailsRepository = _serviceProvider.GetRequiredService<IVirtualNumberDetailsRepository>();
            IVirtualNumberRepository virtualNumberRepository = _serviceProvider.GetRequiredService<IVirtualNumberRepository>();
            IVirtualSessionDetailsRepository virtualSessionDetailsRepository = _serviceProvider.GetRequiredService<IVirtualSessionDetailsRepository>();


            bool anySudo = await sudoRepository.Any(chatId);

            if (anySudo)
            {
                if (data.Equals("AddCountry"))
                {
                    _logger.Information($"Sudo {chatId} افزودن کشور ➕");

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
                        Step = "CountryCode"
                    });

                    await _telegramBotClient.DeleteMessage(chatId, messageId);
                    await _telegramBotClient.SendMessage(chatId, ReplyText._countryCode, parseMode: ParseMode.Html, replyMarkup: ReplyKeyboard.Back());
                }
                else if (data.Contains("Remove_"))
                {
                    string countryCode = data.Replace("Remove_", "").Replace(" ", "");

                    bool anyCountryCode = await virtualNumberDetailsRepository.Any(countryCode);

                    if (!anyCountryCode)
                    {
                        return;
                    }

                    VirtualNumberDetails virtualNumberDetails = await virtualNumberDetailsRepository.Get(countryCode);

                    await virtualNumberDetailsRepository.Remove(virtualNumberDetails);

                    IEnumerable<VirtualNumberDetails> countrys = await virtualNumberDetailsRepository.GetAll();


                    await _telegramBotClient.EditMessageText(chatId, messageId, ReplyText._countryList, replyMarkup: ReplyKeyboard.CountryListEditor(countrys));
                }
                return;
            }

            if (data.Contains("Buye_"))
            {
                string countryCode = data.Replace("Buye_", "").Replace(" ", "");

                bool anyCountryCode = await virtualNumberDetailsRepository.Any(countryCode);

                if (!anyCountryCode)
                {
                    return;
                }

                VirtualNumberDetails virtualNumberDetails = await virtualNumberDetailsRepository.Get(countryCode);

                bool walletBalance = await ApplicationApi.WalletInventoryAsync(chatId, virtualNumberDetails.Price);

                if (!walletBalance)
                {
                    RedirectPayment redierctPayment = await ApplicationApi.RedirectPaymentAsync(chatId, virtualNumberDetails.Price);
                    await _telegramBotClient.SendMessage(chatId, string.Format(ReplyText._notBalance, virtualNumberDetails.Flag, virtualNumberDetails.CountryName), parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.Payment(redierctPayment.Message));
                    return;
                }

                VirtualSessionDetails virtualSessionDetails = await virtualSessionDetailsRepository.Get();

                IEnumerable<VirtualNumber> virtualNumbers = await virtualNumberRepository.GetAll(EStatusOrder.Avilbale);

                if (virtualNumbers.Count() == default)
                {
                    _logger.Warning($"Session {countryCode} Not Availbale");
                    await _telegramBotClient.SendMessage(chatId, string.Format(ReplyText._notAvailbale, virtualNumberDetails.Flag, virtualNumberDetails.CountryName), parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeUser());
                    return;
                }

                Message msgWite = await _telegramBotClient.SendMessage(chatId, ReplyText._wite, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.Remove());

                WTelegramClientManager wTelegramClientManager = new(virtualSessionDetails.ApiId, virtualSessionDetails.ApiHash, Utils.GetSessionsDirPath(_appSettings.SessionPath));

                VirtualNumber? virtualNumber = null;

                foreach (var item in virtualNumbers)
                {
                    try
                    {
                        await wTelegramClientManager.Connect($"{item.CountryCode}{item.Number}");

                        wTelegramClientManager.Loging((_, msg) =>
                        {
                            _logger.Information(msg);
                        });


                        wTelegramClientManager.DisableUpdate();

                        virtualNumber = item;

                        item.EStatusOrder = EStatusOrder.Sold;

                        await virtualNumberRepository.Update(item);

                        await wTelegramClientManager.Disconnect();

                        break;
                    }
                    catch (Exception ex) when (
                                     ex.Message == "AUTH_KEY_UNREGISTERED" ||
                                     ex.Message == "USER_DEACTIVATED" ||
                                     ex.Message == "SESSION_REVOKED" ||
                                     ex.Message == "SESSION_EXPIRED" ||
                                     ex.Message == "USER_DEACTIVATED_BAN"
                                     )
                    {
                        item.EStatusOrder = EStatusOrder.Ban;

                        await virtualNumberRepository.Update(item);
                    }
                }

                if (virtualNumber is null)
                {
                    _logger.Warning($"Session {countryCode} Not Availbale");
                    await _telegramBotClient.DeleteMessage(chatId, msgWite.MessageId);
                    await _telegramBotClient.SendMessage(chatId, string.Format(ReplyText._notAvailbale, virtualNumberDetails.Flag, virtualNumberDetails.CountryName), parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeUser());
                    return;
                }

                await _telegramBotClient.DeleteMessage(chatId, msgWite.MessageId);
                await _telegramBotClient.SendMessage(chatId, string.Format(ReplyText._showPanelLoginCode, $"{virtualNumberDetails.CountryCode}{virtualNumber.Number}", virtualNumberDetails.Flag, virtualNumberDetails.CountryName, virtualNumberDetails.Price.ToString("0,000")), parseMode: ParseMode.Html, replyMarkup: ReplyKeyboard.VirtualNumberBuye(virtualNumber.Number));
            }
            else if (data.Contains("GetCode_"))
            {
                string number = data.Replace("GetCode_", "").Replace(" ", "");

                bool anyNumber = await virtualNumberRepository.Any(number);

                if (!anyNumber)
                {
                    return;
                }

                VirtualNumber virtualNumber = await virtualNumberRepository.Get(number);

                WTelegramClientManager wTelegramClientManager = new(virtualNumber.VirtualSessionDetails.ApiId, virtualNumber.VirtualSessionDetails.ApiHash, Utils.GetSessionsDirPath(_appSettings.SessionPath));

                try
                {
                    await wTelegramClientManager.Connect($"{virtualNumber.CountryCode}{virtualNumber.Number}");

                    wTelegramClientManager.Loging((_, msg) =>
                    {
                        _logger.Information(msg);
                    });

                    wTelegramClientManager.DisableUpdate();

                    const string telegramContact = "+42777";
                    string[] messages = await wTelegramClientManager.GetMessagesText(telegramContact, 1);

                    if (messages.Length == 0)
                    {
                        await _telegramBotClient.SendMessage(chatId, ReplyText._notReciveLoginCode, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.Remove());
                        return;
                    }

                    string? loginCode = Utils.GetLoginCode(messages[0]);

                    if (string.IsNullOrEmpty(loginCode))
                    {
                        await _telegramBotClient.SendMessage(chatId, ReplyText._notReciveLoginCode, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.Remove());
                        return;
                    }


                    await wTelegramClientManager.ChangeOrDisablePassword2Fa(_appSettings.Password2Fa,null);

                    await _telegramBotClient.DeleteMessage(chatId, messageId);
                    await _telegramBotClient.SendMessage(chatId, string.Format(ReplyText._getLoginCode, $"{virtualNumber.CountryCode}{virtualNumber.Number}", loginCode, _appSettings.Password2Fa), parseMode: ParseMode.Html, replyMarkup: ReplyKeyboard.HomeUser());
                
                }
                catch (Exception ex) when (
                                 ex.Message == "AUTH_KEY_UNREGISTERED" ||
                                 ex.Message == "USER_DEACTIVATED" ||
                                 ex.Message == "SESSION_REVOKED" ||
                                 ex.Message == "SESSION_EXPIRED" ||
                                 ex.Message == "USER_DEACTIVATED_BAN"
                                 )
                {
                    virtualNumber.EStatusOrder = EStatusOrder.Ban;
                    await virtualNumberRepository.Update(virtualNumber);

                    await _telegramBotClient.DeleteMessage(chatId, messageId);
                    await _telegramBotClient.SendMessage(chatId, ReplyText._numberDeleted, parseMode: ParseMode.Html, replyMarkup: ReplyKeyboard.HomeUser());

                }
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

            if (message.Type == MessageType.Text)
            {
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
                            await OnAvailableCountry(message, chatId);
                        }
                        break;
                    case "🛍 خرید شماره مجازی":
                        {
                            await OnBuyeVirtualNunber(message, chatId);
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
                    case "👤 گزارش کاربر":
                        {
                            await OnInfoUser(message, chatId);
                        }
                        break;
                    case "📥 آپلود سشن":
                        {
                            await OnUploadSession(message, chatId);
                        }
                        break;
                    case "📊 گزارش سشن":
                        {
                            await OnReportSession(message, chatId);
                        }
                        break;
                    default:
                        {
                            IUserStepRepository userStepRepository = _serviceProvider.GetRequiredService<IUserStepRepository>();
                            IUserRepository userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                            ISudoRepository sudoRepository = _serviceProvider.GetRequiredService<ISudoRepository>();
                            IVirtualNumberDetailsRepository virtualNumberDetailsRepository = _serviceProvider.GetRequiredService<IVirtualNumberDetailsRepository>();

                            bool anyStep = await userStepRepository.Any(chatId);

                            if (!anyStep)
                            {
                                return;
                            }

                            UserStep userStep = await userStepRepository.Get(chatId);

                            string step = userStep.Step;

                            if (step == "Phone")
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
                                    await userStepRepository.Remove(userStep);
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
                            else if (step == "Code")
                            {
                                string code = text.Replace(" ", "");

                                bool isValid = int.TryParse(code, out _);

                                if (!isValid || code.Length != 5)
                                {
                                    _logger.Information($"User {chatId} Code Invalid");
                                    CashManager.Remove(chatId);

                                    await userStepRepository.Remove(userStep);
                                    await _telegramBotClient.SendMessage(chatId, ReplyText._invalidCode, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeUser());
                                    return;
                                }

                                CashSession? cashSession = CashManager.Get(chatId);

                                if (cashSession is null)
                                {
                                    CashManager.Remove(chatId);
                                    await userStepRepository.Remove(userStep);
                                    await _telegramBotClient.SendMessage(chatId, ReplyText._timeLater, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeUser());
                                    return;
                                }

                                cashSession.keyValuePairs.TryGetValue("CodeToken", out string? codeToken);
                                cashSession.keyValuePairs.TryGetValue("Phone", out string? phone);

                                if (string.IsNullOrEmpty(codeToken) || string.IsNullOrEmpty(phone))
                                {
                                    CashManager.Remove(chatId);
                                    await userStepRepository.Remove(userStep);
                                    await _telegramBotClient.SendMessage(chatId, ReplyText._timeLater, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeUser());
                                    return;
                                }

                                bool verifyState = await ApplicationApi.VerifyUserOTPAsync(chatId, phone, code, codeToken);

                                if (!verifyState)
                                {
                                    CashManager.Remove(chatId);
                                    await userStepRepository.Remove(userStep);
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
                            else if (step == "CountryCode")
                            {
                                _logger.Information($"Sudo {chatId} {text}");

                                bool anyCountryCode = await virtualNumberDetailsRepository.Any(text);

                                if (anyCountryCode)
                                {
                                    CashManager.Remove(chatId);
                                    await userStepRepository.Remove(userStep);
                                    await _telegramBotClient.SendMessage(chatId, ReplyText._countryCodeExists, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeSudo());
                                    return;
                                }

                                CashSession cashSession = new();
                                cashSession.keyValuePairs.Add("CountryCode", text);

                                CashManager.AddOrUpdate(chatId, cashSession);

                                await userStepRepository.Remove(userStep);

                                await userStepRepository.Create(new()
                                {
                                    ChatId = chatId,
                                    ExpierTime = DateTime.Now.AddMinutes(_appSettings.TimeOutMinute),
                                    Step = "CountryName"
                                });

                                await _telegramBotClient.SendMessage(chatId, ReplyText._countryName, parseMode: ParseMode.Html, replyMarkup: ReplyKeyboard.Back());

                            }
                            else if (step == "CountryName")
                            {
                                _logger.Information($"Sudo {chatId} {text}");

                                CashSession? cashSession = CashManager.Get(chatId);

                                if (cashSession is null)
                                {
                                    CashManager.Remove(chatId);
                                    await userStepRepository.Remove(userStep);
                                    await _telegramBotClient.SendMessage(chatId, ReplyText._timeLater, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeSudo());
                                    return;
                                }

                                cashSession.keyValuePairs.Add("CountryName", text);
                                CashManager.AddOrUpdate(chatId, cashSession);

                                await userStepRepository.Remove(userStep);

                                await userStepRepository.Create(new()
                                {
                                    ChatId = chatId,
                                    ExpierTime = DateTime.Now.AddMinutes(_appSettings.TimeOutMinute),
                                    Step = "CountryFlag"
                                });

                                await _telegramBotClient.SendMessage(chatId, ReplyText._countryFlag, parseMode: ParseMode.Html, replyMarkup: ReplyKeyboard.Back());
                            }
                            else if (step == "CountryFlag")
                            {
                                _logger.Information($"Sudo {chatId} {text}");

                                bool isFlag = Utils.IsFlagCountry(text);

                                if (!isFlag)
                                {
                                    CashManager.Remove(chatId);
                                    await userStepRepository.Remove(userStep);
                                    await _telegramBotClient.SendMessage(chatId, ReplyText._invalidFlag, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeSudo());
                                    return;
                                }

                                CashSession? cashSession = CashManager.Get(chatId);

                                if (cashSession is null)
                                {
                                    CashManager.Remove(chatId);
                                    await userStepRepository.Remove(userStep);
                                    await _telegramBotClient.SendMessage(chatId, ReplyText._timeLater, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeSudo());
                                    return;
                                }

                                cashSession.keyValuePairs.Add("CountryFlag", text);

                                CashManager.AddOrUpdate(chatId, cashSession);

                                await userStepRepository.Remove(userStep);

                                await userStepRepository.Create(new()
                                {
                                    ChatId = chatId,
                                    ExpierTime = DateTime.Now.AddMinutes(_appSettings.TimeOutMinute),
                                    Step = "CountryPrice"
                                });

                                await _telegramBotClient.SendMessage(chatId, ReplyText._countryPrice, parseMode: ParseMode.Html, replyMarkup: ReplyKeyboard.Back());
                            }
                            else if (step == "CountryPrice")
                            {
                                _logger.Information($"Sudo {chatId} {text}");

                                if (!decimal.TryParse(text, out decimal countryPrice))
                                {
                                    CashManager.Remove(chatId);
                                    await userStepRepository.Remove(userStep);
                                    await _telegramBotClient.SendMessage(chatId, ReplyText._invalidPrice, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeSudo());
                                    return;
                                }

                                CashSession? cashSession = CashManager.Get(chatId);

                                if (cashSession is null)
                                {
                                    CashManager.Remove(chatId);
                                    await userStepRepository.Remove(userStep);
                                    await _telegramBotClient.SendMessage(chatId, ReplyText._timeLater, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeSudo());
                                    return;
                                }

                                cashSession.keyValuePairs.TryGetValue("CountryName", out string? countryName);
                                cashSession.keyValuePairs.TryGetValue("CountryFlag", out string? countryFlag);
                                cashSession.keyValuePairs.TryGetValue("CountryCode", out string? countryCode);

                                if (string.IsNullOrEmpty(countryName) ||
                                   string.IsNullOrEmpty(countryFlag) ||
                                   string.IsNullOrEmpty(countryCode))
                                {
                                    CashManager.Remove(chatId);
                                    await userStepRepository.Remove(userStep);
                                    await _telegramBotClient.SendMessage(chatId, ReplyText._timeLater, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeSudo());
                                    return;
                                }

                                await userStepRepository.Remove(userStep);

                                await virtualNumberDetailsRepository.Add(new()
                                {
                                    CountryCode = countryCode,
                                    CountryName = countryName,
                                    Flag = countryFlag,
                                    Price = countryPrice
                                });

                                IEnumerable<VirtualNumberDetails> countrys = await virtualNumberDetailsRepository.GetAll();

                                await _telegramBotClient.SendMessage(chatId, string.Format(ReplyText._countryCodeAdd, countryFlag, countryName), parseMode: ParseMode.Html, replyMarkup: ReplyKeyboard.CountryListEditor(countrys));
                                await _telegramBotClient.SendMessage(chatId, ReplyText._backHome, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeSudo());
                            }
                            else if (step == "UserChatId")
                            {
                                _logger.Information($"Sudo {chatId} {text}");

                                if (!long.TryParse(text, out long userChatId))
                                {
                                    CashManager.Remove(chatId);
                                    await userStepRepository.Remove(userStep);
                                    await _telegramBotClient.SendMessage(chatId, ReplyText._invalidInput, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeSudo());
                                    return;
                                }

                                bool anyUser = await userRepository.Any(userChatId);

                                if (!anyUser)
                                {
                                    CashManager.Remove(chatId);
                                    await userStepRepository.Remove(userStep);
                                    await _telegramBotClient.SendMessage(chatId, ReplyText._userNotFound, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeSudo());
                                    return;
                                }

                                User user = await userRepository.Get(userChatId);

                                if (!user.IsVerify)
                                {
                                    CashManager.Remove(chatId);
                                    await userStepRepository.Remove(userStep);
                                    await _telegramBotClient.SendMessage(chatId, ReplyText._userNotVerify, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeSudo());
                                    return;
                                }

                                WalletUser walletUser = await ApplicationApi.WalletUserAsync(userChatId);
                                await _telegramBotClient.SendMessage(chatId, string.Format(ReplyText._informashion, userChatId, walletUser.Wallet), parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeSudo());
                            }
                        }
                        break;
                }
            }
            else if (message.Type == MessageType.Document)
            {
                await OnDocument(message, chatId);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "❌ Error");
        }
    }

    private async Task OnBuyeVirtualNunber(Message message, long chatId)
    {
        int messageId = message.Id;

        IUserRepository userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        IVirtualNumberDetailsRepository virtualNumberDetailsRepository = _serviceProvider.GetRequiredService<IVirtualNumberDetailsRepository>();

        bool anyUser = await userRepository.Any(chatId);

        if (!anyUser)
        {
            return;
        }

        _logger.Information($"User {chatId} 🛍 خرید شماره مجازی");

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

        IEnumerable<VirtualNumberDetails> virtualNumberDetails = await virtualNumberDetailsRepository.GetAll();

        await _telegramBotClient.SendMessage(chatId, ReplyText._countryListBuy, parseMode: ParseMode.Html, replyParameters: messageId, replyMarkup: ReplyKeyboard.VirtualNumberPanel(virtualNumberDetails));
    }

    private async Task OnReportSession(Message message, long chatId)
    {
        int messageId = message.Id;

        ISudoRepository sudoRepository = _serviceProvider.GetRequiredService<ISudoRepository>();
        IVirtualNumberRepository virtualNumberRepository = _serviceProvider.GetRequiredService<IVirtualNumberRepository>();

        bool anySudo = await sudoRepository.Any(chatId);

        if (!anySudo)
        {
            return;
        }

        _logger.Information($"Sudo {chatId} 📥 آپلود سشن");

        IEnumerable<VirtualNumber> virtualNumbers = await virtualNumberRepository.GetAll(EStatusOrder.Avilbale);

        var filtersCountryCode = virtualNumbers.GroupBy(x => x.CountryCode)
          .Select(x => (x.Key, x.Count())).ToList();

        await _telegramBotClient.SendMessage(chatId, ReplyText._reportSession, parseMode: ParseMode.Html, replyMarkup: ReplyKeyboard.CountryListInfo(filtersCountryCode));
        await _telegramBotClient.SendMessage(chatId, ReplyText._backHome, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeSudo());
    }

    private async Task OnDocument(Message message, long chatId)
    {
        int messageId = message.Id;

        ISudoRepository sudoRepository = _serviceProvider.GetRequiredService<ISudoRepository>();
        IUserStepRepository userStepRepository = _serviceProvider.GetRequiredService<IUserStepRepository>();
        IVirtualSessionDetailsRepository virtualSessionDetailsRepository = _serviceProvider.GetRequiredService<IVirtualSessionDetailsRepository>();
        IVirtualNumberRepository virtualNumberRepository = _serviceProvider.GetRequiredService<IVirtualNumberRepository>();

        bool anySudo = await sudoRepository.Any(chatId);

        if (!anySudo)
        {
            return;
        }

        bool anyStep = await userStepRepository.Any(chatId);

        if (!anyStep)
        {
            return;
        }

        UserStep userStep = await userStepRepository.Get(chatId);

        if (userStep.Step != "UploadSession")
        {
            return;
        }

        Document? document = message.Document;

        if (document is null) return;

        if (document.MimeType != "application/zip")
        {
            await userStepRepository.Remove(userStep);
            await _telegramBotClient.SendMessage(chatId, ReplyText._invalidInput, replyParameters: messageId, replyMarkup: ReplyKeyboard.Back()); await _telegramBotClient.SendMessage(chatId, ReplyText._userChatId, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeSudo());
            return;
        }

        string? fileName = document.FileName;
        TGFile file = await _telegramBotClient.GetFile(document.FileId);

        if (fileName is null || file.FilePath is null) return;

        Message msgWite = await _telegramBotClient.SendMessage(chatId, ReplyText._wite, parseMode: ParseMode.Html, replyMarkup: ReplyKeyboard.Remove());

        using FileStream fileStream = new(fileName, FileMode.Create);
        await _telegramBotClient.DownloadFile(file.FilePath, fileStream);
        ZipArchive zipArchive = new(fileStream);

        List<ZipArchiveEntry> infoSession = zipArchive.Entries.ToList();

        int countSession = infoSession.Count(x => x.FullName.Contains(".session"));

        if (infoSession.Count == default || countSession == default)
        {
            await _telegramBotClient.DeleteMessage(chatId, msgWite.MessageId);
            await _telegramBotClient.SendMessage(chatId, ReplyText._fileEmpty, parseMode: ParseMode.Html, replyMarkup: ReplyKeyboard.HomeSudo());
            fileStream.Close();
            File.Delete(fileName);
            return;
        }

        ICollection<InfoPhoneNumber> infoPhoneNumbers = new List<InfoPhoneNumber>();
        ICollection<VirtualNumber> virtualNumbers = new List<VirtualNumber>();

        VirtualSessionDetails virtualSessionDetails = await virtualSessionDetailsRepository.Get();

        string sessionsPath = Utils.GetSessionsDirPath(_appSettings.SessionPath);

        foreach (var item in infoSession)
        {
            string phoneNumber = item.FullName.Replace(".session", "");

            bool anyPhone = await virtualNumberRepository.Any(phoneNumber);

            if (anyPhone) continue;

            string sessionPath = Path.Combine(sessionsPath, $"{phoneNumber}.session");

            if (File.Exists(sessionPath)) continue;

            item.ExtractToFile(sessionPath);

            InfoPhoneNumber infoPhoneNumber = Utils.InfoPhoneNumber(phoneNumber);

            infoPhoneNumbers.Add(infoPhoneNumber);

            VirtualNumber virtualNumber = new()
            {
                Number = infoPhoneNumber.PhoneNumber,
                CountryCode = infoPhoneNumber.CodeCountry,
                EStatusOrder = EStatusOrder.Avilbale,
                VirtualSessionDetails = virtualSessionDetails
            };

            await virtualNumberRepository.Add(virtualNumber);
            virtualNumbers.Add(virtualNumber);
        }

        fileStream.Close();

        await _telegramBotClient.DeleteMessage(chatId, msgWite.MessageId);

        if (infoPhoneNumbers.Count == default)
        {
            await userStepRepository.Remove(userStep);
            await _telegramBotClient.SendMessage(chatId, string.Format(ReplyText._importSessionFeild), parseMode: ParseMode.Html, replyMarkup: ReplyKeyboard.HomeSudo());
            File.Delete(fileName);
            return;
        }

        var filtersCountryCode = virtualNumbers.GroupBy(x => x.CountryCode)
            .Select(x => (x.Key, x.Count())).ToList();

        await userStepRepository.Remove(userStep);

        await _telegramBotClient.SendMessage(chatId, string.Format(ReplyText._importSessionSucsessfully, infoPhoneNumbers.Count), parseMode: ParseMode.Html, replyMarkup: ReplyKeyboard.CountryListInfo(filtersCountryCode));
        await _telegramBotClient.SendMessage(chatId, ReplyText._backHome, replyParameters: messageId, replyMarkup: ReplyKeyboard.HomeSudo());

        File.Delete(fileName);
    }

    private async Task OnUploadSession(Message message, long chatId)
    {
        int messageId = message.Id;

        ISudoRepository sudoRepository = _serviceProvider.GetRequiredService<ISudoRepository>();
        IUserStepRepository userStepRepository = _serviceProvider.GetRequiredService<IUserStepRepository>();

        bool anySudo = await sudoRepository.Any(chatId);

        if (!anySudo)
        {
            return;
        }

        _logger.Information($"Sudo {chatId} 📥 آپلود سشن");

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
            Step = "UploadSession"
        });

        await _telegramBotClient.SendMessage(chatId, ReplyText._sendFileSession, replyParameters: messageId, replyMarkup: ReplyKeyboard.Back());
    }

    private async Task OnInfoUser(Message message, long chatId)
    {
        int messageId = message.Id;

        ISudoRepository sudoRepository = _serviceProvider.GetRequiredService<ISudoRepository>();
        IUserStepRepository userStepRepository = _serviceProvider.GetRequiredService<IUserStepRepository>();

        bool anySudo = await sudoRepository.Any(chatId);

        if (!anySudo)
        {
            return;
        }

        _logger.Information($"Sudo {chatId} 👤 گزارش کاربر");

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
            Step = "UserChatId"
        });

        await _telegramBotClient.SendMessage(chatId, ReplyText._userChatId, replyParameters: messageId, replyMarkup: ReplyKeyboard.Back());
    }

    private async Task OnAvailableCountry(Message message, long chatId)
    {
        int messageId = message.Id;

        IVirtualNumberDetailsRepository virtualNumberDetailsRepository = _serviceProvider.GetRequiredService<IVirtualNumberDetailsRepository>();
        IUserRepository userRepository = _serviceProvider.GetRequiredService<IUserRepository>();

        bool anyUser = await userRepository.Any(chatId);

        if (!anyUser)
        {
            return;
        }

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

        _logger.Information($"Sudo {chatId} ♻️ استعلام شماره");

        IEnumerable<VirtualNumberDetails> countrys = await virtualNumberDetailsRepository.GetAll();

        await _telegramBotClient.SendMessage(chatId, ReplyText._countryList, replyParameters: messageId, replyMarkup: ReplyKeyboard.CountryListAvailable(countrys));
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

        await _telegramBotClient.SendMessage(chatId, ReplyText._countryList, replyParameters: messageId, replyMarkup: ReplyKeyboard.CountryListEditor(countrys));
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