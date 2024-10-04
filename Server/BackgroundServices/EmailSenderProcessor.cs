using NCMS_wasm.Server.Logger;
using System.Net.Mail;
using System.Net;
using NCMS_wasm.Shared;
namespace NCMS_wasm.Server.BackgroundServices
{
    public class EmailSenderProcessor : BackgroundService
    {
        private readonly EmailRepository _emailRepository;
        private readonly FileLogger _fileLogger;
        private readonly SmtpClient _smtpClient;
        private readonly IConfiguration _configuration;
        private readonly EmailDetails _emailDetails;
        private string logFileName = string.Empty;
        private string moduleName = "EmailSender Processor";

        public EmailSenderProcessor(EmailRepository emailRepository, IConfiguration configuration, FileLogger fileLogger, EmailDetails emailDetails)
        {
            _emailRepository = emailRepository;
            _configuration = configuration;
            _fileLogger = fileLogger;
            _emailDetails = emailDetails;
            _smtpClient = new SmtpClient(_configuration["Smtp:Host"])
            {
                Port = int.Parse(_configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
                EnableSsl = bool.Parse(_configuration["Smtp:EnableSSL"])
            };

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logFileName = DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
            _fileLogger.Log("Email Sender Processor Started.", logFileName, moduleName);

            while (!stoppingToken.IsCancellationRequested)
            {
                logFileName = DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
                await ProcessEmailsAsync(stoppingToken);
                await Task.Delay(10000, stoppingToken); // Runs every 10 seconds
            }
        }

        private async Task ProcessEmailsAsync(CancellationToken stoppingToken)
        {
            // Retrieve the queued emails from the repository (could be database or a message queue)
            var queuedEmails = await _emailRepository.GetQueuedEmailsAsync();

            foreach (var email in queuedEmails)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                try
                {
                    // Prepare email
                    var mailMessage = new MailMessage(email.From, email.To)
                    {
                        Subject = email.Subject,
                        Body = email.Body,
                        IsBodyHtml = email.IsHtml
                    };

                    // Send email
                    await _smtpClient.SendMailAsync(mailMessage);

                    // Log success
                    _fileLogger.Log($"Email sent successfully: {email.Subject}", logFileName, moduleName);

                    // Mark email as successfully processed
                    await _emailRepository.MarkEmailAsSentAsync(email);
                }
                catch (Exception ex)
                {
                    // Log failure
                    _fileLogger.Log($"Failed to send email: {email.Subject}. Error: {ex.Message}", logFileName, moduleName);

                    // Mark email as failed
                    await _emailRepository.MarkEmailAsFailedAsync(email, ex.Message);
                }
            }
        }
    }

    // Example of EmailRepository to get email details from a database or queue
    public class EmailRepository
    {
        public Task<List<EmailDetails>> GetQueuedEmailsAsync()
        {
            // Implementation to retrieve queued emails from database, message queue, etc.
            throw new NotImplementedException();
        }

        public Task MarkEmailAsSentAsync(EmailDetails email)
        {
            // Implementation to mark email as sent
            throw new NotImplementedException();
        }

        public Task MarkEmailAsFailedAsync(EmailDetails email, string errorMessage)
        {
            // Implementation to mark email as failed with error message
            throw new NotImplementedException();
        }
    }



}
