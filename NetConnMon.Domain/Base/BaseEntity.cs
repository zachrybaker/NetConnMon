using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetConnMon.Domain
{
    public class BaseEntity : IIdentityEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }
    }
}
