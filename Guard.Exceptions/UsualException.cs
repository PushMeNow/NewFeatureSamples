namespace Guard.Exceptions;

public sealed class UsualException(string message) : Exception(message)
{
    public static void ThrowIfNull<T>(T? value, string message) where T : class
    {
        if (value is null)
        {
            throw new UsualException(message);
        }
    }
}