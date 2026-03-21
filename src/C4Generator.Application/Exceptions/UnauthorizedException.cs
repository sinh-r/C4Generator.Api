namespace C4Generator.Application.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}
