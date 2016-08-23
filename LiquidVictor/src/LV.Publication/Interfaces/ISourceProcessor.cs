﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LV.Publication.Interfaces
{
    public interface ISourceProcessor
    {
        Guid Id { get; }
        DateTime LastAttempt { get; }
        Int64 AttemptTimeoutMs { get; }
        Entities.Source Config { get; }

        bool IsActive { get; }

        void Start();
        void Stop();
        bool Pause();
        bool Unpause();


        event ProcessorEventHandler Starting;
        event ProcessorEventHandler Started;
        event ProcessorEventHandler Stopping;
        event ProcessorEventHandler Stopped;
    }

}
