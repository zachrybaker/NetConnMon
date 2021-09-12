using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetConnMon.Domain.Entities;
using NetConnMon.Domain.Enums;
using MediatR;
using NetConnMon.Server.API.Notifications;
using NetConnMon.Domain.Base;
using NetConnMon.Server.API.Requests;
using NetConnMon.Server.Services.Tests;
using NetConnMon.Server.API.Commands;

namespace NetConnMon.Server.Services
{
    

    public class TestService : BackgroundService, INotificationHandler<HandleDataUpdatedNotification<TestDefinition>>
    {
        private readonly IServiceProvider services;
        private readonly IMediator mediator;
        private readonly ILogger logger;
        List<ITest> tests;

        static ConcurrentQueue<DataChangeRecord<TestDefinition>> updates 
             = new ConcurrentQueue<DataChangeRecord<TestDefinition>>();

        public TestService(
            IServiceProvider serviceProvider, IMediator mediator, ILogger<TestService> logger)
        {
            services      = serviceProvider.CreateScope().ServiceProvider;
            this.mediator = mediator;
            this.logger   = logger;
        }

        // An update (from the api side) of a test definition should:
        //  save and stop that test,
        //  apply those changes,
        //  then restart.
        // To manage that we will have our own token source that can be reset by
        // either the service starting, or getting an updated config.
        public Task Handle(HandleDataUpdatedNotification<TestDefinition> notification, CancellationToken cancellationToken)
        {
            updates.Enqueue(notification.ChangeRecord);
            return Task.CompletedTask;
        }

        private void StartTesetFromDefinition(TestDefinition definition, CancellationToken cancellationToken)
        {
            ITest test = null;
            if (definition.Protocol == NetProtocol.ICMP)
                test = new ICMPTest(definition, services.CreateScope().ServiceProvider);
            else if (definition.Protocol == NetProtocol.TCP)
                test = new TCPTest(definition, services.CreateScope().ServiceProvider);
            else if (definition.Protocol == NetProtocol.UDP)
                test = new UDPTest(definition, services.CreateScope().ServiceProvider);

            Task.Factory.StartNew(
                (test6) => { test.RunStateMachineAsync(cancellationToken); },
                test,
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Current);
            tests.Add(test);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await mediator.Send(new DeleteOldEventsCommand(DateTime.UtcNow.AddDays(-1)));
            tests = new List<ITest>();
            var definitions = await mediator.Send(new GetTestDefinitionList(
                null, new NetConnMon.Persistence.Contracts.Specs.GetTestDefinitionsWithLatestEventSpec()
            ));

            foreach (var definition in definitions)
                StartTesetFromDefinition(definition, cancellationToken);
            
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    //APPLY ANY UPDATES
                    if (updates.TryDequeue(out var update))
                    {
                        if (update.DataChangeType == DataChangeType.Add)
                            StartTesetFromDefinition(update.Data, cancellationToken);
                        else
                        {
                            var test = tests.First(x => x.GetTestDefinition().Id == update.Data.Id);
                            if (test.IsStopped)
                            {
                                if (update.DataChangeType == DataChangeType.Delete)
                                {
                                    logger.LogInformation("Removing a stopped test that was deleted.  service continues.");
                                    tests.Remove(test);
                                }
                                else
                                {
                                    logger.LogInformation("a stopped test is being updated then restarted.");
                                    test.ShouldProcessThisChange(update);
                                    Thread thread = new Thread(() => test.RunStateMachineAsync(cancellationToken));
                                    thread.Start();
                                }
                            }
                            else
                            {
                                logger.LogInformation("telling a scheduled test to process a change");
                                test.ShouldProcessThisChange(update);
                            }
                        }
                    }

                    var index = 0;
                    while (index < tests.Count)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            tests.ForEach(x => x.RequestStop(false));

                        if (tests[index].IsStopped && (cancellationToken.IsCancellationRequested || tests[index].IsBeingRemoved))
                        {
                            logger.LogInformation($"test \"{tests[index].GetTestDefinition().TestName}\" is stopped and being removed.");
                            tests.RemoveAt(index);
                        }
                        else
                            index++;
                    }

                }
                catch (Exception e)
                {
                    logger.LogError($"Failure in update or state mgmt: {e.Message} {e.StackTrace}");
                }

                await Task.Delay(250, cancellationToken);
            }
            // now we need to spin each of them down.
            await StopTestsAsync(true, cancellationToken);
        }

        private async Task StopTestsAsync(bool quickly, CancellationToken cancellationToken)
        {
            int stillRunningCount = 0;
            do
            {
                var testsRunning = tests.Where(x => x.GetTestingState() != TestingState.Stopped).ToList();
                stillRunningCount = testsRunning.Count();
                logger.LogWarning($"Waiting for {stillRunningCount} tests to stop");
                testsRunning.ForEach(async x => 
                {
                    logger.LogInformation($"Looking for \"{x.GetTestDefinition().TestName}\" to wrap things up!");
                    await x.RunStateMachineAsync(cancellationToken);
                });

                await Task.Delay(250);
            } while (stillRunningCount > 0);

            logger.LogInformation("Stopped cleanly.");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
            => StopTestsAsync(true, cancellationToken);
    }
}
