using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NetConnMon.Domain.Entities;
using NetConnMon.Server.Services;
using System;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using NetConnMon.Server.API.Notifications;
using NetConnMon.Server.API.Requests;
using System.Linq;
using MailKit.Net.Smtp;
using Hangfire;

namespace NetConnMon.Server.Services
{
    public class Emailer : IEmailer, INotificationHandler<HandleDataUpdatedNotification<EmailSettings>>
    {
        private EmailSettings _settings;
        private readonly ILogger<Emailer> logger;
        private object settingsLock = new();
        IMediator mediator;
        public Emailer(IOptions<EmailSettings> options, ILogger<Emailer> logger, IMediator mediator)
        {
            this.logger = logger;
            _settings = options?.Value;
            this.mediator = mediator;

            if (_settings == null || _settings.IsNotSet)
            {
                _settings = null;
                logger.LogWarning("Settings via config are null, and wont' be fetched till first needed.");
            }
        }

        private EmailSettings GetSettings() 
        {
            EmailSettings emailSettings;
            lock (settingsLock)
            {
                if (_settings == null)
                {
                    logger.LogInformation("Settings are still null, fetching till first needed.");

                    _settings = mediator.Send(new GetEmailSettingsList()).GetAwaiter().GetResult().FirstOrDefault();
                }
                emailSettings = _settings;
            }

            return emailSettings;
        }

        private void SetSettings(EmailSettings emailSettings)
        {
            lock(settingsLock)
            {
                _settings = emailSettings;
            }    
        }

        public Task Handle(HandleDataUpdatedNotification<EmailSettings> notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Received a settings update");
            SetSettings(notification.ChangeRecord.Data);
            return Task.CompletedTask;
        }

        //public Task<string> SendTestEmail( CancellationToken cancellationToken)
        //     => SendEmailAsync(
        //        MakeMimeMessage("Test Email (NetConnMon)", "This is your test email (NetConnMon)"),
        //        cancellationToken);
        public Task<string> SendTestEmail(CancellationToken cancellationToken)
        {
            var msg = MakeMimeMessage("Test Email (NetConnMon)", "This is your test email (NetConnMon)");
            BackgroundJob.Enqueue(() => SendEmailAsync(msg, cancellationToken));
            return Task.FromResult<string>(null);
        }

        private MimeMessage MakeMimeMessage(string subject, string message)
        {
            var settings = GetSettings();
            if (settings?.IsNotSet ?? true)
            {
                logger.LogError("Can't create a message when email settings are not set!");
                return null;
            }

            var messageToSend = new MimeMessage
            {
                Sender = new MailboxAddress(settings.SenderName, settings.SenderEmail),
                Subject = subject,
                Body = new TextPart(settings.TextFormat) { Text = message }
            };

            messageToSend.From.Add(new MailboxAddress(settings.SenderName, settings.SenderEmail));
            messageToSend.To.Add(new MailboxAddress(settings.RecipientName, settings.RecipientEmail));

            return messageToSend;
        }


        /// <summary>
        /// Email is sent on an asynced background thread.
        /// </summary>
        /// <param name="test"></param>
        /// <param name="message"></param>
        /// <param name="completionCallback"></param>
        /// <param name="cancellationToken"></param>
        public void SendEmailStatus(TestDefinition test, string message, Action completionCallback, CancellationToken cancellationToken)
            => SendEmailWithAction(MakeMimeMessage(test.CreateErrorEmailSubject(), message), completionCallback, cancellationToken);


        /// <summary>
        /// Send an email asynchronously, returning an error message if it failed.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> SendEmailAsync(MimeMessage message, CancellationToken cancellationToken)
        {
            if (message == null)
                return "Can't send an email when email settings are not set!";

            SmtpClient client;
            try
            {
                var settings = GetSettings();
                using (client = new SmtpClient())
                {
                    if (cancellationToken.IsCancellationRequested)
                        return null;
                    await client.ConnectAsync(settings.SmtpHost, settings.Port, SecureSocketOptions.Auto, cancellationToken);

                    if (cancellationToken.IsCancellationRequested)
                        return null;
                    if (!string.IsNullOrEmpty(settings.SMTPUsername))
                        await client.AuthenticateAsync(settings.SMTPUsername, settings.SMTPPassword);


                    if (!cancellationToken.IsCancellationRequested)
                        await client.SendAsync(message, cancellationToken);
                    await client.DisconnectAsync(true, cancellationToken);

                    if (!cancellationToken.IsCancellationRequested)
                    logger.LogDebug("Sent email");
                    return null;
                }
            }
            catch (Exception e)
            {
                var error = $"Failure sending email: {e.Message}";
                logger.LogError($"{error}, {e.StackTrace}");
                return error;
            }
        }

        private void SendEmailWithAction(MimeMessage message, Action completionCallback, CancellationToken cancellationToken)
        {
            //Task.Run(async () => {
            BackgroundJob.Enqueue(() => SendEmailAsync(message, cancellationToken));
            //}).FireAndForget();
            
            completionCallback.Invoke();
        }
    }
}
