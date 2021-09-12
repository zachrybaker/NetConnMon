using System;
using MediatR;
using NetConnMon.Domain;
using NetConnMon.Domain.Entities;

namespace NetConnMon.Server.API.Commands
{
    public class SendTestEmailRequest : IRequest<string>
    {
        public SendTestEmailRequest()
        {
        }
    }
}
