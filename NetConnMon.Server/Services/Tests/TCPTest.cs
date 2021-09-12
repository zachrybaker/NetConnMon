using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetConnMon.Domain.Entities;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NetConnMon.Server.Services.Tests
{
    public class TCPTest : BaseTest
    {
        readonly ILogger<TCPTest> logger;
        public TCPTest(TestDefinition test, IServiceProvider services) : base(test, services)
            => logger = services.GetRequiredService<ILogger<TCPTest>>();

        protected override  Task<string> PerformTestAsync(CancellationToken cancellationToken)
        {
            var tcpClient = new TcpClient();
            tcpClient.SendTimeout    = (int)test.TimeoutMSec;
            tcpClient.ReceiveTimeout = (int)test.TimeoutMSec;
            
            string message = null;
            try
            {
                watch.Start();
                var success = tcpClient.ConnectAsync(test.Address, test.Port).Wait((int)test.TimeoutMSec);
                watch.Stop();

                if (success)
                    // good enough. Don't ask for content. And don't disconnect, I guess....
                    message = test.ProcessStatus(true, null, logger);
                else
                    message = test.ProcessStatus(false, "Package sent, but no message received", logger); 

                logger.LogTrace($"TCP connect: {success}.");
            }
            catch (ArgumentNullException ane)
            {
                logger.LogWarning(ane, $"TCP error on hostname: {ane.Message} {ane.StackTrace ?? ""}");
                message = test.ProcessError(ane.ToString(), logger);
            }
            catch (ArgumentOutOfRangeException aor)
            {
                logger.LogError(aor, $"TCP port out of range: {aor.Message} {aor.StackTrace ?? ""}");
                message = test.ProcessError(aor.ToString(), logger);
            }
            catch (System.Net.Sockets.SocketException se)
            {
                logger.LogWarning(se, $"TCP socket access error: {se.Message} {se.StackTrace ?? ""}");
                message = test.ProcessError(se.ToString(), logger);
            }
            catch (ObjectDisposedException ode)
            {
                logger.LogWarning(ode, $"UDP socket was closed: {ode.Message} {ode.StackTrace ?? ""}");
                message = test.ProcessError(ode.ToString(), logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in test: {ex.Message} {ex.StackTrace ?? ""}");
                message = test.ProcessError(ex.ToString(), logger);
            }

            return Task.FromResult(message);
        }
    }
}