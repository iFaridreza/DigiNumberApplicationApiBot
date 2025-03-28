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
                            .AddButtons(["➖ کسر موجودی", "➕ افزایش موجودی"]).AddNewRow()
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

    internal static InlineKeyboardMarkup Support(string username)
    {
        InlineKeyboardMarkup inlineKeyboardMarkup = new();

        inlineKeyboardMarkup.AddButton(new() { Text = "👮 پشتیبانی", Url = $"https://t.me/{username}" });

        return inlineKeyboardMarkup;
    }

    internal static InlineKeyboardMarkup CountryList(IEnumerable<VirtualNumberDetails> virtualNumberDetails)
    {
        InlineKeyboardMarkup inlineKeyboardMarkup = new();

        inlineKeyboardMarkup.AddButton(new InlineKeyboardButton()
        {
            Text = "🌎",
            CallbackData = "Alert"
        });

        inlineKeyboardMarkup.AddButton(new InlineKeyboardButton()
        {
            Text = "🏁",
            CallbackData = "Alert"
        });

        inlineKeyboardMarkup.AddButton(new InlineKeyboardButton()
        {
            Text = "💰",
            CallbackData = "Alert"
        }).AddNewRow();

        foreach (var item in virtualNumberDetails)
        {
            inlineKeyboardMarkup.AddButton(item.CountryName);
            inlineKeyboardMarkup.AddButton($"{item.Flag} {item.CountryCode}");
            inlineKeyboardMarkup.AddButton($"{item.Price.ToString("0,000")}");
        }

        inlineKeyboardMarkup.AddButton(new InlineKeyboardButton()
        {
            Text = "افزودن کشور ➕",
            CallbackData = "AddCountry"
        });
        return inlineKeyboardMarkup;
    }
}