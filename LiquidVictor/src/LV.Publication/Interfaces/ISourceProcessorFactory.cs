using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LV.Publication.Interfaces
{
    public interface ISourceProcessorFactory
    {
        ISourceProcessor GetSource();
    }
}
