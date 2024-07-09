namespace MailClient.InputModel
{
    public class SyncEmailInputModel
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string ImapAddress { get; set; }
        public int ImapPort { get; set; }
        public DateTime DateSync { get; set; }
    }
}
