using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetConnMon.Domain.Entities;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetConnMon.Server.Services.Tests
{
    public class UDPTest : BaseTest
    {
        readonly ILogger<UDPTest> logger;
        public UDPTest(TestDefinition test, IServiceProvider services) : base(test, services)
        {
            logger = services.GetRequiredService<ILogger<UDPTest>>();
        }

        protected override async Task<string> PerformTestAsync(CancellationToken cancellationToken)
        {
            var udpClient = new UdpClient();
            udpClient.Client.SendTimeout    = (int)test.TimeoutMSec;
            udpClient.Client.ReceiveTimeout = (int)test.TimeoutMSec;
            
            string message = null;
            try
            {
                watch.Start();

                udpClient.Connect(test.Address, test.Port);
                var sendBytes = Encoding.ASCII.GetBytes("Well Hello....");
                var countBytesSent = await udpClient.SendAsync(sendBytes, sendBytes.Length);
                // var endpoint = new IPEndPoint(IPAddress.Any, 0);
                var receiveResult = await udpClient.ReceiveAsync();
                watch.Stop();

                Debug.Assert(countBytesSent == sendBytes.Length, "Byte count for sent didn't line up");
                var returnData = Encoding.ASCII.GetString(receiveResult.Buffer);

                if (returnData.Length > 0)
                    message = test.ProcessStatus(true, null, logger);
                else
                    message = test.ProcessStatus(false, "Package sent, but no message received", logger);

                logger.LogTrace($"UDP response countBytesSent {countBytesSent}, {receiveResult.Buffer.Length} received.");
            }
            catch (ObjectDisposedException ode)
            {
                logger.LogWarning(ode, $"UDP socket was closed: {ode.Message} {ode.StackTrace ?? ""}");
                message = test.ProcessError($"UDP socket was closed: {ode.Message}", logger);
            }
            catch (InvalidOperationException ioe)
            {
                logger.LogError(ioe, $"UDP protocol Error in test: {ioe.Message} {ioe.StackTrace ?? ""}");
                message = test.ProcessError(ioe.Message, logger);
            }
            catch (System.Net.Sockets.SocketException se)
            {
                logger.LogWarning(se, $"UDP socket access error: {se.Message} {se.StackTrace ?? ""}");
                message = test.ProcessError(se.Message, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in test: {ex.Message} {ex.StackTrace ?? ""}");
                message = test.ProcessError(ex.Message, logger);
            }

            return message;
        }
    }
}