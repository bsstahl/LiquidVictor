using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LiquidVictor.Data.Postgres
{
    internal class EntityBase
    {
        private Guid _id;

        [Column("id"), Key]
        public Guid Id
        {
            get => _id;
            set => CompareAndUpdate(ref _id, value);
        }

        // Set on creation at the DB
        [Column("createdate")]
        public DateTime CreateDate { get; set; }

        // PostgreSQL does not support computed columns 
        // Per https://www.npgsql.org/efcore/value-generation.html 
        // as of 2018-12-06 
        [Column("lastmodifieddate")]
        public DateTime LastModifiedDate { get; set; }


        protected void CompareAndUpdate<T>(ref T oldValue, T newValue)
        {
            if (oldValue != null || newValue != null)
                if (oldValue == null || !oldValue.Equals(newValue))
                {
                    oldValue = newValue;
                    this.LastModifiedDate = DateTime.UtcNow;
                }
        }

    }
}
