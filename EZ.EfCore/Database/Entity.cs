using System;
using System.ComponentModel.DataAnnotations;

namespace API.Database
{
    public class Entity
    {
        [Key]
        public Guid Id { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}