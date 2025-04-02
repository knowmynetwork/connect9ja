namespace Datify.API.Configuration
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; }
        public string UserName { get; set; }
        public string SenderPassword { get; set; }
        public bool EnableSsl { get; set; }
    }

}
