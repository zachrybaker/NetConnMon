using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using NetConnMon.Domain.Entities;
using NetConnMon.Server.Services;

namespace NetConnMon.Server.Services
{
    public static class TestDefinitionExtensions
    {
        public static T LastItem<T>(this List<T> list) => list?.Count > 0 ? list[list.Count - 1] : default(T);

        /// <summary>
        /// returns true if applying rules determined it should email.
        /// </summary>
        /// <param name="testDefinition"></param>
        /// <param name="canConnect"></param>
        /// <param name="message"></param>
        /// <param name="changedStatus"></param>
        /// <param name="emailer"></param>
        /// <param name="logger"></param>
        /// <param name="duration"> of this event...</param>
        /// <returns>true if should email</returns>
        private static bool ProcessForEmail(
            this TestDefinition testDefinition,
            bool? canConnect,
            string message,
            bool? changedStatus,
            TimeSpan? duration)
        {
            if ( !testDefinition.ShouldEmailStatus)
                return false;

            if(!canConnect.HasValue)
            {
                // error scenario

            }
            else if (canConnect.HasValue && changedStatus.HasValue)
            {
                if (duration.HasValue && duration.Value.TotalSeconds >= testDefinition.MinInterruptionSec)
                {
                    testDefinition.Messages.Add(message);
                    // TBD: but if we didn't email that it went down, should we be emailing that it works again?
                    var shouldEmail = testDefinition.Messages.Count > 0 && canConnect.Value && changedStatus.Value;

                    if (!shouldEmail && testDefinition.Messages.Count > 0 && !canConnect.Value && !changedStatus.Value)
                    {
                        if (!testDefinition.LastEmailed.HasValue)
                            shouldEmail = true;
                        else if (
                            testDefinition.BackoffSec < DateTime.UtcNow.Subtract(testDefinition.LastEmailed.Value).TotalSeconds &&
                            testDefinition.BackoffSec < testDefinition.BackoffMaxSec)
                                shouldEmail = true;
                    }

                    return shouldEmail;
                }
            }

            return false;
        }

        /// <summary>
        /// process the status of a test definition.
        /// If a message should be emailed, it is returned.
        /// </summary>
        /// <param name="testDefinition"></param>
        /// <param name="canConnect"></param>
        /// <param name="inboundMessage"></param>
        /// <param name="logger"></param>
        /// <returns>message.</returns>
        public static string ProcessStatus(
            this TestDefinition testDefinition,
            bool canConnect,
            string inboundMessage,
            ILogger logger)
        {
            bool changedStatus = false;
            TimeSpan? duration = null;
            string message;
            bool noNews = false;
            if (testDefinition.CanConnect != canConnect)
            {
                changedStatus = true;
                Console.WriteLine($"Connection Changed status to {(canConnect ? "Up" : "Down")}");
                testDefinition.CanConnect = canConnect;
                testDefinition.LastUpdated = DateTime.UtcNow;

                if (canConnect)
                {
                    //DownSince = null;
                    testDefinition.UpSince = DateTime.UtcNow;
                    if (testDefinition.DownSince.HasValue)
                        duration = testDefinition.UpSince.Value.Subtract(testDefinition.DownSince.Value);


                    // reset the email backoff scheme
                    testDefinition.BackoffSec = testDefinition.BackoffMinSec; 
                    testDefinition.LastEmailed = null;
                }
                else
                {
                    //UpSince = null;
                    testDefinition.DownSince = DateTime.UtcNow;
                    if (testDefinition.UpSince.HasValue)
                        duration = testDefinition.DownSince.Value.Subtract(testDefinition.UpSince.Value);
                }

                message = duration.HasValue ?
                    $"{DateTime.UtcNow} connectivity changed to {(canConnect ? "Up" : "Down")} ({(canConnect ? "Downtime" : "Uptime")}: {(duration.Value)})" :
                    $"{DateTime.UtcNow} connectivity changed to {(canConnect ? "Up" : "Down")}";

                if (!string.IsNullOrEmpty(inboundMessage ))
                    message += $"\nFurther information from protocol: {inboundMessage}\n\n";

                logger.LogInformation(message);
            }
            else
            {
                if (!testDefinition.DownSince.HasValue && !canConnect)
                {
                    testDefinition.DownSince = DateTime.UtcNow;
                    message = "Connection is DOWN.";
                    logger.LogInformation(message);
                }
                else if (!testDefinition.UpSince.HasValue && canConnect)
                {
                    testDefinition.UpSince = DateTime.UtcNow;
                    message = "Connection is UP.";
                    logger.LogInformation(message);
                }
                else
                {
                    noNews = true;
                    logger.LogTrace(".");
                }

                duration = DateTime.UtcNow.Subtract(canConnect ? testDefinition.UpSince.Value : testDefinition.DownSince.Value);
                message = duration.Value.TotalSeconds > 2 ?
                    $"{DateTime.UtcNow} connectivity {(canConnect ? "Up" : "Down")} ({(canConnect ? "Uptime" : "Downtime")} %s: {(duration.Value)})" :
                    $"{DateTime.UtcNow} connectivity {(canConnect ? "Up" : "Down")}";

                if (!string.IsNullOrEmpty(inboundMessage))
                    message += $"\nFurther information from protocol: {inboundMessage}\n\n";

            }


            // EVENT PROCESSING
            UpDownEvent upDownEvent = testDefinition.Events.LastItem();

            if (upDownEvent == null ||
                upDownEvent.Finished ||
                upDownEvent.ProcessResult(canConnect)
                )
            {
                upDownEvent = new UpDownEvent()
                {
                    IsUpEvent = canConnect,
                    TestDefinitionId = testDefinition.Id,
                    TestDefinition = testDefinition
                };

                message += "\nStarting new Event\n";
                noNews = false;
                testDefinition.Events.Add(upDownEvent);
            }

            if (noNews)
                logger.LogTrace(message);
            else
            {
                if (changedStatus)
                    logger.LogInformation(message);
                else
                    logger.LogDebug(message);
                testDefinition.Messages.Add(message);
            }

            Console.WriteLine(message);

            return
                ProcessForEmail(testDefinition, canConnect, message, changedStatus, duration) ?
             message : null;
        }

        /// <summary>
        /// Process that an error occurred according to a cool-down scenario.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logger"></param>
        /// <returns>message if it is time to email.</returns>
        public static string ProcessError(
            this TestDefinition testDefinition,
            string message,
            ILogger logger)
        {
            // There's a question of whether or not this should end events, vs bump error counts, vs something else.
            // I think events should have counts of errors, or errors should be their own events (basically just timestamps).
            // Will start with just counts.
            testDefinition.LastErrorMsg = message;
            testDefinition.LastUpdated = DateTime.UtcNow;
            testDefinition.Messages.Add(message);
            bool shouldMessage = false;
            // EVENT PROCESSING
            UpDownEvent upDownEvent = testDefinition.Events.LastItem();
            if (upDownEvent != null &&
               !upDownEvent.Finished &&
                upDownEvent.IsUpEvent &&
                upDownEvent.ProcessError())
            {
                // we just killed off an event that was working.
                // need to process for email.
                upDownEvent = null;

                if (ProcessForEmail(testDefinition, false, message, true, null))
                    shouldMessage = true;

                // need to change the status of the test
                testDefinition.CanConnect = false;
                testDefinition.DownSince = DateTime.UtcNow;

                message = "Connection is NOW DOWN because of too many errors.";
                logger.LogInformation(message);
            }
            if (upDownEvent == null ||
                upDownEvent.Finished ||
                upDownEvent.ProcessError()
                )
            {
                upDownEvent = new UpDownEvent()
                {
                    IsUpEvent = false,
                    TestDefinitionId = testDefinition.Id,
                    TestDefinition = testDefinition
                };

                testDefinition.Events?.Add(upDownEvent);
            }


            return shouldMessage ? message : null;
        }



        public static TestDefinition SetStarting(
            this TestDefinition testDefinition)
        {
            if (testDefinition.TimeoutMSec == 0)
                testDefinition.TimeoutMSec = TestDefinition.TimeoutDefaultForProtocol(testDefinition.Protocol);

            // determine if we are restarting, and if so, restart things.
            testDefinition.BackoffSec = testDefinition.BackoffMinSec; // reset the backoff scheme


            testDefinition.RunningSince = DateTime.UtcNow;
            return testDefinition;
        }

        public static TestDefinition SetStopping(this TestDefinition testDefinition, bool andDisable)
        {
            testDefinition.Messages.Add("Stopping...");
            testDefinition.LastUpdated = DateTime.UtcNow;
            
            UpDownEvent upDownEvent = testDefinition.Events.LastItem();
            if (upDownEvent?.Finished ?? false)
                upDownEvent.Ended = DateTime.UtcNow;
            if(andDisable)
                testDefinition.Disabled = andDisable;
            return testDefinition;
        }

        public static string CreateErrorEmailSubject(this TestDefinition test)
        {
            var subject = $"{test.Protocol} connection {test.TestName} ";
            if (test.CanConnect.HasValue)
            {
                if (test.CanConnect.Value)
                    subject += $"is now UP";
                else
                {
                    if (!test.DownSince.HasValue || DateTime.UtcNow.Subtract(test.DownSince.Value).TotalSeconds < 2)
                        subject += " JUST WENT DOWN";
                    else
                        subject += $" DOWN {(test.DownSince.HasValue ? DateTime.UtcNow.Subtract(test.DownSince.Value) : "")}";

                }
            }

            return subject;
        }


        public static void UpdateDefinitionUnderTestFrom(this TestDefinition test, TestDefinition other)
        {
            if (other.Id != test.Id)
                throw new NullReferenceException("Wrong test");
            else if (other.Protocol != test.Protocol)
                throw new ArgumentException("cannot mix and match protocols on a test configuration");

            test.Address = other.Address;
            test.BackoffMaxSec = other.BackoffMaxSec;
            test.BackoffMinSec = other.BackoffMinSec;
            test.BackoffStepSec = other.BackoffStepSec;
            test.CheckIntervalSec = other.CheckIntervalSec;
            test.ConsequtiveErrorsBeforeDisconnected = other.ConsequtiveErrorsBeforeDisconnected;
            test.Disabled = other.Disabled;
            test.ShouldEmailStatus = other.ShouldEmailStatus;
            test.TestName = other.TestName;
            test.TimeoutMSec = other.TimeoutMSec;
            test.BackoffStepSec = other.BackoffStepSec;
            test.SaveInterval = other.SaveInterval;
            test.Port = other.Port;
        }
    }
}
