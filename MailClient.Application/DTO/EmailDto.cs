namespace MailClient.Application.DTO
{
    public record EmailDto(string id, string from, string to, string subject, string body, DateTime date);
}