﻿namespace DigiNumberApplicationApi.TelegramBot.Common;
public static class ReplyText
{
    public static readonly string _start = "👋🏻 سلام، به پنل اپلیکیشن شماره مجازی، دیجی نامبر خوش آمدید …\n\n😄 با اپلیکیشن دیجی نامبر به راحتی هرچه تمام می توانید شماره مجازی دریافت کنید، کاملا خودکار و بالاترین سرعت ممکن، کمترین قیمت و هــــــــزاران امکانات دیگر …\n\n⤵️ برای ادامه کار یک گزینه را انتخاب کنید:";
    public static readonly string _sendPhone = "📱 لطفا ابتدا شماره تلفن را به صورت زیر ارسال کنید\n\n❗️❗️چنانچه در اپلیکیشن دیجی نامبر حساب دارید برای یکسان بودن موجودی میتوانید از همان شماره اپلیکیشن استفاده کنید\n\n<blockquote>⚠️ 09150903333</blockquote>";
    public static readonly string _invalidPhone = "<blockquote>❌ خطا</blockquote>\n\nشماره ارسال شده صحیح نمیباشد";
    public static readonly string _invalidCode = "<blockquote>❌ خطا</blockquote>\n\nکد ارسال شده صحیح نمیباشد";
    public static readonly string _timeLater = "<blockquote>⚠️ توجه</blockquote>\n\nربات درحال بروزرسانی میباشد لطفا بعدا تلاش کنید";
    public static readonly string _userNotFound = "<blockquote>⚠️ توجه</blockquote>\n\nکاربر با ایدی عددی ارسال شده موجود نمیباشد";
    public static readonly string _notReciveLoginCode = "<blockquote>⚠️ توجه</blockquote>\n\nکد ورود به حساب دریافت نشد لطفا شماره خریداری شده را فقط در تلگرام نسخه <strong>پلاس مسنجر یا تلگراف</strong> وارد کنید❗️❗️";
    public static readonly string _userNotVerify = "<blockquote>⚠️ توجه</blockquote>\n\nکاربر با ایدی عددی ارسال شده وریفای نمیباشد";
    public static readonly string _firstSignup = "<blockquote>❌ خطا</blockquote>\n\nلطفا ابتدا در اپلیکیشن ثبت نام و احراز هویت کنید و سپس از خدمات ربات استفاده کنید";
    public static readonly string _sessionTerminate = "<blockquote>❌ خطا</blockquote>\n\nربات در حساب وجود ندارد ، توسط شما خارج شده است";
    public static readonly string _importSessionFeild = "<blockquote>❌ خطا</blockquote>\n\nآپلود سشن با خطا مواجه شد احتمالا سشن تکراری وجود دارد";
    public static readonly string _invalidPrice = "<blockquote>❌ خطا</blockquote>\n\nمبلغ وارد شده اشتباه میباشد";
    public static readonly string _invalidFlag = "<blockquote>❌ خطا</blockquote>\n\nپرچم ارسال شده صحیح نمیباشد";
    public static readonly string _invalidInput = "<blockquote>❌ خطا</blockquote>\n\nورودی ارسال شده صحیح نمیباشد";
    public static readonly string _countryCodeExists = "<blockquote>❌ خطا</blockquote>\n\nکد کشور ارسال شده موجود میباشد";
    public static readonly string _wite = "<blockquote>⚠️ توجه</blockquote>\n\nلطفا کمی صبر کنید";
    public static readonly string _numberDeleted = "<blockquote>⚠️ توجه</blockquote>\n\n❤️ شماره درخواستی شما از سمت تلگرام قادر به دریافت کد ورود نیست لطفا مجدد خرید را انجام دهید و مبلغ به حساب شما بازگشت";
    public static readonly string _notBalance = "<blockquote>⚠️ توجه</blockquote>\n\nموجودی شما برای خرید شماره مجازی کشور <strong>{0} {1}</strong> کافی نیست لطفا از درگاه پرداخت زیر نسبت به شارژ حساب خود اقدام کنید باتشکر";
    public static readonly string _notAvailbale = "<blockquote>⚠️ توجه</blockquote>\n\nسشن برای شماره مجازی کشور <strong>{0} {1}</strong> موجود نیست لطفا در زمان دیگری تلاش کنید باتشکر";
    public static readonly string _fileEmpty = "<blockquote>⚠️ توجه</blockquote>\n\nفایل حاوی سشن نمیباشد";
    public static readonly string _verifySucsess = "<blockquote>✅ موفق</blockquote>\n\nثبت نام شما با موفقیت انجام شد";
    public static readonly string _importSessionSucsessfully = "<blockquote>✅ موفق</blockquote>\n\nافزودن سشن با موفقیت انجام شد";
    public static readonly string _reportSession = "<blockquote>✅ موفق</blockquote>\n\nگزارش سشن به شرح زیر میباشد";
    public static readonly string _cancell = "<blockquote>✅ موفق</blockquote>\n\nعملیات با موفقیت لغو و کنسل شد";
    public static readonly string _reportBot = "<blockquote>✅ گزارش ربات به شرح زیر میباشد</blockquote>\n\n👥 تعداد کاربر ها: <strong>{0}</strong>\n\n💰 مجموع درامد ربات: <strong>{1}</strong> تومان";
    public static readonly string _getLoginCode = "<blockquote>✅ موفق</blockquote>\n\nکد ورود با موفقیت دریافت شد❤️\n\n📱 شماره: <strong>{0}</strong>\n\n🔢 کد ورود: <code>{1}</code>\n\n🔑 در صورت نیاز به رمز دو مرحله ای میتوانید از رمز عبور زیر استفاده کنید\n<code>{2}</code>";
    public static readonly string _logoutSecsess = "<blockquote>✅ موفق</blockquote>\n\nخروج از حساب با موفقیت انجام شد";
    public static readonly string _countryCodeAdd = "<blockquote>✅ موفق</blockquote>\n\nثبت کشور <strong>{0} {1}</strong> با موفقیت انجام شد";
    public static readonly string _showPanelLoginCode = "<blockquote>✅ موفق</blockquote>\n\nخرید با موفقیت انجام شد ✨\n\n☎️ شماره خریداری شده: <code>{0}</code>\n\n🌎 کشور: <strong>{1} {2}</strong>\n\n💰 مبلغ: <strong>{3} تومان</strong>\n\nشماره خریداری شده را فقط در تلگرام نسخه <strong>پلاس مسنجر یا تلگراف</strong> وارد کنید❗️❗️";
    public static readonly string _informashion = "<blockquote>👤 اطلاعات حساب</blockquote>\n\n✨ آیدی عددی <code>{0}</code> \n\n💰 موجودی <code>{1}</code> تومان";
    public static readonly string _codeOtp = "📮 لطفا کد sms شده را جهت احراز هویت ارسال کنید";
    public static readonly string _alert = "❤️ این دکمه صرفا جهت نمایش است";
    public static readonly string _userChatId = "👤 لطفا ایدی عددی کاربر مورد نظر را ارسال کنید";
    public static readonly string _sendFileSession = "📁 لطفا فایل سشن را ارسال کنید و دقت کنید فرمت فایل Zip باشد";
    public static readonly string _support = "👮 جهت ارتباط با پشتیبانی دکمه زیر را لمس کنید";
    public static readonly string _backHome = "🏘 به خانه برگشتید";
    public static readonly string _countryList = "🌐 لیست کشور های ثبت شده برای فروش به شرح زیر میباشد";
    public static readonly string _countryListBuy = "🌐 لیست کشور های قابل فروش به شرح زیر میباشد\n\nلطفا برای خرید روی نام کشور کلیک کنید👇";
    public static readonly string _countryName = "<blockquote>⚠️ توجه</blockquote>\n\n👇 لطفا <strong>نام</strong> کشور را ارسال کنید\n\nهمین نام در پنل قابل مشاهده است";
    public static readonly string _countryFlag = "<blockquote>⚠️ توجه</blockquote>\n\n👇 لطفا <strong>پرچم</strong> کشور را ارسال کنید\n\nهمین نام در پنل قابل مشاهده است";
    public static readonly string _countryPrice = "<blockquote>⚠️ توجه</blockquote>\n\n👇 لطفا <strong>قیمت</strong> کشور را ارسال کنید\n\nهمین نام در پنل قابل مشاهده است";
    public static readonly string _countryCode = "<blockquote>⚠️ توجه</blockquote>\n\n👇 لطفا <strong>کد</strong> کشور را ارسال کنید\n\nهمین نام در پنل قابل مشاهده است";
    public static readonly string _rules = "<blockquote>🌹 لطفا قبل از خرید ، قوانین را مطالعه کنید</blockquote>\n\n1️⃣ لطفا طبق نیاز حساب خود را شارژ کنید برگشت مبلغ امکان پذیر نمیباشد\n\n2️⃣ لطفا حتما برای دریافت شماره مجازی از اپلیکیشن های <strong>تلگراف یا پلاس مسنجر</strong> استفاده کنید در غیر این صورت ربات مسئولیت خراب شدن شماره مجازی را نمیپذیرد\n\n3️⃣ ";
    public static readonly string _logOrder = "<blockquote>✅ گزارش خرید موفق</blockquote>\n\n👤کاربر: <strong>{0}</strong>\n\n📱شماره: <code>{1}</code>\n\n🏳️کشور: <strong>{2}</strong>\n\n💰قیمت: <strong>{3}</strong> تومان\n\n📅تاریخ: <strong>{4}</strong>\n\nدیجی نامبر فروش تخصصی شماره و خدمات مجازی تلگرام ❤️";
} 