namespace Library.ApplicationCore.Services;

/// <summary>
/// Default implementation of IDateTimeProvider that returns the current system time.
/// </summary>
public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
}
