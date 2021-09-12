using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NetConnMon.Domain;
using NetConnMon.Domain.Entities;
using NetConnMon.Persistence;
using NetConnMon.Server.Services;

namespace NetConnMon.Server.API.Commands
{
    public class SendTestEmailHandler : IRequestHandler<SendTestEmailRequest, string>
    {
        IEmailSettingsRepo emailRepo;
        IEmailer emailer;
        public SendTestEmailHandler(IEmailSettingsRepo emailSettingsRepo, IEmailer emailer)
        {
            emailRepo = emailSettingsRepo;
            this.emailer = emailer;
        }

        public async Task<string> Handle(SendTestEmailRequest request, CancellationToken cancellationToken)
        {
            var emailsettings = (await emailRepo.GetListAsync()).FirstOrDefault();
            if (emailsettings != null)
                return await emailer.SendTestEmail(cancellationToken);

            return "No email settings to test yet.";
        }
    }
}
