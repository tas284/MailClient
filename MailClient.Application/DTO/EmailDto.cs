using MailClient.Domain.Entities;

namespace MailClient.Application.DTO
{
    public record EmailDto(string id, string from, string to, string subject, string body, DateTime date)
    {
        public static EmailDto Create(Email entity)
        {
            return new EmailDto(entity.Id.ToString(), entity.EmailFrom, entity.Inbox, entity.Subject, entity.Body, entity.Date);
        } 
    }
}