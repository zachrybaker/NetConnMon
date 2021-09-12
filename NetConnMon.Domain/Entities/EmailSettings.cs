using System;
using MimeKit.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetConnMon.Domain.Entities
{
    public class EmailSettings : BaseEntity, IIdentityEntity
    {

        [StringLength(50, MinimumLength = 9)]
        public string SmtpHost { get; set; }
        [Required]
        public UInt16 Port { get; set; } = 587; // 25
        [Required]
        public bool UseSSL { get; set; } = true;
        [StringLength(50, MinimumLength = 0)]
        public string SMTPUsername { get; set; }
        [StringLength(512, MinimumLength = 0)]
        public string SMTPPassword { get; set; }
        [Required]
        [StringLength(50)]
        public string SenderName { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string SenderEmail { get; set; }
        [Required]
        [StringLength(150)]
        public string RecipientName { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string RecipientEmail { get; set; }
        
        public TextFormat TextFormat { get; set; }


        public bool IsNotSet =>
             !Validator.TryValidateObject(this,
                    new ValidationContext(this),
                    new List<ValidationResult>(), true);
            
        
    }
}
