using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LV.Publication.Enumerations
{
    public enum ProcessorResult
    {
        Success,
        Rollback,
        FailAndContinue
    }
}
