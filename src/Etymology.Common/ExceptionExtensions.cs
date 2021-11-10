namespace Etymology.Common;

public static class ExceptionExtensions
{
    public static bool IsNotCritical(this Exception exception) =>
        exception is not (AccessViolationException
            or AppDomainUnloadedException
            or BadImageFormatException
            or CannotUnloadAppDomainException
            or InvalidProgramException
            or OutOfMemoryException
            or ThreadAbortException);

    public static bool LogErrorWith(this Exception exception, ILogger logger, string message, params object[] args)
    {
        logger.LogError(exception, message, args);
        return false;
    }
}