using DigiNumberApplicationApi.TelegramBot.Core.Domian;
using Telegram.Bot.Types.ReplyMarkups;

namespace DigiNumberApplicationApi.TelegramBot.Common;
public static class ReplyKeyboard
{
    internal static ReplyKeyboardMarkup HomeUser()
    {
        ReplyKeyboardMarkup replyKeyboardMarkup = new();

        replyKeyboardMarkup.AddButtons(["🛍 خرید شماره مجازی"]).AddNewRow()
                            .AddButtons(["👤 حساب کاربری", "♻️ استعلام شماره"]).AddNewRow()
                            .AddButtons(["☎️ پشتیبانی", "🔖 قوانین"]);

        replyKeyboardMarkup.ResizeKeyboard = true;

        return replyKeyboardMarkup;
    }

    internal static ReplyKeyboardMarkup HomeSudo()
    {
        ReplyKeyboardMarkup replyKeyboardMarkup = new();

        replyKeyboardMarkup.AddButtons(["📥 آپلود سشن", "📊 گزارش سشن"]).AddNewRow()
                            .AddButtons(["📈 گزارش ربات", "👤 گزارش کاربر"]).AddNewRow()
                            .AddButton("📋 لیست کشور");

        replyKeyboardMarkup.ResizeKeyboard = true;

        return replyKeyboardMarkup;
    }

    internal static ReplyKeyboardMarkup Back()
    {
        ReplyKeyboardMarkup replyKeyboardMarkup = new();

        replyKeyboardMarkup.AddButtons(["🔙"]);

        replyKeyboardMarkup.ResizeKeyboard = true;

        return replyKeyboardMarkup;
    }

    internal static ReplyKeyboardRemove Remove() => new ReplyKeyboardRemove();

    internal static InlineKeyboardMarkup Support(string username)
    {
        InlineKeyboardMarkup inlineKeyboardMarkup = new();

        inlineKeyboardMarkup.AddButton(new() { Text = "👮 پشتیبانی", Url = $"https://t.me/{username}" });

        return inlineKeyboardMarkup;
    }

    internal static InlineKeyboardMarkup Payment(string url)
    {
        InlineKeyboardMarkup inlineKeyboardMarkup = new();

        inlineKeyboardMarkup.AddButton(new() { Text = "💵 درگاه پرداخت", Url = url });

        return inlineKeyboardMarkup;
    }

    internal static InlineKeyboardMarkup CountryListEditor(IEnumerable<VirtualNumberDetails> virtualNumberDetails)
    {
        InlineKeyboardMarkup inlineKeyboardMarkup = new();

        inlineKeyboardMarkup.AddButton(new()
        {
            Text = "🌎",
            CallbackData = "Alert"
        });

        inlineKeyboardMarkup.AddButton(new()
        {
            Text = "🏁",
            CallbackData = "Alert"
        });

        inlineKeyboardMarkup.AddButton(new()
        {
            Text = "💰",
            CallbackData = "Alert"
        });

        inlineKeyboardMarkup.AddButton(new()
        {
            Text = "❌",
            CallbackData = "Alert"
        }).AddNewRow();

        foreach (var item in virtualNumberDetails)
        {
            inlineKeyboardMarkup.AddButton(item.CountryName);
            inlineKeyboardMarkup.AddButton($"{item.Flag} {item.CountryCode}");
            inlineKeyboardMarkup.AddButton($"{item.Price.ToString("0,000")}");
            inlineKeyboardMarkup.AddButton(new()
            {
                Text = "🗑",
                CallbackData = $"Remove_{item.CountryCode}"
            }).AddNewRow();
        }

        inlineKeyboardMarkup.AddButton(new()
        {
            Text = "افزودن کشور ➕",
            CallbackData = "AddCountry"
        });

        return inlineKeyboardMarkup;
    }

    internal static InlineKeyboardMarkup CountryListAvailable(IEnumerable<VirtualNumberDetails> virtualNumberDetails)
    {
        InlineKeyboardMarkup inlineKeyboardMarkup = new();

        inlineKeyboardMarkup.AddButton(new()
        {
            Text = "🌎",
            CallbackData = "Alert"
        });

        inlineKeyboardMarkup.AddButton(new()
        {
            Text = "🏁",
            CallbackData = "Alert"
        });

        inlineKeyboardMarkup.AddButton(new()
        {
            Text = "💰",
            CallbackData = "Alert"
        }).AddNewRow();


        foreach (var item in virtualNumberDetails)
        {
            inlineKeyboardMarkup.AddButton(item.CountryName);
            inlineKeyboardMarkup.AddButton($"{item.Flag} {item.CountryCode}");
            inlineKeyboardMarkup.AddButton($"{item.Price.ToString("0,000")}").AddNewRow();
        }

        return inlineKeyboardMarkup;
    }

    internal static InlineKeyboardMarkup CountryListInfo(IEnumerable<(string CountryCode, int Count)> virtualNumber)
    {
        InlineKeyboardMarkup inlineKeyboardMarkup = new();

        inlineKeyboardMarkup.AddButton(new()
        {
            Text = "🌎",
            CallbackData = "Alert"
        });

        inlineKeyboardMarkup.AddButton(new()
        {
            Text = "📱",
            CallbackData = "Alert"
        }).AddNewRow();

        foreach (var item in virtualNumber)
        {
            inlineKeyboardMarkup.AddButton(item.CountryCode);
            inlineKeyboardMarkup.AddButton($"{item.Count}").AddNewRow();
        }

        return inlineKeyboardMarkup;
    }

    internal static InlineKeyboardMarkup VirtualNumberPanel(IEnumerable<VirtualNumberDetails> virtualNumberDetails)
    {
        InlineKeyboardMarkup inlineKeyboardMarkup = new();

        inlineKeyboardMarkup.AddButton(new()
        {
            Text = "🌎",
            CallbackData = "Alert"
        });


        inlineKeyboardMarkup.AddButton(new()
        {
            Text = "💰",
            CallbackData = "Alert"
        }).AddNewRow();


        foreach (var item in virtualNumberDetails)
        {
            inlineKeyboardMarkup.AddButton(new()
            {
                Text= $"{item.Flag} {item.CountryName}",
                CallbackData = $"Buye_{item.CountryCode}"
            });
            inlineKeyboardMarkup.AddButton($"{item.Price.ToString("0,000")}").AddNewRow();
        }

        return inlineKeyboardMarkup;
    }

    internal static InlineKeyboardMarkup VirtualNumberBuye(string number)
    {
        InlineKeyboardMarkup inlineKeyboardMarkup = new();

        inlineKeyboardMarkup.AddButton(new()
        {
            Text = "🔢 دریافت کد",
            CallbackData = $"GetCode_{number}"
        }).AddNewRow();


        inlineKeyboardMarkup.AddButton(new()
        {
            Text = "❌ لغو",
            CallbackData = $"Cancell_{number}"
        }).AddNewRow();


        return inlineKeyboardMarkup;
    }

}