using System;
using System.Collections.Generic;
using NetConnMon.Domain.Entities;
using NetConnMon.Domain.Enums;

namespace NetConnMon.Data
{
    public enum LoadState
    {
        Loading,
        Ready
    };

    /// <summary>
    /// Since we simply reuse entities here instead of using DTOs, we trade the
    /// complexities of fluent validation, mappers, and so on.
    /// The two side effects are we need a place to add client-oriented data
    /// logic to DTOs (this), and we can't enforce they are immutable (records in C#)
    /// </summary>
    public static class DataExtensions
    {
        public static T LastItem<T>(this List<T> list) => list?.Count > 0 ? list[list.Count - 1] : default(T);
        public static bool IsTrue(this bool? bVal) => bVal.HasValue && bVal.Value;

        public static string StatusText(this TestDefinition test)
            => test.CanConnect.HasValue ?
                (test.CanConnect.Value ? "Connected" : "Disconnected") :
                "Unknown";

        public static bool? RunningWithErrors(this TestDefinition test)
        {
            if (test.LastErrored.HasValue && test.RunningSince.HasValue)
            {
                return test.LastErrored.Value > test.RunningSince.Value;
            }
            if (test.Events.LastItem() != null)
                return test.Events.LastItem().Errors > 0;

            return null;
        }

        public static EmailSettings DefaultEmailSettingsValues()
            => new EmailSettings (){
                LastUpdated = DateTime.UtcNow,
                Port = 587,
                SmtpHost = "smtp.gmail.com",
                UseSSL = true,
                SenderName = "NetConnMon"
            };

        public static TestDefinition DefaultTestDefinitionValues()
            => new TestDefinition()
            {
                Address = "",
                Protocol = NetProtocol.ICMP,
                CheckIntervalSec = 2,
                ShouldEmailStatus = true,
                TimeoutMSec = TestDefinition.TimeoutDefaultForProtocol(NetProtocol.ICMP),
                ConsequtiveErrorsBeforeDisconnected = 3
            };
        
        public static string DefaultTestNamePerPort(this TestDefinition model)
            => model.Protocol switch
                {
                    NetProtocol.ICMP => $"Ping",
                    NetProtocol.TCP => "Http Get",
                    NetProtocol.UDP => "UDP Test",
                    _ => "???"
                };
}
}
