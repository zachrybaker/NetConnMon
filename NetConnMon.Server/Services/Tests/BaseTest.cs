using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetConnMon.Domain.Base;
using NetConnMon.Domain.Entities;
using NetConnMon.Domain.Enums;
using NetConnMon.Server.API.Commands;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NetConnMon.Server.Services.Tests
{
    public abstract class BaseTest : ITest
    {
        protected TestDefinition test;
        protected Stopwatch watch;
        int executionCount = 0;
        const int countsBeforeDiagnostics = 10;
        protected readonly IEmailer emailer;
        IServiceProvider serviceProvider;
        readonly ILogger<BaseTest> logger;
        TestingState _testingState;
        private object stateLocker;
        private CancellationTokenSource workTokenSource;
        private IMediator mediator;
        DataChangeRecord<TestDefinition> changeRecord = null;
        public TestDefinition GetTestDefinition() => test;

        /// <summary>
        /// returns a message to email if it should be emailed.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected abstract Task<string> PerformTestAsync(CancellationToken cancellationToken);

        public BaseTest(TestDefinition test, IServiceProvider services)
        {
            emailer = services.GetRequiredService<IEmailer>();
            logger = services.GetRequiredService<ILogger<BaseTest>>();
            this.test = test;
            watch = new Stopwatch();
            stateLocker = new object();
            serviceProvider = services;
            mediator = services.GetRequiredService<IMediator>();
        }

        // do test work will keep running until the cancellation token happens
        protected async Task DoTestWorkAsync(CancellationToken serviceToken)
        {
            test.SetStarting();
            watch.Reset();
            workTokenSource = new CancellationTokenSource();

            while (!serviceToken.IsCancellationRequested &&
                !workTokenSource.Token.IsCancellationRequested &&
                !test.Disabled)
            {
                executionCount++;
                string message = null;
                try
                {
                    message = await PerformTestAsync(serviceToken);
                }catch(Exception e)
                {
                    message = $"Uncaught exception in {test.TestName} {test.Protocol} test: {e.Message} {e.StackTrace}";
                    logger.LogError(message);
                }
                if (!serviceToken.IsCancellationRequested &&
                    !workTokenSource.Token.IsCancellationRequested &&
                    !string.IsNullOrEmpty(message))
                {
                        emailer.SendEmailStatus(test, message, () =>
                    {
                        test.LastEmailed = DateTime.UtcNow;
                        test.BackoffSec += test.BackoffStepSec;
                    }, serviceToken);

                    // start a new list of messages.
                    test.Messages.Clear();
                }
                if (!serviceToken.IsCancellationRequested &&
                    !workTokenSource.Token.IsCancellationRequested)

                    await Task.Delay(1000 * test.CheckIntervalSec, serviceToken);

                if (executionCount % countsBeforeDiagnostics == 0)
                {
                    Console.WriteLine($"{countsBeforeDiagnostics} {test.Protocol} tests took {watch.Elapsed}.");
                    executionCount = 0;
                    watch.Reset();
                }
                if (test.SaveInterval > 0 && executionCount % test.SaveInterval == 0)
                    await SaveTheTest(serviceToken);
            }

            test.SetStopping(false);
        }


        public bool IsStopped => GetTestingState() == TestingState.Stopped;

        public async Task RunStateMachineAsync(CancellationToken serviceToken)
        {
            var state = GetTestingState();
            while (state < TestingState.Stopped && !serviceToken.IsCancellationRequested)
            {
                if (state == TestingState.Initialized)
                    SetTestingState(test.Disabled ? TestingState.Stopped : TestingState.RunningTest);
                else if (state == TestingState.RunningTest)
                {
                    if (test.Disabled)
                        SetTestingState(TestingState.Stopped);
                    else
                        await DoTestWorkAsync(serviceToken);
                }
                else if (state == TestingState.ApplyUpdates)
                {
                    if (changeRecord == null)
                        throw new NullReferenceException("change record missing in apply updates step");

                    if (changeRecord.IsChangeType(DataChangeType.Edit) &&
                        changeRecord.Data != null)
                    {
                        test.UpdateDefinitionUnderTestFrom(changeRecord.Data);
                        changeRecord = null;
                        // get back to work
                        SetTestingState(test.Disabled ? TestingState.SaveThenStop : TestingState.SaveThenRun);
                    }
                    else if (changeRecord.IsChangeType(DataChangeType.Delete))
                    {
                        // leave the change type.
                        SetTestingState(TestingState.Stopped);
                    }
                    else
                        UnexpectedState(state, $"unexpected change type: {changeRecord.DataChangeType}", true);
                }
                else if (state == TestingState.SaveThenRun)
                {
                    await SaveTheTest(serviceToken);
                    SetTestingState(serviceToken.IsCancellationRequested ? TestingState.Stopped : TestingState.RunningTest);
                }
                else if (state == TestingState.SaveThenStop)
                {
                    await SaveTheTest(serviceToken);
                    SetTestingState(TestingState.Stopped);
                }
                else if (state == TestingState.Stop)
                    SetTestingState(TestingState.Stopped);
                else if (state == TestingState.Stopped)
                    if (changeRecord != null && !changeRecord.IsChangeType(DataChangeType.Delete) && !test.Disabled)
                        UnexpectedState(state, "ALREADY stopped", false);
                    else
                        UnexpectedState(state, "NOT A KNOWN STATE", true);
                
                state = GetTestingState();
            }

            // we are in the shutdown sequence now.
            while (state < TestingState.Stopped)
            {
                switch(state)
                {
                    case TestingState.Initialized:
                    case TestingState.ApplyUpdates:
                    case TestingState.Stop:
                        SetTestingState(TestingState.Stopped);
                        break;
                    case TestingState.SaveThenRun:
                    case TestingState.SaveThenStop:
                    case TestingState.RunningTest:
                        await SaveTheTest(serviceToken);
                        SetTestingState(TestingState.Stopped);
                        break;
                }

                state = GetTestingState();
            }
            logger.LogInformation("BaseTest shut down");
        }
        

        private void UnexpectedState(TestingState state, string msg, bool throwException)
        {
            var message = $"{test.TestName} ({test.Protocol}) in RunStateMachine unexpected state: {state}. {msg}";
            logger.LogError(message);
            if (throwException)
                throw new Exception(message);
        }

        public TestingState GetTestingState()
        {
            TestingState testingState;
            lock (stateLocker)
            {
                testingState = _testingState;
            }

            return testingState;
        }
        private void SetTestingState(TestingState testingState)
        {
            lock(stateLocker)
            {
                if (_testingState == TestingState.RunningTest)
                    workTokenSource.Cancel();

                _testingState = testingState;
            }

            logger.LogInformation($"{test.TestName} ({test.Protocol}) state now {testingState}");
        }

        public bool IsBeingRemoved => this.changeRecord?.IsChangeType(DataChangeType.Delete) ?? false;
        public void RequestStop(bool andSave)
        {
            workTokenSource.Cancel();
            if (GetTestingState() < TestingState.Stop)
                SetTestingState(andSave ? TestingState.SaveThenStop : TestingState.Stop);
        }
        public void ShouldProcessThisChange(DataChangeRecord<TestDefinition> changeRecord)
        {
            this.changeRecord = changeRecord;
            SetTestingState(TestingState.ApplyUpdates);
        }

        public TestDefinition GiveTestDefinition()
        {
            if (GetTestingState() != TestingState.Stopped)
                UnexpectedState(GetTestingState(), "Can't yield the test if not stopped", true);
            TestDefinition testDefinition = test;
            test = null;
            return testDefinition;
        }

        private async Task SaveTheTest(CancellationToken serviceToken)
        {
            await mediator.Send(new SaveTestStatus(test));
            if (!test.Disabled && (test.CanConnect ?? false))
                await emailer.RetrySendingEmailAsync(serviceToken);
        }
    }
}
