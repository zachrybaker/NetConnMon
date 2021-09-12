using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetConnMon.Domain.Entities
{
    public class UpDownEvent : BaseEntity, IIdentityEntity
    {
        public int       TestDefinitionId    { get; set; }

        public DateTime  Started             { get; set; } = DateTime.UtcNow;
        public DateTime? Ended               { get; set; }
        public int       Heartbeats          { get; set; } = 1;
        public bool      IsUpEvent           { get; set; }
        public Int64     Errors              { get; set; }
        public int       ConsecutiveErrors   { get; set; }

        [ForeignKey("TestDefinitionId")]
        public TestDefinition TestDefinition { get; set; }
        public bool Finished => Ended.HasValue;

        /// <summary>
        /// Process error and returns true if it ended this event.
        /// </summary>
        /// <returns>True if event was ended.</returns>
        public bool ProcessError()
        {
            if(Finished)
                throw new Exception("Cannot process an ended event");

            Errors++;

            if (IsUpEvent)
                ConsecutiveErrors++;
            if (TestDefinition?.ConsequtiveErrorsBeforeDisconnected == ConsecutiveErrors)
            {
                Ended = DateTime.UtcNow;
                return true;
            }

            return false;
                 
        }
        /// <summary>
        /// processes connection status and returns true if it ended this event.
        /// </summary>
        /// <param name="canConnect"></param>
        /// <returns>True if event was ended</returns>
        public bool ProcessResult(bool canConnect)
        {
            if (Finished)
                throw new Exception("Cannot process an ended event");

            {
                Heartbeats++;

                if (IsUpEvent != canConnect)
                {
                    Ended = DateTime.UtcNow;

                    return true;
                }

                if (canConnect)
                    ConsecutiveErrors = 0;

                return false;
            }
        }
    }
}
