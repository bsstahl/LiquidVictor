using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LV.Publication.Entities
{
    public class Source
    {
        public long AttemptTimeoutMs { get; set; }

        public Source(long attemptTimeoutMs)
        {
            this.AttemptTimeoutMs = attemptTimeoutMs;
        }
    }
}
