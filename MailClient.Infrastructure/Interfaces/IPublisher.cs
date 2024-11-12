namespace MailClient.Infrastructure.Interfaces
{
    public interface IPublisher
    {
        public void Publish(dynamic message);
    }
}
