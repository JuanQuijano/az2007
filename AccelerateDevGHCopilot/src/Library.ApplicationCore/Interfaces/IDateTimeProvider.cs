namespace Library.ApplicationCore;

/// <summary>
/// Abstraction for date/time operations to enable testability.
/// </summary>
public interface IDateTimeProvider
{
    DateTime Now { get; }
}
