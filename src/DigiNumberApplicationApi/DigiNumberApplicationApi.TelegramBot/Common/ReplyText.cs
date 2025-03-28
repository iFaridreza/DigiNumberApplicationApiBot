namespace DigiNumberApplicationApi.TelegramBot.Common;
public static class ReplyText
{
    public static readonly string _start = "👋🏻 سلام، به پنل اپلیکیشن شماره مجازی، دیجی نامبر خوش آمدید …\n\n😄 با اپلیکیشن دیجی نامبر به راحتی هرچه تمام می توانید شماره مجازی دریافت کنید، کاملا خودکار و بالاترین سرعت ممکن، کمترین قیمت و هــــــــزاران امکانات دیگر …\n\n⤵️ برای ادامه کار یک گزینه را انتخاب کنید:";
    public static readonly string _sendPhone = "📱 لطفا ابتدا شماره تلفنی که در اپلیکشین ثبت نام کردید به صورت زیر ارسال کنید\n\n<blockquote>⚠️ 09150903333</blockquote>";
    public static readonly string _invalidPhone = "<blockquote>❌ خطا</blockquote>\n\nشماره ارسال شده صحیح نمیباشد";
    public static readonly string _invalidCode = "<blockquote>❌ خطا</blockquote>\n\nکد ارسال شده صحیح نمیباشد";
    public static readonly string _timeLater = "<blockquote>⚠️ توجه</blockquote>\n\nربات درحال بروزرسانی میباشد لطفا بعدا تلاش کنید";
    public static readonly string _firstSignup = "<blockquote>⚠️ خطا</blockquote>\n\nلطفا ابتدا در اپلیکیشن ثبت نام و احراز هویت کنید و سپس از خدمات ربات استفاده کنید";
    public static readonly string _verifySucsess = "<blockquote>✅ موفق</blockquote>\n\nثبت نام شما با موفقیت انجام شد";
    public static readonly string _informashion = "<blockquote>👤 اطلاعات حساب</blockquote>\n\n✨ آیدی عددی <code>{0}</code> \n\n💰 موجودی <code>{1}</code> تومان";
    public static readonly string _codeOtp = "📮 لطفا کد sms شده را جهت احراز هویت ارسال کنید";
    public static readonly string _support = "👮 جهت ارتباط با پشتیبانی دکمه زیر را لمس کنید";
    public static readonly string _countryList = "🌐 لیست کشور های ثبت شده برای فروش به شرح زیر میباشد";
    public static readonly string _rules = "<blockquote>🌹 لطفا قبل از خرید ، قوانین را مطالعه کنید</blockquote>\n\n1️⃣ لطفا طبق نیاز حساب خود را شارژ کنید برگشت مبلغ امکان پذیر نمیباشد\n\n2️⃣ لطفا حتما برای دریافت شماره مجازی از اپلیکیشن های <code>تلگراف یا پلاس مسنجر</code> استفاده کنید در غیر این صورت ربات مسئولیت خراب شدن شماره مجازی را نمیپذیرد\n\n3️⃣ ";
}