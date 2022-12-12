namespace Tgs.GameInterface.SM01Service.Core.Exceptions;

public class GameProviderResponseErrorException : Exception
{
    public GameProviderResponseErrorException(string? errorCode, string? message) : base(
       $"The game manufacturer responds with an error code({errorCode}), please contact the engineer or game supplier for assistance.({message})")
    {
    }

    public GameProviderResponseErrorException(string? errorMessage) : base(
       $"The game manufacturer responds with an error , please contact the engineer or game supplier for assistance.({errorMessage})")
    {
    }
}
