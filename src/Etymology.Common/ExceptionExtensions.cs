namespace Etymology.Common;

using System;
using System.Threading;
using Microsoft.Extensions.Logging;

public static class ExceptionExtensions
{
    public static bool IsNotCritical(this Exception exception) =>
        !(exception is AccessViolationException
          || exception is AppDomainUnloadedException
          || exception is BadImageFormatException
          || exception is CannotUnloadAppDomainException
          || exception is InvalidProgramException
          || exception is OutOfMemoryException
          || exception is ThreadAbortException);

    public static bool LogErrorWith(this Exception exception, ILogger logger, string message, params object[] args)
    {
        logger.LogError(exception, message, args);
        return false;
    }
}