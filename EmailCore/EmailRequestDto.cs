namespace EmailCore
{
    public class EmailRequestDto
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FromAddress { get; set; }
        public int SecondsOfDelay { get; set; }
    }
}
