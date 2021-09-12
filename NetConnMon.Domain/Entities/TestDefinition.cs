using NetConnMon.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetConnMon.Domain.Entities
{
    
    public class TestDefinition : BaseEntity, IIdentityEntity
    {
     
        ///////////////////////////
        // test configurtion
        ///////////////////////////
        [Required]
        [StringLength(50,MinimumLength =2)]
        public string            TestName     { get; set; }

        [Required]
        public NetProtocol       Protocol     { get; set; }


        [Required]
        public UInt32            TimeoutMSec  { get; set; }
        [Required]
        [MinLength(7)]
        public string            Address      { get; set; }
        [Required]
        public UInt16            Port         { get; set; }
        [Required]
        /// <summary>
        /// Seconds between test runs
        /// </summary>
        public UInt16 CheckIntervalSec { get; set; } = 2;

        ///////////////////////////
        // interval configurations
        ///////////////////////////
        /// <summary>
        /// Seconds between Connection going down and needing to first email about it. 
        /// </summary>
        [Required]
        public UInt16 MinInterruptionSec  { get; set; } = 5;
        [Required]
        public UInt16 BackoffMinSec       { get; set; } = 60;
        [Required]
        public UInt16 BackoffMaxSec       { get; set; } = 3600;
        [Required]
        public UInt16 BackoffStepSec      { get; set; } = 300;
        [Required]
        public bool ShouldEmailStatus     { get; set; } = true;
        [Required]
        public UInt16 ConsequtiveErrorsBeforeDisconnected { get; set; } = 5;

        [Required]
        public UInt32 SaveInterval        { get; set; } = 10;

        /// <summary>
        /// Current backoff value.
        /// </summary>
        public int BackoffSec          { get; set; }
        /// <summary>
        /// Starts out as an unknown state.  Errors 
        /// </summary>
        public bool?             CanConnect   { get; set; }
        public bool              Disabled     { get; set; } = false;

        public DateTime?         RunningSince { get; set; } // set when test service starts this configuration
        public DateTime?         DownSince    { get; set; }
        public DateTime?         UpSince      { get; set; }
        public DateTime?         LastEmailed  { get; set; }
        public DateTime?         LastErrored  { get; set; }
        public Int64             Errors       { get; set; }

        public string            LastErrorMsg { get; set; }

        public List<UpDownEvent> Events       { get; set; } = new List<UpDownEvent>();

        /// <summary>
        /// Used to note changes and to track if it is time to email.
        /// </summary>
        [NotMapped]
        public List<string> Messages { get; set; } = new List<string>();

        ///////////////////////////
        /// SELF-CONTAINED LOGIC
        ///////////////////////////
        public static UInt32 TimeoutDefaultForProtocol(NetProtocol protocol) =>
            protocol switch
            {
                NetProtocol.ICMP => 3000,
                NetProtocol.TCP  => 5000,
                NetProtocol.UDP  => 5000,
                _ => throw new ArgumentException(message: "Unsupported protocol", paramName: "NetProtocol")
            };
    }
}
