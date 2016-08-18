using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LV.Publication.Interfaces
{
    public interface ISourceProcessor
    {
        Guid Id { get; }
        DateTime LastAttempt { get; }
        Int64 AttemptTimeoutMs { get; set; }
        Entities.Source Config { get; }

        bool IsActive { get; }

        void Start();
        void Stop();
    }

}
