using NCMS_wasm.Server.Logger;
using System.Net.Mail;
using System.Net;
using NCMS_wasm.Shared;
using NCMS_wasm.Server.Repository;
namespace NCMS_wasm.Server.BackgroundServices
{
    public class EmailSenderProcessor : BackgroundService
    {
        private readonly EmailRepository _emailRepository;
        private readonly FileLogger _fileLogger;
        private readonly SmtpClient _smtpClient;
        private readonly IConfiguration _configuration;
        private readonly string FromAddress;
        private string logFileName = string.Empty;
        private string moduleName = "EmailSender Processor";

        public EmailSenderProcessor(EmailRepository emailRepository, IConfiguration configuration)
        {
            _emailRepository = emailRepository;
            _configuration = configuration;
            _fileLogger = new FileLogger(configuration);
            FromAddress = _configuration["Smtp:Username"];
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
                await Task.Delay(5000, stoppingToken); // Runs every 5 seconds
            }
        }

        private async Task ProcessEmailsAsync(CancellationToken stoppingToken)
        {
            // Retrieve the queued emails from the repository 
            var queuedEmails = await _emailRepository.GetQueuedEmailsAsync();

            foreach (var email in queuedEmails)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                try
                {
                    // Prepare email
                    var mailMessage = new MailMessage(FromAddress, email.ToAddress)
                    {
                        Subject = email.Subject,
                        Body = email.Body,
                        IsBodyHtml = true
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
                    await _emailRepository.MarkEmailAsFailedAsync(email);
                }
            }
        }
    }

   



}
