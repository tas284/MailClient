namespace MailClient.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException()
            : base("The requested item was not found.")
        {
        }

        public NotFoundException(string message)
            : base(message)
        {
        }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}