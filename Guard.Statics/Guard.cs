using Guard.Exceptions;

namespace Guard.Statics;

/// <summary>
/// It is approach when whole logic with checking and throwing exception is locating in one class `Guard`.
/// Main advantage of approach is all logic in one file and main components are reusable (DRY, KISS).
/// Main deficiency is when there are many exceptions the class will be able to become super huge,
/// also system logic will be able mixed with business logic.
/// </summary>
public static class Guard
{
    public static void ThrowUsualExceptionIfNull<T>(T? value, string message)
    {
        if (value is null)
        {
            throw new UsualException(message);
        }
    }
}