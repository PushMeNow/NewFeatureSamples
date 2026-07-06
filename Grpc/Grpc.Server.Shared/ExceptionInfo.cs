namespace Grpc.Server;

public class ExceptionInfo
{
	public ExceptionInfo()
	{
	}

	public ExceptionInfo(BlException exception, bool includeStackTrace = false)
	{
		if (exception is null)
		{
			throw new ArgumentNullException(nameof(exception));
		}

		Type = exception.GetType().FullName;
		Message = exception.Message;
		Source = exception.Source;
		StackTrace = includeStackTrace ? exception.StackTrace : null;
	}

	public string Type { get; set; }
	public string Message { get; set; }
	public string Source { get; set; }
	public string StackTrace { get; set; }

	public string PublicMessage { get; set; }
}
