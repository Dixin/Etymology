namespace Etymology.Data.Common
{
    using System;
    using System.Threading;

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
    }
}