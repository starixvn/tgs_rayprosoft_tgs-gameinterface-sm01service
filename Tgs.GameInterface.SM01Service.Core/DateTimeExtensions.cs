namespace Tgs.GameInterface.SM01Service.Core;

public static class DateTimeExtensions
{
	public static long MillisecondsTimestamp(this DateTime date)
	{
		DateTime baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		return (long)(date.ToUniversalTime() - baseDate).TotalMilliseconds;
	}

	public static string DatetimeConvertToString(this DateTime date)
	{
		var _timeZone = TimeZoneInfo.CreateCustomTimeZone(id: "Asia/Hong_Kong", baseUtcOffset: TimeSpan.FromHours(8), "(UTC+08:00) Hong Kong Standard Time", "Hong Kong Standard Time");
		return TimeZoneInfo.ConvertTime(date.ToUniversalTime(), _timeZone).ToString("yyyy-MM-dd HH:mm:ss");
	}
}
