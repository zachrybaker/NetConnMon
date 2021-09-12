using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetConnMon.Domain.Entities;
using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace NetConnMon.Server.Services.Tests
{
    public class ICMPTest : BaseTest
    {
        readonly ILogger<ICMPTest> logger;
        public ICMPTest(TestDefinition test, IServiceProvider services) : base(test, services)
        {
            logger = services.GetRequiredService<ILogger<ICMPTest>>();
        }

        protected override async Task<string> PerformTestAsync(CancellationToken cancellationToken)
        {
            Ping ping = new Ping();
            string message = null;
            try
            {
                watch.Start();
                PingReply reply = await ping.SendPingAsync(test.Address, (int)test.TimeoutMSec);
                watch.Stop();
                logger.LogTrace($"replied {reply.Address} {reply?.Status}");

                if (reply == null)
                {
                    logger.LogError("Null ping response");
                    message = test.ProcessError("No response", logger);
                }
                else if (reply.Status == IPStatus.Success)
                {
                    message = test.ProcessStatus(true, null, logger);
                }
                else
                {
                    message = test.ProcessStatus(false, reply.Status.ToString(), logger);
                }
            }
            catch (PingException pex)
            {
                logger.LogError(pex, $"Ping Error in ping: {pex.Message} {pex.StackTrace ?? ""}");
                message = test.ProcessError(pex.ToString(), logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in test: {ex.Message} {ex.StackTrace ?? ""}");
                message = test.ProcessError(ex.ToString(), logger);
            }

            return message;
        }
    }
}