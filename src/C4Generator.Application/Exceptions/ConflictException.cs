namespace C4Generator.Application.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
