using System;

namespace Submitter.Models
{
    public class MailerSettings
    {
        public string From { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public short Port { get; set; }
        public Boolean EnableSSL { get; set; }
    }
}