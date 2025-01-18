namespace Academy.Application.Exceptions;

public class AcademyException : Exception
{
    public ExceptionType ExceptionType { get; set; }

    public AcademyException(ExceptionType exceptionType, string message) : base(message)
        => ExceptionType = exceptionType;
}