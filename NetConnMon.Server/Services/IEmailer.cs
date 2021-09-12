using System;
using System.Threading;
using System.Threading.Tasks;
using MimeKit;
using NetConnMon.Domain.Entities;

namespace NetConnMon.Server.Services
{
    public interface IEmailer
    {
        Task<string> SendTestEmail( CancellationToken cancellationToken);
        void SendEmailStatus(TestDefinition test, string message, Action completionCallback, CancellationToken cancellationToken);
        Task<string> SendEmailAsync(MimeMessage message, CancellationToken cancellationToken);
    }
}
