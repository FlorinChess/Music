namespace Music.APIs.Spotify.Exceptions
{
    public sealed class MissingAccessTokenException : Exception
    {
        public MissingAccessTokenException()
        {
        }

        public MissingAccessTokenException(string? message) : base(message)
        {
        }

        public MissingAccessTokenException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
