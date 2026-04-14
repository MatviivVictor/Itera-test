namespace Claims.Application.Extensions;

public static class DateTimeExtensions
{
    public static DateTime UtcToday()
    {
        return DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
    }
    
    public static DateTime UtcDate(this DateTime dateTime)
    {
        var dateTimeUtc = dateTime.Kind == DateTimeKind.Local
            ? TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Utc)
            : dateTime;

        return new DateTime(dateTimeUtc.Year, dateTimeUtc.Month, dateTimeUtc.Day, 0, 0, 0, DateTimeKind.Utc);
    }

    public static DateTime Min(this DateTime baseDate, params DateTime[] dates)
    {
        return dates.Aggregate(baseDate, (current, date) => current < date ? current : date);
    }
}

