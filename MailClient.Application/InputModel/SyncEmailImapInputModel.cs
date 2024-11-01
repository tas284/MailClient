namespace MailClient.Application.InputModel
{
    public class SyncEmailImapInputModel
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string ImapAddress { get; set; }
        public int ImapPort { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
