using System.Text.Json.Serialization;

namespace MailClient.Application.InputModel
{
    public class ImapInputModel
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string ImapAddress { get; set; }
        public int ImapPort { get; set; }
        public DateTime StartDate { get; set; }

        [JsonIgnore]
        public DateTime EndDate { get; set; }
        [JsonIgnore]
        public string Validations { get; set; }
    }
}
