using System;
using System.ComponentModel.DataAnnotations;

namespace Common
{
    public class Entity
    {
        [Key]
        public Guid Id { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}