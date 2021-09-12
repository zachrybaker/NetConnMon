using System.Threading;
using System.Threading.Tasks;
using NetConnMon.Domain.Base;
using NetConnMon.Domain.Entities;

namespace NetConnMon.Server.Services.Tests
{
    public enum TestingState
    {
        Initialized,    // -> RunningTest
        RunningTest,    // -> ApplyUpdates, SaveThenStop, SaveThenRun, or Stop
        ApplyUpdates,   // -> (applies, then) -> SavethenRun
        SaveThenStop,   // -> (saves, then) -> Stopped
        SaveThenRun,    // -> (saves, then) -> RunningTest
        Stop,           // -> Stopped
        Stopped         // state machine will no longer run, must be restarted on a new thread.
    }
    public interface ITest
    {
        /// <summary>
        /// Do work.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RunStateMachineAsync(CancellationToken cancellationToken);

        // requests
        void RequestStop(bool andSave);
        bool IsStopped { get; }
        bool IsBeingRemoved { get; }
        void ShouldProcessThisChange(DataChangeRecord<TestDefinition> changeRecord);

        TestDefinition GetTestDefinition();
        TestingState   GetTestingState();
    }
}
