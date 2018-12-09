using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LiquidVictor.Data.Postgres
{
    internal class SortableEntityBase : EntityBase
    {
        [Column("sortorder"), Required]
        public int SortOrder { get; set; }
    }
}
