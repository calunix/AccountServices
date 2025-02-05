using System.Net.Mail;

namespace AccountServices.Classes
{
    public class Email
    {
        private string _smtpServer { get; init; }
        private string _sender { get; init; }
        private string _domain { get; init; }

        public Email(IConfiguration config)
        {
            _smtpServer = config.GetValue<string>("Email:SmtpServer");
            _sender = config.GetValue<string>("Email:Sender");
            _domain = config.GetValue<string>("ActiveDirectory:DomainName");
        }

        public (bool, string) SendCode(string recipient, string code)
        {
            bool sendSuccess = false;
            string errorMessage = "none";
            SmtpClient smtpClient = new SmtpClient(_smtpServer, 25);
            string subject = $"{_domain} Password Reset Code";
            string body = $"Your code is: {code}";

            MailMessage message = new MailMessage(_sender, recipient, subject, body);

            try
            {
                smtpClient.Send(message);
                sendSuccess = true;
            }
            catch (SmtpException ex)
            {
                SmtpStatusCode statusCode = ex.StatusCode;
                errorMessage = $"{ex.Message} Status Code: {statusCode}";
            }
            finally
            {
                smtpClient.Dispose();
            }            
            return (sendSuccess, errorMessage);
        }
    }
}
